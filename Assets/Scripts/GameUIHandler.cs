using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameUIHandler : MonoBehaviour {
    GameObject playerBattleMenu;
    GameObject unitInfoMenu; 
    GameObject battleEventScreen;
    GameObject itemMenu;
    GameObject playerPhaseTransitionImage;
    GameObject enemyPhaseTransitionImage;
    GameMaster gameMaster;

    //keep track of what's on/off display
    bool battleEventScreenDisplayed;
    bool playerBattleMenuDisplayed;
    bool isPhaseTransitionRunning;
    bool allEnemiesHaveMoved;
    bool itemMenuDisplayed;

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
        attackButton.onClick.AddListener(onAttackButtonClick);
        itemButton.onClick.AddListener(onItemButtonClick);
        waitButton.onClick.AddListener(onWaitButtonClick);
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;

        // unit info menu
        unitInfoMenu = GameObject.Find("UnitInfoMenu");
        unitInfoMenu.SetActive(false);

        battleEventScreen = GameObject.Find("BattleEventScreen");
        battleEventScreenDisplayed = false;

        // Convert the screenpoint to ui rectangle local point
        moveCanvasToGlobalPoint(battleEventScreen, new Vector3(0,0,0));
        battleEventScreen.SetActive(false);

        // item menu
        itemMenu = GameObject.Find("ItemMenu");
        itemMenu.SetActive(false);
        itemMenuDisplayed = false;

        //Set player/enemy phase transition images in the right place, turn them off for now
        playerPhaseTransitionImage = GameObject.Find("PlayerPhaseImg");
        enemyPhaseTransitionImage = GameObject.Find("EnemyPhaseImg");
        Vector2 leftMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2 (Camera.main.pixelWidth, Camera.main.pixelHeight/2));
        leftMostScreenPosition.x *= -1;
        playerPhaseTransitionImage.transform.position = leftMostScreenPosition;
        enemyPhaseTransitionImage.transform.position = leftMostScreenPosition;
        playerPhaseTransitionImage.SetActive(false);
        enemyPhaseTransitionImage.SetActive(false);
        isPhaseTransitionRunning = false;
        allEnemiesHaveMoved = false;
    }

    public void openPlayerItemMenu() {
        PlayerInfo clickedPlayerInfo = gameMaster.clickedPlayerNode.getPlayerInfo();
        ConsumableItemManager consumableItemManager = clickedPlayerInfo.consumableItemManager;
        Dictionary<string,int> currentConsumableItemsInventory = consumableItemManager.currentConsumableItemsInventory;
        Dictionary<string,ConsumableItem> currentConsumableItems = consumableItemManager.consumableItems;
        //create a new item prefab to add to the menu
        itemMenu.SetActive(true);
        itemMenuDisplayed = true;
        int startHeight = 70;
        //yes I know this looks dumb and I could just add a tag or use find, but eh whatever
        GameObject itemSlotContainer = itemMenu.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        foreach (KeyValuePair<string, int> pair in currentConsumableItemsInventory) {
            string itemName = pair.Key;
            int quantity = pair.Value;
            print("Found item " + itemName + " qty: " + quantity.ToString());
            Sprite itemSprite = currentConsumableItems[itemName].itemSprite;
            GameObject itemRow = itemMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemRow.name = "ID-" + itemSlotContainer.GetComponentsInChildren<Transform>().Length.ToString();
            print(itemRow);
            GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
            Button button = buttonGameObj.GetComponent<Button>();
            button.onClick.AddListener(onConsumableItemButtonClick);
            buttonGameObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = itemName;
            buttonGameObj.transform.GetChild(2).gameObject.GetComponent<Text>().text = quantity.ToString() + "/" + ConsumableItemManager.numMaxItemsPerSlot.ToString();
            buttonGameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = itemSprite;
            itemRow.transform.SetParent(itemSlotContainer.transform, true);
            Vector3 itemRowPosition = new Vector3(0, startHeight, itemRow.transform.position.z);
            itemRow.GetComponent<RectTransform>().anchoredPosition = itemRowPosition;
            startHeight += 30;
        }
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

    public void openUnitInfoMenu(PlayerInfo playerInfo) {
        unitInfoMenu.SetActive(true);
        PlayerAnimator animator = playerInfo.animator;
        Image playerFace = GameObject.Find("PlayerFace").GetComponent<Image>() as Image;
        Text hp = GameObject.Find("HPDisplay").GetComponent<Text>() as Text;
        Text atk = GameObject.Find("AtkDisplay").GetComponent<Text>() as Text;
        Text def = GameObject.Find("DefDisplay").GetComponent<Text>() as Text;
        Text mov = GameObject.Find("MovDisplay").GetComponent<Text>() as Text;
        playerFace.sprite = animator.playerPortrait;
        hp.text = "HP " + playerInfo.currentHealth.ToString() + "/" + Mathf.Max(playerInfo.baseHealth, playerInfo.currentHealth).ToString();
        atk.text = "ATK " + playerInfo.currentAttack.ToString();
        def.text = "DEF " + playerInfo.currentDefense.ToString();
        mov.text = "MOV " + playerInfo.currentMov.ToString();
    }

    public IEnumerator translatePhaseImage(string phase) {
        isPhaseTransitionRunning = true;
        if (phase == "PlayerPhase") {
            yield return new WaitForSeconds(1);
            Vector2 rightMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight/2));
            playerPhaseTransitionImage.SetActive(true);
            Vector2 leftMostScreenPosition = playerPhaseTransitionImage.transform.position;

            for(float t = 0.0f; t < 1.0f; t+=0.1f) {
                playerPhaseTransitionImage.GetComponent<ImageTransitions>().TranslateAcrossScreen(leftMostScreenPosition, rightMostScreenPosition, t);

                if (t == 0.5f) {
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.05f);
            }
            playerPhaseTransitionImage.transform.position = leftMostScreenPosition;
            playerPhaseTransitionImage.SetActive(false);
        } else if (phase == "EnemyPhase") {
            yield return new WaitForSeconds(1);
            Vector2 rightMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2 (Camera.main.pixelWidth, Camera.main.pixelHeight/2));
            enemyPhaseTransitionImage.SetActive(true);
            Vector2 leftMostScreenPosition = enemyPhaseTransitionImage.transform.position;

            for(float t = 0.0f; t < 1.0f; t+=0.1f) {
                enemyPhaseTransitionImage.GetComponent<ImageTransitions>().TranslateAcrossScreen(leftMostScreenPosition, rightMostScreenPosition, t);

                if (t == 0.5f) {
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.05f);
            }
            enemyPhaseTransitionImage.transform.position = leftMostScreenPosition;
            enemyPhaseTransitionImage.SetActive(false);
        }
        isPhaseTransitionRunning = false;
    }

    void onAttackButtonClick() {
        print("Attack Button was clicked!");
        closePlayerBattleMenu();
        gameMaster.preProcessAttackState();
        gameMaster.ChangeState(GameState.AttackState);
    }

    void onItemButtonClick() {
        print("Item Button was clicked! TODO: Implement items");
        openPlayerItemMenu();
        gameMaster.ChangeState(GameState.ShowItemMenuState);
    }

    void onWaitButtonClick() {
        print("Wait Button was clicked!");
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.TurnEndState);
    }

    void moveCanvasToGlobalPoint(GameObject go, Vector3 globalPos) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(globalPos);
        Vector2 movePos;
        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(go.transform as RectTransform, screenPos, Camera.main, out movePos);
        //Convert the local point to world point
        go.transform.position = new Vector3(movePos.x, movePos.y, 0);      
    }

    public void closePlayerItemMenu() {
        //make sure to destroy all children in the item slot container
        GameObject itemSlotContainer = GameObject.Find("ItemSlotContainer").gameObject;
        foreach (Transform child in itemSlotContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }
        itemMenu.SetActive(false);
        itemMenuDisplayed = false;
    }

    public void closePlayerBattleMenu() {
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }

    public void closeUnitInfoMenu() {
        unitInfoMenu.SetActive(false);
    }

    void onConsumableItemButtonClick() {
        print("Button clicked!");
        GameObject itemRowButton = EventSystem.current.currentSelectedGameObject;
        string itemKey = itemRowButton.transform.GetChild(1).gameObject.GetComponent<Text>().text;
        PlayerInfo clickedPlayerInfo = gameMaster.clickedPlayerNode.getPlayerInfo();
        clickedPlayerInfo.ConsumeItem(itemKey);
        closePlayerItemMenu();
        closePlayerBattleMenu();
        gameMaster.ChangeState(GameState.TurnEndState);
    }

    public bool IsBattleEventScreenDisplayed() {
        return battleEventScreenDisplayed;
    }

    public bool IsPlayerBattleMenuDisplayed() {
        return playerBattleMenuDisplayed;
    }

    public bool IsPhaseTransitionRunning() {
        return isPhaseTransitionRunning;
    }

    public bool HaveAllEnemiesHaveMoved() {
        return allEnemiesHaveMoved;
    }

    public void SetAllEnemiesHaveMoved(bool haveMoved) {
        allEnemiesHaveMoved = haveMoved;
    }

    public bool IsItemMenuDisplayed() {
        return itemMenuDisplayed;
    }
}
