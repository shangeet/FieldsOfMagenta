using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateProcessor : MonoBehaviour, StateProcessorInterface {

    public SharedResourceBus sharedResourceBus;
    public UIHandler uiHandler;

    void Awake() {
        sharedResourceBus = GameObject.Find("SharedResourceBus").GetComponent<SharedResourceBus>();
        uiHandler = GameObject.Find("UIHandler").GetComponent<UIHandler>();
    }

    public virtual bool Process() {
        return false;
    }

    public void ChangeState(GameState state) {
        print("Changed state to " + state.ToString());
        sharedResourceBus.SetCurrentGameState(state);
    }

    public void ResetTurnStateData() {
        sharedResourceBus.SetClickedNode(null);
        sharedResourceBus.SetClickedPlayerNode(null);
        sharedResourceBus.SetPreviousClickedNode(null);
        sharedResourceBus.SetClickedNodePath(null);
    }

    public void CancelPlayerMove() {
        Node previousClickedNode = sharedResourceBus.GetPreviousClickedNode();
        Node clickedNode = sharedResourceBus.GetClickedNode();
        if (previousClickedNode != null) {
            PlayerAnimator playerToAnimate = clickedNode.getPlayerInfo().playerAnimator;
            playerToAnimate.PlayerReturnToTile(sharedResourceBus.GetTileMap(), previousClickedNode);
            //update player information in nodes
            Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
            SwapNodeInfoOnSpriteMove(clickedPlayerNode, previousClickedNode);            
        }
    }

    public void SwapNodeInfoOnSpriteMove(Node source, Node dest) {
        PlayerInfo srcPlayerInfo = source.getPlayerInfo();
        source.setPlayerInfo(dest.getPlayerInfo());
        dest.setPlayerInfo(srcPlayerInfo);
        //finally, we update the node dict to reflect this
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        nodeDict[source.getPosition()] = source;
        nodeDict[dest.getPosition()] = dest;
        sharedResourceBus.SetNodeDict(nodeDict);
    }

    public bool CheckIfPlayerDied(PlayerInfo info) {
        if (info.currentHealth <= 0) {
            if (info.getIsEnemy()) {
                List<PlayerInfo> retreatedEnemies = sharedResourceBus.GetRetreatedEnemies();
                Dictionary<string, bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
                retreatedEnemies.Add(info);
                enemyTurnEndedDict[info.getPlayerId()] = true;
                sharedResourceBus.SetRetreatedEnemies(retreatedEnemies);
                sharedResourceBus.SetEnemyTurnEndedDict(enemyTurnEndedDict);
            } else {
                List<PlayerInfo> retreatedPlayers = sharedResourceBus.GetRetreatedPlayers();
                Dictionary<string, bool> playerTurnEndedDict = sharedResourceBus.GetPlayerTurnEndedDict();
                retreatedPlayers.Add(info);
                playerTurnEndedDict[info.getPlayerId()] = true;
                sharedResourceBus.SetRetreatedPlayers(retreatedPlayers);
                sharedResourceBus.SetPlayerTurnEndedDict(playerTurnEndedDict);
            }
            GameObject playerToDestroy = GameObject.Find(info.getPlayerId());
            Destroy(playerToDestroy);
            GameObject healthBarToDestroy = GameObject.Find("HB-" + info.getPlayerId());
            Destroy(healthBarToDestroy);
            GameObject statusContainerToDestroy = GameObject.Find("SC-" + info.getPlayerId());
            Destroy(statusContainerToDestroy);
            Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
            clickedPlayerNode.setPlayerInfo(null); 
            sharedResourceBus.SetClickedPlayerNode(clickedPlayerNode);
            return true;             
        }
        return false;
    }

    public void ProcessStatusEffects() {
        Dictionary<Vector3Int, Node> newNodeDict = new Dictionary<Vector3Int, Node>();
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
        foreach (KeyValuePair<Vector3Int, Node> pair in nodeDict) {
            PlayerInfo currentInfo = pair.Value.getPlayerInfo();
            if (currentInfo != null) { //player exists on node. update
                PlayerInfo newInfo = HandlePlayerStatuses(currentInfo);
                bool playerDied = CheckIfPlayerDied(newInfo);
                if (!playerDied) {
                    pair.Value.setPlayerInfo(newInfo);
                } else {
                    pair.Value.setPlayerInfo(null);
                }
                if (clickedPlayerNode != null && currentInfo.getPlayerId() == clickedPlayerNode.getPlayerId()) {
                    clickedPlayerNode = pair.Value;
                }
                newNodeDict[pair.Key] = pair.Value;                
            } else { //no player exists on node. keep as is
                newNodeDict[pair.Key] = pair.Value;
            }
        }
        sharedResourceBus.SetNodeDict(newNodeDict);         
    }

    public PlayerInfo HandlePlayerStatuses(PlayerInfo info) {

        if (info.statusList.Count == 0) {
            return info;
        }

        List<StatusEffect> updatedStatusList = new List<StatusEffect>();

        foreach(StatusEffect effect in info.statusList) {

            if (effect.remainingActiveTurns != 0) {
                if (effect.effectType == EffectType.BURNED && effect.maxTurns == effect.remainingActiveTurns) { //applied only once
                    info.currentAttack = Mathf.RoundToInt((info.baseAttack * (0.5f)));
                    effect.remainingActiveTurns--;
                    updatedStatusList.Add(effect);
                } else if (effect.effectType == EffectType.POISONED) {
                    info.currentHealth = Mathf.RoundToInt((info.currentHealth * (0.75f)));
                    effect.remainingActiveTurns--;
                    updatedStatusList.Add(effect);
                } else if (effect.effectType == EffectType.PARALYZED && effect.maxTurns == effect.remainingActiveTurns) {
                    info.currentMov = info.currentMov == 1 ? 0 : Mathf.RoundToInt(info.currentMov * 0.5f);
                    effect.remainingActiveTurns--;
                    updatedStatusList.Add(effect);
                } //applied only once                       
            }
        }

        info.statusList = updatedStatusList;
        return info;
    }

    public bool CheckIfGameEnded() {
        NovelEventManager novelEventManager = sharedResourceBus.GetNovelEventManager();
        print("Checking if game ended...");
        print(novelEventManager.AllEventsPlayed());
        print("Current event running? " + novelEventManager.IsEventRunning().ToString());
        if (!novelEventManager.AllEventsPlayed() || novelEventManager.IsEventRunning()) {
            return false;
        }

        List<PlayerInfo> retreatedEnemies = sharedResourceBus.GetRetreatedEnemies();
        List<PlayerInfo> retreatedPlayers = sharedResourceBus.GetRetreatedPlayers();
        Dictionary<string,bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
        Dictionary<string,bool> playerTurnEndedDict = sharedResourceBus.GetPlayerTurnEndedDict();
        if (retreatedEnemies.Count == enemyTurnEndedDict.Keys.Count) {
            sharedResourceBus.SetPlayerVictory(true);
            ChangeState(GameState.GameEndState);
            return true;
        } else if (retreatedPlayers.Count == playerTurnEndedDict.Keys.Count) {
            sharedResourceBus.SetPlayerVictory(false);
            ChangeState(GameState.GameEndState);
            return true;
        }
        return false;
    }

    public void CalculateBattleEventDisplayBattleUI(BattleEventScreen battleEventScreen, Node attackerNode, Node defenderNode, Dictionary<Vector3Int, Node> nodeDict) {
        PlayerInfo attackerPI = attackerNode.getPlayerInfo();
        PlayerInfo defenderPI = defenderNode.getPlayerInfo();
        PlayerInfo newAttackerPI = PlayerInfo.Clone(attackerPI);
        PlayerInfo newDefenderPI = PlayerInfo.Clone(defenderPI);
        int attackerHealth = attackerPI.currentHealth;
        int defenderHealth = defenderPI.currentHealth;
        int attackerAtk = attackerPI.currentAttack;
        int defenderAtk = defenderPI.currentAttack;
        int attackerDef = attackerPI.currentDefense;
        int defenderDef = defenderPI.currentDefense;

        //attacker attacks defender
        int dmgDoneToDefender = attackerAtk - defenderDef;
        newDefenderPI.currentHealth = defenderHealth - Mathf.Max(0, dmgDoneToDefender);
        if (newDefenderPI.currentHealth <= 0) {
            newDefenderPI.currentHealth = 0;
            if (newDefenderPI.getIsEnemy()) {
                //player gains experience
                newAttackerPI.gainExp(newDefenderPI.level * 50);
            } 
            CheckIfPlayerDied(newDefenderPI);
            //show the actual battle on-screen
            attackerNode.setPlayerInfo(newAttackerPI);
            defenderNode.setPlayerInfo(null);
        } else {
            int dmgDoneToAttacker = defenderAtk - attackerDef;
            newAttackerPI.currentHealth = attackerHealth - Mathf.Max(0, dmgDoneToAttacker);    
            if (newAttackerPI.currentHealth <= 0) {
                newAttackerPI.currentHealth = 0;
                CheckIfPlayerDied(newAttackerPI);
                //update node dict's player info
                attackerNode.setPlayerInfo(null);
                defenderNode.setPlayerInfo(newDefenderPI);
            } else {
                attackerNode.setPlayerInfo(newAttackerPI);
                defenderNode.setPlayerInfo(newDefenderPI);
            }
        } 
        StartCoroutine(battleEventScreen.openBattleEventScreen(attackerPI, defenderPI, newAttackerPI, newDefenderPI, true, true));
        nodeDict[attackerNode.getPosition()] = attackerNode;
        nodeDict[defenderNode.getPosition()] = defenderNode;
        sharedResourceBus.SetNodeDict(nodeDict);

        if (!attackerPI.getIsEnemy() && newAttackerPI.currentHealth > 0 && newDefenderPI.currentHealth == 0) {
            newAttackerPI.gainExp(1500);
            int timesLeveledUp = newAttackerPI.level - attackerPI.level;
            List<int> totalExpToLevelUp = newAttackerPI.getTotalExpListLevels(attackerPI.level, newAttackerPI.level);
            PlayerInfo oldBattlePI = attackerPI;
            PlayerInfo newBattlePI = newAttackerPI;
            bool showExpGainBar = true;
            updateBattleVars(timesLeveledUp, totalExpToLevelUp, oldBattlePI, newBattlePI, showExpGainBar);
        } else if (attackerPI.getIsEnemy() && newAttackerPI.currentHealth == 0 && newDefenderPI.currentHealth > 0) {
            newDefenderPI.gainExp(1500);
            int timesLeveledUp = newDefenderPI.level - defenderPI.level;
            List<int> totalExpToLevelUp = newDefenderPI.getTotalExpListLevels(defenderPI.level, newDefenderPI.level);
            PlayerInfo oldBattlePI = defenderPI;
            PlayerInfo newBattlePI = newDefenderPI;
            bool showExpGainBar = true;
            updateBattleVars(timesLeveledUp, totalExpToLevelUp, oldBattlePI, newBattlePI, showExpGainBar);
        }
    }

    public void CalculateHealEventDisplayBattleUI(BattleEventScreen battleEventScreen, Node healerNode, Node targetNode, Dictionary<Vector3Int, Node> nodeDict) {
        PlayerInfo healerPI = healerNode.getPlayerInfo();
        PlayerInfo targetPI = targetNode.getPlayerInfo();
        PlayerInfo newHealerPI = PlayerInfo.Clone(healerPI);
        PlayerInfo newTargetPI = PlayerInfo.Clone(targetPI);

        float healVal = ((float) healerPI.currentMagicAttack / (float) targetPI.baseHealth) * 100;

        int healingAmt = Mathf.RoundToInt(healVal);

        newTargetPI.currentHealth = Mathf.Min(newTargetPI.baseHealth, newTargetPI.currentHealth + healingAmt);

        newHealerPI.gainExp(newHealerPI.level * 50);

        healerNode.setPlayerInfo(newHealerPI);
        targetNode.setPlayerInfo(newTargetPI);
        
        StartCoroutine(battleEventScreen.openHealEventScreen(healerPI, targetPI, newHealerPI, newTargetPI));
        nodeDict[healerNode.getPosition()] = healerNode;
        nodeDict[targetNode.getPosition()] = targetNode;
        sharedResourceBus.SetNodeDict(nodeDict);

        int timesLeveledUp = newHealerPI.level - healerPI.level;
        List<int> totalExpToLevelUp = newHealerPI.getTotalExpListLevels(healerPI.level, newHealerPI.level);
        updateBattleVars(timesLeveledUp, totalExpToLevelUp, healerPI, newHealerPI, true);
    }

    private void updateBattleVars(int timesLeveledUp, List<int> totalExpToLevelUp, PlayerInfo oldBattlePI, PlayerInfo newBattlePI, bool showExpGainBar) {
        sharedResourceBus.SetTimesLeveledUp(timesLeveledUp);
        sharedResourceBus.SetTotalExpToLevelUp(totalExpToLevelUp);
        sharedResourceBus.SetOldBattlePI(oldBattlePI);
        sharedResourceBus.SetNewBattlePI(newBattlePI);
        sharedResourceBus.SetShowExpGainBar(showExpGainBar);
    }

}
