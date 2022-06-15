using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSetupStateProcessor : StateProcessor {

    [SerializeField]
    public string levelName;
    public PlayerSetupMenu playerSetupMenu;

    private List<Vector2> validPawnPlacements;
    private List<Vector2> currentPlacements;
    private bool swappableColored = false;
    private Node firstNodeClicked = null;
    private StaticTileHandler staticTileHandler;

    void Start() {
        //read possible pawn placements
        validPawnPlacements = FileUtils.GetValidPlayerPlacements(levelName);
        playerSetupMenu = uiHandler.GetPlayerSetupMenu();
        staticTileHandler = sharedResourceBus.GetStaticTileHandler();
        sharedResourceBus.SetMaxActivePlayers(FileUtils.GetMaxNumberActivePlayers(levelName));
    }

    public override bool Process() {
        SetupState currentSetupState = sharedResourceBus.GetPlayerSetupMenuState();
        Node currentNodeClicked = sharedResourceBus.GetCurrentClickedNode();

        switch(currentSetupState) {
            case SetupState.SETUP_MENU_OPENED_MODE:
                playerSetupMenu.OpenPlayerSetupMenu();
                sharedResourceBus.SetPlayerSetupMenuState(SetupState.WAIT_INPUT_MODE);
                return false;
            case SetupState.WAIT_INPUT_MODE:
                return false;
            case SetupState.PLAYER_SWAP_MODE:

                if (Input.GetMouseButtonDown(1)) { //cancel swap state
                    if (firstNodeClicked != null) {
                        staticTileHandler.DespawnTile(firstNodeClicked.getPosition());
                        firstNodeClicked = null;                        
                    }
                    playerSetupMenu.OpenPlayerSetupMenu();
                    sharedResourceBus.SetPlayerSetupMenuState(SetupState.WAIT_INPUT_MODE);
                    return false;
                }
                
                
                if (firstNodeClicked != null && currentNodeClicked != null) { //activate swapping logic
                    // swap if one or both of them contains a player
                    
                    if (isValidPlacement(firstNodeClicked, currentNodeClicked)) {
                        //swap node info and sprites
                        PlayerInfo playerInfoFirst = firstNodeClicked.getPlayerInfo();
                        PlayerInfo playerInfoSecond = currentNodeClicked.getPlayerInfo();

                        if (playerInfoFirst != null) {
                            playerInfoFirst.playerAnimator.PlayerReturnToTile(sharedResourceBus.GetTileMap(), currentNodeClicked);
                        }

                        if (playerInfoSecond != null) {
                            playerInfoSecond.playerAnimator.PlayerReturnToTile(sharedResourceBus.GetTileMap(), firstNodeClicked);
                        }
                    
                        SwapNodeInfoOnSpriteMove(firstNodeClicked, currentNodeClicked);

                        //remove colored tile + reset node
                        staticTileHandler.DespawnTile(firstNodeClicked.getPosition());
                        firstNodeClicked = null;
                    }
                } else { //set first node if it's valid
                    if (currentNodeClicked != null) {
                        Vector3Int nodePos = currentNodeClicked.getPosition();
                        Vector2 nodePos2D = new Vector2(nodePos.x, nodePos.y);
                        if (validPawnPlacements.Contains(nodePos2D)) {
                            firstNodeClicked = currentNodeClicked;
                            staticTileHandler.SpawnTile(nodePos, "YellowTile");   
                        }                         
                    }
                }
                return false;
            case SetupState.UNIT_SETUP_MODE:
                //exit to main menu if left click
                if (Input.GetMouseButtonDown(1)) { //cancel unit setup state
                    firstNodeClicked = null;
                    playerSetupMenu.CloseUnitMenu();
                    sharedResourceBus.SetPlayerToSwap(null);
                    //despawn all tiles with players
                    foreach (KeyValuePair<Vector2, PlayerInfo> pair in sharedResourceBus.GetPawnInfoDict()) {
                        if (!pair.Value.getIsEnemy()) {
                            Vector3Int pos3D = new Vector3Int(Mathf.RoundToInt(pair.Key.x), Mathf.RoundToInt(pair.Key.y), 0);
                            staticTileHandler.DespawnTile(pos3D);                                
                        }
                    }                    
                    playerSetupMenu.OpenPlayerSetupMenu();
                    sharedResourceBus.SetPlayerSetupMenuState(SetupState.WAIT_INPUT_MODE);
                    return false;
                }                


                if (firstNodeClicked != null) {
                    //check to see if we need to swap on the backend
                    PlayerInfo playerToSwap = sharedResourceBus.GetPlayerToSwap();
                    if (playerToSwap != null) {
                        Vector3Int nodePos = firstNodeClicked.getPosition();
                        Dictionary<Vector2, PlayerInfo> pawnInfoDict = sharedResourceBus.GetPawnInfoDict();
                        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
                        PawnSpawnManager pawnSpawnManager = sharedResourceBus.GetPawnSpawnManager();
                        Tilemap tileMap = sharedResourceBus.GetTileMap();

                        // delete old player sprite
                        PlayerInfo oldPlayer = firstNodeClicked.getPlayerInfo();
                        pawnSpawnManager.DeletePlayerSpriteOnGrid(oldPlayer);

                        // setup new player sprite
                        playerToSwap = pawnSpawnManager.AddSpriteToGrid(tileMap, nodePos, playerToSwap);
                        
                        // setup new player data
                        playerToSwap.setupBattleStats(true);

                        //despawn red tile, revert to yellow
                        staticTileHandler.DespawnTile(nodePos);
                        
                        // update data in nodes
                        nodeDict[nodePos].setPlayerInfo(playerToSwap);
                        pawnInfoDict[new Vector2(nodePos.x, nodePos.y)] = playerToSwap;
                        sharedResourceBus.SetNodeDict(nodeDict);
                        sharedResourceBus.SetPawnInfoDict(pawnInfoDict);
                        sharedResourceBus.ResetPlayerTurnEndedDict();
                        sharedResourceBus.SetPlayerToSwap(null);
                        sharedResourceBus.SetFirstPlayerClicked(false);
                        firstNodeClicked = null;
                        playerSetupMenu.CloseUnitMenu();
                        return true;
                    }
                    return false;
                } else {
                    //color the map yellow for swappable players
                    if (!swappableColored) {
                        foreach (KeyValuePair<Vector2, PlayerInfo> pair in sharedResourceBus.GetPawnInfoDict()) {
                            if (!pair.Value.getIsEnemy()) {
                                Vector3Int pos3D = new Vector3Int(Mathf.RoundToInt(pair.Key.x), Mathf.RoundToInt(pair.Key.y), 0);
                                staticTileHandler.SpawnTile(pos3D, "YellowTile");                                
                            }
                        }
                        swappableColored = true;                    
                    }

                    if (clickedNodeIsPlayer(currentNodeClicked)) {
                        firstNodeClicked = currentNodeClicked;
                        Vector3Int nodePos = firstNodeClicked.getPosition();
                        staticTileHandler.DespawnTile(nodePos);
                        staticTileHandler.SpawnTile(nodePos, "RedTile");
                        sharedResourceBus.SetFirstPlayerClicked(true);
                        playerSetupMenu.OpenUnitMenu();
                        return true;
                    }
                    return false;
                }
            case SetupState.SETUP_MENU_CLOSED_MODE:
                playerSetupMenu.ClosePlayerSetupMenu();
                ChangeState(GameState.PlayerTurnStart);
                return true;
            default:
                return false;
        }
    }

    public bool processPlayerSetupPhase() {
        return true;
    }

    public List<Vector2> GetValidPawnPlacements() {
        return validPawnPlacements;
    }

    public List<Vector2> GetCurrentPlacements() {
       return currentPlacements; 
    }

    public bool IsFinishedPawnSetup() {
        return true;
    }

    private void changePawnPosition() {}

    private bool isValidPlacement(Node firstNodeClicked, Node currentNodeClicked) {
        if ((firstNodeClicked.isOccupied() && firstNodeClicked.getPlayerInfo().getIsEnemy()) || (currentNodeClicked.isOccupied() && currentNodeClicked.getPlayerInfo().getIsEnemy())) {
            return false;
        }
        Vector3Int nodePos = currentNodeClicked.getPosition();
        Vector2 currNodePos2D = new Vector2(nodePos.x, nodePos.y);
        bool isValidNodePlacement = validPawnPlacements.Contains(currNodePos2D) && firstNodeClicked.getPosition() != currentNodeClicked.getPosition();
        return (firstNodeClicked.isOccupied() || currentNodeClicked.isOccupied()) && isValidNodePlacement;
    }

    private bool clickedNodeIsPlayer(Node node) {
        return node != null && node.isOccupied() &&  !node.getPlayerInfo().getIsEnemy();
    }

}
