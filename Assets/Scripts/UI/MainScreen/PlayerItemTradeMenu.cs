using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerItemTradeMenu : MonoBehaviour {
    
    private GameObject unitManagementMenu;
    private GameObject playerItemTradeMenu;
    private GameObject tradePortraitPlayerOne;
    private GameObject tradePortraitPlayerTwo;
    private GameObject leftSlotContent;
    private GameObject rightSlotContent;
    private Button buttonMoveToRight;
    private Button buttonMoveToLeft;
    private Button buttonSwapTwoItems;
    private string mode = "";
    private int leftSlotIdxSelected = -1;
    private int rightSlotIdxSelected = -1;
    private PlayerInfo playerInfoRightSlot;
    private PlayerInfo playerInfoLeftSlot;
    MasterGameStateController gameStateInstance;


    void Awake() {
        setupUIElements();
    }

    void Start() {
        gameStateInstance = GameObject.Find("GameMasterInstance").GetComponent<MasterGameStateController>().instance;
    }

    private void setupUIElements() {
        unitManagementMenu = GameObject.Find("UnitManagementMenu").gameObject;
        playerItemTradeMenu = unitManagementMenu.transform.Find("PlayerItemTradeMenu").gameObject;
        tradePortraitPlayerOne = playerItemTradeMenu.transform.Find("TradePortraits/LeftPortrait").gameObject;
        tradePortraitPlayerTwo = playerItemTradeMenu.transform.Find("TradePortraits/RightPortrait").gameObject;
        leftSlotContent = playerItemTradeMenu.transform.Find("LeftSlot/PlayerInventoryScrollView/PlayerInventoryViewport/Content").gameObject;
        rightSlotContent = playerItemTradeMenu.transform.Find("RightSlot/PlayerInventoryScrollView/PlayerInventoryViewport/Content").gameObject;
        buttonMoveToRight = playerItemTradeMenu.transform.Find("SwapRightButton").GetComponent<Button>();
        buttonMoveToRight.onClick.AddListener(onButtonMoveToRightClick);
        buttonMoveToLeft = playerItemTradeMenu.transform.Find("SwapLeftButton").GetComponent<Button>();
        buttonMoveToLeft.onClick.AddListener(onButtonMoveToLeftClick);
        buttonSwapTwoItems = playerItemTradeMenu.transform.Find("SwapBothButton").GetComponent<Button>();
        buttonSwapTwoItems.onClick.AddListener(onButtonSwapTwoItemsClick);        
        unitManagementMenu.SetActive(false);
    }

    public void OpenPlayerItemTradeMenu(string modeType, PlayerInfo playerInfoLeft, PlayerInfo playerInfoRight) {
        mode = modeType;
        playerItemTradeMenu.SetActive(true);
        buttonSwapTwoItems.gameObject.SetActive(mode.Equals("Trade"));
        playerInfoLeftSlot = playerInfoLeft;
        playerInfoRightSlot = playerInfoRight;
        populateImage(playerInfoLeft, playerInfoRight);
        populateContent(playerInfoLeft, playerInfoRight);
    }

    private void populateImage(PlayerInfo playerInfoLeft, PlayerInfo playerInfoRight) {
        if (playerInfoLeft != null) {
            tradePortraitPlayerOne.GetComponent<Image>().sprite = PlayerInfoDatabase.GetPlayerSpriteFromPath(playerInfoLeft.portraitRefPath);
        } else {
            tradePortraitPlayerOne.GetComponent<Image>().sprite = null;
        }

        if (playerInfoRight != null) {
            tradePortraitPlayerTwo.GetComponent<Image>().sprite = PlayerInfoDatabase.GetPlayerSpriteFromPath(playerInfoRight.portraitRefPath);
        } else {
            tradePortraitPlayerTwo.GetComponent<Image>().sprite = null;
        }
    }

    private void populateContent(PlayerInfo playerInfoLeft, PlayerInfo playerInfoRight) {
        clearContent(leftSlotContent);
        clearContent(rightSlotContent);
        populateImage(playerInfoLeft, playerInfoRight);
        if (mode.Equals("Inventory")) {
            if (playerInfoLeft != null) {
                ConsumableItemManager playerItemManager = playerInfoLeft.GetConsumableItemManager();
                //fill for player
                addPlayerConsumableItemToContent(leftSlotContent, playerItemManager, true);                
            }
            List<ConsumableItem> itemInventory = gameStateInstance.GetAllConsumableItems();
            //now fill for inventory
            addInventoryConsumableItemToContent(rightSlotContent, itemInventory, false);              
        } else if (mode.Equals("Equip")) {            
            if (playerInfoLeft != null) {
                EquipmentItemManager playerEquipItemManager = playerInfoLeft.GetEquipmentItemManager();
                addPlayerEquipmentItemToContent(leftSlotContent, playerEquipItemManager, true);                   
            }
            List<EquipmentItem> itemInventory = gameStateInstance.GetAllEquipmentItems();
            addInventoryEquipmentItemToContent(rightSlotContent, itemInventory, false);
        } else if (mode.Equals("Trade")) {
            if (playerInfoLeft != null) {
                ConsumableItemManager leftPlayerItemManager = playerInfoLeft.GetConsumableItemManager();
                addPlayerConsumableItemToContent(leftSlotContent, leftPlayerItemManager, true);
            }

            if (playerInfoRight != null) {
                ConsumableItemManager rightPlayerItemManager = playerInfoRight.GetConsumableItemManager();
                addPlayerConsumableItemToContent(rightSlotContent, rightPlayerItemManager, false);  
            }
        }
    }

    public void addPlayerConsumableItemToContent(GameObject content, ConsumableItemManager playerItemManager, bool isLeftPanel) {
        int itemCount = 0;
        foreach (KeyValuePair<string, int> pair in playerItemManager.GetCurrentConsumableItemsInventory()) {
            ConsumableItem item = playerItemManager.GetConsumableItem(pair.Key);
            GameObject itemSlot = playerItemTradeMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemSlot.name = itemCount.ToString();
            int maxItemQuantity = playerItemManager.GetMaxItemsPerSlot();
            int currentItemQuantity = pair.Value;
            print(pair.Key + " QUANTITY: " + currentItemQuantity.ToString());
            Dictionary<string,string> propertiesDict = new Dictionary<string, string>() {
                {"ItemQuantity", currentItemQuantity.ToString()},
                {"Selected", "images/ui/RegularButton"},
                {"Deselected", "images/ui/MainPanel"} 
            };
            fillItemSlot(content, itemSlot, item, propertiesDict, maxItemQuantity, currentItemQuantity, isLeftPanel);
            itemCount++;
        }

        //fill player empty slots if needed
        int emptySlots = playerItemManager.GetMaxSlots() - itemCount;
        for (int i = 0; i < emptySlots; i++) {
            GameObject itemSlot = playerItemTradeMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemSlot.name = itemCount.ToString();
            Set_Size_Scale(itemSlot, 4.0f, 0.85f, 0.0275f, 0.0275f);
            itemSlot.transform.SetParent(content.transform);
            itemCount++;
        }
    }

    public void addInventoryConsumableItemToContent(GameObject content, List<ConsumableItem> globalInventory, bool isLeftPanel) {
        int itemCount = 0;
        foreach(ConsumableItem item in globalInventory) {
            //int itemQty = 1;
            //ConsumableItem item = playerItemManager.GetConsumableItem(pair.Key);
            GameObject itemSlot = playerItemTradeMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemSlot.name = itemCount.ToString();
            Dictionary<string,string> propertiesDict = new Dictionary<string, string>() {
                {"Selected", "images/ui/RegularButton"},
                {"Deselected", "images/ui/MainPanel"} 
            };
            fillTransactionItemSlot(content, itemSlot, item, propertiesDict, isLeftPanel);
            itemCount++;
        }
    }

    public void addPlayerEquipmentItemToContent(GameObject content, EquipmentItemManager equipmentItemManager, bool isLeftPanel) {
        int itemCount = 0;
        foreach (EquipmentItem item in equipmentItemManager.GetCurrentEquipment()) {
            if (item != null) {
                GameObject itemSlot = playerItemTradeMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
                itemSlot.name = itemCount.ToString();
                Dictionary<string,string> propertiesDict = new Dictionary<string, string>() {
                    {"EquipType", ((int) (item.GetEquipType())).ToString()},
                    {"Selected", "images/ui/RegularButton"},
                    {"Deselected", "images/ui/MainPanel"} 
                };
                fillTransactionItemSlot(content, itemSlot, item, propertiesDict, isLeftPanel);
                itemCount++;                
            }
        }
        //fill player empty slots if needed
        int emptySlots = equipmentItemManager.GetMaxSlots() - itemCount;
        for (int i = 0; i < emptySlots; i++) {
            GameObject itemSlot = playerItemTradeMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemSlot.name = itemCount.ToString();
            Dictionary<string,string> propertiesDict = new Dictionary<string, string>() {
                {"Selected", "images/ui/RegularButton"},
                {"Deselected", "images/ui/MainPanel"} 
            };
            GameObject itemButton = itemSlot.transform.Find("Button").gameObject;
            Button itemButtonComp = itemButton.GetComponent<Button>();
            if (isLeftPanel) {
                itemButtonComp.onClick.AddListener(onItemClickLeftPanel);
            } else {
                itemButtonComp.onClick.AddListener(onItemClickRightPanel);
            }
            Set_Size_Scale(itemSlot, 4.0f, 0.85f, 0.0275f, 0.0275f);
            itemSlot.transform.SetParent(content.transform);
            itemCount++;
        }
    }

    public void addInventoryEquipmentItemToContent(GameObject content, List<EquipmentItem> equipmentItemInventory, bool isLeftPanel) {
        int itemCount = 0;
        foreach(EquipmentItem item in equipmentItemInventory) {
            GameObject itemSlot = playerItemTradeMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemSlot.name = itemCount.ToString();
            Dictionary<string,string> propertiesDict = new Dictionary<string, string>() {
                {"EquipType", ((int) (item.GetEquipType())).ToString()},
                {"Selected", "images/ui/RegularButton"},
                {"Deselected", "images/ui/MainPanel"} 
            };
            fillTransactionItemSlot(content, itemSlot, item, propertiesDict, isLeftPanel);
            itemCount++;            
        }
    }

    public void fillItemSlot(GameObject content, GameObject itemSlot, Item item, Dictionary<string,string> propertiesDict, int maxItemQuantity, int currentItemQuantity, bool isLeftPanel) {
        Set_Size_Scale(itemSlot, 4.0f, 0.85f, 0.0275f, 0.0275f);
        GameObject itemButton = itemSlot.transform.Find("Button").gameObject;
        Button itemButtonComp = itemButton.GetComponent<Button>();
        if (isLeftPanel) {
            itemButtonComp.onClick.AddListener(onItemClickLeftPanel);
        } else {
            itemButtonComp.onClick.AddListener(onItemClickRightPanel);
        }
        Image itemImage = itemButton.transform.Find("Image").GetComponent<Image>();
        Text itemName = itemButton.transform.Find("ItemName").GetComponent<Text>();
        Text itemQuantity = itemButton.transform.Find("Quantity").GetComponent<Text>();
        itemImage.sprite = item.GetSpriteFromSpritePath();
        itemName.text = item.itemName;
        itemQuantity.text = System.String.Format("{0}/{1}", currentItemQuantity.ToString(), maxItemQuantity.ToString());
        SlotAdditionalProperties properties = itemButton.AddComponent<SlotAdditionalProperties>();
        foreach (KeyValuePair<string,string> pair in propertiesDict) {
            properties.AddProperty(pair.Key, pair.Value);
        }
        itemSlot.transform.SetParent(content.transform);
    }

    public void fillTransactionItemSlot(GameObject content, GameObject itemSlot, Item item, Dictionary<string, string> propertiesDict, bool isLeftPanel) {
        
        Set_Size_Scale(itemSlot, 4.0f, 0.85f, 0.0275f, 0.0275f);
        GameObject itemButton = itemSlot.transform.Find("Button").gameObject;
        Button itemButtonComp = itemButton.GetComponent<Button>();
        if (isLeftPanel) {
            itemButtonComp.onClick.AddListener(onItemClickLeftPanel);
        } else {
            itemButtonComp.onClick.AddListener(onItemClickRightPanel);
        }
        Image itemImage = itemButton.transform.Find("Image").GetComponent<Image>();
        Text itemName = itemButton.transform.Find("ItemName").GetComponent<Text>();
        Text itemQuantity = itemButton.transform.Find("Quantity").GetComponent<Text>();

        if (item == null) {
            itemImage.sprite = null;
            itemName.text = "----";
            itemQuantity.text = "";
        } else {
            itemImage.sprite = item.GetSpriteFromSpritePath();
            itemName.text = item.itemName;
            itemQuantity.text = "";            
        }

        SlotAdditionalProperties properties = itemButton.AddComponent<SlotAdditionalProperties>();
        foreach (KeyValuePair<string,string> pair in propertiesDict) {
            properties.AddProperty(pair.Key, pair.Value);
        }
        itemSlot.transform.SetParent(content.transform, true); 
    }

    public void UpdateContent(PlayerInfo playerInfoLeft, PlayerInfo playerInfoRight) {
        playerInfoLeftSlot = playerInfoLeft;
        playerInfoRightSlot = playerInfoRight;
        populateContent(playerInfoLeftSlot, playerInfoRightSlot);
    }

    private void clearContent(GameObject contentSlot) {
        foreach (Transform child in contentSlot.transform) {
            Destroy(child.gameObject);
        }
    }

    private void onItemClickLeftPanel() {
        int newSlotSelectedIdx = System.Int32.Parse(EventSystem.current.currentSelectedGameObject.transform.parent.name);
        Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (leftSlotIdxSelected != -1) {
            //deselect button
            deselectButtonAtIdx(leftSlotIdxSelected, leftSlotContent);
        }
        leftSlotIdxSelected = newSlotSelectedIdx;
        selectButton(selectedButton);
    }

    private void onItemClickRightPanel() {
        int newSlotSelectedIdx = System.Int32.Parse(EventSystem.current.currentSelectedGameObject.transform.parent.name);
        Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (rightSlotIdxSelected != -1) {
            //deselect button
            deselectButtonAtIdx(rightSlotIdxSelected, rightSlotContent);
        }
        rightSlotIdxSelected = newSlotSelectedIdx;
        selectButton(selectedButton);
    }

    private void selectButton(Button selectedButton) {
        string newButtonImagePath = selectedButton.GetComponent<SlotAdditionalProperties>().GetProperty("Selected");
        selectedButton.image.sprite = Resources.Load<Sprite>(newButtonImagePath);
    }

    private void deselectButtonAtIdx(int idx, GameObject slotContent) {
        Button buttonToDeselect = slotContent.transform.GetChild(idx).gameObject.transform.Find("Button").gameObject.GetComponent<Button>();
        deselectButton(buttonToDeselect);
    }

    private Button getButtonAtIdx(int idx, GameObject slotContent) {
        Button selectedButton = slotContent.transform.GetChild(idx).gameObject.transform.Find("Button").gameObject.GetComponent<Button>();
        return selectedButton;
    }

    private void deselectButton(Button buttonToDeselect) {
        string newButtonImagePath = buttonToDeselect.GetComponent<SlotAdditionalProperties>().GetProperty("Deselected");
        buttonToDeselect.image.sprite = Resources.Load<Sprite>(newButtonImagePath);        
    }

    private void onButtonMoveToRightClick() {
        if (leftSlotIdxSelected != -1) {
            if (mode.Equals("Inventory")) { //move from player to inventory
                string itemName = getButtonAtIdx(leftSlotIdxSelected, leftSlotContent).transform.Find("ItemName").GetComponent<Text>().text;
                ConsumableItemManager itemManager = playerInfoLeftSlot.GetConsumableItemManager();
                ConsumableItem itemToMove = itemManager.GetConsumableItem(itemName);
                itemManager.ConsumeItem(itemToMove);
                gameStateInstance.AddItemToInventory(itemToMove);
                gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                refreshScreen();
            } else if (mode.Equals("Equip")) { //unequip from player move to inventory
                int equipType = System.Int32.Parse(getButtonAtIdx(leftSlotIdxSelected, leftSlotContent).gameObject.GetComponent<SlotAdditionalProperties>().GetProperty("EquipType"));
                EquipmentItemManager equipManager = playerInfoLeftSlot.GetEquipmentItemManager();
                EquipmentItem equipItemToMove = equipManager.GetCurrentEquipment()[equipType];
                print("Unequipping: " + equipItemToMove.itemName);
                playerInfoLeftSlot.UnEquipItem(equipItemToMove);
                gameStateInstance.AddItemToInventory(equipItemToMove);
                gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                refreshScreen();
            } else if (mode.Equals("Trade")) { //move from player left to right
                string itemName = getButtonAtIdx(leftSlotIdxSelected, leftSlotContent).transform.Find("ItemName").GetComponent<Text>().text;
                ConsumableItemManager itemManagerLeftPI = playerInfoLeftSlot.GetConsumableItemManager();
                ConsumableItemManager itemManagerRightPI = playerInfoRightSlot.GetConsumableItemManager();
                ConsumableItem itemToTrade = itemManagerLeftPI.GetConsumableItem(itemName);
                int itemQty = itemManagerLeftPI.GetQuantityOfItemInInventory(itemName);
                if (itemManagerRightPI.CanAddItem(itemToTrade, itemQty)) {
                    itemManagerRightPI.AddItemWithQuantity(itemToTrade, itemQty);
                    itemManagerLeftPI.RemoveItem(itemToTrade);
                    gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                    gameStateInstance.UpdatePlayerInfo(playerInfoRightSlot);
                    refreshScreen();                    
                }
            }            
        }
    }

    private void onButtonMoveToLeftClick() {
        if (rightSlotIdxSelected != -1) {
            if (mode.Equals("Inventory")) { //move from inventory to player
                string itemName = getButtonAtIdx(rightSlotIdxSelected, rightSlotContent).transform.Find("ItemName").GetComponent<Text>().text;
                ConsumableItemManager itemManager = playerInfoLeftSlot.GetConsumableItemManager();  
                ConsumableItem itemToMove = ItemsDatabase.GetConsumableItemByName(itemName);
                int qty = 1;
                if (itemManager.CanAddItem(itemToMove, qty)) {
                    gameStateInstance.RemoveItemFromInventory(itemToMove);
                    itemManager.AddItemWithQuantity(itemToMove, qty);
                    gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                    refreshScreen();
                }             
            } else if (mode.Equals("Equip")) { //equip to player from inventory
                string itemName = getButtonAtIdx(rightSlotIdxSelected, rightSlotContent).transform.Find("ItemName").GetComponent<Text>().text;
                EquipmentItem itemToEquip = ItemsDatabase.GetEquipmentItemByName(itemName);
                int equipType = (int) itemToEquip.GetEquipType();
                EquipmentItemManager itemManager = playerInfoLeftSlot.GetEquipmentItemManager();
                if (itemManager.CanEquipItem(itemToEquip)) {
                    gameStateInstance.RemoveItemFromInventory(itemToEquip);
                    playerInfoLeftSlot.EquipItem(itemToEquip);
                    gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                    refreshScreen();
                }
            } else if (mode.Equals("Trade")) { //right player to left player
                string itemName = getButtonAtIdx(rightSlotIdxSelected, rightSlotContent).transform.Find("ItemName").GetComponent<Text>().text;
                ConsumableItemManager itemManagerLeftPI = playerInfoLeftSlot.GetConsumableItemManager();
                ConsumableItemManager itemManagerRightPI = playerInfoRightSlot.GetConsumableItemManager();
                ConsumableItem itemToTrade = itemManagerRightPI.GetConsumableItem(itemName);
                int itemQty = itemManagerRightPI.GetQuantityOfItemInInventory(itemName);
                if (itemManagerLeftPI.CanAddItem(itemToTrade, itemQty)) {
                    itemManagerLeftPI.AddItemWithQuantity(itemToTrade, itemQty);
                    itemManagerRightPI.RemoveItem(itemToTrade);
                    gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                    gameStateInstance.UpdatePlayerInfo(playerInfoRightSlot);
                    refreshScreen();                    
                }
            }
        }
    }

    private void onButtonSwapTwoItemsClick() {
        if (leftSlotIdxSelected != -1 && rightSlotIdxSelected != -1) {
            if (mode.Equals("Trade")) {
                string itemNameLeft = getButtonAtIdx(leftSlotIdxSelected, leftSlotContent).gameObject.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
                string itemNameRight = getButtonAtIdx(rightSlotIdxSelected, rightSlotContent).gameObject.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
                ConsumableItemManager itemManagerLeftPI = playerInfoLeftSlot.GetConsumableItemManager();
                ConsumableItemManager itemManagerRightPI = playerInfoRightSlot.GetConsumableItemManager();
                ConsumableItem rightItemToTrade = itemManagerRightPI.GetConsumableItem(itemNameRight);
                ConsumableItem leftItemToTrade = itemManagerLeftPI.GetConsumableItem(itemNameLeft);
                int itemQtyRight = itemManagerRightPI.GetQuantityOfItemInInventory(rightItemToTrade);
                int itemQtyLeft = itemManagerLeftPI.GetQuantityOfItemInInventory(leftItemToTrade);
                //temporarily remove items
                itemManagerRightPI.RemoveItem(rightItemToTrade);
                itemManagerLeftPI.RemoveItem(leftItemToTrade);
                if (itemManagerLeftPI.CanAddItem(rightItemToTrade, itemQtyRight) && itemManagerRightPI.CanAddItem(leftItemToTrade, itemQtyLeft)) {
                    itemManagerLeftPI.AddItemWithQuantity(rightItemToTrade, itemQtyRight);
                    itemManagerRightPI.AddItemWithQuantity(leftItemToTrade, itemQtyLeft);
                    gameStateInstance.UpdatePlayerInfo(playerInfoLeftSlot);
                    gameStateInstance.UpdatePlayerInfo(playerInfoRightSlot);
                    refreshScreen();                  
                } else {
                    //add items back
                    itemManagerLeftPI.AddItemWithQuantity(leftItemToTrade, itemQtyLeft);
                    itemManagerRightPI.AddItemWithQuantity(rightItemToTrade, itemQtyRight);
                }                  
            }
        }
    }

    private void refreshScreen() {
        leftSlotIdxSelected = -1;
        rightSlotIdxSelected = -1;
        populateContent(playerInfoLeftSlot, playerInfoRightSlot);
    }

    public void ClosePlayerItemTradeMenu() {
        if (unitManagementMenu.activeSelf) {
            tradePortraitPlayerOne.GetComponent<Image>().sprite = null;
            tradePortraitPlayerTwo.GetComponent<Image>().sprite = null;
            clearContent(leftSlotContent);
            clearContent(rightSlotContent);     
            playerItemTradeMenu.SetActive(false);     
        }
    }

    public static void Set_Size_Scale(GameObject gameObject, float width, float height, float xScale, float yScale) {
        if (gameObject != null)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform != null) {
                rectTransform.sizeDelta = new Vector2(width, height);
                gameObject.transform.localScale = new Vector3(xScale, yScale, 1.0f);
            }				
        }
    }
}
