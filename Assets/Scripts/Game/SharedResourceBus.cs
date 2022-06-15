using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SetupState { SETUP_MENU_OPENED_MODE , WAIT_INPUT_MODE, PLAYER_SWAP_MODE, UNIT_SETUP_MODE, SETUP_MENU_CLOSED_MODE }
public class SharedResourceBus : MonoBehaviour {

    /*
    Handlers for Tiles, Novel Events
    */
    private StaticTileHandler staticTileHandler;
    private TileEventManager tileEventManager;
    private NovelEventManager novelEventManager; 
    private PawnSpawnManager pawnSpawnManager;

    /*
    Gamestate Info
    */
    private GameState currentState;
    private MasterGameStateController gameStateInstance;

    /*
    Tilemap and Node related Vars
    */
    private Dictionary<Vector2, PlayerInfo> pawnInfoDict = new Dictionary<Vector2, PlayerInfo>();
    private Dictionary<Vector3Int, Node> nodeDict = new Dictionary<Vector3Int, Node>();
    private Tilemap tileMap;
    private List<Node> clickedNodePath;
    private Node clickedPlayerNode;
    private Node previousClickedNode;
    private Node clickedNode;
    private Node currentClickedNode;

    /*
    Key logic related vars
    */
    private bool allEnemiesHaveMoved = false;
    private bool playerCurrentlyMoving = false;
    private bool handleStatusEnemyTurn = false;
    private bool highlightedPossibleSwapPartner = false;
    
    // Misc
    private bool playerVictory;
    private bool isEnemyTurn;

    private Dictionary<string, bool> playerTurnEndedDict = new Dictionary<string, bool>();
    private Dictionary<string, bool> enemyTurnEndedDict = new Dictionary<string, bool>();
    private List<PlayerInfo> retreatedPlayers = new List<PlayerInfo>();
    private List<PlayerInfo> retreatedEnemies = new List<PlayerInfo>();

    /*
    Battle + Exp. Gain related vars
    */
    PlayerInfo oldBattlePI = null;
    PlayerInfo newBattlePI = null;
    int timesLeveledUp = 0;
    List<int> totalExpToLevelUp = new List<int>();
    bool showExpGainBar = false;

    /*
    Substate vars
    */
    private SetupState playerSetupMenuState;

    /*
    State shared vars
    */
    private int maxActivePlayers;
    private bool firstPlayerClicked = false;
    private PlayerInfo playerToSwap;
    private bool unitMenuOpened = false;


    void Awake() {
        //setup handlers + managers
        staticTileHandler = gameObject.GetComponent<StaticTileHandler>();
        tileEventManager = gameObject.GetComponent<TileEventManager>();
        novelEventManager = gameObject.GetComponent<NovelEventManager>();
        pawnSpawnManager = gameObject.GetComponent<PawnSpawnManager>();
        
        gameStateInstance = GameObject.Find("GameMasterInstance").AddComponent<MasterGameStateController>().instance;

        //get tilemap
        tileMap = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();  
        playerSetupMenuState = SetupState.SETUP_MENU_OPENED_MODE;
    }

    /*
    Getters/Setters
    */
    public GameState GetCurrentGameState() {
        return currentState;
    }

    public void SetCurrentGameState(GameState newState) {
        currentState = newState;
    }

    public MasterGameStateController GetMasterGameStateController() {
        return gameStateInstance;
    }
     
    public void SetMasterGameStateController(MasterGameStateController controller) {
        gameStateInstance = controller;
    }

    public Dictionary<Vector2, PlayerInfo> GetPawnInfoDict() {
        return pawnInfoDict;
    }

    public void SetPawnInfoDict(Dictionary<Vector2, PlayerInfo> newPawnInfoDict) {
        pawnInfoDict = newPawnInfoDict;
    }

    public Dictionary<Vector3Int, Node> GetNodeDict() {
        return nodeDict;
    }

    public void SetNodeDict(Dictionary<Vector3Int, Node> newNodeDict) {
        nodeDict = newNodeDict;
    }

    public Tilemap GetTileMap() {
        return tileMap;
    }

    public List<Node> GetClickedNodePath() {
        return clickedNodePath;
    }

    public void SetClickedNodePath(List<Node> newClickedNodePath) {
        clickedNodePath = newClickedNodePath;
    }

