using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleMenu : AbstractMenu {

    GameObject playerBattleMenu;
    bool playerBattleMenuDisplayed;

    protected override void Awake() {
        base.Awake();
        setupUIElements();
    }

    void Start() {}

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
        PlayerInfo pInfo = sharedResourceBus.GetClickedPlayerNode().getPlayerInfo();
        Vector3 playerPosition = GameObject.Find(pInfo.getPlayerId()).transform.position;

        Vector3 canvasPosition = new Vector3(playerPosition.x + 1, playerPosition.y + 1, 1);
        playerBattleMenu.transform.position = canvasPosition;

    }

    void onAttackButtonClick() {
        closePlayerBattleMenu();
        ChangeState(GameState.AttackState);
    }

    void onItemButtonClick() {
        closePlayerBattleMenu();
        ChangeState(GameState.ShowItemMenuState);
    }

    void onWaitButtonClick() {
        closePlayerBattleMenu();
        ChangeState(GameState.HandleTileState);
    }

    void onSwapItemButtonClick() {
        closePlayerBattleMenu();
        ChangeState(GameState.SwapItemState);
    }

    public bool IsPlayerBattleMenuDisplayed() {
        return playerBattleMenuDisplayed;
    }

    public void closePlayerBattleMenu() {
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }

}
