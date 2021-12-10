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
        print("Player Battle menu opened");
        //TODO: Fix this
        //string playerId = clickedPlayerNode.getPlayerInfo().getPlayerId();
        //Player playerClicked = GameObject.Find(playerId).GetComponent<Player>();
        //Vector3 playerSpriteVector = playerClicked.transform.position; //global position where I want the menu to appear
        //print(playerSpriteVector); //(-0.5, 1.5, 0)
        //RectTransform rt = playerBattleMenu.transform.GetChild(0).GetComponent<RectTransform>();
        //Vector3 newPos = new Vector3(playerSpriteVector.x + 2, playerSpriteVector.y + 2, 0);
        //rt.anchoredPosition = newPos;
    }

    void onAttackButtonClick() {
        print("Attack Button was clicked!");
        closePlayerBattleMenu();
        gameMaster.preProcessAttackState();
        gameMaster.ChangeState(GameState.AttackState);
    }

    void onItemButtonClick() {
        print("Item Button was clicked! TODO: Implement items");
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.ShowItemMenuState);
    }

    void onWaitButtonClick() {
        print("Wait Button was clicked!");
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.TurnEndState);
    }

    void onSwapItemButtonClick() {
        print("Swap Item Button was clicked!");
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
