using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

//Responsible for handling the entire game state, turn management, commands to move/animate sprite, create subwindow battle and destroy it
public class GameMaster : MonoBehaviour {

    private Dictionary<Vector2, PlayerInfo> pawnInfoDict;
    private Dictionary<Vector3Int, Node> nodeDict = new Dictionary<Vector3Int, Node>();
    [SerializeField] private List<Vector2> obstaclesList;
    Tilemap tileMap;
    List<Node> clickedNodePath;
    Node clickedPlayerNode;
    Node previousClickedNode;
    Node clickedNode;
    private Dictionary<string, bool> playerTurnEndedDict = new Dictionary<string, bool>();
    private Dictionary<string, bool> enemyTurnEndedDict = new Dictionary<string, bool>();
    private List<string> retreatedPlayers = new List<string>();
    private List<string> retreatedEnemies = new List<string>(); 

    // UI elements and state tracking
    GameState currentState;
    GameObject playerBattleMenu;
    GameObject unitInfoMenu; 
    GameObject battleEventScreen;
    GameObject playerPhaseTransitionImage;
    GameObject enemyPhaseTransitionImage;

    bool battleEventScreenDisplayed;
    bool playerBattleMenuDisplayed;
    bool isPhaseTransitionRunning;
    bool startedTranslation;
    bool allEnemiesHaveMoved;

    void Awake() {
        //Setup: We will delete this after we have a way to transfer info a file/read from file
        populateGridSetupData();
        //Setup the UI's. Disable them at start
        setupUIElements();
        //Keep track of player turn ends. Reset this all to false; 
        resetPlayerTurnEndedDict();
        resetEnemyTurnEndedDict();
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
                PlayerInfo pInfo = currentClickedNode.getPlayerInfo();
                openUnitInfoMenu(pInfo);
                currentState = GameState.ShowUnitInfoState;
            } else if (Utils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == false) {
                clickedNodePath = Utils.getViableNodesPaths(clickedNode.getPlayerInfo().mov, clickedNode, nodeDict);
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
        if (currentClickedNode != null) {
            if (Utils.nodeClickedIsPlayer(currentClickedNode) && playerTurnEndedDict[currentClickedNode.getPlayerInfo().getPlayerId()] == true) { //Player clicked already ended turn, just show details
                //update UI with new details
                PlayerInfo pInfo = currentClickedNode.getPlayerInfo();
                openUnitInfoMenu(pInfo);
                currentState = GameState.ShowUnitInfoState;
            } else if(Utils.nodeClickedIsEnemy(currentClickedNode)) { //Player clicked is an enemy. Just show details
                //update UI with new details
                PlayerInfo pInfo = currentClickedNode.getPlayerInfo();
                openUnitInfoMenu(pInfo);
                currentState = GameState.ShowUnitInfoState;
            } else if(Utils.nodeClickedIsPlayer(currentClickedNode)) { //Player clicked is a player and they haven't ended their turn. Start move state
                closeUnitInfoMenu();
                processPlayerTurnStartState(currentClickedNode);
            } else { //Node wasn't a player or enemy. Disable UI element and go back to Start state
                closeUnitInfoMenu();
                currentState = GameState.PlayerTurnStart;
            }  
        }
        //else we stay in this state
    }

