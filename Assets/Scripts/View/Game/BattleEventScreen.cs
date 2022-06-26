using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventScreen : AbstractMenu
{
    GameObject battleEventScreen;
    GameMaster gameMaster;

    bool battleEventScreenDisplayed;

    protected override void Awake() {
        base.Awake();
        setupUIElements();
    }

    void Start() {
        gameMaster = gameObject.GetComponent<GameMaster>();
    }

    void setupUIElements() {
        battleEventScreen = GameObject.Find("BattleEventScreen");
        battleEventScreenDisplayed = false;
        // Convert the screenpoint to ui rectangle local point
        moveCanvasToGlobalPoint(battleEventScreen, new Vector3(0,0,0));
        battleEventScreen.SetActive(false);
    }

    public IEnumerator openBattleEventScreen(PlayerInfo atkPI, PlayerInfo defPI, PlayerInfo newAtkPI, PlayerInfo newDefPI, bool atkHit, bool defHit) {
        //re-enable the battle event screen
        battleEventScreen.SetActive(true);
        battleEventScreenDisplayed = true;
        //get attacker and defender position values
        GameObject attackerHealthBarGameObj = battleEventScreen.transform.GetChild(1).gameObject;
        GameObject defenderHealthBarGameObj = battleEventScreen.transform.GetChild(2).gameObject;
        GameObject foreground = battleEventScreen.transform.Find("BattleBackground/Foreground").gameObject;
        Vector3 attackerPos = new Vector3(attackerHealthBarGameObj.transform.position.x, foreground.transform.position.y - 2, 0.0f);
        Vector3 defenderPos = new Vector3(defenderHealthBarGameObj.transform.position.x, foreground.transform.position.y - 2, 0.0f);
        //setup healthbar initial fill values
        setHealthBarOnBattleEventScreen(attackerHealthBarGameObj, atkPI.currentHealth, atkPI.baseHealth);
        setHealthBarOnBattleEventScreen(defenderHealthBarGameObj, defPI.currentHealth, defPI.baseHealth);

        //setup attacker w/ original stats
        GameObject attackerPlayer = addPlayerToBattleEventScreen(atkPI.getPlayerId(), attackerPos, atkPI, true);
        //setup defender w/ original stats
        GameObject defenderPlayer = addPlayerToBattleEventScreen(defPI.getPlayerId(), defenderPos, defPI, false);
        animateAttackSequence(attackerPlayer, attackerPos, defenderPlayer, defenderPos, true);
        StartCoroutine(animateHealthReduction(defenderHealthBarGameObj, defPI.currentHealth, newDefPI.currentHealth, newDefPI.baseHealth));
        yield return new WaitForSeconds(2.0f);

        if (newDefPI.currentHealth != 0) {
            animateAttackSequence(defenderPlayer, defenderPos, attackerPlayer, attackerPos, false);
            StartCoroutine(animateHealthReduction(attackerHealthBarGameObj, atkPI.currentHealth, newAtkPI.currentHealth, newAtkPI.baseHealth));
            yield return new WaitForSeconds(2.0f);            
        } else {
            animateDeathSequence(defenderPlayer);
            yield return new WaitForSeconds(1.0f);
        }

        if (newAtkPI.currentHealth == 0) {
            animateDeathSequence(attackerPlayer);
            yield return new WaitForSeconds(1.0f);
        }
        
        //TODO apply animation logic for attacks
        Destroy(attackerPlayer);
        Destroy(defenderPlayer);
        battleEventScreen.SetActive(false);
        battleEventScreenDisplayed = false;
    }

    public IEnumerator openHealEventScreen(PlayerInfo healPI, PlayerInfo targetPI, PlayerInfo newHealPI, PlayerInfo newTargetPI) {
        //re-enable the battle event screen
        battleEventScreen.SetActive(true);
        battleEventScreenDisplayed = true;
        //get healer and target position values
        GameObject healerHealthBarGameObj = battleEventScreen.transform.GetChild(1).gameObject;
        GameObject targetHealthBarGameObj = battleEventScreen.transform.GetChild(2).gameObject;
        GameObject foreground = battleEventScreen.transform.Find("BattleBackground/Foreground").gameObject;
        Vector3 healerPos = new Vector3(healerHealthBarGameObj.transform.position.x, foreground.transform.position.y - 2, 0.0f);
        Vector3 targetPos = new Vector3(targetHealthBarGameObj.transform.position.x, foreground.transform.position.y - 2, 0.0f);
        //setup healthbar initial fill values
        setHealthBarOnBattleEventScreen(healerHealthBarGameObj, healPI.currentHealth, healPI.baseHealth);
        setHealthBarOnBattleEventScreen(targetHealthBarGameObj, targetPI.currentHealth, targetPI.baseHealth);

        //setup healer w/ original stats
        GameObject healerPlayer = addPlayerToBattleEventScreen(healPI.getPlayerId(), healerPos, healPI, true);
        //setup target w/ original stats
        GameObject targetPlayer = addPlayerToBattleEventScreen(targetPI.getPlayerId(), targetPos, targetPI, false);
        animateHealSequence(healerPlayer, targetPlayer);
        StartCoroutine(animateHealthReduction(targetHealthBarGameObj, targetPI.currentHealth, newTargetPI.currentHealth, newTargetPI.baseHealth));
        yield return new WaitForSeconds(2.0f);
        Destroy(healerPlayer);
        Destroy(targetPlayer);
        battleEventScreen.SetActive(false);
        battleEventScreenDisplayed = false;
    }

    public IEnumerator openBardEventScreen(PlayerInfo bardPI, PlayerInfo targetPI, PlayerInfo newBardPI, PlayerInfo newTargetPI) {
        //re-enable the battle event screen
        battleEventScreen.SetActive(true);
        battleEventScreenDisplayed = true;

        //get bard and target position values
        GameObject bardHealthBarGameObj = battleEventScreen.transform.GetChild(1).gameObject;
        GameObject targetHealthBarGameObj = battleEventScreen.transform.GetChild(2).gameObject;
        GameObject foreground = battleEventScreen.transform.Find("BattleBackground/Foreground").gameObject;
        Vector3 bardPos = new Vector3(bardHealthBarGameObj.transform.position.x, foreground.transform.position.y - 2, 0.0f);
        Vector3 targetPos = new Vector3(targetHealthBarGameObj.transform.position.x, foreground.transform.position.y - 2, 0.0f);

        //setup bard w/ original stats
        GameObject bardPlayer = addPlayerToBattleEventScreen(bardPI.getPlayerId(), bardPos, bardPI, true);
        //setup target w/ original stats
        GameObject targetPlayer = addPlayerToBattleEventScreen(targetPI.getPlayerId(), targetPos, targetPI, false);
        animateBuffSequence(bardPlayer, targetPlayer);
        yield return new WaitForSeconds(2.0f);
        Destroy(bardPlayer);
        Destroy(targetPlayer);
        battleEventScreen.SetActive(false);
        battleEventScreenDisplayed = false;        
    }

    public GameObject addPlayerToBattleEventScreen(string playerId, Vector3 position, PlayerInfo playerInfo, bool isAttacker) {
        playerId += "-temp";
        GameObject playerToSpawn = new GameObject(playerId);
        PawnSpawnManager pawnSpawnManager = sharedResourceBus.GetPawnSpawnManager();
        playerToSpawn = pawnSpawnManager.SetupPlayerAnimator(playerToSpawn, playerInfo);
        PlayerAnimator playerAnimator = playerToSpawn.GetComponent<PlayerAnimator>() as PlayerAnimator;
        playerAnimator.setAnimatorMode("battle", playerId, playerInfo.getBattleControllerPath(), playerInfo.portraitRefPath);
        playerAnimator.name = playerId;
        if (playerAnimator) {
            playerAnimator.AddPlayerToParallax(position, isAttacker);
        }
        playerToSpawn.transform.parent = battleEventScreen.transform;
        playerAnimator.spriteRenderer.sortingOrder = 5;        
        return playerToSpawn;
    }

    public void animateAttackSequence(GameObject attackerPlayer, Vector3 attackerPos, GameObject defenderPlayer, Vector3 defenderPos, bool isAttacker) {
        PlayerAnimator attackPlayerAnimator = attackerPlayer.GetComponent<PlayerAnimator>() as PlayerAnimator;
        StartCoroutine(attackPlayerAnimator.AttackEnemy(defenderPlayer, defenderPos, isAttacker));
    }

    public void animateDeathSequence(GameObject playerToAnimate) {
        PlayerAnimator playerAnimator = playerToAnimate.GetComponent<PlayerAnimator>() as PlayerAnimator;
        StartCoroutine(playerAnimator.AnimateFaint());
    }

    public void animateHealSequence(GameObject healerToAnimate, GameObject targetToAnimate) {
        HealerPlayerAnimator healerPlayerAnimator = healerToAnimate.GetComponent<PlayerAnimator>() as HealerPlayerAnimator;
        StartCoroutine(healerPlayerAnimator.HealTarget(targetToAnimate));
    }

    public void animateBuffSequence(GameObject bardToAnimate, GameObject targetToAnimate) {
        BardPlayerAnimator bardPlayerAnimator = bardToAnimate.GetComponent<PlayerAnimator>() as BardPlayerAnimator;
        StartCoroutine(bardPlayerAnimator.BuffTarget(targetToAnimate));
    }

    public void setHealthBarOnBattleEventScreen(GameObject healthBar, int currentHP, int baseHP) {
        float value = ((float) currentHP) / baseHP;
        healthBar.GetComponent<HealthBar>().SetHealth(value);
    }

    public IEnumerator animateHealthReduction(GameObject healthBar, int oldHP, int newHP, int baseHP) {
        float currentHP = (float) oldHP;
        float expectedHP = (float) newHP;
        float step = 0.05f;
        while ((currentHP - expectedHP) > 0.05f) {
            currentHP -= step;
            float value = ((float) currentHP) / baseHP;
            healthBar.GetComponent<HealthBar>().SetHealth(value);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void moveCanvasToGlobalPoint(GameObject go, Vector3 globalPos) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(globalPos);
        Vector2 movePos;
        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(go.transform as RectTransform, screenPos, Camera.main, out movePos);
        //Convert the local point to world point
        go.transform.position = new Vector3(movePos.x, movePos.y, 0);      
    }

    public bool IsBattleEventScreenDisplayed() {
        return battleEventScreenDisplayed;
    }

}
