using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UIHandler : MonoBehaviour {
    
    [SerializeField]
    public GameObject swapItemMenuGo;
    [SerializeField]
    public GameObject playerBattleMenuGo;
    [SerializeField]
    public GameObject unitInfoMenuGo;
    [SerializeField]
    public GameObject battleEventScreenGo;
    [SerializeField]
    public GameObject itemMenuGo;
    [SerializeField]
    public GameObject playerExpScreenGo;
    [SerializeField]
    public GameObject expGainMenuGo;
    [SerializeField]
    public GameObject playerPhaseImageGo;
    [SerializeField]
    public GameObject enemyPhaseImageGo;
    [SerializeField]
    public GameObject playerVictoryScreen;
    [SerializeField]
    public GameObject playerDefeatScreen;
    [SerializeField]
    public GameObject playerSetupMenuGo;

    private SwapItemsMenu swapItemsMenu;
    private PlayerBattleMenu playerBattleMenu;
    private UnitInfoMenu unitInfoMenu;
    private BattleEventScreen battleEventScreen;
    private ItemMenu itemMenu;
    private PlayerSetupMenu playerSetupMenu;
    private PhaseTransitionUIHandler phaseTransitionUIHandler;
    private PlayerExpScreen playerExpScreen;
    private SharedResourceBus sharedResourceBus;
    
    /*
    UI related toggle vars
    */
    public bool startedTranslation = false;
    private bool inputEnabled = true;

    void Awake() {
        //Setup the UI's. Disable them at start
        sharedResourceBus = GameObject.Find("SharedResourceBus").GetComponent<SharedResourceBus>();
        setupUIElements();
    }


    private void setupUIElements() {

        swapItemMenuGo = Instantiate<GameObject>(swapItemMenuGo);
        swapItemMenuGo.name = "ItemSwapMenu";
        playerBattleMenuGo = Instantiate<GameObject>(playerBattleMenuGo);
        playerBattleMenuGo.name = "PlayerBattleMenu";
        unitInfoMenuGo = Instantiate<GameObject>(unitInfoMenuGo);
        unitInfoMenuGo.name = "UnitInfoMenu";
        itemMenuGo = Instantiate<GameObject>(itemMenuGo);
        itemMenuGo.name = "ItemMenu";
        battleEventScreenGo = Instantiate<GameObject>(battleEventScreenGo);
        battleEventScreenGo.name = "BattleEventScreen";
        expGainMenuGo = Instantiate<GameObject>(expGainMenuGo);
        expGainMenuGo.name = "ExpGainMenu";
        playerExpScreenGo = Instantiate<GameObject>(playerExpScreenGo);
        playerExpScreenGo.name = "PlayerExpScreen";
        playerPhaseImageGo = Instantiate<GameObject>(playerPhaseImageGo);
        playerPhaseImageGo.name = "PlayerPhaseImg";
        enemyPhaseImageGo = Instantiate<GameObject>(enemyPhaseImageGo);
        enemyPhaseImageGo.name = "EnemyPhaseImg";
        playerSetupMenuGo = Instantiate<GameObject>(playerSetupMenuGo);
        playerSetupMenuGo.name = "PlayerSetupMenu";


        swapItemsMenu = gameObject.AddComponent<SwapItemsMenu>();
        playerBattleMenu = gameObject.AddComponent<PlayerBattleMenu>();  
        unitInfoMenu = gameObject.AddComponent<UnitInfoMenu>();
        itemMenu = gameObject.AddComponent<ItemMenu>();
        battleEventScreen = gameObject.AddComponent<BattleEventScreen>();
        playerExpScreen = gameObject.AddComponent<PlayerExpScreen>();
        phaseTransitionUIHandler = gameObject.AddComponent<PhaseTransitionUIHandler>();
        playerSetupMenu = gameObject.AddComponent<PlayerSetupMenu>();
        playerVictoryScreen = Instantiate<GameObject>(playerVictoryScreen);
        playerDefeatScreen = Instantiate<GameObject>(playerDefeatScreen);
        playerVictoryScreen.SetActive(false);
        playerDefeatScreen.SetActive(false);
    }

    public PlayerBattleMenu GetPlayerBattleMenu() {
        return playerBattleMenu;
    }

    public ItemMenu GetItemMenu() {
        return itemMenu;
    }

    public BattleEventScreen GetBattleEventScreen() {
        return battleEventScreen;
    }

    public PlayerExpScreen GetPlayerExpScreen() {
        return playerExpScreen;
    }

    public PhaseTransitionUIHandler GetPhaseTransitionUIHandler() {
        return phaseTransitionUIHandler;
    }

    public bool StartedTranslation() {
        return startedTranslation;
    }

    public void SetStartedTranslation(bool hasStartedTranslation) {
        startedTranslation = hasStartedTranslation;
    }

    public GameObject GetPlayerVictoryScreen() {
        return playerVictoryScreen;
    }

    public GameObject GetPlayerDefeatScreen() {
        return playerDefeatScreen;
    }

    public bool UIAnimationsPlaying() {
        return battleEventScreen.IsBattleEventScreenDisplayed() || phaseTransitionUIHandler.IsPhaseTransitionRunning() || playerExpScreen.IsExperienceScreenProcessing();
    }

    public Node GetNodePositionOnClick() {
        if (Input.GetMouseButtonDown(0) && inputEnabled) {
            print("Getting new input...");
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 globalPosition = Camera.main.ScreenToWorldPoint(mousePos);
            Tilemap tileMap = sharedResourceBus.GetTileMap();
            Vector3Int localPosition = tileMap.WorldToCell(globalPosition);
            Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
            return nodeDict.ContainsKey(localPosition) ? nodeDict[localPosition] : null;
        }
        return null;
    }

    public void DisableInput() {
        // EventSystem eventSystem = GameObject.Find("GameEventSystem").GetComponent<EventSystem>();
        // eventSystem.enabled = false;
        inputEnabled = false;
    }

    public void EnableInput() {
        // EventSystem eventSystem = GameObject.Find("GameEventSystem").GetComponent<EventSystem>();
        // eventSystem.enabled = true;        
        inputEnabled = true;
    }

    public bool IsInputEnabled() {
        return inputEnabled;
    }

    public SwapItemsMenu GetSwapItemMenu() {
        return swapItemsMenu;
    }

    public UnitInfoMenu GetUnitInfoMenu() {
        return unitInfoMenu;
    }

    public PlayerSetupMenu GetPlayerSetupMenu() {
        return playerSetupMenu;
    }

}