    void processMovePlayerStartState(Node currentClickedNode) {

        if (Input.GetMouseButtonDown(1)) {//track back
            //resetTurnStateData();
            foreach (Node node in clickedNodePath) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
            currentState = GameState.PlayerTurnStart;
        } else if (currentClickedNode == clickedNode) { //player node clicked on again, open battle menu and reset colors
            currentState = GameState.ShowBattleMenuState;
            foreach (Node node in clickedNodePath) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
        } else if (currentClickedNode != null) {
            previousClickedNode = clickedNode;
            clickedNode = currentClickedNode;
            if(clickedNodePath != null && clickedNodePath.Contains(clickedNode)) {
                PlayerAnimator playerToAnimate = clickedPlayerNode.getPlayerInfo().animator;
                //move player
                List<Node> pathToTake = Utils.getShortestPathNodes(clickedPlayerNode, clickedNode, clickedNodePath, Heuristic.NodeDistanceHeuristic, nodeDict);
                playerToAnimate.MovePlayerToTile(tileMap, pathToTake);
                //update player information in nodes
                swapNodeInfoOnSpriteMove(pathToTake[0], pathToTake[pathToTake.Count - 1]);
                clickedPlayerNode = pathToTake[pathToTake.Count - 1];
                //reset state after moving 
                foreach (Node node in clickedNodePath) {
                    tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                }
                currentState = GameState.ShowBattleMenuState; 
            } else { //outside node. go back to turn start state and reset info
                foreach (Node node in clickedNodePath) {
                    tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                }            
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
        if (previousClickedNode != null) {
            PlayerAnimator playerToAnimate = clickedNode.getPlayerInfo().animator;
            //move player
            print(clickedPlayerNode.getPosition().x + "," + clickedPlayerNode.getPosition().y);
            print(previousClickedNode.getPosition().x + "," + previousClickedNode.getPosition().y);
            List<Node> pathToTake = new List<Node> {clickedPlayerNode, previousClickedNode};
            playerToAnimate.MovePlayerToTile(tileMap, pathToTake);
            //update player information in nodes
            swapNodeInfoOnSpriteMove(clickedPlayerNode, previousClickedNode);            
        }
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

            Node enemyNodeToAttack = currentClickedNode;
            Node playerAttacking = clickedPlayerNode;
            //display battle
            calculateBattleEventDisplayBattleUI(playerAttacking, enemyNodeToAttack);
            //turn is over for player
            currentState = GameState.TurnEndState;
        } 

        if (Input.GetMouseButtonDown(1) && !battleEventScreenDisplayed) { //cancel movement and track back
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
            cancelPlayerMove();
            closePlayerBattleMenu();
            resetTurnStateData();
            currentState = GameState.PlayerTurnStart;
        }
        //Didn't click on anything. Stay in state
    }

    void calculateBattleEventDisplayBattleUI(Node attackerNode, Node defenderNode) {
        PlayerInfo attackerPI = attackerNode.getPlayerInfo();
        PlayerInfo defenderPI = defenderNode.getPlayerInfo();
        PlayerInfo newAttackerPI = attackerPI;
        PlayerInfo newDefenderPI = defenderPI;
        int attackerHealth = attackerPI.currentHealth;
        int defenderHealth = defenderPI.currentHealth;
        int attackerAtk = attackerPI.baseAttack;
        int defenderAtk = defenderPI.baseAttack;
        int attackerDef = attackerPI.baseDefense;
        int defenderDef = defenderPI.baseDefense;

        //attacker attacks defender
        print("Attacker " + attackerPI.getPlayerId() + "attacks " + defenderPI.getPlayerId());
        int dmgDoneToDefender = attackerAtk - defenderDef;
        newDefenderPI.currentHealth = defenderHealth - dmgDoneToDefender;
        if (newDefenderPI.currentHealth <= 0) {
            print("Defender has no health!");
            newDefenderPI.currentHealth = 0;
            if (newDefenderPI.getIsEnemy()) {
                retreatedEnemies.Add(newDefenderPI.getPlayerId());
                enemyTurnEndedDict[newDefenderPI.getPlayerId()] = true;
            } else {
                retreatedPlayers.Add(newDefenderPI.getPlayerId());
                playerTurnEndedDict[newDefenderPI.getPlayerId()] = true;
            }
            //destroy the gameobject
            GameObject playerToDestroy = GameObject.Find(newDefenderPI.getPlayerId());
            playerToDestroy.SetActive(false);
            //show the actual battle on-screen
            print("Attacker " + newAttackerPI.getPlayerId() + "has health " + newAttackerPI.currentHealth);
            print("Defender " + newDefenderPI.getPlayerId() + "has health " + newAttackerPI.currentHealth);
            attackerNode.setPlayerInfo(newAttackerPI);
            defenderNode.setPlayerInfo(null);
        } else {
            print("Defender still has health!");
            int dmgDoneToAttacker = defenderAtk - attackerDef;
            newAttackerPI.currentHealth = attackerHealth - dmgDoneToAttacker;    
            if (newAttackerPI.currentHealth <= 0) {
                print("Attacker has no health!");
                newAttackerPI.currentHealth = 0;
                if (newAttackerPI.getIsEnemy()) {
                    retreatedEnemies.Add(newAttackerPI.getPlayerId());
                    enemyTurnEndedDict[newAttackerPI.getPlayerId()] = true;
                } else {
                    retreatedPlayers.Add(newAttackerPI.getPlayerId());
                    playerTurnEndedDict[newAttackerPI.getPlayerId()] = true;
                }
                GameObject playerToDestroy = GameObject.Find(newAttackerPI.getPlayerId());
                playerToDestroy.SetActive(false);
                //update node dict's player info
                print("Attacker " + newAttackerPI.getPlayerId() + "has health " + attackerPI.currentHealth);
                print("Defender " + newDefenderPI.getPlayerId() + "has health " + defenderPI.currentHealth);
                attackerNode.setPlayerInfo(null);
                defenderNode.setPlayerInfo(newDefenderPI);
            } else {
                print("Attacker " + newAttackerPI.getPlayerId() + "has health " + attackerPI.currentHealth);
                print("Defender " + newDefenderPI.getPlayerId() + "has health " + defenderPI.currentHealth);
                attackerNode.setPlayerInfo(newAttackerPI);
                defenderNode.setPlayerInfo(newDefenderPI);
            }
        } 
        StartCoroutine(openBattleEventScreen(attackerPI, defenderPI, newAttackerPI, newDefenderPI, true, true));
        nodeDict[attackerNode.getPosition()] = attackerNode;
        nodeDict[defenderNode.getPosition()] = defenderNode; 
    }
    void processTurnEndState() {
        //record end of unit's turn in dict, but only if it exists (if it died, we ignore since that node is now null)
        if (clickedNode.getPlayerId() != null) {
            playerTurnEndedDict[clickedPlayerNode.getPlayerInfo().getPlayerId()] = true; 
        }
        
        //auto turn-end if all units are done
        if (!playerTurnEndedDict.ContainsValue(false) && !isPhaseTransitionRunning && !battleEventScreenDisplayed) {
            print("Player Turn ended");
            resetPlayerTurnEndedDict();
            StartCoroutine(translatePhaseImage("EnemyPhase"));
            currentState = GameState.EnemyTurnState;
            resetTurnStateData();
        } else if (!isPhaseTransitionRunning && !battleEventScreenDisplayed) { //else we go back to the player turn start state, but wait for transition to not run
            currentState = GameState.PlayerTurnStart;         
            //reset turn state data
            resetTurnStateData();
        }    
    }

    //Enemy logic handling// Do NOT move this to external class
    void processEnemyTurnState() {
        //process action for each enemy only if someone else is not attacking and we have a valid enemy that hasn't ended their turn
        if (!battleEventScreenDisplayed && !allEnemiesHaveMoved && !isPhaseTransitionRunning) {
            PlayerInfo enemy = pickAvailableEnemy();

            if (enemy == null) { //no available enemy. All enemies have moved.
                allEnemiesHaveMoved = true;
            } else {
                //if player in line of sight, move and attack
                Node enemyNode = Utils.findEnemyNode(enemy.getPlayerId(), nodeDict);
                List<Node> nodeRange = Utils.getViableNodesPaths(enemyNode.getPlayerInfo().mov, enemyNode, nodeDict);
                Node candidatePlayerNode = Utils.findPlayerNodeNearEnemy(enemyNode, enemyNode.getPlayerInfo().mov, nodeDict);
                if (candidatePlayerNode != null) {            
                    //check if we have to move or not
                    Node attackerNode = enemyNode;
                    if (!Utils.getNearbyNodes(enemyNode, nodeDict).Contains(candidatePlayerNode)) { //move and attack else just attack
                        //move and attack
                        List<Node> pathToMove = Utils.getShortestPathNodes(enemyNode, candidatePlayerNode, nodeRange, Heuristic.NodeDistanceHeuristic, nodeDict);
                        PlayerAnimator enemyToAnimate = enemy.animator;
                        //Player enemyToMove = GameObject.Find(enemy.getPlayerId()).GetComponent<Player>();
                        pathToMove.RemoveAt(pathToMove.Count - 1); //remove the last element since that's the player
                        enemyToAnimate.MoveEnemyNextToPlayer(tileMap, pathToMove);
                        swapNodeInfoOnSpriteMove(enemyNode, pathToMove[pathToMove.Count - 1]); //swap node data with new tile   
                        attackerNode = pathToMove[pathToMove.Count - 1]; //update the attackerNode                     
                        print("Move and attack.");
                    }
                    enemyTurnEndedDict[enemy.getPlayerId()] = true;
                    calculateBattleEventDisplayBattleUI(attackerNode, candidatePlayerNode);
                } else { //nothing to do, just end turn
                    enemyTurnEndedDict[enemy.getPlayerId()] = true;
                }
            }
        } else if (!isPhaseTransitionRunning && !startedTranslation && allEnemiesHaveMoved) {
            print("Started coroutine");
            startedTranslation = true;
            StartCoroutine(translatePhaseImage("PlayerPhase")); 
        } else if (!isPhaseTransitionRunning && startedTranslation) { //done processing, end turn
            resetEnemyTurnEndedDict();
            startedTranslation = false;
            allEnemiesHaveMoved = false;
            currentState = GameState.PlayerTurnStart;
        } //else the translation is still moving/isn't ready to be moved yet. Wait for the next frame.
    }

    PlayerInfo pickAvailableEnemy() {
        foreach (PlayerInfo enemy in pawnInfoDict.Values) {
            if (enemy.getIsEnemy() && !retreatedEnemies.Contains(enemy.getPlayerId()) && enemyTurnEndedDict[enemy.getPlayerId()] == false) {
                return enemy;
            }
        }
        return null; 
    }


    // Critical information handling // Do NOT move this to an external class
    void populateGridSetupData() {
        pawnInfoDict = new Dictionary<Vector2, PlayerInfo>();
        PlayerInfo testOne = new PlayerInfo("fakeid", "images/sprites/SampleSprite", false, BattleClass.Warrior);
        PlayerInfo testTwo = new PlayerInfo("fakeid2", "images/sprites/SampleSprite", false, BattleClass.Warrior);
        testOne.setupBaseStats();
        testOne.setupBattleStats();
        testTwo.setupBaseStats();
        testTwo.setupBattleStats();
        testOne.portraitRefPath = "images/portraits/test_face";
        testTwo.portraitRefPath = "images/portraits/test_face";
        pawnInfoDict[new Vector2(0, 0)] = testOne;
        pawnInfoDict[new Vector2(3,4)] = testTwo;
        PlayerInfo enemyOne = new PlayerInfo("fakeenemyid", "images/sprites/SampleSprite", true, BattleClass.Warrior);
        PlayerInfo enemyTwo = new PlayerInfo("fakeenemyid2", "images/sprites/SampleSprite", true, BattleClass.Warrior);
        enemyOne.setupBaseStats();
        enemyOne.setupBattleStats();
        enemyTwo.setupBaseStats();
        enemyTwo.setupBattleStats();
        enemyOne.portraitRefPath = "images/portraits/test_face";
        enemyTwo.portraitRefPath = "images/portraits/test_face";
        pawnInfoDict[new Vector2(2,2)] = enemyOne;
        pawnInfoDict[new Vector2(-3,-3)] = enemyTwo;
        Vector2 obstaclePos = new Vector2(0,1);
        obstaclesList = new List<Vector2>();
        obstaclesList.Add(obstaclePos);  
        //set node data for player and enemies MAP TILEMAP POSITION TO NODE POSITION
        tileMap = transform.GetComponentInParent<Tilemap>();
        foreach (var position in tileMap.cellBounds.allPositionsWithin) {
            Vector2 pos = new Vector2(position.x, position.y);
            bool hasObstacle = false;
            PlayerInfo playerInfo = null;
            if (pawnInfoDict.ContainsKey(pos)) {
                playerInfo = pawnInfoDict[pos];
                populateGridWithSprite(pos, pawnInfoDict, position, playerInfo);
            } else if (obstaclesList.Contains(pos)) {
                hasObstacle = true;
            }
            tileMap.SetTileFlags(position, TileFlags.None); //this is so we can change the color freely
            Color originalTileColor = tileMap.GetColor(position);
            nodeDict[position] = new Node(position, hasObstacle, playerInfo, originalTileColor);
        }
    }

    void resetPlayerTurnEndedDict() {
        foreach (PlayerInfo pi in pawnInfoDict.Values) {
            if (!pi.getIsEnemy()) {
                if (retreatedPlayers.Contains(pi.getPlayerId())) {
                    playerTurnEndedDict[pi.getPlayerId()] = true;
                } else {
                    playerTurnEndedDict[pi.getPlayerId()] = false;                
                }                
            }
        }
    }

    void resetEnemyTurnEndedDict() {
        foreach (PlayerInfo pi in pawnInfoDict.Values) {
            if (pi.getIsEnemy()) {
                if (retreatedPlayers.Contains(pi.getPlayerId())) {
                    enemyTurnEndedDict[pi.getPlayerId()] = true;
                } else if (pi.getIsEnemy()) {
                    enemyTurnEndedDict[pi.getPlayerId()] = false;                
                }                
            }
        }        
    }

    void populateGridWithSprite(Vector2 pos2D, Dictionary<Vector2, PlayerInfo> infoDict, Vector3Int pos3D, PlayerInfo playerInfo) {
        string playerId = playerInfo.getPlayerId();
        GameObject objToSpawn = new GameObject(playerId);
        PlayerAnimator newPawn = objToSpawn.AddComponent<PlayerAnimator>() as PlayerAnimator;
        newPawn.Setup(playerInfo);
        newPawn.name = playerId;
        pawnInfoDict[pos2D].animator = newPawn;
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
        // player battle menu
        playerBattleMenu = GameObject.Find("PlayerBattleMenu");
        Button attackButton = GameObject.Find("AttackButton").GetComponent<Button>() as Button;
        Button itemButton = GameObject.Find("ItemButton").GetComponent<Button>() as Button;
        Button waitButton = GameObject.Find("WaitButton").GetComponent<Button>() as Button;
        attackButton.onClick.AddListener(onAttackButtonClick);
        itemButton.onClick.AddListener(onItemButtonClick);
        waitButton.onClick.AddListener(onWaitButtonClick);
        playerBattleMenu.SetActive(false);
        playerBattleMenuDisplayed = false;

        //unit info menu
        unitInfoMenu = GameObject.Find("UnitInfoMenu");
        unitInfoMenu.SetActive(false);

        battleEventScreen = GameObject.Find("BattleEventScreen");
        battleEventScreenDisplayed = false;

        //Convert the screenpoint to ui rectangle local point
        moveCanvasToGlobalPoint(battleEventScreen, new Vector3(0,0,0));
        battleEventScreen.SetActive(false);

        //Set player/enemy phase transition images in the right place, turn them off for now
        playerPhaseTransitionImage = GameObject.Find("PlayerPhaseImg");
        enemyPhaseTransitionImage = GameObject.Find("EnemyPhaseImg");
        Vector2 leftMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2 (Camera.main.pixelWidth, Camera.main.pixelHeight/2));
        leftMostScreenPosition.x *= -1;
        playerPhaseTransitionImage.transform.position = leftMostScreenPosition;
        enemyPhaseTransitionImage.transform.position = leftMostScreenPosition;
        playerPhaseTransitionImage.SetActive(false);
        enemyPhaseTransitionImage.SetActive(false);
        isPhaseTransitionRunning = false;
        allEnemiesHaveMoved = false;
    }

