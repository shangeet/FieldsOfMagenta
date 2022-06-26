using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class QuestsMenu : MonoBehaviour {

    GameObject questMenu;
    GameObject questViewContent;

    void Awake() {
        SetupUIElements();
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            CloseQuestsMenu();
        }        
    }
    
    void SetupUIElements() {
        questMenu = GameObject.Find("QuestMenu").gameObject;
        questViewContent = questMenu.transform.Find("QuestScrollView/QuestViewport/Content").gameObject;
        questMenu.SetActive(false);
    }

    public void OpenQuestsMenu(List<Quest> availableQuests) {

        questMenu.SetActive(true);
        float startHeight = 148.9f;
        float spawnHeight = 146.0f;
        float initialDiff = startHeight - spawnHeight;
        float diff = 1.8f;

        foreach(Quest quest in availableQuests) {
            GameObject questRow = questMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            Button questRowButton = questRow.transform.Find("Button").gameObject.GetComponent<Button>();
            questRowButton.onClick.AddListener(onButtonClick);
            SlotAdditionalProperties properties = questRow.AddComponent<SlotAdditionalProperties>();
            properties.AddProperty("NaniNovelPath", quest.naninovelQuestScenePath);
            properties.AddProperty("SlotId", quest.questId.ToString());
            TextMeshProUGUI questName = questRow.transform.Find("Button/QuestName").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI questDesc = questRow.transform.Find("Button/QuestDescription").gameObject.GetComponent<TextMeshProUGUI>();
            questName.text = quest.questName;
            questDesc.text = quest.questDescription;
            Vector3 oldPos = questRow.transform.position;
            questRow.transform.position = new Vector3(oldPos.x, initialDiff, oldPos.z);
            questRow.transform.SetParent(questViewContent.transform, true);
            initialDiff -= diff;
        }
    }

    public void onButtonClick() {
        string scenePath = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.GetComponent<SlotAdditionalProperties>().GetProperty("NaniNovelPath");
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    public void CleanQuestsMenuScreen() {
        foreach(Transform transform in questViewContent.transform) {
            Destroy(transform.gameObject);
        }
    }

    public void CloseQuestsMenu() {
        CleanQuestsMenuScreen();
        questMenu.SetActive(false);
    }

}
