using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using Naninovel;

//Responsible for handling the entire battle state, turn management, commands to move/animate sprite, create subwindow battle and destroy it
public class GameMaster : MonoBehaviour { 

    /*
    Callable Processes
    */
    PlayerSetupStateProcessor playerSetupStateProc;
    PlayerTurnStartProcessor playerTurnStartProc;
    MovePlayerStartStateProcessor movePlayerStartStateProc;
    ShowBattleMenuStateProcessor showBattleMenuStateProc;
    ShowItemMenuStateProcessor showItemMenuStateProc;
    ShowSwapItemMenuStateProcessor showSwapItemMenuStateProc;
    ActionStateProcessor actionStateProc;
    ShowExpGainStateProcessor showExpGainStateProc;
    HandleTileStateProcessor handleTileStateProc;
    PlayerTurnEndProcessor playerTurnEndProc;
    EnemyTurnStateProcessor enemyTurnStateProc;
    GameEndStateProcessor gameEndStateProc;

    // state tracking
    private SharedResourceBus sharedResourceBus;
    private UIHandler uiHandler;

    void Start() {
        sharedResourceBus = GameObject.Find("SharedResourceBus").GetComponent<SharedResourceBus>();
        uiHandler = GameObject.Find("UIHandler").GetComponent<UIHandler>();
        setupProcessors();
        //populate grid with node data
        setupGridComponents();

        //Keep track of player turn ends. Reset this all to false; 
        sharedResourceBus.ResetPlayerTurnEndedDict();
        sharedResourceBus.ResetEnemyTurnEndedDict();
        sharedResourceBus.SetCurrentGameState(GameState.PlayerSetupState);
    }

    // Update is called once per frame
    void Update() {

        Node currentClickNode = uiHandler.GetNodePositionOnClick();
        GameState currentState = sharedResourceBus.GetCurrentGameState();
        sharedResourceBus.SetCurrentClickedNode(currentClickNode);
        
        if (!sharedResourceBus.GetNovelEventManager().IsEventRunning()) {
            switch (currentState) {
                case GameState.PlayerSetupState:
                    playerSetupStateProc.Process();
                    break;
                case GameState.PlayerTurnStart:
                    playerTurnStartProc.Process();                    
                    break;
                case GameState.MovePlayerStartState:
                    movePlayerStartStateProc.Process();
                    break;
                case GameState.ShowBattleMenuState:
                    showBattleMenuStateProc.Process();
                    break;
                case GameState.ShowExpGainState:
                    showExpGainStateProc.Process();
                    break;
                case GameState.ShowItemMenuState:
                    showItemMenuStateProc.Process();
                    break;
                case GameState.SwapItemState:
                    showSwapItemMenuStateProc.Process();
                    break;
                case GameState.AttackState: case GameState.HealState: case GameState.BuffState:
                    actionStateProc.Process();
                    break;
                case GameState.HandleTileState:
                    handleTileStateProc.Process();
                    break;
                case GameState.TurnEndState:
                    playerTurnEndProc.Process();
                    break;
                case GameState.EnemyTurnState:
                    enemyTurnStateProc.Process();
                    break;
                case GameState.GameEndState:
                    gameEndStateProc.Process();
                    break;
            }
        }
    }

    // Critical grid handling setup//
    private void setupGridComponents() {
        TileEventManager tileEventManager = sharedResourceBus.GetTileEventManager();
        StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
        PawnSpawnManager pawnSpawnManager = sharedResourceBus.GetPawnSpawnManager();
        MasterGameStateController gameStateInstance = sharedResourceBus.GetMasterGameStateController();
        Tilemap tileMap = sharedResourceBus.GetTileMap();
        tileEventManager.Setup(tileMap);
        staticTileHandler.Setup(tileMap);
        Dictionary<Vector2, PlayerInfo> pawnInfoDict = sharedResourceBus.GetPawnInfoDict();
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        pawnSpawnManager.Setup(tileMap, pawnInfoDict, nodeDict, gameStateInstance, playerSetupStateProc.GetValidPawnPlacements()); 
        gameStateInstance.SaveInfoBeforeBattle(); 
    }

    private void setupProcessors() {
        playerSetupStateProc = gameObject.GetComponent<PlayerSetupStateProcessor>();
        playerTurnStartProc = gameObject.AddComponent<PlayerTurnStartProcessor>();
        movePlayerStartStateProc = gameObject.AddComponent<MovePlayerStartStateProcessor>();
        showBattleMenuStateProc = gameObject.AddComponent<ShowBattleMenuStateProcessor>();
        showItemMenuStateProc = gameObject.AddComponent<ShowItemMenuStateProcessor>();
        showSwapItemMenuStateProc = gameObject.AddComponent<ShowSwapItemMenuStateProcessor>();
        actionStateProc = gameObject.AddComponent<ActionStateProcessor>();
        showExpGainStateProc = gameObject.AddComponent<ShowExpGainStateProcessor>();
        handleTileStateProc = gameObject.AddComponent<HandleTileStateProcessor>();
        playerTurnEndProc = gameObject.AddComponent<PlayerTurnEndProcessor>();
        enemyTurnStateProc = gameObject.AddComponent<EnemyTurnStateProcessor>();
        gameEndStateProc = gameObject.AddComponent<GameEndStateProcessor>();
    }

}
