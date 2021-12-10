using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

//Responsible for handling the entire game state, turn management, commands to move/animate sprite, create subwindow battle and destroy it
public class GameMaster : MonoBehaviour {

    private Dictionary<Vector2, PlayerInfo> pawnInfoDict;
    private Dictionary<Vector3Int, Node> nodeDict = new Dictionary<Vector3Int, Node>();
    [SerializeField] private List<Vector2> obstaclesList;
    public Tilemap tileMap;
    public List<Node> clickedNodePath;
    public Node clickedPlayerNode;
    public Node previousClickedNode;
    public Node clickedNode;
    private Dictionary<string, bool> playerTurnEndedDict = new Dictionary<string, bool>();
    private Dictionary<string, bool> enemyTurnEndedDict = new Dictionary<string, bool>();
    private List<string> retreatedPlayers = new List<string>();
    private List<string> retreatedEnemies = new List<string>(); 

    // UI elements and state tracking
    public GameState currentState;
    public SwapItemsMenu swapItemMenu;
    public PlayerBattleMenu playerBattleMenu;
    public UnitInfoMenu unitInfoMenu;
    public BattleEventScreen battleEventScreen;
    public ItemMenu itemMenu;
    public PhaseTransitionUIHandler phaseTransitionUIHandler;
    public bool startedTranslation = false;
    public bool highlightedPossibleSwapPartner = false;
    bool allEnemiesHaveMoved = false;

    void Awake() {
        //Setup: We will delete this after we have a way to transfer info a file/read from file
        populateGridSetupData();
        //Setup the UI's. Disable them at start
        //setupUIElements();
        //Keep track of player turn ends. Reset this all to false; 
        resetPlayerTurnEndedDict();
        resetEnemyTurnEndedDict();
        currentState = GameState.PlayerTurnStart;
    }

    void Start() { 
        swapItemMenu = gameObject.AddComponent<SwapItemsMenu>();  
        playerBattleMenu = gameObject.AddComponent<PlayerBattleMenu>();   
        unitInfoMenu = gameObject.AddComponent<UnitInfoMenu>();
        itemMenu = gameObject.AddComponent<ItemMenu>();
        battleEventScreen = gameObject.AddComponent<BattleEventScreen>();
        phaseTransitionUIHandler = gameObject.AddComponent<PhaseTransitionUIHandler>();
    }

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
            case GameState.SwapItemState:
                processShowSwapItemMenuState(currentClickNode);
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



    /*
        GameState.PlayerTurnStart Related Functions
    */
    void processPlayerTurnStartState(Node currentClickedNode) {
        if (currentClickedNode != null) {
            clickedNode = currentClickedNode;
            if (Utils.nodeClickedIsEnemy(clickedNode) || (Utils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == true)) {
                PlayerInfo pInfo = currentClickedNode.getPlayerInfo();
                unitInfoMenu.openUnitInfoMenu(pInfo);
                ChangeState(GameState.ShowUnitInfoState);
            } else if (Utils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == false) {
                clickedNodePath = Utils.getViableNodesPaths(clickedNode.getPlayerInfo().currentMov, clickedNode, nodeDict);
                clickedPlayerNode = clickedNode;
                foreach (Node node in clickedNodePath) {
                    tileMap.SetColor(node.getPosition(), Color.blue);
                }
                ChangeState(GameState.MovePlayerStartState);
            }
        }
        //else nothing happens
    }


    /*
        GameState.ShowUnitInfoState Related Functions
    */
    void processShowUnitInfoState(Node currentClickedNode) {
        if (currentClickedNode != null) {
            if (Utils.nodeClickedIsPlayer(currentClickedNode) && playerTurnEndedDict[currentClickedNode.getPlayerInfo().getPlayerId()] == true) { //Player clicked already ended turn, just show details
                //update UI with new details
                PlayerInfo pInfo = currentClickedNode.getPlayerInfo();
                unitInfoMenu.openUnitInfoMenu(pInfo);
                ChangeState(GameState.ShowUnitInfoState);
            } else if(Utils.nodeClickedIsEnemy(currentClickedNode)) { //Player clicked is an enemy. Just show details
                //update UI with new details
                PlayerInfo pInfo = currentClickedNode.getPlayerInfo();
                unitInfoMenu.openUnitInfoMenu(pInfo);
                ChangeState(GameState.ShowUnitInfoState);
            } else if(Utils.nodeClickedIsPlayer(currentClickedNode)) { //Player clicked is a player and they haven't ended their turn. Start move state
                unitInfoMenu.closeUnitInfoMenu();
                processPlayerTurnStartState(currentClickedNode);
            } else { //Node wasn't a player or enemy. Disable UI element and go back to Start state
                unitInfoMenu.closeUnitInfoMenu();
                ChangeState(GameState.PlayerTurnStart);
            }  
        }
        //else we stay in this state
    }