    IEnumerator translatePhaseImage(string phase) {
        isPhaseTransitionRunning = true;
        if (phase == "PlayerPhase") {
            yield return new WaitForSeconds(1);
            Vector2 rightMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight/2));
            playerPhaseTransitionImage.SetActive(true);
            Vector2 leftMostScreenPosition = playerPhaseTransitionImage.transform.position;

            for(float t = 0.0f; t < 1.0f; t+=0.1f) {
                playerPhaseTransitionImage.GetComponent<ImageTransitions>().TranslateAcrossScreen(leftMostScreenPosition, rightMostScreenPosition, t);

                if (t == 0.5f) {
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.05f);
            }
            playerPhaseTransitionImage.transform.position = leftMostScreenPosition;
            playerPhaseTransitionImage.SetActive(false);
        } else if (phase == "EnemyPhase") {
            yield return new WaitForSeconds(1);
            Vector2 rightMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2 (Camera.main.pixelWidth, Camera.main.pixelHeight/2));
            enemyPhaseTransitionImage.SetActive(true);
            Vector2 leftMostScreenPosition = enemyPhaseTransitionImage.transform.position;

            for(float t = 0.0f; t < 1.0f; t+=0.1f) {
                enemyPhaseTransitionImage.GetComponent<ImageTransitions>().TranslateAcrossScreen(leftMostScreenPosition, rightMostScreenPosition, t);

                if (t == 0.5f) {
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.05f);
            }
            enemyPhaseTransitionImage.transform.position = leftMostScreenPosition;
            enemyPhaseTransitionImage.SetActive(false);
        }
        isPhaseTransitionRunning = false;
    }


    void moveCanvasToGlobalPoint(GameObject go, Vector3 globalPos) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(globalPos);
        Vector2 movePos;
        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(go.transform as RectTransform, screenPos, Camera.main, out movePos);
        //Convert the local point to world point
        go.transform.position = new Vector3(movePos.x, movePos.y, 0);      
    }

    void openUnitInfoMenu(PlayerInfo playerInfo) {
        unitInfoMenu.SetActive(true);
        PlayerAnimator animator = playerInfo.animator;
        Image playerFace = GameObject.Find("PlayerFace").GetComponent<Image>() as Image;
        Text hp = GameObject.Find("HPDisplay").GetComponent<Text>() as Text;
        Text atk = GameObject.Find("AtkDisplay").GetComponent<Text>() as Text;
        Text def = GameObject.Find("DefDisplay").GetComponent<Text>() as Text;
        Text mov = GameObject.Find("MovDisplay").GetComponent<Text>() as Text;
        playerFace.sprite = animator.playerPortrait;
        hp.text = "HP " + playerInfo.currentHealth.ToString() + "/" + playerInfo.baseHealth.ToString();
        atk.text = "ATK " + playerInfo.baseAttack.ToString();
        def.text = "DEF " + playerInfo.baseDefense.ToString();
        mov.text = "MOV " + playerInfo.mov.ToString();
    }

    void closeUnitInfoMenu() {
        unitInfoMenu.SetActive(false);
    }

    IEnumerator openBattleEventScreen(PlayerInfo atkPI, PlayerInfo defPI, PlayerInfo newAtkPI, PlayerInfo newDefPI, bool atkHit, bool defHit) {
        //re-enable the battle event screen
        battleEventScreen.SetActive(true);
        battleEventScreenDisplayed = true;
        //get attacker and defender position values
        GameObject attackerHealthBarGameObj = battleEventScreen.transform.GetChild(1).gameObject;
        GameObject defenderHealthBarGameObj = battleEventScreen.transform.GetChild(2).gameObject;
        Vector3 attackerPos = new Vector3(-1.5f, attackerHealthBarGameObj.transform.position.y - 1, 0.0f);
        Vector3 defenderPos = new Vector3(1.5f, defenderHealthBarGameObj.transform.position.y - 1, 0.0f);
        //setup attacker w/ original stats
        PlayerAnimator attackerPlayer = addPlayerToBattleEventScreen(atkPI.getPlayerId(), attackerPos, atkPI);
        //setup defender w/ original stats
        PlayerAnimator defenderPlayer = addPlayerToBattleEventScreen(defPI.getPlayerId(), defenderPos, defPI);

        //setup healthbar fill values
        setHealthBarOnBattleEventScreen(attackerHealthBarGameObj, atkPI.currentHealth, atkPI.baseHealth);
        setHealthBarOnBattleEventScreen(defenderHealthBarGameObj, defPI.currentHealth, defPI.baseHealth);
        yield return new WaitForSeconds(1.0f);
        //update healthbar final values
        setHealthBarOnBattleEventScreen(attackerHealthBarGameObj, newAtkPI.currentHealth, newAtkPI.baseHealth);
        setHealthBarOnBattleEventScreen(defenderHealthBarGameObj, newDefPI.currentHealth, newDefPI.baseHealth); 
        yield return new WaitForSeconds(1.0f);
        //TODO apply animation logic for attacks
        Destroy(attackerPlayer);
        Destroy(defenderPlayer);
        battleEventScreen.SetActive(false);
        battleEventScreenDisplayed = false;
    }

    PlayerAnimator addPlayerToBattleEventScreen(string playerId, Vector3 position, PlayerInfo playerInfo) {
        playerId += "-temp";
        GameObject playerToSpawn = new GameObject(playerId);
        PlayerAnimator player = playerToSpawn.AddComponent<PlayerAnimator>() as PlayerAnimator;
        player.Setup(playerInfo);
        player.name = playerId;
        if (player) {
            player.AddPlayerToParallax(position);
        }
        playerToSpawn.transform.parent = battleEventScreen.transform;
        player.spriteRenderer.sortingOrder = 5;        
        print("Sprite added at position: " + position.x + "," + position.y);
        return player;
    }

    void setHealthBarOnBattleEventScreen(GameObject healthBar, int currentHP, int baseHP) {
        float value = ((float) currentHP) / baseHP;
        healthBar.GetComponent<HealthBar>().SetHealth(value);
        print("Health bar value set to: " + value);
    }

    void openPlayerBattleMenu() {       
        playerBattleMenu.SetActive(true);
        playerBattleMenuDisplayed = true;
        print("Player Battle menu opened");
        //TODO: Fix this
        //string playerId = clickedPlayerNode.getPlayerInfo().getPlayerId();
        //Player playerClicked = GameObject.Find(playerId).GetComponent<Player>();
        //Vector3 playerSpriteVector = playerClicked.transform.position; //global position where I want the menu to appear
        //print(playerSpriteVector); //(-0.5, 1.5, 0)
        //RectTransform rt = playerBattleMenu.transform.GetChild(0).GetComponent<RectTransform>();
        //Vector3 newPos = new Vector3(playerSpriteVector.x + 2, playerSpriteVector.y + 2, 0);
        //rt.anchoredPosition = newPos;
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



