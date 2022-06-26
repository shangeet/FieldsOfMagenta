using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadSlotPopulator : MonoBehaviour {

    private GameObject saveLoadMenu;
    private GameObject loadBox;
    private GameObject slotContents;
    private MasterGameStateController gameStateInstance;
    private int slotIdChosen = -1;    

    void Awake() {
        setupUIElements();
    }

    private void setupUIElements() {
        gameStateInstance = GameObject.Find("GameMasterInstance").GetComponent<MasterGameStateController>().instance;
        saveLoadMenu = GameObject.Find("SaveLoadMenu").gameObject;
        loadBox = saveLoadMenu.transform.Find("LoadBox").gameObject;
        slotContents = saveLoadMenu.transform.Find("SlotContents").gameObject;
        Button confirmSaveButton = loadBox.transform.Find("YesButton").gameObject.GetComponent<Button>();
        Button cancelButton = loadBox.transform.Find("NoButton").gameObject.GetComponent<Button>();
        confirmSaveButton.onClick.AddListener(onConfirmSaveButtonClick);
        cancelButton.onClick.AddListener(onCancelButtonClick);    
        loadBox.SetActive(false);  
    }

    public void OpenMenu() {
        openWithLoadSlotMode();
    }

    private void openWithLoadSlotMode() {
            saveLoadMenu.SetActive(true);
            populateSlotContents();
    }

    private void populateSlotContents() {
        float xStart = -4.0f;
        float yStart = 3.0f;
        int rowCount = 0;
        int maxSlots = MasterGameStateController.MAX_SAVE_SLOTS;
        for (int slotId = 0; slotId < maxSlots; slotId++) {
            GameObject loadSlot = saveLoadMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            TextMeshProUGUI saveSlotName = loadSlot.transform.Find("SlotName").gameObject.GetComponent<TextMeshProUGUI>();
            loadSlot.name = slotId.ToString();
            if (FileUtils.FileSlotExists(slotId)) {
                SaveFileModel tempModel = FileUtils.LoadFromFile(slotId);
                saveSlotName.text = System.String.Format("Slot {0} - {1}", slotId, tempModel.saveTimestamp);
            } else {
                saveSlotName.text = System.String.Format("Slot {0} - Empty", slotId);
            }        
            Button slotButton = loadSlot.GetComponent<Button>();
            slotButton.onClick.AddListener(onSlotClick);
            loadSlot.transform.SetParent(slotContents.transform);
            loadSlot.transform.position = new Vector3(xStart, yStart, 1.0f);
            rowCount += 1;
            if ((rowCount + 1) % 4 == 0) {
                yStart = 3.0f;
                xStart = 4.0f;
            } else {
                yStart -= 3.0f;
            }
        }
    }

    private void refreshSlotContents() {
        ClearSlotContents();
        populateSlotContents();
    }

    private void ClearSlotContents() {
        foreach(Transform trans in slotContents.transform) {
            Destroy(trans.gameObject);
        }
    }

    public void onSlotClick() {
        slotIdChosen = System.Int32.Parse(EventSystem.current.currentSelectedGameObject.name);
        loadBox.SetActive(true);
    }

    public void onConfirmSaveButtonClick() {
        gameStateInstance.LoadFromFile(slotIdChosen);
        refreshSlotContents();
        loadBox.SetActive(false);
        slotIdChosen = -1;
    }

    public void onCancelButtonClick() {
        loadBox.SetActive(false);
        slotIdChosen = -1;
        loadBox.SetActive(false);
    }

    public void CloseMenu() {
        slotIdChosen = -1;
        ClearSlotContents();
        loadBox.SetActive(false);
    }
}
