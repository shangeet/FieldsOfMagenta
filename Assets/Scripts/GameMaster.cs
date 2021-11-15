using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

//Responsible for handling the entire game state, turn management, commands to move/animate sprite, create subwindow battle and destroy it
public class GameMaster : MonoBehaviour {

    private Dictionary<Vector2, PlayerInfo> playerInfoDict;
    private Dictionary<Vector2, PlayerInfo> enemyInfoDict;
    private Dictionary<Vector3Int, Node> nodeDict = new Dictionary<Vector3Int, Node>();
    [SerializeField] private List<Vector2> obstaclesList;
    Tilemap tileMap;
    List<Node> clickedNodePath;
    Node clickedPlayerNode;
    Node previousClickedNode;
    Node clickedNode;

    private Dictionary<string, bool> playerTurnEndedDict = new Dictionary<string, bool>();
    private List<string> retreatedPlayers = new List<string>(); 

    // UI elements and state tracking
    GameObject playerBattleMenu; 
    bool playerBattleMenuDisplayed;

    Node enemyNodeToAttack;

    GameState currentState;

    void Awake() {
        print("Awake");
        //Setup: We will delete this after we have a way to transfer info a file/read from file
        populateGridSetupData();
        //Setup the UI's. Disable them at start
        setupUIElements();
        //Keep track of player turn ends. Reset this all to false; 
        resetPlayerTurnEndedDict();
        currentState = GameState.PlayerTurnStart;
    }
    void Start() {}

    // Update is called once per frame
    void Update() {

        Node currentClickNode = getNodePositionOnClick();

        switch (currentState) {
            case GameState.PlayerTurnStart:
                processPlayerTurnStartState(currentClickNode);                    
                break;
            case GameState.ShowUnitInfoState:
                processShowUnitInfoState(currentClickNode);
                break;
            case GameState.MovePlayerStartState:
                processMovePlayerStartState(currentClickNode);
                break;
            case GameState.ShowBattleMenuState:
                processShowBattleMenuState();
                break;
            case GameState.ShowItemMenuState:
                processShowItemMenuState();
                break;
            case GameState.UseItemState:
                processUseItemState();
                break;
            case GameState.AttackState:
                processAttackState(currentClickNode);
                break;
            case GameState.ShowAttackBattleUIState:
                processShowAttackBattleUIState();
                break;
            case GameState.TurnEndState:
                processTurnEndState();
                break;
            case GameState.EnemyTurnState:
                processEnemyTurnState();
                break;
        }

    } 

    // High level State handling functions
    void processPlayerTurnStartState(Node currentClickedNode) {
        if (currentClickedNode != null) {
            clickedNode = currentClickedNode;
            if (Utils.nodeClickedIsEnemy(clickedNode) || (Utils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == true)) {
                currentState = GameState.ShowUnitInfoState;
            } else if (Utils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == false) {
                clickedNodePath = Utils.getViableNodesPaths(clickedNode.getPlayerInfo().getMov(), clickedNode, nodeDict);
                clickedPlayerNode = clickedNode;
                foreach (Node node in clickedNodePath) {
                    tileMap.SetColor(node.getPosition(), Color.blue);
                }
                currentState = GameState.MovePlayerStartState;
            }
        }
        //else nothing happens
    }

    void processShowUnitInfoState(Node currentClickedNode) {
        if (currentClickedNode == null) {
            //we stay in this state
            print("TODO Implement UI for ShowUnitInfoState");          
        } else {
            if (Utils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == true) { //Player clicked already ended turn, just show details
                //update UI with new details
                currentState = GameState.ShowUnitInfoState;
            } else if(Utils.nodeClickedIsEnemy(clickedNode)) { //Player clicked is an enemy. Just show details
                //update UI with new details
                currentState = GameState.ShowUnitInfoState;
            } else if(Utils.nodeClickedIsPlayer(clickedNode)) { //Player clicked is a player and they haven't ended their turn. Start move state
                currentState = GameState.MovePlayerStartState;
            } else { //Node wasn't a player or enemy. Disable UI element and go back to Start state 
                currentState = GameState.PlayerTurnStart;
            }  
        }
    }

