using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerSetupMenu : AbstractMenu {

    private Button playerSwapModeButton;
    private Button unitMenuModeButton;
    private Button startBattleButton;
    private GameObject playerSetupMenuGo;
    private GameObject mainMenuGo;
    private GameObject unitMenuGo;
    private bool added = false;

    protected override void Awake() {
        base.Awake();
        setupUIElements();
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}

    void setupUIElements() {
        // item menu
        playerSetupMenuGo = GameObject.Find("PlayerSetupMenu");
        mainMenuGo = GameObject.Find("PlayerSetupMenu/MainMenu");
        unitMenuGo = GameObject.Find("PlayerSetupMenu/UnitSelectionMenu");

        playerSwapModeButton = GameObject.Find("PlayerSetupMenu/MainMenu/PlayerSwapModeButton").GetComponent<Button>();
        unitMenuModeButton = GameObject.Find("PlayerSetupMenu/MainMenu/UnitMenuButton").GetComponent<Button>();
        startBattleButton = GameObject.Find("PlayerSetupMenu/MainMenu/StartBattleButton").GetComponent<Button>();

        playerSwapModeButton.onClick.AddListener(onPlayerSwapModeButtonClick);
        unitMenuModeButton.onClick.AddListener(onUnitMenuModeButtonClick);
        startBattleButton.onClick.AddListener(onStartBattleButtonClick);

        ClosePlayerSetupMenu();

    }

    public void OpenPlayerSetupMenu() {
        playerSetupMenuGo.SetActive(true);
        mainMenuGo.SetActive(true);
        unitMenuGo.SetActive(false);
    }

    public void ClosePlayerSetupMenu() {
        playerSetupMenuGo.SetActive(false);
    }

    private void onPlayerSwapModeButtonClick() {
        sharedResourceBus.SetPlayerSetupMenuState(SetupState.PLAYER_SWAP_MODE);
        ClosePlayerSetupMenu();
    }

    private void onUnitMenuModeButtonClick() {
        mainMenuGo.SetActive(false);
        sharedResourceBus.SetPlayerSetupMenuState(SetupState.UNIT_SETUP_MODE);
    }

    private void onStartBattleButtonClick() {
        sharedResourceBus.SetPlayerSetupMenuState(SetupState.SETUP_MENU_CLOSED_MODE);
    }

    public void OpenUnitMenu() {
        sharedResourceBus.SetUnitMenuOpened(true);
        unitMenuGo.SetActive(true);
        populatePlayerSlots();
    }

    public void CloseUnitMenu() {
        sharedResourceBus.SetUnitMenuOpened(false);
        clearContainer();
        unitMenuGo.SetActive(false);
    }

    private void populatePlayerSlots() {    
        MasterGameStateController gameStateInstance = sharedResourceBus.GetMasterGameStateController();
        if (!added) {
            PlayerInfo newPlayerThree = new PlayerInfo("fakeid3", "Swagger", false, new Mage(), "images/portraits/test_face");
            newPlayerThree.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/ally_mage_m",
                "Animations/AllyMageM/AllyMageMGame",
                "Animations/AllyMageM/AllyMageMBattle");
            gameStateInstance.AddNewPlayer(newPlayerThree);
            sharedResourceBus.SetMasterGameStateController(gameStateInstance);     
            added = true;       
        }

        MasterGameStateController globalState = sharedResourceBus.GetMasterGameStateController();

        List<PlayerInfo> allSlots = globalState.GetAllPlayerInfos();
        int maxActive = sharedResourceBus.GetMaxActivePlayers();
        List<PlayerInfo> currentActive = new List<PlayerInfo>();
        currentActive.AddRange(sharedResourceBus.GetPawnInfoDict().Values);

        List<PlayerInfo> reservePlayers = new List<PlayerInfo>();

        if (allSlots.Count > maxActive) {
            foreach (PlayerInfo player in allSlots) {
                if (!currentActive.Contains(player)) {
                    reservePlayers.Add(player);
                }
            }
        }

    
        GameObject reservePlayerContainer = unitMenuGo.transform.Find("ReservePlayerContainer/ScrollView/Viewport/Content").gameObject;

        //fill reserves
        fillUnitSlots(reservePlayerContainer, reservePlayers);
    }

    private void fillUnitSlots(GameObject objectToFill, List<PlayerInfo> playerInfoList) {
        foreach (PlayerInfo info in playerInfoList) {
            GameObject unitSlot = playerSetupMenuGo.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            unitSlot.name = "USID-" + info.id;
            Image playerFace = unitSlot.transform.Find("Button/PlayerFace").GetComponent<Image>();
            TextMeshProUGUI levelText = unitSlot.transform.Find("Button/Level").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI expText = unitSlot.transform.Find("Button/Exp").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI statsText = unitSlot.transform.Find("Button/Stats").GetComponent<TextMeshProUGUI>();
            Texture2D portraitTexture = Resources.Load<Texture2D>(info.portraitRefPath);
            playerFace.sprite = Sprite.Create(portraitTexture, new Rect(0.0f, 0.0f, portraitTexture.width, portraitTexture.height), new Vector2(0.5f, 0.5f));
            levelText.text = System.String.Format("Lvl {0}", info.level.ToString());
            expText.text = System.String.Format("Exp: {0}/{1}", info.totalExperience, info.GetExpNeededToLevelUp());
            statsText.text = System.String.Format("HP: {0} ATK: {1} DEF: {2} MATK: {3} \n MDEF: {4} DEX: {5} LUK: {6} MOV: {7}",
                                             info.baseHealth, info.baseAttack, info.baseDefense, info.baseMagicAttack,
                                             info.baseMagicDefense, info.baseDexterity, info.baseLuck, info.baseMov);

            //add click event
            Button unitSlotButton = unitSlot.transform.Find("Button").GetComponent<Button>();
            unitSlotButton.onClick.AddListener(onUnitSlotClick);

            unitSlot.transform.SetParent(objectToFill.transform, true);
            //unitSlot.transform.position = unitSlot.transform.position;
        }
    }

    private void onUnitSlotClick() {
        GameObject unitSlotButton = EventSystem.current.currentSelectedGameObject;
        string playerInfoId = unitSlotButton.transform.parent.gameObject.name.Split("-")[1];
        PlayerInfo playerToSwap = sharedResourceBus.GetMasterGameStateController().GetPlayerInfoById(playerInfoId);
        if (playerToSwap != null) {
            sharedResourceBus.SetPlayerToSwap(playerToSwap);
            clearContainer();
            CloseUnitMenu();
        }
    }

    private void clearContainer() {
        GameObject reservePlayerContainer = unitMenuGo.transform.Find("ReservePlayerContainer/ScrollView/Viewport/Content").gameObject;
        foreach (Transform child in reservePlayerContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

}
