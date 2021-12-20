using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwapItemsMenu : MonoBehaviour {
    public GameMaster gameMaster;
    public GameObject itemSwapMenu;
    public Button confirmButton;
    public Button currentPlayerClickedItemButton = null;
    public Button targetPlayerClickedItemButton = null;

    bool hasTraded = false;

    bool swapItemMenuDisplayed;

    public Button[] currentItemLocations;
    public Button[] targetItemLocations;

    public Color buttonUnselectedColor = Color.black;
    public Color buttonSelectedColor = Color.yellow;

    PlayerInfo currentPlayer;
    PlayerInfo targetPlayer;

    void Start() {
        gameMaster = gameObject.GetComponent<GameMaster>();
        setupUI();
    }

    void Update() {
        if (!hasTraded && Input.GetMouseButtonDown(1) && gameMaster.GetCurrentState() == GameState.SwapItemState) {
            closeSwapItemMenu();
            currentPlayer = null;
            targetPlayer = null;
            gameMaster.endShowSwapItemMenuStateToShowBattleMenuState();
        }
    }

    void setupUI() {
        itemSwapMenu = GameObject.Find("ItemSwapMenu");
        confirmButton = itemSwapMenu.transform.Find("ConfirmButton").gameObject.GetComponent<Button>();
        confirmButton.onClick.AddListener(onConfirmButtonClick);
        itemSwapMenu.SetActive(false);
        swapItemMenuDisplayed = false;
    }

    public void OpenSwapItemMenu(PlayerInfo currentPlayerInfo, PlayerInfo targetPlayerInfo) {
        currentPlayer = currentPlayerInfo;
        targetPlayer = targetPlayerInfo;
        itemSwapMenu.SetActive(true);
        swapItemMenuDisplayed = true;
        //Set player portraits
        PlayerAnimator currentPlayerAnimator = currentPlayer.playerAnimator;
        PlayerAnimator targetPlayerAnimator = targetPlayer.playerAnimator;
        Image currentPlayerImage = itemSwapMenu.transform.Find("UnitInfoMenuPlayerCurrent/PlayerCurrentInfo/CurrentPlayerFace").GetComponent<Image>();
        Image targetPlayerImage = itemSwapMenu.transform.Find("UnitInfoMenuPlayerTarget/PlayerCurrentInfo/TargetPlayerFace").GetComponent<Image>();
        currentPlayerImage.sprite = currentPlayerAnimator.playerPortrait;
        targetPlayerImage.sprite = targetPlayerAnimator.playerPortrait;
        populateItemList(currentPlayerInfo, false);
        populateItemList(targetPlayerInfo, true);
    }

    void populateItemList(PlayerInfo playerInfo, bool isTarget) {
        GameObject itemSlotContainer;
        if (isTarget) {
            itemSlotContainer = itemSwapMenu.transform.Find("DropbackItemsPlayerTarget/ItemSlotContainer").gameObject; 
        } else {
            itemSlotContainer = itemSwapMenu.transform.Find("DropbackItemsPlayerCurrent/ItemSlotContainer").gameObject;
        }
        populateContainer(playerInfo, itemSlotContainer, isTarget);
    }

    void populateContainer(PlayerInfo playerInfo, GameObject itemSlotContainer, bool isTarget) {
        //Set items in container
        ConsumableItemManager consumableItemManager = playerInfo.consumableItemManager;
        Dictionary<string,int> currentConsumableItemsInventory = consumableItemManager.currentConsumableItemsInventory;
        Dictionary<string,ConsumableItem> currentConsumableItems = consumableItemManager.consumableItems;
        int maxItems = ConsumableItemManager.numMaxSlots;

        if (isTarget) {
            targetItemLocations = new Button[maxItems];
        } else {
            currentItemLocations = new Button[maxItems];
        }
        
        int index = 0;
        int startHeight = 70;
        foreach (KeyValuePair<string, int> pair in currentConsumableItemsInventory) {
            string itemName = pair.Key;
            int quantity = pair.Value;
            print("Found item " + itemName + " qty: " + quantity.ToString());
            Sprite itemSprite = currentConsumableItems[itemName].itemSprite;
            GameObject itemRow = GameObject.Find("ItemSwapMenu").gameObject.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            itemRow.name = isTarget? "Target-" + itemName : "Current-" + itemName;
            print(itemRow); 
            GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
            Button button = buttonGameObj.GetComponent<Button>();
            button.onClick.AddListener(onConsumableItemButtonClick);
            buttonGameObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = itemName;
            buttonGameObj.transform.GetChild(2).gameObject.GetComponent<Text>().text = quantity.ToString() + "/" + ConsumableItemManager.numMaxItemsPerSlot.ToString();
            buttonGameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = itemSprite;
            itemRow.transform.SetParent(itemSlotContainer.transform, true);
            if (isTarget) {
                targetItemLocations[index] = button;
            } else {
                currentItemLocations[index] = button;
            }
            Vector3 itemRowPosition = new Vector3(0, startHeight, itemRow.transform.position.z);
            itemRow.GetComponent<RectTransform>().anchoredPosition = itemRowPosition;
            startHeight -= 30;
            index += 1;
        }

        //fill the rest with empty item rows
        while (index != maxItems) {
            GameObject itemRow = GameObject.Find("ItemSwapMenu").gameObject.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
            Button button = buttonGameObj.GetComponent<Button>();
            button.onClick.AddListener(onConsumableItemButtonClick);
            itemRow.transform.SetParent(itemSlotContainer.transform, true);
            if (isTarget) {
                targetItemLocations[index] = null;
            } else {
                currentItemLocations[index] = null;
            }
            Vector3 itemRowPosition = new Vector3(0, startHeight, itemRow.transform.position.z);
            itemRow.GetComponent<RectTransform>().anchoredPosition = itemRowPosition;
            startHeight -= 30;
            button.name = "Empty-" + index.ToString();
            itemRow.name = isTarget? "Target-" + index.ToString() + "-Empty" : "Current-" + index.ToString() + "-Empty";
            index += 1;
        }
    }

    void processSwap(bool currentEmpty, bool targetEmpty) {
        ConsumableItemManager cpConsumableItemManager = currentPlayer.consumableItemManager;
        Dictionary<string,int> cpCurrentConsumableItemsInventory = cpConsumableItemManager.currentConsumableItemsInventory;
        ConsumableItemManager tpConsumableItemManager = targetPlayer.consumableItemManager;
        Dictionary<string,int> tpCurrentConsumableItemsInventory = tpConsumableItemManager.currentConsumableItemsInventory;

        int currentPlayerInventoryCount = cpCurrentConsumableItemsInventory.Count;
        int targetPlayerInventoryCount = tpCurrentConsumableItemsInventory.Count;
        int maxInventoryCount = ConsumableItemManager.numMaxSlots;

        //if both empty, deselect both
        if (currentEmpty && targetEmpty) {
            print("Both empty. You're trolling");
            playErrorNoise();
            deSelectButton(currentPlayerClickedItemButton);
            deSelectButton(targetPlayerClickedItemButton);
            currentPlayerClickedItemButton = null;
            targetPlayerClickedItemButton = null;
        } else if (currentEmpty) { //if currentEmpty, proceed with targetInventory-- currentInventory++
            string itemName = targetPlayerClickedItemButton.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
            int expectedCurrentInventoryCount = currentPlayerInventoryCount++;
            if (expectedCurrentInventoryCount <= maxInventoryCount && !cpCurrentConsumableItemsInventory.ContainsKey(itemName)) {
                //swap
                print("Swapping items current empty");
                swapItems(currentEmpty, targetEmpty);
            } else {
                //play error noise & reset
                playErrorNoise();
                deSelectButton(currentPlayerClickedItemButton);
                deSelectButton(targetPlayerClickedItemButton);
                currentPlayerClickedItemButton = null;
                targetPlayerClickedItemButton = null;
            }
        } else if (targetEmpty) { //if targetEmpty, proceed with currentInventory-- targetInventory++\
            string itemName = currentPlayerClickedItemButton.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
            int expectedTargetInventoryCount = targetPlayerInventoryCount++;
            if (expectedTargetInventoryCount <= maxInventoryCount && !tpCurrentConsumableItemsInventory.ContainsKey(itemName)) {
                //swap
                print("Swapping items target empty");
                swapItems(currentEmpty, targetEmpty);
            } else {
                //play error noise & reset
                playErrorNoise();
                deSelectButton(currentPlayerClickedItemButton);
                deSelectButton(targetPlayerClickedItemButton);
                currentPlayerClickedItemButton = null;
                targetPlayerClickedItemButton = null;
            }            
        } else { //if neither empty, proceed with inventory qty unchanged
            string currentItemName = currentPlayerClickedItemButton.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
            string targetItemName = targetPlayerClickedItemButton.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
            if (!tpCurrentConsumableItemsInventory.ContainsKey(currentItemName) && !cpCurrentConsumableItemsInventory.ContainsKey(targetItemName)) { 
                print("Both valid. Swapping.");
                swapItems(currentEmpty, targetEmpty);                
            } else {
                playErrorNoise();
                deSelectButton(currentPlayerClickedItemButton);
                deSelectButton(targetPlayerClickedItemButton);
                currentPlayerClickedItemButton = null;
                targetPlayerClickedItemButton = null;
            }
        }
    }

    void onConfirmButtonClick() {
        print("Clicked on confirm button");
        closeSwapItemMenu();
        gameMaster.endShowSwapItemMenuStateToEndTurnState(currentPlayer);
        currentPlayer = null;
        targetPlayer = null;
    }

    void playErrorNoise() {
        print("Played error noise.");
    }

    void swapItems(bool currentEmpty, bool targetEmpty) {
        GameObject currentItemSlotContainer = itemSwapMenu.transform.Find("DropbackItemsPlayerCurrent/ItemSlotContainer").gameObject;
        GameObject targetItemSlotContainer = itemSwapMenu.transform.Find("DropbackItemsPlayerTarget/ItemSlotContainer").gameObject;
        int idxTargetItemToSwitch = -1;
        int idxCurrentItemToSwitch = -1;
        string currentItemToSwap = null;
        string targetItemToSwap = null;

        if (!currentEmpty) {
            currentItemToSwap = currentPlayerClickedItemButton.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
            for (int idx = 0; idx < currentItemLocations.Length; idx++) {
                if (currentItemLocations[idx] != null && currentItemLocations[idx].transform.Find("ItemName").gameObject.GetComponent<Text>().text == currentItemToSwap) {
                    print("Got current item index: " + idx.ToString());
                    idxCurrentItemToSwitch = idx;
                }
            }  
        } else {
            for (int idx = 0; idx < currentItemLocations.Length; idx++) {
                if (currentItemLocations[idx] == null) {
                    idxCurrentItemToSwitch = idx;
                    break;
                }
            }
        }

        if (!targetEmpty) {
            targetItemToSwap = targetPlayerClickedItemButton.transform.Find("ItemName").gameObject.GetComponent<Text>().text;
            for (int idx = 0; idx < targetItemLocations.Length; idx++) {
                if (targetItemLocations[idx] != null && targetItemLocations[idx].transform.Find("ItemName").gameObject.GetComponent<Text>().text == targetItemToSwap) {
                    print("Got target item index: " + idx.ToString());
                    idxTargetItemToSwitch = idx;
                }
            }
        } else {
            for (int idx = 0; idx < targetItemLocations.Length; idx++) {
                if (targetItemLocations[idx] == null) {
                    idxTargetItemToSwitch = idx;
                    break;
                }
            }
        }

        //switch the positions on UI (also need to swap names)
        print("Switching positions in UI memory");
        Button currentButton = currentItemLocations[idxCurrentItemToSwitch];
        Button targetButton = targetItemLocations[idxTargetItemToSwitch];
        if (!currentEmpty && !targetEmpty) {
            currentItemLocations[idxCurrentItemToSwitch] = targetButton;
            targetItemLocations[idxTargetItemToSwitch] = currentButton;
        } else if (targetEmpty) {
            targetItemLocations[idxTargetItemToSwitch] = currentButton;
            currentItemLocations[idxCurrentItemToSwitch] = null;
        } else if (currentEmpty) {
            currentItemLocations[idxCurrentItemToSwitch] = targetButton;
            targetItemLocations[idxTargetItemToSwitch] = null;
        }

        currentPlayerClickedItemButton = null;
        targetPlayerClickedItemButton = null;

        //swap in the backend
        print("Swapping in backend");
        hasTraded = true;
        PlayerInfo.SwitchConsumableItems(currentPlayer, targetPlayer, currentItemToSwap, targetItemToSwap);

        //Destroy old lists and repopulate
        print("Destroy and repopulate");
        destroyItemsInContainer(currentItemSlotContainer);
        destroyItemsInContainer(targetItemSlotContainer);
        repopulateContainer(currentItemSlotContainer, currentItemLocations, false);
        repopulateContainer(targetItemSlotContainer, targetItemLocations, true);            
    }

    void repopulateContainer(GameObject itemContainer, Button[] itemLocations, bool isTarget) {
        int startHeight = 70;
        for (int index = 0; index < itemLocations.Length; index++) {
            Button button = itemLocations[index];
            GameObject itemRow = GameObject.Find("ItemSwapMenu").gameObject.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            if (button != null) {
                string itemName = button.transform.GetChild(1).gameObject.GetComponent<Text>().text;
                string itemQty = button.transform.GetChild(2).gameObject.GetComponent<Text>().text;
                Sprite sprite = button.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;

                GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
                Button newButton = itemRow.transform.GetChild(0).GetComponent<Button>();
                newButton.enabled = true;
                newButton.onClick.AddListener(onConsumableItemButtonClick);
                buttonGameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = sprite;
                buttonGameObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = itemName;
                buttonGameObj.transform.GetChild(2).gameObject.GetComponent<Text>().text = itemQty;
                itemRow.name = isTarget? "Target-" + itemName : "Current-" + itemName;
                itemLocations[index] = newButton;                 
            } else {
                GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
                Button b = buttonGameObj.GetComponent<Button>();
                b.onClick.AddListener(onConsumableItemButtonClick);
                b.name = "Empty-" + index.ToString();
                itemRow.name = isTarget? "Target-" + index.ToString() : "Current-" + index.ToString();
            }
            itemRow.transform.SetParent(itemContainer.transform, true);
            Vector3 itemRowPosition = new Vector3(0, startHeight, itemRow.transform.position.z);
            itemRow.GetComponent<RectTransform>().anchoredPosition = itemRowPosition;
            startHeight -= 30;
        }
    }

    void destroyItemsInContainer(GameObject itemSlotContainer) {
        foreach (Transform child in itemSlotContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }       
    }

    void onConsumableItemButtonClick() { 
        //GameObject buttonGameObj = itemRow.transform.GetChild(0).gameObject;
        Button itemRowButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        GameObject itemRow = itemRowButton.transform.parent.gameObject;
        string rowName = itemRow.name;
        bool isTarget = rowName.Contains("Target-");
        toggleButtonSelected(itemRowButton, isTarget);
    }

    void toggleButtonSelected(Button itemRowButton, bool isTarget) {

        if (isTarget) { //no targets clicked 
            if (targetPlayerClickedItemButton == null) {
                print("Selected target button");
                targetPlayerClickedItemButton = itemRowButton;
                selectButton(itemRowButton);
            } else { //deselect previous before swapping
                //string previousTargetItemName = targetPlayerClickedItemButton.transform.GetChild(1).gameObject.GetComponent<Text>().text;
                if (targetPlayerClickedItemButton == itemRowButton) {
                    //just deselect as it's the same button
                    print("Deselected target button");
                    deSelectButton(itemRowButton);
                    targetPlayerClickedItemButton = null;
                } else {
                    print("Deselected one. Selected the other");
                    deSelectButton(targetPlayerClickedItemButton);
                    selectButton(itemRowButton);
                    targetPlayerClickedItemButton = itemRowButton;
                }
            }
        } else {
            if (currentPlayerClickedItemButton == null) {
                print("Selected current button");
                selectButton(itemRowButton);
                currentPlayerClickedItemButton = itemRowButton;
            } else {
                //string previousCurrentItemName = currentPlayerClickedItemButton.transform.GetChild(1).gameObject.GetComponent<Text>().text;
                if (currentPlayerClickedItemButton == itemRowButton) {
                    print("Deselected current button");
                    deSelectButton(itemRowButton);
                    currentPlayerClickedItemButton = null;
                } else {
                    print("Deselect old select new current button");
                    deSelectButton(currentPlayerClickedItemButton);
                    selectButton(itemRowButton);
                    currentPlayerClickedItemButton = itemRowButton;
                }
            }
        }
        if (currentPlayerClickedItemButton != null && targetPlayerClickedItemButton != null) {
            print("Both clicked! Process Swap!");
            bool currentSlotEmpty = currentPlayerClickedItemButton.name.Contains("Empty");
            bool targetSlotEmpty = targetPlayerClickedItemButton.name.Contains("Empty");
            processSwap(currentSlotEmpty, targetSlotEmpty);
        }
    }

    void selectButton(Button button) {
        ColorBlock cb = button.colors;
        cb.normalColor = buttonSelectedColor;
        button.colors = cb;
    }

    void deSelectButton(Button button) {
        EventSystem.current.SetSelectedGameObject(null);
        ColorBlock cb = button.colors;
        cb.normalColor = buttonUnselectedColor;
        button.colors = cb;
    }

    void closeSwapItemMenu() {
        currentPlayerClickedItemButton = null;
        targetPlayerClickedItemButton = null;
        currentItemLocations = null;
        targetItemLocations = null;
        itemSwapMenu.SetActive(false);
        swapItemMenuDisplayed = false;
    }

    public bool IsSwapItemMenuDisplayed() {
        return swapItemMenuDisplayed;
    }

}