    void processMovePlayerStartState(Node currentClickedNode) {
        print("Entered move player start state");
        if (currentClickedNode == clickedNode) { //player node clicked on again, open battle menu and reset colors
            currentState = GameState.ShowBattleMenuState;
            foreach (Node node in clickedNodePath) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
        } else if (currentClickedNode != null) {
            previousClickedNode = clickedNode;
            clickedNode = currentClickedNode;
            if(clickedNodePath != null && clickedNodePath.Contains(clickedNode)) {
                string playerId = clickedPlayerNode.getPlayerInfo().getPlayerId();
                Player playerClicked = GameObject.Find(playerId).GetComponent<Player>();
                //move player
                List<Node> pathToTake = Utils.getShortestPathNodes(clickedPlayerNode, clickedNode, clickedNodePath, Heuristic.NodeDistanceHeuristic, nodeDict);
                playerClicked.MovePlayerToTile(tileMap, pathToTake);
                //update player information in nodes
                swapNodeInfoOnSpriteMove(pathToTake[0], pathToTake[pathToTake.Count - 1]);
                clickedPlayerNode = pathToTake[pathToTake.Count - 1];
                //reset state after moving 
                foreach (Node node in clickedNodePath) {
                    tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                }
                currentState = GameState.ShowBattleMenuState; 
            } else { //outside node. go back to turn start state and reset info
                currentState = GameState.PlayerTurnStart;
                resetTurnStateData();
            }
        }
        //else we didn't click a node. stay in this state
    }

    void processShowItemMenuState() {
        openItemMenu();
    }

    void processUseItemState() {
        print("TODO: Process Use Item State");
    }

    void openUnitInfoMenu() {
        print("TODO Implement Unit Info Menu");
    }

    void openItemMenu() {
        print("TODO Implement Item Menu");
    }

    void processShowBattleMenuState() {
        if (!playerBattleMenuDisplayed) {
            openPlayerBattleMenu();            
        }

        if (Input.GetMouseButtonDown(1)) { //cancel movement and track back
            cancelPlayerMove();
            closePlayerBattleMenu();
            resetTurnStateData();
            currentState = GameState.PlayerTurnStart;
        }
    }

    void cancelPlayerMove() {
        print("Cancel Player Move called...");
        string playerId = clickedPlayerNode.getPlayerInfo().getPlayerId();
        Player playerClicked = GameObject.Find(playerId).GetComponent<Player>();
        //move player
        print(clickedPlayerNode.getPosition().x + "," + clickedPlayerNode.getPosition().y);
        print(previousClickedNode.getPosition().x + "," + previousClickedNode.getPosition().y);
        List<Node> pathToTake = new List<Node> {clickedPlayerNode, previousClickedNode};
        playerClicked.MovePlayerToTile(tileMap, pathToTake);
        //update player information in nodes
        swapNodeInfoOnSpriteMove(clickedPlayerNode, previousClickedNode);
    }

    void preProcessAttackState() {
        int playerAttackRange = 1; //TODO Implement this based on player's class
        List<Node> validAttackNodes = Utils.getViableAttackNodes(playerAttackRange, clickedPlayerNode, nodeDict);
        foreach (Node node in validAttackNodes) {
            tileMap.SetColor(node.getPosition(), Color.red);
        }        
    }

    void processAttackState(Node currentClickedNode) {
        int playerAttackRange = 1; //TODO Implement this based on player's class
        List<Node> validAttackNodes = Utils.getViableAttackNodes(playerAttackRange, clickedPlayerNode, nodeDict);
        if (currentClickedNode != null && validAttackNodes.Contains(currentClickedNode)) { //clicked on a valid attack node. Time to attack
            print("Begin attack sequence");
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
            enemyNodeToAttack = currentClickedNode;
            currentState = GameState.ShowAttackBattleUIState;
        } else if (currentClickedNode != null) { // Clicked on a node, but it wasn't a valid one. Go back to battle menu
            currentState = GameState.ShowBattleMenuState;
        }
        //Didn't click on anything. Stay in state
    }

    void processShowAttackBattleUIState() {
        print("TODO: Implement ShowAttackBattleUIState");
        currentState = GameState.TurnEndState;       
    }