    public Node GetClickedPlayerNode() {
        return clickedPlayerNode;
    }

    public void SetClickedPlayerNode(Node newClickedPlayerNode) {
        clickedPlayerNode = newClickedPlayerNode;
    }

    public Node GetPreviousClickedNode() {
        return previousClickedNode;
    }

    public void SetPreviousClickedNode(Node newPreviousClickedNode) {
        previousClickedNode = newPreviousClickedNode;
    }

    public Node GetClickedNode() {
        return clickedNode;
    }

    public void SetClickedNode(Node newClickedNode) {
        clickedNode = newClickedNode;
    }

    public Node GetCurrentClickedNode() {
        return currentClickedNode;
    }

    public void SetCurrentClickedNode(Node newCurrentClickedNode) {
        currentClickedNode = newCurrentClickedNode;
    }

    public bool AllEnemiesHaveMoved() {
        return allEnemiesHaveMoved;
    }

    public void ToggleAllEnemiesHaveMoved() {
        allEnemiesHaveMoved = !allEnemiesHaveMoved;
    }

    public void SetAllEnemiesHaveMoved(bool enemiesHaveMoved) {
        allEnemiesHaveMoved = enemiesHaveMoved;
    }

    public bool PlayerCurrentlyMoving() {
        return playerCurrentlyMoving;
    }

    public void TogglePlayerCurrentlyMoving() {
        playerCurrentlyMoving = !playerCurrentlyMoving;
    }

    public void SetPlayerCurrentlyMoving(bool isPlayerMoving) {
        playerCurrentlyMoving = isPlayerMoving;
    }

    public bool HandleStatusEnemyTurn() {
        return handleStatusEnemyTurn;
    }

    public void ToggleHandleStatusEnemyTurn() {
        handleStatusEnemyTurn = !handleStatusEnemyTurn;
    }

    public void SetHandleStatusEnemyTurn(bool isHandleStatusEnemyTurn) {
        handleStatusEnemyTurn = isHandleStatusEnemyTurn;
    }

    public bool HighlightedPossibleSwapPartner() {
        return highlightedPossibleSwapPartner;
    }

    public void ToggleHighlightedPossibleSwapPartner() {
        highlightedPossibleSwapPartner = !highlightedPossibleSwapPartner;
    }

    public void SetHighlightedPossibleSwapPartner(bool isHighlighted) {
        highlightedPossibleSwapPartner = isHighlighted;
    }

    public Dictionary<string, bool> GetPlayerTurnEndedDict() {
        return playerTurnEndedDict;
    }

    public void SetPlayerTurnEndedDict(Dictionary<string, bool> newPlayerTurnEndedDict) {
        playerTurnEndedDict = newPlayerTurnEndedDict;
    }

    public Dictionary<string, bool> GetEnemyTurnEndedDict() {
        return enemyTurnEndedDict;
    }

    public void SetEnemyTurnEndedDict(Dictionary<string, bool> newEnemyTurnEndedDict) {
        enemyTurnEndedDict = newEnemyTurnEndedDict;
    }

    public List<PlayerInfo> GetRetreatedPlayers() {
        return retreatedPlayers;
    }

    public void SetRetreatedPlayers(List<PlayerInfo> newRetreatedPlayers) {
        retreatedPlayers = newRetreatedPlayers;
    }

    public List<PlayerInfo> GetEnemiesPlayers() {
        return retreatedEnemies;
    }

    public List<PlayerInfo> GetRetreatedEnemies() {
        return retreatedEnemies;
    }

    public void SetRetreatedEnemies(List<PlayerInfo> newRetreatedEnemies) {
        retreatedEnemies = newRetreatedEnemies;
    }
    
    public StaticTileHandler GetStaticTileHandler() {
        return staticTileHandler;
    }

    public TileEventManager GetTileEventManager() {
        return tileEventManager;
    }

    public NovelEventManager GetNovelEventManager() {
        return novelEventManager;
    }

    public PawnSpawnManager GetPawnSpawnManager() {
        return pawnSpawnManager;
    }

    public PlayerInfo GetOldBattlePI() {
        return oldBattlePI;
    }

    public void SetOldBattlePI(PlayerInfo battlePI) {
        oldBattlePI = battlePI;
    }

    public PlayerInfo GetNewBattlePI() {
        return newBattlePI;
    }

