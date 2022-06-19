using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTurnStateProcessor : StateProcessor {

    public override bool Process() {
        //check if game over before processing starts
        if (CheckIfGameEnded()) {
            return true;
        }
        sharedResourceBus.SetEnemyTurn(true);
        BattleEventScreen battleEventScreen = uiHandler.GetBattleEventScreen();
        PlayerExpScreen playerExpScreen = uiHandler.GetPlayerExpScreen();
        PhaseTransitionUIHandler phaseTransitionUIHandler = uiHandler.GetPhaseTransitionUIHandler();
        //process action for each enemy only if someone else is not attacking and we have a valid enemy that hasn't ended their turn
        if (!battleEventScreen.IsBattleEventScreenDisplayed() && !sharedResourceBus.AllEnemiesHaveMoved() && !phaseTransitionUIHandler.IsPhaseTransitionRunning()) {
            PlayerInfo enemy = pickAvailableEnemy();

            if (enemy == null) { //no available enemy. All enemies have moved.
                sharedResourceBus.SetAllEnemiesHaveMoved(true);
            } else {
                Node enemyNode = NodeUtils.findEnemyNode(enemy.getPlayerId(), sharedResourceBus.GetNodeDict());
                if (enemyNode.getPlayerInfo().IsHealer()) {
                    processEnemyHealer(enemyNode, battleEventScreen);
                } else  {
                    processEnemyAttacker(enemyNode, battleEventScreen);
                }
            }
            return true;
        } else if (!sharedResourceBus.HandleStatusEnemyTurn() && sharedResourceBus.AllEnemiesHaveMoved()) {
            ProcessStatusEffects();
            sharedResourceBus.SetHandleStatusEnemyTurn(true);
            return true;
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !uiHandler.StartedTranslation() && sharedResourceBus.AllEnemiesHaveMoved() && !battleEventScreen.IsBattleEventScreenDisplayed() && !playerExpScreen.IsExperienceScreenProcessing()) {
            if (!CheckIfGameEnded()) {
                uiHandler.SetStartedTranslation(true);
                StartCoroutine(phaseTransitionUIHandler.translatePhaseImage("PlayerPhase"));                
            }
            return true;
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && uiHandler.StartedTranslation()) { //done processing, end turn
            sharedResourceBus.ResetEnemyTurnEndedDict();
            sharedResourceBus.ResetPlayerTurnEndedDict();
            uiHandler.SetStartedTranslation(false);
            sharedResourceBus.SetAllEnemiesHaveMoved(false);
            sharedResourceBus.SetEnemyTurn(false);
            sharedResourceBus.SetHandleStatusEnemyTurn(false);
            ChangeState(GameState.PlayerTurnStart);
            return true;
        } //else the translation is still moving/isn't ready to be moved yet. Wait for the next frame.   
        return false;     
    } 

    private PlayerInfo pickAvailableEnemy() {
        Dictionary<Vector2, PlayerInfo> pawnInfoDict = sharedResourceBus.GetPawnInfoDict();
        Dictionary<string,bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
        List<PlayerInfo> retreatedEnemies = sharedResourceBus.GetRetreatedEnemies();
        foreach (PlayerInfo enemy in pawnInfoDict.Values) {
            if (enemy.getIsEnemy() && !retreatedEnemies.Contains(enemy) && enemyTurnEndedDict[enemy.getPlayerId()] == false) {
                return enemy;
            }
        }
        return null; 
    }   

    private void processEnemyAttacker(Node enemyNode, BattleEventScreen battleEventScreen) {
        PlayerInfo enemy = enemyNode.getPlayerInfo();
        //if player in line of sight, move and attack (or heaL ally)
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        TileEventManager tileEventManager = sharedResourceBus.GetTileEventManager();
        List<Node> nodeRange = NodeUtils.getViableNodesPaths(enemyNode, nodeDict, tileEventManager);
        Node candidatePlayerNode = NodeUtils.findPlayerNodeNearEnemy(enemyNode, nodeDict, tileEventManager);
        if (candidatePlayerNode != null) {         
            sharedResourceBus.SetClickedPlayerNode(candidatePlayerNode);  
            //check if we have to move
            Node attackerNode = enemyNode;
            if (!NodeUtils.getNearbyNodes(enemyNode, nodeDict).Contains(candidatePlayerNode)) { //move and attack else just attack
                //move and attack
                List<Node> pathToMove = NodeUtils.getShortestPathNodes(enemyNode, candidatePlayerNode, nodeRange, Heuristic.NodeDistanceHeuristic, nodeDict);
                PlayerAnimator enemyToAnimate = enemy.playerAnimator;
                //Player enemyToMove = GameObject.Find(enemy.getPlayerId()).GetComponent<Player>();
                pathToMove.RemoveAt(pathToMove.Count - 1); //remove the last element since that's the player\
                Tilemap tileMap = sharedResourceBus.GetTileMap();
                enemyToAnimate.MoveEnemyNextToPlayer(tileMap, pathToMove);
                SwapNodeInfoOnSpriteMove(enemyNode, pathToMove[pathToMove.Count - 1]); //swap node data with new tile   
                attackerNode = pathToMove[pathToMove.Count - 1]; //update the attackerNode                     
            }
            Dictionary<string, bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
            enemyTurnEndedDict[enemy.getPlayerId()] = true;
            sharedResourceBus.SetEnemyTurnEndedDict(enemyTurnEndedDict);
            CalculateBattleEventDisplayBattleUI(battleEventScreen, attackerNode, candidatePlayerNode, nodeDict);
            ChangeState(GameState.ShowExpGainState);
        } else { //nothing to do, just end turn
            Dictionary<string,bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
            enemyTurnEndedDict[enemy.getPlayerId()] = true;
            sharedResourceBus.SetEnemyTurnEndedDict(enemyTurnEndedDict);
        }
    }

    private void processEnemyHealer(Node enemyNode, BattleEventScreen battleEventScreen) {
        PlayerInfo enemy = enemyNode.getPlayerInfo();
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        TileEventManager tileEventManager = sharedResourceBus.GetTileEventManager();
        List<Node> nodeRange = NodeUtils.getViableNodesPaths(enemyNode, nodeDict, tileEventManager);
        Node candidateEnemyNode = NodeUtils.findPlayerNodeEnemyAlly(enemyNode, nodeDict, tileEventManager);

        if (candidateEnemyNode != null) {
            sharedResourceBus.SetClickedPlayerNode(candidateEnemyNode);
            Node healerNode = enemyNode;
            if (!NodeUtils.getNearbyNodes(enemyNode, nodeDict).Contains(candidateEnemyNode)) { //move and heal else just heal
                //move and attack
                List<Node> pathToMove = NodeUtils.getShortestPathNodes(enemyNode, candidateEnemyNode, nodeRange, Heuristic.NodeDistanceHeuristic, nodeDict);
                PlayerAnimator enemyToAnimate = enemy.playerAnimator;
                //Player enemyToMove = GameObject.Find(enemy.getPlayerId()).GetComponent<Player>();
                pathToMove.RemoveAt(pathToMove.Count - 1); //remove the last element since that's the player\
                Tilemap tileMap = sharedResourceBus.GetTileMap();
                enemyToAnimate.MoveEnemyNextToPlayer(tileMap, pathToMove);
                SwapNodeInfoOnSpriteMove(enemyNode, pathToMove[pathToMove.Count - 1]); //swap node data with new tile   
                healerNode = pathToMove[pathToMove.Count - 1]; //update the healerNode                     
            }
            Dictionary<string, bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
            enemyTurnEndedDict[enemy.getPlayerId()] = true;
            sharedResourceBus.SetEnemyTurnEndedDict(enemyTurnEndedDict);
            CalculateHealEventDisplayBattleUI(battleEventScreen, healerNode, candidateEnemyNode, nodeDict);
            ChangeState(GameState.ShowExpGainState);            
        } else {
            Dictionary<string,bool> enemyTurnEndedDict = sharedResourceBus.GetEnemyTurnEndedDict();
            enemyTurnEndedDict[enemy.getPlayerId()] = true;
            sharedResourceBus.SetEnemyTurnEndedDict(enemyTurnEndedDict);            
        }
    }

}