    /*
        GameState.MovePlayerStartState Related Functions
    */
    void processMovePlayerStartState(Node currentClickedNode) {

        if (Input.GetMouseButtonDown(1)) {//track back
            //resetTurnStateData();
            foreach (Node node in clickedNodePath) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
            ChangeState(GameState.PlayerTurnStart);
        } else if (currentClickedNode == clickedNode) { //player node clicked on again, open battle menu and reset colors
            ChangeState(GameState.ShowBattleMenuState);
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
                ChangeState(GameState.ShowBattleMenuState); 
            } else { //outside node. go back to turn start state and reset info
                foreach (Node node in clickedNodePath) {
                    tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                }            
                ChangeState(GameState.PlayerTurnStart);
                resetTurnStateData();
            }
        }
        //else we didn't click a node. stay in this state
    }

    /*
        GameState.ShowBattleMenuState Related Functions
    */
    void processShowBattleMenuState() {
        if (!playerBattleMenu.IsPlayerBattleMenuDisplayed()) {
            playerBattleMenu.openPlayerBattleMenu();            
        }

        if (Input.GetMouseButtonDown(1)) { //cancel movement and track back
            cancelPlayerMove();
            playerBattleMenu.closePlayerBattleMenu();
            resetTurnStateData();
            ChangeState(GameState.PlayerTurnStart);
        }
    }

    /*
        GameState.ShowItemMenuState Related Functions
    */
    void processShowItemMenuState() {

        if (!itemMenu.IsItemMenuDisplayed()) {
            print("Opened item menu");
            itemMenu.openPlayerItemMenu();
        }

        if (Input.GetMouseButtonDown(1)) { //cancel movement and track back
            itemMenu.closePlayerItemMenu();
            ChangeState(GameState.ShowBattleMenuState);
        }
    }


    /*
        GameState.SwapItemState Related Functions
    */
    void processShowSwapItemMenuState(Node currentClickNode) {
        if (!swapItemMenu.IsSwapItemMenuDisplayed()) {
            if (!highlightedPossibleSwapPartner) {
                //Look for valid partners
                if (clickedNode != null) {
                    List<Node> nearbyPlayerNodes = Utils.getNearbyPlayerNodes(clickedNode, nodeDict);
                    if (nearbyPlayerNodes.Count == 0) { //no nodes exist. go back to battle state
                        ChangeState(GameState.ShowBattleMenuState);
                    } else {
                        foreach (Node n in nearbyPlayerNodes) {
                            if (Utils.nodeClickedIsPlayer(n)) {
                                tileMap.SetColor(n.getPosition(), Color.yellow);
                            }
                        }
                        highlightedPossibleSwapPartner = true;                        
                    }
                }
            } else { //highlighted. open menu if valid node click
                if (currentClickNode != null) {
                    List<Node> nearbyPlayerNodes = Utils.getNearbyPlayerNodes(clickedNode, nodeDict);
                    if (Utils.nodeClickedIsPlayer(currentClickNode) && nearbyPlayerNodes.Contains(currentClickNode)) {
                        print("Open swap menu!!!!");
                        swapItemMenu.OpenSwapItemMenu(clickedNode.getPlayerInfo(), currentClickNode.getPlayerInfo());
                    } else { //invalid node. go back to show battle menu state
                        print("Invalid. Go back");
                        endShowSwapItemMenuStateToShowBattleMenuState();
                    }                    
                }
            }
        }
    }

    public void endShowSwapItemMenuStateToShowBattleMenuState() {
        List<Node> nearbyPlayerNodes = Utils.getNearbyPlayerNodes(clickedNode, nodeDict);
        foreach (Node n in nearbyPlayerNodes) {
            if (Utils.nodeClickedIsPlayer(n)) {
                tileMap.SetColor(n.getPosition(), n.getOriginalColor());
            }
        }
        ChangeState(GameState.ShowBattleMenuState);
        highlightedPossibleSwapPartner = false;
    }

    public void endShowSwapItemMenuStateToEndTurnState(PlayerInfo currentPlayerInfo) {
        List<Node> nearbyPlayerNodes = Utils.getNearbyPlayerNodes(clickedNode, nodeDict);
        foreach (Node n in nearbyPlayerNodes) {
            if (Utils.nodeClickedIsPlayer(n)) {
                tileMap.SetColor(n.getPosition(), n.getOriginalColor());
            }
        }
        //playerTurnEndedDict[currentPlayerInfo.getPlayerId()] = true;
        highlightedPossibleSwapPartner = false;
        ChangeState(GameState.TurnEndState);
    }

    /*
        GameState.AttackState Related Functions
    */

    public void preProcessAttackState() {
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
            ChangeState(GameState.TurnEndState);
        } else if (currentClickedNode != null && !validAttackNodes.Contains(currentClickedNode)) {
            //Remove the red highlight and go back to battle menu state
            foreach (Node node in validAttackNodes) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
            ChangeState(GameState.ShowBattleMenuState);       
        } else if (Input.GetMouseButtonDown(1) && !battleEventScreen.IsBattleEventScreenDisplayed()) { //cancel movement and track back
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
                tileMap.SetColor(node.getPosition(), node.getOriginalColor());
            }
            cancelPlayerMove();
            playerBattleMenu.closePlayerBattleMenu();
            resetTurnStateData();
            ChangeState(GameState.PlayerTurnStart);
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
        int attackerAtk = attackerPI.currentAttack;
        int defenderAtk = defenderPI.currentAttack;
        int attackerDef = attackerPI.currentDefense;
        int defenderDef = defenderPI.currentDefense;

        //attacker attacks defender
        print("Attacker " + attackerPI.getPlayerId() + "attacks " + defenderPI.getPlayerId());
        int dmgDoneToDefender = attackerAtk - defenderDef;
        print("Dmg Done to Defender: " + dmgDoneToDefender.ToString());
        newDefenderPI.currentHealth = defenderHealth - Mathf.Max(0, dmgDoneToDefender);
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
            newAttackerPI.currentHealth = attackerHealth - Mathf.Max(0, dmgDoneToAttacker);    
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
        StartCoroutine(battleEventScreen.openBattleEventScreen(attackerPI, defenderPI, newAttackerPI, newDefenderPI, true, true));
        nodeDict[attackerNode.getPosition()] = attackerNode;
        nodeDict[defenderNode.getPosition()] = defenderNode; 
    }

    /*
        GameState.TurnEndState Related Functions
    */
    void processTurnEndState() {
        //record end of unit's turn in dict, but only if it exists (if it died, we ignore since that node is now null)
        if (clickedNode.getPlayerId() != null) {
            playerTurnEndedDict[clickedPlayerNode.getPlayerInfo().getPlayerId()] = true; 
        }
        
        //auto turn-end if all units are done
        if (!playerTurnEndedDict.ContainsValue(false) && !phaseTransitionUIHandler.IsPhaseTransitionRunning() && !battleEventScreen.IsBattleEventScreenDisplayed()) {
            print("Player Turn ended");
            resetPlayerTurnEndedDict();
            StartCoroutine(phaseTransitionUIHandler.translatePhaseImage("EnemyPhase"));
            ChangeState(GameState.EnemyTurnState);
            resetTurnStateData();
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !battleEventScreen.IsBattleEventScreenDisplayed()) { //else we go back to the player turn start state, but wait for transition to not run
            ChangeState(GameState.PlayerTurnStart);         
            //reset turn state data
            resetTurnStateData();
        }    
    }

    /*
        GameState.ProcessEnemyState Related Functions
    */

    //Enemy logic handling// Do NOT move this to external class
    void processEnemyTurnState() {
        //process action for each enemy only if someone else is not attacking and we have a valid enemy that hasn't ended their turn
        if (!battleEventScreen.IsBattleEventScreenDisplayed() && !HaveAllEnemiesHaveMoved() && !phaseTransitionUIHandler.IsPhaseTransitionRunning()) {
            PlayerInfo enemy = pickAvailableEnemy();

            if (enemy == null) { //no available enemy. All enemies have moved.
                SetAllEnemiesHaveMoved(true);
            } else {
                //if player in line of sight, move and attack
                Node enemyNode = Utils.findEnemyNode(enemy.getPlayerId(), nodeDict);
                List<Node> nodeRange = Utils.getViableNodesPaths(enemyNode.getPlayerInfo().currentMov, enemyNode, nodeDict);
                Node candidatePlayerNode = Utils.findPlayerNodeNearEnemy(enemyNode, enemyNode.getPlayerInfo().currentMov, nodeDict);
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
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !startedTranslation && HaveAllEnemiesHaveMoved() && !battleEventScreen.IsBattleEventScreenDisplayed()) {
            print("Started coroutine");
            startedTranslation = true;
            StartCoroutine(phaseTransitionUIHandler.translatePhaseImage("PlayerPhase")); 
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && startedTranslation) { //done processing, end turn
            resetEnemyTurnEndedDict();
            startedTranslation = false;
            SetAllEnemiesHaveMoved(false);
            ChangeState(GameState.PlayerTurnStart);
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


    /*
        Private Utility Related Functions (E.g. board setup, cancel player move, etc.)
    */
    void cancelPlayerMove() {
        if (previousClickedNode != null) {
            PlayerAnimator playerToAnimate = clickedNode.getPlayerInfo().animator;
            //move player
            //print(clickedPlayerNode.getPosition().x + "," + clickedPlayerNode.getPosition().y);
            //print(previousClickedNode.getPosition().x + "," + previousClickedNode.getPosition().y);
            List<Node> pathToTake = new List<Node> {clickedPlayerNode, previousClickedNode};
            playerToAnimate.MovePlayerToTile(tileMap, pathToTake);
            //update player information in nodes
            swapNodeInfoOnSpriteMove(clickedPlayerNode, previousClickedNode);            
        }
    }

    // Critical information handling // Do NOT move this to an external class
    void populateGridSetupData() {
        pawnInfoDict = new Dictionary<Vector2, PlayerInfo>();
        PlayerInfo testOne = new PlayerInfo("fakeid", "images/sprites/SampleSprite", false, BattleClass.Warrior);
        PlayerInfo testTwo = new PlayerInfo("fakeid2", "images/sprites/SampleSprite", false, BattleClass.Warrior);
        List<int> stats = new List<int> {1, 10, 10, 8, 10, 10, 10, 10, 2, 0};
        testOne.setupBaseStats(stats);
        testOne.setupBattleStats();
        testTwo.setupBaseStats(stats);
        testTwo.setupBattleStats();
        testOne.portraitRefPath = "images/portraits/test_face";
        testTwo.portraitRefPath = "images/portraits/test_face";
        pawnInfoDict[new Vector2(0, 0)] = testOne;
        pawnInfoDict[new Vector2(1,0)] = testTwo;
        PlayerInfo enemyOne = new PlayerInfo("fakeenemyid", "images/sprites/SampleSprite", true, BattleClass.Warrior);
        PlayerInfo enemyTwo = new PlayerInfo("fakeenemyid2", "images/sprites/SampleSprite", true, BattleClass.Warrior);
        enemyOne.setupBaseStats(stats);
        enemyOne.setupBattleStats();
        enemyTwo.setupBaseStats(stats);
        enemyTwo.setupBattleStats();
        enemyOne.portraitRefPath = "images/portraits/test_face";
        enemyTwo.portraitRefPath = "images/portraits/test_face";
        //setup items
        ConsumableItem healthPotion = (ConsumableItem) AssetDatabase.LoadAssetAtPath("Assets/Resources/Items/MinorHealthPotion.asset", typeof(ConsumableItem));
        healthPotion.itemSprite = (Sprite) AssetDatabase.LoadAssetAtPath("Assets/Resources/images/ui/Icons/HealthPotionIcon.png", typeof(Sprite));
        ConsumableItem manaPotion = (ConsumableItem) AssetDatabase.LoadAssetAtPath("Assets/Resources/Items/MinorManaPotion.asset", typeof(ConsumableItem));
        manaPotion.itemSprite = (Sprite) AssetDatabase.LoadAssetAtPath("Assets/Resources/images/ui/Icons/ManaPotionIcon.png", typeof(Sprite));
        EquipmentItem vest = (EquipmentItem) AssetDatabase.LoadAssetAtPath("Assets/Resources/Items/LeatherVest.asset", typeof(EquipmentItem));
        testOne.equipmentItemManager = new EquipmentItemManager();
        testOne.consumableItemManager = new ConsumableItemManager();
        List<ConsumableItem> cIList = new List<ConsumableItem> {healthPotion, healthPotion};
        EquipmentItem[] eIList = new EquipmentItem[System.Enum.GetNames(typeof(EquipType)).Length];
        eIList[(int) vest.equipType] = vest;
        testOne.LoadItems(cIList, eIList);
        testTwo.equipmentItemManager = new EquipmentItemManager();
        testTwo.consumableItemManager = new ConsumableItemManager();
        testTwo.LoadItems(new List<ConsumableItem>(){healthPotion, manaPotion}, new EquipmentItem[System.Enum.GetNames(typeof(EquipType)).Length]);
        //set enemy locations
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

    public void ChangeState(GameState state) {
        currentState = state;
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

    public bool HaveAllEnemiesHaveMoved() {
        return allEnemiesHaveMoved;
    }

    public void SetAllEnemiesHaveMoved(bool haveMoved) {
        allEnemiesHaveMoved = haveMoved;
    }

}