    public void SetNewBattlePI(PlayerInfo battlePI) {
        newBattlePI = battlePI;
    }

    public int GetTimesLeveledUp() {
        return timesLeveledUp;
    }

    public void SetTimesLeveledUp(int leveledUp) {
        timesLeveledUp = leveledUp;
    }

    public List<int> GetTotalExpToLevelUp() {
        return totalExpToLevelUp;
    }

    public void SetTotalExpToLevelUp(List<int> newTotalExpToLevelUp) {
        totalExpToLevelUp = newTotalExpToLevelUp;
    }

    public bool ShowExpGainBar() {
        return showExpGainBar;
    }

    public void SetShowExpGainBar(bool showBar) {
        showExpGainBar = showBar;
    }

    public bool PlayerVictory() {
        return playerVictory;
    }

    public void SetPlayerVictory(bool isPlayerVictory) {
        playerVictory = isPlayerVictory;
    }

    public bool IsEnemyTurn() {
        return isEnemyTurn;
    }

    public void SetEnemyTurn(bool enemyTurn) {
        isEnemyTurn = enemyTurn;
    }

    public void ResetPlayerTurnEndedDict() {
        playerTurnEndedDict = new Dictionary<string, bool>();
        foreach (PlayerInfo pi in pawnInfoDict.Values) {
            if (!pi.getIsEnemy()) {
                if (retreatedPlayers.Contains(pi)) {
                    playerTurnEndedDict[pi.getPlayerId()] = true;
                } else {
                    playerTurnEndedDict[pi.getPlayerId()] = false;
                    pi.playerAnimator.AnimateRevertSpriteTurnStarted();                
                }                
            }
        }
    }

    public void ResetEnemyTurnEndedDict() {
        foreach (PlayerInfo pi in pawnInfoDict.Values) {
            if (pi.getIsEnemy()) {
                if (retreatedEnemies.Contains(pi)) {
                    enemyTurnEndedDict[pi.getPlayerId()] = true;
                } else if (pi.getIsEnemy()) {
                    enemyTurnEndedDict[pi.getPlayerId()] = false;                
                }                
            }
        }     
    }

    public Node getNodeAtPosition(Vector3Int pos) {
        return nodeDict.ContainsKey(pos)? nodeDict[pos] : null;
    }

    public PlayerInfo GetPlayerInfoAtPos(Vector3Int pos) {
        return nodeDict.ContainsKey(pos) ? nodeDict[pos].getPlayerInfo() : null;
    }

    public List<PlayerInfo> GetAllPlayerInfos() {
        List<PlayerInfo> allPIs = new List<PlayerInfo>();

        foreach (Node node in nodeDict.Values) {
            if (node.isOccupied()) {
                allPIs.Add(node.getPlayerInfo());
            }
        }

        foreach (PlayerInfo info in retreatedEnemies) {
            allPIs.Add(info);
        }
        
        foreach (PlayerInfo info in retreatedPlayers) {
            allPIs.Add(info);
        }

        return allPIs;
    }

    public Vector3Int GetPlayerPosition(PlayerInfo info) {
        foreach (Vector3Int pos in nodeDict.Keys) {
            Node node = nodeDict[pos];
            if (node.isOccupied() && node.getPlayerInfo().id.Equals(info.id)) {
                return pos;
            }
        }
        return new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }

    public SetupState GetPlayerSetupMenuState() {
        return playerSetupMenuState;
    }

    public void SetPlayerSetupMenuState(SetupState newSetupState) {
        playerSetupMenuState = newSetupState;
    }

    public int GetMaxActivePlayers() {
        return maxActivePlayers;
    }

    public void SetMaxActivePlayers(int activePlayers) {
        maxActivePlayers = activePlayers;
    }

    public bool GetFirstPlayerClicked() {
        return firstPlayerClicked;
    }

    public void SetFirstPlayerClicked(bool playerClicked) {
        firstPlayerClicked = playerClicked;
    }

    public PlayerInfo GetPlayerToSwap() {
        return playerToSwap;
    }

    public void SetPlayerToSwap(PlayerInfo player) {
        playerToSwap = player;
    }

    public bool IsUnitMenuOpened() {
        return unitMenuOpened;
    }

    public void SetUnitMenuOpened(bool isOpen) {
        unitMenuOpened = isOpen;
    }
}
