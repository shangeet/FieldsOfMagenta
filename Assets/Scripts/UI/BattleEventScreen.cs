using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventScreen : MonoBehaviour
{
    GameObject battleEventScreen;
    GameMaster gameMaster;

    bool battleEventScreenDisplayed;

    void Awake() {
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
        print("Attacker Original Health: " + atkPI.currentHealth.ToString() + "/" + atkPI.baseHealth.ToString());
        print("Attacker Current Health: " + newAtkPI.currentHealth.ToString() + "/" + newAtkPI.baseHealth.ToString());
        print("Defender Original Health: " + defPI.currentHealth.ToString() + "/" + defPI.baseHealth.ToString());
        print("Defender Current Health: " + newDefPI.currentHealth.ToString() + "/" + newDefPI.baseHealth.ToString());
        //get attacker and defender position values
        GameObject attackerHealthBarGameObj = battleEventScreen.transform.GetChild(1).gameObject;
        GameObject defenderHealthBarGameObj = battleEventScreen.transform.GetChild(2).gameObject;
        Vector3 attackerPos = new Vector3(-1.5f, attackerHealthBarGameObj.transform.position.y - 1, 0.0f);
        Vector3 defenderPos = new Vector3(1.5f, defenderHealthBarGameObj.transform.position.y - 1, 0.0f);
        //setup attacker w/ original stats
        PlayerAnimator attackerPlayer = addPlayerToBattleEventScreen(atkPI.getPlayerId(), attackerPos, atkPI);
        //setup defender w/ original stats
        PlayerAnimator defenderPlayer = addPlayerToBattleEventScreen(defPI.getPlayerId(), defenderPos, defPI);

        //setup healthbar fill values
        setHealthBarOnBattleEventScreen(attackerHealthBarGameObj, atkPI.currentHealth, atkPI.baseHealth);
        setHealthBarOnBattleEventScreen(defenderHealthBarGameObj, defPI.currentHealth, defPI.baseHealth);
        yield return new WaitForSeconds(1.0f);
        //update healthbar final values
        setHealthBarOnBattleEventScreen(attackerHealthBarGameObj, newAtkPI.currentHealth, newAtkPI.baseHealth);
        setHealthBarOnBattleEventScreen(defenderHealthBarGameObj, newDefPI.currentHealth, newDefPI.baseHealth); 
        yield return new WaitForSeconds(1.0f);
        //TODO apply animation logic for attacks
        Destroy(attackerPlayer);
        Destroy(defenderPlayer);
        battleEventScreen.SetActive(false);
        battleEventScreenDisplayed = false;
    }

    public PlayerAnimator addPlayerToBattleEventScreen(string playerId, Vector3 position, PlayerInfo playerInfo) {
        playerId += "-temp";
        GameObject playerToSpawn = new GameObject(playerId);
        PlayerAnimator player = playerToSpawn.AddComponent<PlayerAnimator>() as PlayerAnimator;
        player.Setup(playerInfo);
        player.name = playerId;
        if (player) {
            player.AddPlayerToParallax(position);
        }
        playerToSpawn.transform.parent = battleEventScreen.transform;
        player.spriteRenderer.sortingOrder = 5;        
        print("Sprite added at position: " + position.x + "," + position.y);
        return player;
    }

    void setHealthBarOnBattleEventScreen(GameObject healthBar, int currentHP, int baseHP) {
        float value = ((float) currentHP) / baseHP;
        healthBar.GetComponent<HealthBar>().SetHealth(value);
        print("Health bar value set to: " + value);
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
