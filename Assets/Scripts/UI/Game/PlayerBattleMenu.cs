using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleMenu : AbstractMenu {

    GameObject playerBattleMenu;
    bool playerBattleMenuDisplayed;
    bool canHeal = false;

    protected override void Awake() {
        base.Awake();
        setupUIElements();
    }

    void Start() {}

    void setupUIElements() {
        // player battle menu
        playerBattleMenu = GameObject.Find("PlayerBattleMenu");
        Button actionButton = GameObject.Find("ActionButton").GetComponent<Button>() as Button;
        Button itemButton = GameObject.Find("ItemButton").GetComponent<Button>() as Button;
        Button waitButton = GameObject.Find("WaitButton").GetComponent<Button>() as Button;
        Button swapButton = GameObject.Find("ItemSwapButton").GetComponent<Button>() as Button;
        actionButton.onClick.AddListener(onActionButtonClick);
        itemButton.onClick.AddListener(onItemButtonClick);
        waitButton.onClick.AddListener(onWaitButtonClick);
        swapButton.onClick.AddListener(onSwapItemButtonClick);
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }

    void updateUIElements() {
        Text actionButtonText = GameObject.Find("ActionButton/Text").GetComponent<Text>();
        if (canHeal) {
            actionButtonText.text = "Heal";    
        } else {
            actionButtonText.text = "Attack";
        }
    }

    public void openPlayerBattleMenu(bool isHealingClass) {       
        playerBattleMenu.SetActive(true);
        playerBattleMenuDisplayed = true;
        PlayerInfo pInfo = sharedResourceBus.GetClickedPlayerNode().getPlayerInfo();
        Vector3 playerPosition = GameObject.Find(pInfo.getPlayerId()).transform.position;

        Vector3 canvasPosition = new Vector3(playerPosition.x + 1, playerPosition.y + 1, 1);
        playerBattleMenu.transform.position = canvasPosition;

        canHeal = isHealingClass;
        updateUIElements();

    }

    void onActionButtonClick() {
        closePlayerBattleMenu();
        if (canHeal) {
            ChangeState(GameState.HealState);
        } else {
            ChangeState(GameState.AttackState);    
        } 
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
