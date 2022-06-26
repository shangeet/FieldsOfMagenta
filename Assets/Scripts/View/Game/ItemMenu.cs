using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMenu : AbstractMenu
{
    
    GameObject itemMenu;
    bool itemMenuDisplayed;

    protected override void Awake() {
        base.Awake();
        setupUIElements();
    }

    void Start() {}

    void setupUIElements() {
        // item menu
        itemMenu = GameObject.Find("ItemMenu");
        itemMenu.SetActive(false);
        itemMenuDisplayed = false;
    }

    public void openPlayerItemMenu() {
        PlayerInfo clickedPlayerInfo = sharedResourceBus.GetClickedPlayerNode().getPlayerInfo();
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
            Sprite itemSprite = currentConsumableItems[itemName].itemSprite;
            GameObject itemRow = itemMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemRow.name = "ID-" + itemSlotContainer.GetComponentsInChildren<Transform>().Length.ToString();
            GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
            Button button = buttonGameObj.GetComponent<Button>();
            button.onClick.AddListener(onConsumableItemButtonClick);
            buttonGameObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = itemName;
            buttonGameObj.transform.GetChild(2).gameObject.GetComponent<Text>().text = quantity.ToString() + "/" + ConsumableItemManager.numMaxItemsPerSlot.ToString();
            buttonGameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = itemSprite;
            itemRow.transform.SetParent(itemSlotContainer.transform, true);
            Vector3 itemRowPosition = new Vector3(0, startHeight, itemRow.transform.position.z);
            itemRow.GetComponent<RectTransform>().anchoredPosition = itemRowPosition;
            startHeight -= 30;
        }
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

    void onConsumableItemButtonClick() {
        GameObject itemRowButton = EventSystem.current.currentSelectedGameObject;
        string itemKey = itemRowButton.transform.GetChild(1).gameObject.GetComponent<Text>().text;
        PlayerInfo clickedPlayerInfo = sharedResourceBus.GetClickedPlayerNode().getPlayerInfo();
        clickedPlayerInfo.ConsumeItem(itemKey);
        closePlayerItemMenu();
        ChangeState(GameState.HandleTileState);
    }

    public bool IsItemMenuDisplayed() {
        return itemMenuDisplayed;
    }

}
