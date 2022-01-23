using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveSlotPopulator : MonoBehaviour, SaveInterface {

    private GameObject saveLoadMenu;
    private GameObject overwriteBox;
    private GameObject slotContents;
    private MasterGameStateController gameStateInstance;
    private int slotIdChosen = -1;    

    void Awake() {
        setupUIElements();
    }

    private void setupUIElements() {
        gameStateInstance = GameObject.Find("GameMasterInstance").GetComponent<MasterGameStateController>().instance;
        saveLoadMenu = GameObject.Find("SaveLoadMenu").gameObject;
        overwriteBox = saveLoadMenu.transform.Find("OverwriteBox").gameObject;
        slotContents = saveLoadMenu.transform.Find("SlotContents").gameObject;
        Button confirmSaveButton = overwriteBox.transform.Find("YesButton").gameObject.GetComponent<Button>();
        Button cancelButton = overwriteBox.transform.Find("NoButton").gameObject.GetComponent<Button>();
        confirmSaveButton.onClick.AddListener(onConfirmSaveButtonClick);
        cancelButton.onClick.AddListener(onCancelButtonClick);    
        overwriteBox.SetActive(false);   
    }

    public void OpenMenu() {
        openWithSaveSlotMode();
    }

    private void openWithSaveSlotMode() {
        saveLoadMenu.SetActive(true);
        populateSlotContents();
    }

    private void populateSlotContents() {
        float xStart = -4.0f; //8
        float yStart = 3.0f; //3
        int rowCount = 0;
        int maxSlots = MasterGameStateController.MAX_SAVE_SLOTS;
        for (int slotId = 0; slotId < maxSlots; slotId++) {
            GameObject saveSlot = saveLoadMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            TextMeshProUGUI saveSlotName = saveSlot.transform.Find("SlotName").gameObject.GetComponent<TextMeshProUGUI>();
            saveSlot.name = slotId.ToString();
            if (FileUtils.FileSlotExists(slotId)) {
                string timestamp = FileUtils.LoadTimeStampFromSlot(slotId);
                saveSlotName.text = System.String.Format("Slot {0} - {1}", slotId, timestamp);
            } else {
                saveSlotName.text = System.String.Format("Slot {0} - Empty", slotId);
            }        
            Button slotButton = saveSlot.GetComponent<Button>();
            slotButton.onClick.AddListener(onSlotClick);
            saveSlot.transform.position = new Vector3(xStart, yStart, 1.0f);
            saveSlot.transform.SetParent(slotContents.transform);
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
        overwriteBox.SetActive(true);
    }

    public void onConfirmSaveButtonClick() {
        gameStateInstance.SaveToFile(slotIdChosen);
        refreshSlotContents();
        overwriteBox.SetActive(false);
        slotIdChosen = -1;
    }

    public void onCancelButtonClick() {
        overwriteBox.SetActive(false);
        slotIdChosen = -1;
    }

    public void CloseMenu() {
        slotIdChosen = -1;
        ClearSlotContents();
        overwriteBox.SetActive(false);
    }
    
}
