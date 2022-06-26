using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemShopMenu : MonoBehaviour {

    GameObject itemShopMenu;
    GameObject playerInventoryContent;
    GameObject shopInventoryContent;
    GameObject goldDisplay;
    GameObject playerInventorySelMenu;
    GameObject shopInventorySelMenu;
    GameObject itemHoverMenu;
    MasterGameStateController gameStateInstance;
    GameObject currentlySelectedItem;


    void Awake() {
        setupUIElements();
    }

    void Start() {
        gameStateInstance = GameObject.Find("GameMasterInstance").GetComponent<MasterGameStateController>().instance;
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            CloseItemShopMenu();
        }
    }

    private void setupUIElements() {
        itemShopMenu = GameObject.Find("ItemShopMenu").gameObject;
        playerInventoryContent = itemShopMenu.transform.Find("PlayerInventory/PlayerInventoryScrollView/PlayerInventoryViewport/Content").gameObject;
        shopInventoryContent = itemShopMenu.transform.Find("ShopInventory/ShopInventoryScrollView/ShopInventoryViewport/Content").gameObject;
        goldDisplay = itemShopMenu.transform.Find("GoldCanvas/GoldAmount").gameObject;
        playerInventorySelMenu = itemShopMenu.transform.Find("PlayerInventorySelectionMenu").gameObject;
        playerInventorySelMenu.transform.Find("SellButton").GetComponent<Button>().onClick.AddListener(onButtonPressPlayerInventorySelMenu);
        shopInventorySelMenu = itemShopMenu.transform.Find("ShopInventorySelectionMenu").gameObject;
        shopInventorySelMenu.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(onButtonPressShopInventorySelMenu);
        itemHoverMenu = itemShopMenu.transform.Find("ItemHoverDescription").gameObject;
        itemShopMenu.SetActive(false);
    }

    public void OpenItemShopMenu() {
        itemShopMenu.SetActive(true);
        itemHoverMenu.SetActive(false);
        playerInventorySelMenu.SetActive(false);
        shopInventorySelMenu.SetActive(false);
        List<ConsumableItem> consumableItems = gameStateInstance.GetAllConsumableItems();
        List<EquipmentItem> equipmentItems = gameStateInstance.GetAllEquipmentItems();
        List<int> playerCompletedQuestIds = gameStateInstance.GetPlayerCompletedQuestIds();
        List<ConsumableItem> unlockedConsumableItems = ItemsDatabase.GetAllUnlockedConsumableItems(playerCompletedQuestIds);
        List<EquipmentItem> unlockedEquipmentItems = ItemsDatabase.GetAllUnlockedEquipmentItems(playerCompletedQuestIds);
        int playerGold = gameStateInstance.GetPlayerGold();
        populateInventory(consumableItems, equipmentItems, "PlayerInventory", true);
        populateInventory(unlockedConsumableItems, unlockedEquipmentItems, "ShopInventory", false);
        updateGoldDisplay(playerGold);
    }

    public void CloseItemShopMenu() {
        destroyPlayerInventory();
        destroyShopInventory();
        clearItemHoverMenu();
        itemShopMenu.SetActive(false);
        itemHoverMenu.SetActive(false);        
    }

    private void populateInventory(List<ConsumableItem> consumableItems, List<EquipmentItem> equipmentItems, string inventoryId, bool isPlayerInventory) {
        
        int itemIdx = 0;

        foreach (ConsumableItem item in consumableItems) {
            GameObject itemRow = itemShopMenu.transform.Find(inventoryId).GetComponent<ItemMenuSpawner>().SpawnPrefab();
            Button itemRowButton = itemRow.transform.Find("Button").gameObject.GetComponent<Button>();
            Image itemRowSprite = itemRow.transform.Find("Button/ItemSprite").GetComponent<Image>();
            TextMeshProUGUI itemName = itemRow.transform.Find("Button/ItemName").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemGoldVal = itemRow.transform.Find("Button/GoldPrice").gameObject.GetComponent<TextMeshProUGUI>();
            SlotAdditionalProperties properties = itemRowButton.gameObject.AddComponent<SlotAdditionalProperties>();
            properties.AddProperty("ItemDescription", item.itemDescription);
            properties.AddProperty("AssetPath", item.assetPath);
            itemRowSprite.sprite = ItemsDatabase.GetConsumableItemByName(item.itemName).GetSpriteFromSpritePath();
            itemName.text = item.itemName;
            itemGoldVal.text = item.goldValue.ToString() + "G";
            itemRow.name = "C-" + itemIdx.ToString();
            if (isPlayerInventory) {
                itemRowButton.onClick.AddListener(onPlayerInventoryItemClick); 
                itemRow.transform.SetParent(playerInventoryContent.transform, true);
            } else {
                itemRowButton.onClick.AddListener(onShopInventoryItemClick);
                itemRow.transform.SetParent(shopInventoryContent.transform, true);
            }
            itemIdx++;
        }

        foreach(EquipmentItem item in equipmentItems) {
            GameObject itemRow = itemShopMenu.transform.Find(inventoryId).GetComponent<ItemMenuSpawner>().SpawnPrefab();
            Button itemRowButton = itemRow.transform.Find("Button").gameObject.GetComponent<Button>();
            Image itemRowSprite = itemRow.transform.Find("Button/ItemSprite").GetComponent<Image>();
            TextMeshProUGUI itemName = itemRow.transform.Find("Button/ItemName").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemGoldVal = itemRow.transform.Find("Button/GoldPrice").gameObject.GetComponent<TextMeshProUGUI>();
            SlotAdditionalProperties properties = itemRowButton.gameObject.AddComponent<SlotAdditionalProperties>();
            properties.AddProperty("ItemDescription", item.itemDescription);
            properties.AddProperty("AssetPath", item.assetPath);
            itemRowSprite.sprite = ItemsDatabase.GetEquipmentItemByName(item.itemName).GetSpriteFromSpritePath();
            itemName.text = item.itemName;
            itemGoldVal.text = item.goldValue.ToString() + "G";
            itemRow.name = "E-" + itemIdx.ToString();
            if (isPlayerInventory) {
                itemRowButton.onClick.AddListener(onPlayerInventoryItemClick); 
                itemRow.transform.SetParent(playerInventoryContent.transform);
            } else {
                itemRowButton.onClick.AddListener(onShopInventoryItemClick);
                itemRow.transform.SetParent(shopInventoryContent.transform);
            }
            itemIdx++;            
        }

    }

    private void updateGoldDisplay(int playerGold) {
        goldDisplay.GetComponent<TextMeshProUGUI>().text = "Gold: " + playerGold.ToString() + "G";
    }

    private void onPlayerInventoryItemClick() {
        currentlySelectedItem = EventSystem.current.currentSelectedGameObject;
        populateItemHoverMenu(currentlySelectedItem, true);
    }

    private void onShopInventoryItemClick() {
        currentlySelectedItem = EventSystem.current.currentSelectedGameObject;
        populateItemHoverMenu(currentlySelectedItem, false);
    }

    private void populateItemHoverMenu(GameObject currentlySelectedObject, bool isPlayerInventorySel) {
        itemHoverMenu.SetActive(true);

        if (isPlayerInventorySel) {
            playerInventorySelMenu.SetActive(true);
        } else {
            shopInventorySelMenu.SetActive(true);
        }

        Image itemSprite = itemHoverMenu.transform.Find("ItemIcon").GetComponent<Image>();
        TextMeshProUGUI itemDescription = itemHoverMenu.transform.Find("DescriptionBackground/Description").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI itemPrice = itemHoverMenu.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
        itemSprite.sprite = currentlySelectedObject.transform.Find("ItemSprite").GetComponent<Image>().sprite;
        string itemDesc = currentlySelectedObject.GetComponent<SlotAdditionalProperties>().GetProperty("ItemDescription");
        string itemTypeDesc = currentlySelectedObject.transform.parent.name.Contains("C") ? "Consumable Item\n" : "Equipment Item\n";
        string finalItemDesc = itemTypeDesc + itemDesc;
        itemDescription.text = finalItemDesc;
        itemPrice.text = currentlySelectedObject.transform.Find("GoldPrice").GetComponent<TextMeshProUGUI>().text;
    }

    private void clearItemHoverMenu() {
        Image itemSprite = itemHoverMenu.transform.Find("ItemIcon").GetComponent<Image>();
        TextMeshProUGUI itemDescription = itemHoverMenu.transform.Find("DescriptionBackground/Description").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI itemPrice = itemHoverMenu.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();  
        itemSprite.sprite = null;
        itemDescription.text = "";
        itemPrice.text = "";      
    }

    private void onButtonPressPlayerInventorySelMenu() {
        //sell item
        string goldValueText = currentlySelectedItem.transform.Find("GoldPrice").GetComponent<TextMeshProUGUI>().text;
        int goldValue = System.Int32.Parse(goldValueText.Substring(0, goldValueText.Length - 1));
        //update game status gold value
        gameStateInstance.SetPlayerGold(gameStateInstance.GetPlayerGold() + goldValue);
        //update gold display
        updateGoldDisplay(gameStateInstance.GetPlayerGold());
        //update playerinventorydisplay (remove)
        refreshPlayerInventory(false);
        playerInventorySelMenu.SetActive(false);
        itemHoverMenu.SetActive(false);
    }

    private void onButtonPressShopInventorySelMenu() {
        string goldValueText = currentlySelectedItem.transform.Find("GoldPrice").GetComponent<TextMeshProUGUI>().text;
        int goldValue = System.Int32.Parse(goldValueText.Substring(0, goldValueText.Length - 1));
        //update game status gold value
        gameStateInstance.SetPlayerGold(gameStateInstance.GetPlayerGold() - goldValue);
        //update gold display
        updateGoldDisplay(gameStateInstance.GetPlayerGold()); 
        //update playerinventorydisplay (add)      
        refreshPlayerInventory(true);
        shopInventorySelMenu.SetActive(false);
        itemHoverMenu.SetActive(false);
    }

    private void refreshPlayerInventory(bool isAdded) {
        destroyPlayerInventory();
        if (isAdded) {
            if (currentlySelectedItem.transform.parent.name.Contains("C")) {
                ConsumableItem itemToAdd = ItemsDatabase.GetConsumableItemByName(currentlySelectedItem.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text);
                gameStateInstance.AddItemToInventory(itemToAdd);
            } else {
                EquipmentItem itemToAdd = ItemsDatabase.GetEquipmentItemByName(currentlySelectedItem.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text);
                gameStateInstance.AddItemToInventory(itemToAdd);
            }
        } else {
            if (currentlySelectedItem.transform.parent.name.Contains("C")) {
                ConsumableItem itemToRemove = ItemsDatabase.GetConsumableItemByName(currentlySelectedItem.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text);
                gameStateInstance.RemoveItemFromInventory(itemToRemove);
            } else {
                EquipmentItem itemToRemove = ItemsDatabase.GetEquipmentItemByName(currentlySelectedItem.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text);
                gameStateInstance.RemoveItemFromInventory(itemToRemove);
            }            
        }
        populateInventory(gameStateInstance.GetAllConsumableItems(), gameStateInstance.GetAllEquipmentItems(), "PlayerInventory", true);
    }

    private void destroyPlayerInventory() {
        foreach (Transform child in playerInventoryContent.transform) {
            Destroy(child.gameObject);
        }
    }

    private void destroyShopInventory() {
        foreach (Transform child in shopInventoryContent.transform) {
            Destroy(child.gameObject);
        }
    }
    
}