    void processTurnEndState() {
        //record end of unit's turn in dict
        playerTurnEndedDict[clickedPlayerNode.getPlayerInfo().getPlayerId()] = true;
        //auto turn-end if all units are done
        if (!playerTurnEndedDict.ContainsValue(false)) {
            print("Player Turn ended");
            resetPlayerTurnEndedDict();
            currentState = GameState.EnemyTurnState;
        } else { //else we go back to the player turn start state.
            currentState = GameState.PlayerTurnStart; 
        }    
        //reset turn state data
        resetTurnStateData();
    }

    //Enemy logic handling// Do NOT move this to external class
    void processEnemyTurnState() {
        //process action for each enemy 
        foreach (PlayerInfo enemy in enemyInfoDict.Values) {
            //if player in line of sight, move and attack
            Node enemyNode = Utils.findEnemyNode(enemy.getPlayerId(), nodeDict);
            List<Node> nodeRange = Utils.getViableNodesPaths(enemyNode.getPlayerInfo().getMov(), enemyNode, nodeDict);
            Node candidatePlayerNode = Utils.findPlayerNodeNearEnemy(enemyNode, enemyNode.getPlayerInfo().getMov(), nodeDict);
            print(candidatePlayerNode);
            if (candidatePlayerNode != null) {            
                //TODO Attack logic
                //check if we have to move or not
                if (Utils.getNearbyNodes(enemyNode, nodeDict).Contains(candidatePlayerNode)) { //just attack
                    print("Just attack.");
                } else { //move and attack
                    List<Node> pathToMove = Utils.getShortestPathNodes(enemyNode, candidatePlayerNode, nodeRange, Heuristic.NodeDistanceHeuristic, nodeDict);
                    Player enemyToMove = GameObject.Find(enemy.getPlayerId()).GetComponent<Player>();
                    pathToMove.RemoveAt(pathToMove.Count - 1); //remove the last element since that's the player
                    enemyToMove.MoveEnemyNextToPlayer(tileMap, pathToMove);
                    swapNodeInfoOnSpriteMove(enemyNode, pathToMove[pathToMove.Count - 1]); //swap node data with new tile                        
                    print("Move and attack.");
                }
            } else { //if not in line of sight, wait
                print("No player found.");
            }
        }
        currentState = GameState.PlayerTurnStart;
        //done processing, end turn
    }

    // Critical information handling // Do NOT move this to an external class
    void populateGridSetupData() {
        playerInfoDict = new Dictionary<Vector2, PlayerInfo>();
        PlayerInfo testOne = new PlayerInfo("fakeid", "images/sprites/SampleSprite", false, 2);
        PlayerInfo testTwo = new PlayerInfo("fakeid2", "images/sprites/SampleSprite", false, 2);
        playerInfoDict[new Vector2(0, 0)] = testOne;
        playerInfoDict[new Vector2(3,4)] = testTwo;
        enemyInfoDict = new Dictionary<Vector2, PlayerInfo>();
        PlayerInfo enemyOne = new PlayerInfo("fakeenemyid", "images/sprites/SampleSprite", true, 4);
        PlayerInfo enemyTwo = new PlayerInfo("fakeenemyid2", "images/sprites/SampleSprite", true, 4);
        enemyInfoDict[new Vector2(2,2)] = enemyOne;
        //enemyInfoDict[new Vector2(-3,-3)] = enemyTwo;
        Vector2 obstaclePos = new Vector2(0,1);
        obstaclesList = new List<Vector2>();
        obstaclesList.Add(obstaclePos);  
        //set node data for player and enemies MAP TILEMAP POSITION TO NODE POSITION
        tileMap = transform.GetComponentInParent<Tilemap>();
        foreach (var position in tileMap.cellBounds.allPositionsWithin) {
            Vector2 pos = new Vector2(position.x, position.y);
            bool hasObstacle = false;
            PlayerInfo playerInfo = null;
            if (playerInfoDict.ContainsKey(pos)) {
                playerInfo = playerInfoDict[pos];
                populateGridWithSprite(pos, playerInfoDict, position, playerInfo);
            } else if (enemyInfoDict.ContainsKey(pos)) {
                playerInfo = enemyInfoDict[pos];
                populateGridWithSprite(pos, enemyInfoDict, position, playerInfo);
            } else if (obstaclesList.Contains(pos)) {
                hasObstacle = true;
            }
            tileMap.SetTileFlags(position, TileFlags.None); //this is so we can change the color freely
            Color originalTileColor = tileMap.GetColor(position);
            nodeDict[position] = new Node(position, hasObstacle, playerInfo, originalTileColor);
        }
    }

