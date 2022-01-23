using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitManagementMenu : MonoBehaviour {

    private GameObject unitManagementMenu;
    private GameObject mainPanelSelectScreen;
    private GameObject playerSelectScreen;
    private PlayerItemTradeMenu itemTradeMenu;
    private GameObject playerStatsScreen;
    MasterGameStateController gameStateInstance;
    private string selectedPlayerSlotOne = "";
    private string selectedPlayerSlotTwo = "";
    private string selectionMode = "Single";
    private string currentState = "";

    private Button tradeButton;
    private Button inventoryButton;
    private Button browseUnitsButton;
    private Button equipButton;

    void Awake() {
        setupUIElements();
    }

    void Start() {
        gameStateInstance = GameObject.Find("GameMasterInstance").GetComponent<MasterGameStateController>().instance;
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            CloseUnitManagementMenu();
        }
    }

    private void setupUIElements() {
        unitManagementMenu = GameObject.Find("UnitManagementMenu").gameObject;
        GameObject playerTradeMenuGo = unitManagementMenu.transform.Find("PlayerItemTradeMenu").gameObject;
        itemTradeMenu = playerTradeMenuGo.AddComponent<PlayerItemTradeMenu>(); 
        mainPanelSelectScreen = unitManagementMenu.transform.Find("UnitManagementPanel/MainPanelSelectScreen").gameObject;
        playerSelectScreen = unitManagementMenu.transform.Find("PlayerSelectScreen").gameObject;
        playerStatsScreen = unitManagementMenu.transform.Find("PlayerStatsScreen").gameObject;
        tradeButton = mainPanelSelectScreen.transform.GetChild(2).gameObject.GetComponent<Button>();
        inventoryButton = mainPanelSelectScreen.transform.GetChild(1).gameObject.GetComponent<Button>();
        browseUnitsButton = mainPanelSelectScreen.transform.GetChild(0).gameObject.GetComponent<Button>();
        equipButton = mainPanelSelectScreen.transform.GetChild(3).gameObject.GetComponent<Button>();
        tradeButton.onClick.AddListener(onTradeButtonClick);
        tradeButton.GetComponent<SlotAdditionalProperties>().AddProperty("Selected", "images/ui/RegularButton");
        tradeButton.GetComponent<SlotAdditionalProperties>().AddProperty("Deselected", "images/ui/MainPanel");
        inventoryButton.onClick.AddListener(onInventoryButtonClick);
        inventoryButton.GetComponent<SlotAdditionalProperties>().AddProperty("Selected", "images/ui/RegularButton");
        inventoryButton.GetComponent<SlotAdditionalProperties>().AddProperty("Deselected", "images/ui/MainPanel");
        browseUnitsButton.onClick.AddListener(onBrowseUnitsButtonClick);
        browseUnitsButton.GetComponent<SlotAdditionalProperties>().AddProperty("Selected", "images/ui/RegularButton");
        browseUnitsButton.GetComponent<SlotAdditionalProperties>().AddProperty("Deselected", "images/ui/MainPanel"); 
        equipButton.onClick.AddListener(onEquipButtonClick);
        equipButton.GetComponent<SlotAdditionalProperties>().AddProperty("Selected", "images/ui/RegularButton");
        equipButton.GetComponent<SlotAdditionalProperties>().AddProperty("Deselected", "images/ui/MainPanel");       
        playerStatsScreen.SetActive(false);
        mainPanelSelectScreen.SetActive(false);
        playerSelectScreen.SetActive(false);        
        unitManagementMenu.SetActive(false);
    }

    public void OpenUnitManagementMenu() {
        unitManagementMenu.SetActive(true); 
        mainPanelSelectScreen.SetActive(true); 
        playerSelectScreen.SetActive(true);
        populatePlayerSelectScreen(gameStateInstance.GetAllPlayerInfos());        
    }

    private void populatePlayerSelectScreen(List<PlayerInfo> players) {
        float xPos = -5.0f; //move by 2 each time max 6 per row
        float yPos = 1.0f; //move down by 2 for each row
        int rowCount = 0;
        foreach (PlayerInfo player in players) {
            GameObject playerSlot = playerSelectScreen.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            Image image = playerSlot.transform.Find("Image").GetComponent<Image>();
            SpriteRenderer baseSpriteRenderer = playerSlot.transform.Find("BaseSprite").GetComponent<SpriteRenderer>();
            TextMeshProUGUI playerName = playerSlot.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
            Button slotButton = playerSlot.transform.Find("Button").GetComponent<Button>();
            SlotAdditionalProperties properties = slotButton.GetComponent<SlotAdditionalProperties>();
            properties.AddProperty("Selected", "images/ui/RegularButton");
            properties.AddProperty("Deselected", "images/ui/MainPanel");
            slotButton.onClick.AddListener(onPlayerSlotButtonClick);
            playerSlot.name = player.id;
            image.sprite = PlayerInfoDatabase.GetPlayerSpriteFromPath(player.portraitRefPath);
            baseSpriteRenderer.sprite = PlayerInfoDatabase.GetPlayerSpriteFromPath(player.baseSpritePath);
            playerName.text = player.name;
            Vector3 playerSlotPos = new Vector3(xPos, yPos, 1.0f);
            playerSlot.transform.SetParent(playerSelectScreen.transform, true);
            playerSlot.transform.localPosition = playerSlotPos;
            rowCount++;
            if (rowCount % 6 == 0 && rowCount != 0) {
                xPos += -5.0f;    
                yPos += 2.0f;
            } else {
                xPos += 2.0f;
            }
        }
    }

    private void onPlayerSlotButtonClick() {
        string selectedSlotId = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        if (selectionMode.Equals("Single")) {
            if (!(selectedPlayerSlotOne.Equals(""))) {
                if (selectedPlayerSlotOne.Equals(selectedSlotId)) {
                    deselectSlotWithId(selectedSlotId);
                    selectedPlayerSlotOne = "";
                } else {
                    deselectSlotWithId(selectedPlayerSlotOne);
                    selectedPlayerSlotOne = selectedSlotId; 
                    selectSlotWithId(selectedSlotId);                      
                }
            } else {
                selectedPlayerSlotOne = selectedSlotId;   
                selectSlotWithId(selectedSlotId);               
            }
        } else if (selectionMode.Equals("Double")) {
            //case two buttons selected
            if (!selectedPlayerSlotOne.Equals("") && !selectedPlayerSlotTwo.Equals("")) {
                if (selectedPlayerSlotOne.Equals(selectedSlotId)) {
                    deselectSlotWithId(selectedPlayerSlotOne);
                    selectedPlayerSlotOne = "";
                } else if (selectedPlayerSlotTwo.Equals(selectedSlotId)) {
                    deselectSlotWithId(selectedPlayerSlotTwo);
                    selectedPlayerSlotTwo = "";
                } else {
                    deselectSlotWithId(selectedPlayerSlotOne);
                    selectedPlayerSlotOne = selectedSlotId;
                    selectSlotWithId(selectedSlotId);
                }
            } else if (selectedPlayerSlotOne.Equals(selectedSlotId)) { //case button already selected
                    deselectSlotWithId(selectedSlotId);
                    selectedPlayerSlotOne = "";
            } else if (selectedPlayerSlotTwo.Equals(selectedSlotId)) {
                    deselectSlotWithId(selectedSlotId);
                    selectedPlayerSlotTwo = "";
            } else if (selectedPlayerSlotOne.Equals("")) { //case one button selected
                selectedPlayerSlotOne = selectedSlotId;
                selectSlotWithId(selectedPlayerSlotOne);
            } else if (selectedPlayerSlotTwo.Equals("")) { 
                selectedPlayerSlotTwo = selectedSlotId; 
                selectSlotWithId(selectedPlayerSlotTwo);
            }            
        }
        if (currentState.Equals("BROWSE_UNITS")) {
            updatePlayerStatsScreen();
        } else {
            PlayerInfo playerInfoLeft = selectedPlayerSlotOne.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotOne);
            PlayerInfo playerInfoRight = selectedPlayerSlotTwo.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotTwo);
            itemTradeMenu.UpdateContent(playerInfoLeft, playerInfoRight);            
        }
    }

    private void deselectSlotWithId(string playerSlotId) {
        if (playerSlotId != "") {
            Button buttonToDeselect = playerSelectScreen.transform.Find(playerSlotId + "/Button").gameObject.GetComponent<Button>();
            deSelectButton(buttonToDeselect);          
        }
    }

    private void selectSlotWithId(string playerSlotId) {
        Button buttonToSelect = playerSelectScreen.transform.Find(playerSlotId + "/Button").gameObject.GetComponent<Button>();
        selectButton(buttonToSelect);
    }

    void selectButton(Button button) {
        print("Selecting button");
        //EventSystem.current.SetSelectedGameObject(null);
        string newButtonImagePath = button.GetComponent<SlotAdditionalProperties>().GetProperty("Selected");
        print(Resources.Load<Sprite>(newButtonImagePath));
        button.image.sprite = Resources.Load<Sprite>(newButtonImagePath);
    }

    void deSelectButton(Button button) {
        //EventSystem.current.SetSelectedGameObject(null);
        string newButtonImagePath = button.GetComponent<SlotAdditionalProperties>().GetProperty("Deselected");
        button.image.sprite = Resources.Load<Sprite>(newButtonImagePath);        
    }

    private void onBrowseUnitsButtonClick() {
        itemTradeMenu.ClosePlayerItemTradeMenu();
        currentState = "BROWSE_UNITS";
        selectionMode = "Single";
        Button browseUnitsButton = mainPanelSelectScreen.transform.GetChild(0).gameObject.GetComponent<Button>();
        selectButton(browseUnitsButton);
        deSelectButton(tradeButton);
        deSelectButton(inventoryButton);
        deSelectButton(equipButton);
        deselectSlotWithId(selectedPlayerSlotTwo);
        selectedPlayerSlotTwo = "";
        playerStatsScreen.SetActive(true);
        updatePlayerStatsScreen();
    }

    private void onTradeButtonClick() {
        selectionMode = "Double";
        currentState = "TRADE";
        closePlayerStatsScreen();
        selectButton(tradeButton);
        deSelectButton(browseUnitsButton);
        deSelectButton(inventoryButton);
        deSelectButton(equipButton);
        PlayerInfo playerInfoLeft = selectedPlayerSlotOne.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotOne);
        PlayerInfo playerInfoRight = selectedPlayerSlotTwo.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotTwo);
        itemTradeMenu.OpenPlayerItemTradeMenu("Trade", playerInfoLeft, playerInfoRight);
    }

    private void onInventoryButtonClick() {
        selectionMode = "Single";
        currentState = "INVENTORY";
        closePlayerStatsScreen();
        selectButton(inventoryButton);
        deSelectButton(tradeButton);
        deSelectButton(browseUnitsButton);
        deSelectButton(equipButton);
        deselectSlotWithId(selectedPlayerSlotTwo);
        selectedPlayerSlotTwo = "";
        PlayerInfo playerInfoLeft = selectedPlayerSlotOne.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotOne);
        PlayerInfo playerInfoRight = selectedPlayerSlotTwo.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotTwo);  
        itemTradeMenu.OpenPlayerItemTradeMenu("Inventory", playerInfoLeft, playerInfoRight);
    }
    
    private void onEquipButtonClick() {
        selectionMode = "Single";
        currentState = "EQUIP";
        closePlayerStatsScreen();
        selectButton(equipButton);
        deSelectButton(tradeButton);
        deSelectButton(inventoryButton);
        deSelectButton(browseUnitsButton);
        deselectSlotWithId(selectedPlayerSlotTwo);
        selectedPlayerSlotTwo = "";
        PlayerInfo playerInfoLeft = selectedPlayerSlotOne.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotOne);
        PlayerInfo playerInfoRight = selectedPlayerSlotTwo.Equals("") ? null : gameStateInstance.GetPlayerInfoById(selectedPlayerSlotTwo);        
        itemTradeMenu.OpenPlayerItemTradeMenu("Equip", playerInfoLeft, playerInfoRight);
    }
    
    private void updatePlayerStatsScreen() {
        if (!selectedPlayerSlotOne.Equals("")) {
            populatePlayerStatsScreen();
        } else {
            clearPlayerStatsScreen();
        }
    }

    private void populatePlayerStatsScreen() {
        PlayerInfo player = gameStateInstance.GetPlayerInfoById(selectedPlayerSlotOne);
        Image icon = playerStatsScreen.transform.Find("PlayerIcon").GetComponent<Image>();
        TextMeshProUGUI level = playerStatsScreen.transform.Find("Level").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI exp = playerStatsScreen.transform.Find("Exp").GetComponent<TextMeshProUGUI>();
        GameObject stats = playerStatsScreen.transform.Find("BaseStatsBackdrop").gameObject;
        icon.sprite = PlayerInfoDatabase.GetPlayerSpriteFromPath(player.portraitRefPath);
        level.text = "LVL " + player.level.ToString();
        exp.text = "EXP " + player.totalExperience.ToString();
        Dictionary<string, int> playerBaseStats =  new Dictionary<string, int>() {
            {"HP", player.baseHealth},
            {"ATK", player.baseAttack},
            {"DEF", player.baseDefense},
            {"MATK", player.baseMagicAttack},
            {"MDEF", player.baseMagicDefense},
            {"DEX", player.baseDexterity},
            {"LUK", player.baseLuck},
            {"MOV", player.baseMov}
        };
        int childToChange = 0;
        foreach (string key in playerBaseStats.Keys) {
            TextMeshProUGUI stat = stats.transform.GetChild(childToChange).gameObject.GetComponent<TextMeshProUGUI>();
            stat.text = key + " " + playerBaseStats[key].ToString();
            childToChange++;
        }            
    }

    private void clearPlayerStatsScreen() {
        Image icon = playerStatsScreen.transform.Find("PlayerIcon").GetComponent<Image>();
        TextMeshProUGUI level = playerStatsScreen.transform.Find("Level").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI exp = playerStatsScreen.transform.Find("Exp").GetComponent<TextMeshProUGUI>();
        GameObject stats = playerStatsScreen.transform.Find("BaseStatsBackdrop").gameObject;
        icon.sprite = null;
        level.text = "LVL ";
        exp.text = "EXP ";
        List<string> playerBaseKeys = new List<string>() {"HP", "ATK", "DEF", "MATK", "MDEF", "DEX", "LUK", "MOV"};
        int childToChange = 0;
        foreach (string key in playerBaseKeys) {
            TextMeshProUGUI stat = stats.transform.GetChild(childToChange).gameObject.GetComponent<TextMeshProUGUI>();
            stat.text = key;
            childToChange++;
        }        
    }

    private void closePlayerStatsScreen() {
        if (playerStatsScreen.activeSelf) {
            clearPlayerStatsScreen();
            playerStatsScreen.SetActive(false);            
        }
    }

    private void clearPlayerSelectScreen() {
        foreach (Transform transform in playerSelectScreen.transform) {
            if (!transform.gameObject.name.Equals("Backdrop")) {
                Destroy(transform.gameObject); 
            }     
        }
    }

    public void CloseUnitManagementMenu() {
        closePlayerStatsScreen();
        clearPlayerSelectScreen();
        deselectSlotWithId(selectedPlayerSlotOne);
        deselectSlotWithId(selectedPlayerSlotTwo);
        selectedPlayerSlotOne = "";
        selectedPlayerSlotTwo = "";
        selectionMode = "Single";
        currentState = "";
        itemTradeMenu.ClosePlayerItemTradeMenu();
        playerSelectScreen.SetActive(false);
        unitManagementMenu.SetActive(false);
    }

}
