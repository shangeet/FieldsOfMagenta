using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SaveType { SAVE, LOAD }
public class SaveLoadMenu : MonoBehaviour {
    
    private GameObject saveLoadMenu;
    private LoadSlotPopulator loadSlotPopulator;
    private SaveSlotPopulator saveSlotPopulator;
    private SaveType mode;

    void Awake() {
        setupUIElements();
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            CloseSaveLoadMenu();
        }
    }

    private void setupUIElements() {
        saveLoadMenu = GameObject.Find("SaveLoadMenu").gameObject;
        loadSlotPopulator = saveLoadMenu.AddComponent<LoadSlotPopulator>();
        saveSlotPopulator = saveLoadMenu.AddComponent<SaveSlotPopulator>();
        saveLoadMenu.SetActive(false);
    }

    public void OpenSaveLoadMenu(SaveType modeToOpen) {
        saveLoadMenu.SetActive(true);
        mode = modeToOpen;
        if (mode == SaveType.SAVE) {
            saveSlotPopulator.OpenMenu();
        } else if (mode == SaveType.LOAD) {
            loadSlotPopulator.OpenMenu();
        }
    }

    public void CloseSaveLoadMenu() {
        if (mode == SaveType.SAVE) {
            saveSlotPopulator.CloseMenu();
        } else if (mode == SaveType.LOAD) {
            loadSlotPopulator.CloseMenu();
        }        
        saveLoadMenu.SetActive(false);
    }

}