    void resetPlayerTurnEndedDict() {
        foreach (PlayerInfo pi in playerInfoDict.Values) {
            playerTurnEndedDict[pi.getPlayerId()] = false;
        }
    }

    void populateGridWithSprite(Vector2 pos2D, Dictionary<Vector2, PlayerInfo> infoDict, Vector3Int pos3D, PlayerInfo playerInfo) {
        string playerId = playerInfo.getPlayerId();
        GameObject objToSpawn = new GameObject(playerId);
        Player newPawn = objToSpawn.AddComponent<Player>() as Player;
        newPawn.Setup(playerInfo);
        newPawn.name = playerId;
        if (newPawn) {
            newPawn.AddSpriteToTile(tileMap, pos3D);
        }        
        print("Sprite added at position: " + pos2D.x + "," + pos2D.y);
    }

    void swapNodeInfoOnSpriteMove(Node source, Node dest) {
        PlayerInfo playerInfo = source.getPlayerInfo();
        dest.setPlayerInfo(playerInfo);
        source.setPlayerInfo(null);
        //finally, we update the node dict to reflect this
        nodeDict[source.getPosition()] = source;
        nodeDict[dest.getPosition()] = dest;
        print("Updated node info!"); 
    }

    void resetTurnStateData() {
        clickedNode = null;
        clickedPlayerNode = null;
        previousClickedNode = null;
        clickedNodePath = null;
        enemyNodeToAttack = null;
    }

    //UI related logic
    Node getNodePositionOnClick() {
        if (Input.GetMouseButtonDown(0)) {
            print("Click detected");
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 globalPosition = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3Int localPosition = tileMap.WorldToCell(globalPosition);
            Utils.printPositionToConsole(localPosition);
            print(nodeDict.ContainsKey(localPosition));
            return nodeDict.ContainsKey(localPosition) ? nodeDict[localPosition] : null;
        }
        return null;
    }

    void setupUIElements() {
        playerBattleMenu = GameObject.Find("PlayerBattleMenu");
        Button attackButton = GameObject.Find("AttackButton").GetComponent<Button>() as Button;
        print(attackButton);
        Button itemButton = GameObject.Find("ItemButton").GetComponent<Button>() as Button;
        Button waitButton = GameObject.Find("WaitButton").GetComponent<Button>() as Button;
        attackButton.onClick.AddListener(onAttackButtonClick);
        itemButton.onClick.AddListener(onItemButtonClick);
        waitButton.onClick.AddListener(onWaitButtonClick);
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }

    void openPlayerBattleMenu() {       
        playerBattleMenu.SetActive(true);
        playerBattleMenuDisplayed = true;
        print(playerBattleMenuDisplayed);
        print("Player Battle menu opened");
        string playerId = clickedPlayerNode.getPlayerInfo().getPlayerId();
        print(playerId);
        Player playerClicked = GameObject.Find(playerId).GetComponent<Player>();
        Vector3 playerSpriteVector = playerClicked.transform.position; //global position where I want the menu to appear
        print(playerSpriteVector); //(-0.5, 1.5, 0)
        RectTransform rt = playerBattleMenu.transform.GetChild(0).GetComponent<RectTransform>();
        Vector3 newPos = new Vector3(playerSpriteVector.x + 2, playerSpriteVector.y + 2, 0);
        rt.anchoredPosition = newPos;
    }

    void closePlayerBattleMenu() {
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;
    }
    void onAttackButtonClick() {
        print("Attack Button was clicked!");
        closePlayerBattleMenu();
        preProcessAttackState();
        currentState = GameState.AttackState;
    }
    void onItemButtonClick() {
        print("Item Button was clicked! TODO: Implement items");
        closePlayerBattleMenu();
        currentState = GameState.TurnEndState;
    }
    void onWaitButtonClick() {
        print("Wait Button was clicked!");
        closePlayerBattleMenu();
        currentState = GameState.TurnEndState;
    }
}



