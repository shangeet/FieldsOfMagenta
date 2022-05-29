using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenController : MonoBehaviour {
    
    private MasterGameStateController gameStateInstance;
    private List<Quest> activeQuests;
    private List<ConsumableItem> storeConsumableItems;
    private List<EquipmentItem> storeEquipmentItems;

    //UI Elements
    private Button questMenuButton;
    private QuestsMenu questsMenu;
    private Button itemShopButton;
    private ItemShopMenu itemShopMenu;
    private UnitManagementMenu unitManagementMenu;
    private SaveLoadMenu saveLoadMenu;
    private Button unitManagementButton;
    private Button saveButton;
    private Button loadButton;

    void Awake() {
        //gameStateInstance = GameObject.Find("GameMasterInstance").gameObject.GetComponent<MasterGameStateController>().instance;
        PlayerInfo newPlayer = new PlayerInfo("1", "Shan", false, new Bard(), "images/portraits/test_face");
        newPlayer.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        gameStateInstance = GameObject.Find("GameMasterInstance").AddComponent<MasterGameStateController>().instance;
        gameStateInstance.CreateNewSaveInstance(newPlayer);
        //add one more player
        PlayerInfo newPlayerTwo = new PlayerInfo("2", "Bobby", false, new Warrior(), "images/portraits/test_face");
        newPlayerTwo.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        gameStateInstance.AddNewPlayer(newPlayerTwo);
        List<int> playerCompletedQuestIds = gameStateInstance.GetPlayerCompletedQuestIds();
        //activeQuests = QuestsDatabase.GetAllActiveQuests(playerCompletedQuestIds);
        activeQuests = QuestsDatabase.GetAllQuests();
        //storeConsumableItems = ItemsDatabase.getAllUnlockedConsumableItems(playerCompletedQuestIds);
        storeConsumableItems = ItemsDatabase.GetAllConsumableItems();
        //storeEquipmentItems = ItemsDatabase.getAllUnlockedEquipmentItems(playerCompletedQuestIds);
        storeEquipmentItems = ItemsDatabase.GetAllEquipmentItems();
        AddUIElements();
    }

    void AddUIElements() {
        questMenuButton = gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        questMenuButton.onClick.AddListener(onQuestButtonClick);
        questsMenu = gameObject.AddComponent<QuestsMenu>();
        itemShopButton = gameObject.transform.GetChild(2).gameObject.GetComponent<Button>();
        itemShopButton.onClick.AddListener(onItemShopButtonClick);
        itemShopMenu = gameObject.AddComponent<ItemShopMenu>();
        unitManagementButton = gameObject.transform.GetChild(3).gameObject.GetComponent<Button>();
        unitManagementButton.onClick.AddListener(onUnitManagmentButtonClick);
        unitManagementMenu = gameObject.AddComponent<UnitManagementMenu>();
        saveButton = gameObject.transform.GetChild(4).gameObject.GetComponent<Button>();
        saveButton.onClick.AddListener(onSaveButtonClick);
        loadButton = gameObject.transform.GetChild(5).gameObject.GetComponent<Button>();
        loadButton.onClick.AddListener(onLoadButtonClick);
        saveLoadMenu = gameObject.AddComponent<SaveLoadMenu>();
    }

    void onQuestButtonClick() {
        questsMenu.OpenQuestsMenu(activeQuests);
    }

    void onItemShopButtonClick() {
        itemShopMenu.OpenItemShopMenu();
    }

    void onUnitManagmentButtonClick() {
        unitManagementMenu.OpenUnitManagementMenu();
    }

    void onSaveButtonClick() {
        saveLoadMenu.OpenSaveLoadMenu(SaveType.SAVE);
    }

    void onLoadButtonClick() {
        saveLoadMenu.OpenSaveLoadMenu(SaveType.LOAD);
    }

}
