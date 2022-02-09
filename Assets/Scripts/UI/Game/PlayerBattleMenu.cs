using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleMenu : MonoBehaviour {

    GameObject playerBattleMenu;
    GameMaster gameMaster;
    bool playerBattleMenuDisplayed;

    void Awake() {
        setupUIElements();
    }

    void Start() {
        gameMaster = gameObject.GetComponent<GameMaster>();
    }

    void setupUIElements() {
        // player battle menu
        playerBattleMenu = GameObject.Find("PlayerBattleMenu");
        Button attackButton = GameObject.Find("AttackButton").GetComponent<Button>() as Button;
        Button itemButton = GameObject.Find("ItemButton").GetComponent<Button>() as Button;
        Button waitButton = GameObject.Find("WaitButton").GetComponent<Button>() as Button;
        Button swapButton = GameObject.Find("ItemSwapButton").GetComponent<Button>() as Button;
        attackButton.onClick.AddListener(onAttackButtonClick);
        itemButton.onClick.AddListener(onItemButtonClick);
        waitButton.onClick.AddListener(onWaitButtonClick);
        swapButton.onClick.AddListener(onSwapItemButtonClick);
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }

    public void openPlayerBattleMenu() {       
        playerBattleMenu.SetActive(true);
        playerBattleMenuDisplayed = true;
        PlayerInfo pInfo = gameMaster.clickedPlayerNode.getPlayerInfo();
        Vector3 playerPosition = GameObject.Find(pInfo.getPlayerId()).transform.position;

        Vector3 canvasPosition = new Vector3(playerPosition.x + 1, playerPosition.y + 1, 1);
        playerBattleMenu.transform.position = canvasPosition;

    }

    void onAttackButtonClick() {
        closePlayerBattleMenu();
        gameMaster.preProcessAttackState();
        gameMaster.ChangeState(GameState.AttackState);
    }

    void onItemButtonClick() {
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.ShowItemMenuState);
    }

    void onWaitButtonClick() {
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.HandleTileState);
    }

    void onSwapItemButtonClick() {
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.SwapItemState);
    }

    public bool IsPlayerBattleMenuDisplayed() {
        return playerBattleMenuDisplayed;
    }

    public void closePlayerBattleMenu() {
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }

}
