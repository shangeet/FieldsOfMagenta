using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

//Responsible for handling the entire battle state, turn management, commands to move/animate sprite, create subwindow battle and destroy it
public class GameMaster : MonoBehaviour {

    private Dictionary<Vector2, PlayerInfo> pawnInfoDict;
    private Dictionary<Vector3Int, Node> nodeDict = new Dictionary<Vector3Int, Node>();
    private StaticTileHandler staticTileHandler;
    private TileEventManager tileEventManager;
    [SerializeField] 
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
    private MasterGameStateController gameStateInstance;
    public GameState currentState;
    public GameObject swapItemMenuGo;
    public SwapItemsMenu swapItemMenu;
    public GameObject playerBattleMenuGo;
    public PlayerBattleMenu playerBattleMenu;
    public GameObject unitInfoMenuGo;
    public UnitInfoMenu unitInfoMenu;
    public GameObject battleEventScreenGo;
    public BattleEventScreen battleEventScreen;
    public GameObject itemMenuGo;
    public ItemMenu itemMenu;
    public PhaseTransitionUIHandler phaseTransitionUIHandler;
    public GameObject playerExpScreenGo;
    public PlayerExpScreen playerExpScreen;
    public GameObject expGainMenuGo;
    public bool startedTranslation = false;
    public bool highlightedPossibleSwapPartner = false;
    bool allEnemiesHaveMoved = false;
    public bool playerCurrentlyMoving = false;
    PlayerInfo oldBattlePI;
    PlayerInfo newBattlePI;
    int timesLeveledUp;
    List<int> totalExpToLevelUp;
    bool showExpGainBar = false;

    // Misc
    bool playerVictory;
    bool isEnemyTurn;
    public GameObject playerPhaseImageGo;
    public GameObject enemyPhaseImageGo;
    public GameObject playerVictoryScreen;
    public GameObject playerDefeatScreen;

    void Awake() {
        //setup handlers + managers
        staticTileHandler = gameObject.GetComponent<StaticTileHandler>();
        tileEventManager = gameObject.GetComponent<TileEventManager>();
        //populate grid with node data
        populateGridSetupData();
        //Setup the UI's. Disable them at start
        setupUIElements();
        //Keep track of player turn ends. Reset this all to false; 
        resetPlayerTurnEndedDict();
        resetEnemyTurnEndedDict();
        currentState = GameState.PlayerTurnStart;

    }

    void Start() {
        playerVictoryScreen.SetActive(false);
        playerDefeatScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

        Node currentClickNode = getNodePositionOnClick();

        switch (currentState) {
            
            case GameState.PlayerTurnStart:
                processPlayerTurnStartState(currentClickNode);                    
                break;
            case GameState.MovePlayerStartState:
                processMovePlayerStartState(currentClickNode);
                break;
            case GameState.ShowBattleMenuState:
                processShowBattleMenuState();
                break;
            case GameState.ShowExpGainState:
                processShowExpGainState();
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
            case GameState.HandleTileState:
                processHandleTileState();
                break;
            case GameState.TurnEndState:
                processTurnEndState();
                break;
            case GameState.EnemyTurnState:
                processEnemyTurnState();
                break;
            case GameState.GameEndState:
                processGameEndState();
                break;
        }

    } 

    void setupUIElements() {
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
        swapItemMenu = gameObject.AddComponent<SwapItemsMenu>();
        playerBattleMenu = gameObject.AddComponent<PlayerBattleMenu>();  
        unitInfoMenu = gameObject.AddComponent<UnitInfoMenu>();
        itemMenu = gameObject.AddComponent<ItemMenu>();
        battleEventScreen = gameObject.AddComponent<BattleEventScreen>();
        playerExpScreen = gameObject.AddComponent<PlayerExpScreen>();
        phaseTransitionUIHandler = gameObject.AddComponent<PhaseTransitionUIHandler>();
    }

    /*
        GameState.PlayerTurnStart Related Functions
    */
    void processPlayerTurnStartState(Node currentClickedNode) {
        if (currentClickedNode != null) {
            clickedNode = currentClickedNode;           
            if (NodeUtils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == false) {
                clickedNodePath = NodeUtils.getViableNodesPaths(clickedNode, nodeDict, tileEventManager);
                clickedPlayerNode = clickedNode;
                foreach (Node node in clickedNodePath) {
                    staticTileHandler.SpawnTile(node.getPosition(), "BlueTile");
                }
                ChangeState(GameState.MovePlayerStartState);
            }
        }
        //else nothing happens
    }

    /*
        GameState.MovePlayerStartState Related Functions
    */
    void processMovePlayerStartState(Node currentClickedNode) {

        if (!playerCurrentlyMoving) {
            if (Input.GetMouseButtonDown(1)) { //track back
                foreach (Node node in clickedNodePath) {
                    //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                    staticTileHandler.DespawnTile(node.getPosition());
                }
                ChangeState(GameState.PlayerTurnStart);
            } else if (currentClickedNode == clickedNode) { //player node clicked on again, open battle menu and reset colors
                foreach (Node node in clickedNodePath) {
                    //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                    staticTileHandler.DespawnTile(node.getPosition());
                }
                ChangeState(GameState.ShowBattleMenuState);
            } else if (currentClickedNode != null) {
                previousClickedNode = clickedNode;
                clickedNode = currentClickedNode;
                if(clickedNodePath != null && clickedNodePath.Contains(clickedNode)) {
                    PlayerAnimator playerToAnimate = clickedPlayerNode.getPlayerInfo().playerAnimator;
                    //move player
                    List<Node> pathToTake = NodeUtils.getShortestPathNodes(clickedPlayerNode, clickedNode, clickedNodePath, Heuristic.NodeDistanceHeuristic, nodeDict);
                    
                    StartCoroutine(playerToAnimate.MovePlayerToTile(tileMap, pathToTake));
                    
                    //update player information in nodes
                    swapNodeInfoOnSpriteMove(pathToTake[0], pathToTake[pathToTake.Count - 1]);
                    clickedPlayerNode = pathToTake[pathToTake.Count - 1];
                    //reset state after moving 
                    foreach (Node node in clickedNodePath) {
                        //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                        staticTileHandler.DespawnTile(node.getPosition());
                    }
                    ChangeState(GameState.ShowBattleMenuState); 
                } else { //outside node. go back to turn start state and reset info
                    foreach (Node node in clickedNodePath) {
                        //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                        staticTileHandler.DespawnTile(node.getPosition());
                    }            
                    ChangeState(GameState.PlayerTurnStart);
                    resetTurnStateData();
                }
            }
        }
        //else we didn't click a node. stay in this state
    }

    /*
        GameState.ShowBattleMenuState Related Functions
    */
    void processShowBattleMenuState() {
        if (!playerBattleMenu.IsPlayerBattleMenuDisplayed() && !playerCurrentlyMoving) {
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
                    List<Node> nearbyPlayerNodes = NodeUtils.getNearbyPlayerNodes(clickedNode, nodeDict);
                    if (nearbyPlayerNodes.Count == 0) { //no nodes exist. go back to battle state
                        ChangeState(GameState.ShowBattleMenuState);
                    } else {
                        foreach (Node n in nearbyPlayerNodes) {
                            if (NodeUtils.nodeClickedIsPlayer(n)) {
                                staticTileHandler.SpawnTile(n.getPosition(), "YellowTile");
                            }
                        }
                        highlightedPossibleSwapPartner = true;                        
                    }
                }
            } else { //highlighted. open menu if valid node click
                if (currentClickNode != null) {
                    List<Node> nearbyPlayerNodes = NodeUtils.getNearbyPlayerNodes(clickedNode, nodeDict);
                    if (NodeUtils.nodeClickedIsPlayer(currentClickNode) && nearbyPlayerNodes.Contains(currentClickNode)) {
                        swapItemMenu.OpenSwapItemMenu(clickedNode.getPlayerInfo(), currentClickNode.getPlayerInfo());
                    } else { //invalid node. go back to show battle menu state
                        endShowSwapItemMenuStateToShowBattleMenuState();
                    }                    
                }
            }
        }
    }

    public void endShowSwapItemMenuStateToShowBattleMenuState() {
        List<Node> nearbyPlayerNodes = NodeUtils.getNearbyPlayerNodes(clickedNode, nodeDict);
        foreach (Node n in nearbyPlayerNodes) {
            if (NodeUtils.nodeClickedIsPlayer(n)) {
                //tileMap.SetColor(n.getPosition(), n.getOriginalColor());
                staticTileHandler.DespawnTile(n.getPosition());
            }
        }
        ChangeState(GameState.ShowBattleMenuState);
        highlightedPossibleSwapPartner = false;
    }

    public void endShowSwapItemMenuStateToHandleTileState(PlayerInfo currentPlayerInfo) {
        List<Node> nearbyPlayerNodes = NodeUtils.getNearbyPlayerNodes(clickedNode, nodeDict);
        foreach (Node n in nearbyPlayerNodes) {
            if (NodeUtils.nodeClickedIsPlayer(n)) {
                //tileMap.SetColor(n.getPosition(), n.getOriginalColor());
                staticTileHandler.DespawnTile(n.getPosition());
            }
        }
        //playerTurnEndedDict[currentPlayerInfo.getPlayerId()] = true;
        highlightedPossibleSwapPartner = false;
        ChangeState(GameState.HandleTileState);
    }

    /*
        GameState.AttackState Related Functions
    */

    public void preProcessAttackState() {
        int playerAttackRange = 1; //TODO Implement this based on player's class
        List<Node> validAttackNodes = NodeUtils.getViableAttackNodes(playerAttackRange, clickedPlayerNode, nodeDict);
        foreach (Node node in validAttackNodes) {
            //tileMap.SetColor(node.getPosition(), Color.red);
            staticTileHandler.SpawnTile(node.getPosition(), "RedTile");
        }        
    }

    void processAttackState(Node currentClickedNode) {
        int playerAttackRange = 1; //TODO Implement this based on player's class
        List<Node> validAttackNodes = NodeUtils.getViableAttackNodes(playerAttackRange, clickedPlayerNode, nodeDict);
        if (currentClickedNode != null && validAttackNodes.Contains(currentClickedNode)) { //clicked on a valid attack node. Time to attack
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }

            Node enemyNodeToAttack = currentClickedNode;
            Node playerAttacking = clickedPlayerNode;
            //display battle
            calculateBattleEventDisplayBattleUI(playerAttacking, enemyNodeToAttack);
            //turn is over for player
            ChangeState(GameState.ShowExpGainState);
        } else if (currentClickedNode != null && !validAttackNodes.Contains(currentClickedNode)) {
            //Remove the red highlight and go back to battle menu state
            foreach (Node node in validAttackNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }
            ChangeState(GameState.ShowBattleMenuState);       
        } else if (Input.GetMouseButtonDown(1) && !battleEventScreen.IsBattleEventScreenDisplayed()) { //cancel movement and track back
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
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
        PlayerInfo newAttackerPI = PlayerInfo.Clone(attackerPI);
        PlayerInfo newDefenderPI = PlayerInfo.Clone(defenderPI);
        int attackerHealth = attackerPI.currentHealth;
        int defenderHealth = defenderPI.currentHealth;
        int attackerAtk = attackerPI.currentAttack;
        int defenderAtk = defenderPI.currentAttack;
        int attackerDef = attackerPI.currentDefense;
        int defenderDef = defenderPI.currentDefense;

        //attacker attacks defender
        int dmgDoneToDefender = attackerAtk - defenderDef;
        newDefenderPI.currentHealth = defenderHealth - Mathf.Max(0, dmgDoneToDefender);
        if (newDefenderPI.currentHealth <= 0) {
            newDefenderPI.currentHealth = 0;
            if (newDefenderPI.getIsEnemy()) {
                //player gains experience
                newAttackerPI.gainExp(newDefenderPI.level * 50);
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
            attackerNode.setPlayerInfo(newAttackerPI);
            defenderNode.setPlayerInfo(null);
        } else {
            int dmgDoneToAttacker = defenderAtk - attackerDef;
            newAttackerPI.currentHealth = attackerHealth - Mathf.Max(0, dmgDoneToAttacker);    
            if (newAttackerPI.currentHealth <= 0) {
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
                attackerNode.setPlayerInfo(null);
                defenderNode.setPlayerInfo(newDefenderPI);
            } else {
                attackerNode.setPlayerInfo(newAttackerPI);
                defenderNode.setPlayerInfo(newDefenderPI);
            }
        } 
        StartCoroutine(battleEventScreen.openBattleEventScreen(attackerPI, defenderPI, newAttackerPI, newDefenderPI, true, true));
        nodeDict[attackerNode.getPosition()] = attackerNode;
        nodeDict[defenderNode.getPosition()] = defenderNode;

        if (!attackerPI.getIsEnemy() && newAttackerPI.currentHealth > 0 && newDefenderPI.currentHealth == 0) {
            newAttackerPI.gainExp(1500);
            timesLeveledUp = newAttackerPI.level - attackerPI.level;
            totalExpToLevelUp = newAttackerPI.getTotalExpListLevels(attackerPI.level, newAttackerPI.level);
            oldBattlePI = attackerPI;
            newBattlePI = newAttackerPI;
            showExpGainBar = true;
        } else if (attackerPI.getIsEnemy() && newAttackerPI.currentHealth == 0 && newDefenderPI.currentHealth > 0) {
            newDefenderPI.gainExp(1500);
            timesLeveledUp = newDefenderPI.level - defenderPI.level;
            totalExpToLevelUp = newDefenderPI.getTotalExpListLevels(defenderPI.level, newDefenderPI.level);
            oldBattlePI = defenderPI;
            newBattlePI = newDefenderPI;
            showExpGainBar = true;
        }
    }

    /*
        GameState.ShowExpGainState Related Functions
    */
    void processShowExpGainState() {
        if (showExpGainBar == false && !battleEventScreen.IsBattleEventScreenDisplayed()) {
            ChangeState(GameState.HandleTileState);
        } else {
            if (!battleEventScreen.IsBattleEventScreenDisplayed() && !playerExpScreen.IsExperienceScreenProcessing()) {
                Vector3 playerPos = clickedPlayerNode.getPosition();
                playerExpScreen.ShowPlayerGainExpScreen(oldBattlePI, newBattlePI, totalExpToLevelUp, timesLeveledUp, playerPos);
                showExpGainBar = false;
                oldBattlePI = null;
                newBattlePI = null;
                ChangeState(GameState.HandleTileState);
            }
        }
    }

    void processHandleTileState() {
        //get current PI
        if (clickedPlayerNode != null) {
            PlayerInfo currentPI = clickedPlayerNode.getPlayerInfo();
            tileEventManager.ProcessTile(clickedPlayerNode, currentPI, gameStateInstance);
            CheckIfPlayerDied(currentPI);
        }
        ChangeState(GameState.TurnEndState);
    }

    /*
        GameState.TurnEndState Related Functions
    */
    void processTurnEndState() {

        //record end of unit's turn in dict, but only if it exists (if it died, we ignore since that node is now null)
        if (!isEnemyTurn) {
            if (clickedNode.getPlayerId() != null) {
                playerTurnEndedDict[clickedPlayerNode.getPlayerInfo().getPlayerId()] = true;
                clickedPlayerNode.getPlayerInfo().playerAnimator.AnimateSpriteTurnEnded();
            }    
            //auto turn-end if all units are done
            if (!playerTurnEndedDict.ContainsValue(false) && !phaseTransitionUIHandler.IsPhaseTransitionRunning() && !playerExpScreen.IsExperienceScreenProcessing()) {
                resetPlayerTurnEndedDict();
                StartCoroutine(phaseTransitionUIHandler.translatePhaseImage("EnemyPhase"));
                //check if all enemies or players have died
                checkIfGameEnded(); 
                ChangeState(GameState.EnemyTurnState);
                resetTurnStateData();
            } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !playerExpScreen.IsExperienceScreenProcessing()) { //else we go back to the player turn start state, but wait for transition to not run
                //check if all enemies or players have died
                checkIfGameEnded(); 
                ChangeState(GameState.PlayerTurnStart);         
                //reset turn state data
                resetTurnStateData();
            }        
        } else {
            //check if all enemies or players have died
            checkIfGameEnded(); 
            ChangeState(GameState.EnemyTurnState);
        }
   
    }

    /*
        GameState.ProcessEnemyState Related Functions
    */

    //Enemy logic handling// Do NOT move this to external class
    void processEnemyTurnState() {
        isEnemyTurn = true;
        //process action for each enemy only if someone else is not attacking and we have a valid enemy that hasn't ended their turn
        if (!battleEventScreen.IsBattleEventScreenDisplayed() && !HaveAllEnemiesHaveMoved() && !phaseTransitionUIHandler.IsPhaseTransitionRunning()) {
            PlayerInfo enemy = pickAvailableEnemy();

            if (enemy == null) { //no available enemy. All enemies have moved.
                SetAllEnemiesHaveMoved(true);
            } else {
                //if player in line of sight, move and attack
                Node enemyNode = NodeUtils.findEnemyNode(enemy.getPlayerId(), nodeDict);
                List<Node> nodeRange = NodeUtils.getViableNodesPaths(enemyNode, nodeDict, tileEventManager);
                Node candidatePlayerNode = NodeUtils.findPlayerNodeNearEnemy(enemyNode, nodeDict, tileEventManager);
                if (candidatePlayerNode != null) {      
                    clickedPlayerNode = candidatePlayerNode;      
                    //check if we have to move
                    Node attackerNode = enemyNode;
                    if (!NodeUtils.getNearbyNodes(enemyNode, nodeDict).Contains(candidatePlayerNode)) { //move and attack else just attack
                        //move and attack
                        List<Node> pathToMove = NodeUtils.getShortestPathNodes(enemyNode, candidatePlayerNode, nodeRange, Heuristic.NodeDistanceHeuristic, nodeDict);
                        PlayerAnimator enemyToAnimate = enemy.playerAnimator;
                        //Player enemyToMove = GameObject.Find(enemy.getPlayerId()).GetComponent<Player>();
                        pathToMove.RemoveAt(pathToMove.Count - 1); //remove the last element since that's the player
                        enemyToAnimate.MoveEnemyNextToPlayer(tileMap, pathToMove);
                        swapNodeInfoOnSpriteMove(enemyNode, pathToMove[pathToMove.Count - 1]); //swap node data with new tile   
                        attackerNode = pathToMove[pathToMove.Count - 1]; //update the attackerNode                     
                    }
                    enemyTurnEndedDict[enemy.getPlayerId()] = true;
                    calculateBattleEventDisplayBattleUI(attackerNode, candidatePlayerNode);
                    ChangeState(GameState.ShowExpGainState);
                } else { //nothing to do, just end turn
                    enemyTurnEndedDict[enemy.getPlayerId()] = true;
                }
            }
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !startedTranslation && HaveAllEnemiesHaveMoved() && !battleEventScreen.IsBattleEventScreenDisplayed() && !playerExpScreen.IsExperienceScreenProcessing()) {
            if (!checkIfGameEnded()) {
                startedTranslation = true;
                StartCoroutine(phaseTransitionUIHandler.translatePhaseImage("PlayerPhase"));                
            }
        } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && startedTranslation) { //done processing, end turn
            resetEnemyTurnEndedDict();
            startedTranslation = false;
            SetAllEnemiesHaveMoved(false);
            isEnemyTurn = false;
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
        GameState.ProcessEnemyState Related Functions
    */

    void processGameEndState() {
        if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !playerExpScreen.IsExperienceScreenProcessing()) {
            if (playerVictory) {
                playerVictoryScreen.SetActive(true);
            } else {
                playerDefeatScreen.SetActive(true);
            }            
        }
    }


    /*
        Private Utility Related Functions (E.g. board setup, cancel player move, etc.)
    */
    void cancelPlayerMove() {
        if (previousClickedNode != null) {
            PlayerAnimator playerToAnimate = clickedNode.getPlayerInfo().playerAnimator;
            playerToAnimate.PlayerReturnToTile(tileMap, previousClickedNode);
            //update player information in nodes
            swapNodeInfoOnSpriteMove(clickedPlayerNode, previousClickedNode);            
        }
    }

    void CheckIfPlayerDied(PlayerInfo info) {
        if (info.currentHealth <= 0) {
            if (info.getIsEnemy()) {
                retreatedEnemies.Add(info.getPlayerId());
                enemyTurnEndedDict[info.getPlayerId()] = true;
            } else {
                retreatedPlayers.Add(info.getPlayerId());
                playerTurnEndedDict[info.getPlayerId()] = true;
            }
            GameObject playerToDestroy = GameObject.Find(info.getPlayerId());
            playerToDestroy.SetActive(false); 
            clickedPlayerNode.setPlayerInfo(null);              
        }
    }

    // Critical information handling // Do NOT move this to an external class
    void populateGridSetupData() {
        pawnInfoDict = new Dictionary<Vector2, PlayerInfo>();
        PlayerInfo newPlayer = new PlayerInfo("fakeid", "Shan", false, BattleClass.Warrior, "images/portraits/test_face");
        newPlayer.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        gameStateInstance = GameObject.Find("GameMasterInstance").AddComponent<MasterGameStateController>().instance;
        gameStateInstance.CreateNewSaveInstance(newPlayer);
        //add one more player
        PlayerInfo newPlayerTwo = new PlayerInfo("fakeid2", "Bobby", false, BattleClass.Warrior, "images/portraits/test_face");
        newPlayerTwo.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        gameStateInstance.AddNewPlayer(newPlayerTwo);
        PlayerInfo testOne = gameStateInstance.GetPlayerInfoById("fakeid");
        PlayerInfo testTwo = gameStateInstance.GetPlayerInfoById("fakeid2");
        testOne.setupBattleStats(true);
        testTwo.setupBattleStats(true);
        pawnInfoDict[new Vector2(7, -1)] = testOne;
        pawnInfoDict[new Vector2(7,-2)] = testTwo;
        PlayerInfo enemyOne = new PlayerInfo("fakeenemyid", "Mr.Evil", true, BattleClass.Warrior, "images/portraits/test_face");
        enemyOne.setAnimationPaths("images/sprites/CharacterSpriteSheets/Enemy/EnemyOne",
            "Animations/EnemyFighter/Enemy_Game",
            "Animations/EnemyFighter/Enemy_Battle");
        PlayerInfo enemyTwo = new PlayerInfo("fakeenemyid2", "Kim", true, BattleClass.Warrior, "images/portraits/test_face");
        enemyTwo.setAnimationPaths("images/sprites/CharacterSpriteSheets/Enemy/EnemyOne",
            "Animations/EnemyFighter/Enemy_Game",
            "Animations/EnemyFighter/Enemy_Battle");
        enemyOne.setupBattleStats(true);
        enemyTwo.setupBattleStats(true);
        //setup items
        ConsumableItem healthPotion = (ConsumableItem) Resources.Load("Items/Stock/ConsumableItems/MinorHealthPotion", typeof(ConsumableItem));
        healthPotion.itemSprite = (Sprite) Resources.Load("images/ui/Icons/HealthPotionIcon", typeof(Sprite));
        ConsumableItem manaPotion = (ConsumableItem) Resources.Load("Items/Stock/ConsumableItems/MinorManaPotion", typeof(ConsumableItem));
        manaPotion.itemSprite = (Sprite) Resources.Load("images/ui/Icons/ManaPotionIcon", typeof(Sprite));
        EquipmentItem vest = (EquipmentItem) Resources.Load("Items/Stock/EquipmentItems/LeatherVest", typeof(EquipmentItem));
        List<ConsumableItem> cIList = new List<ConsumableItem> {healthPotion, healthPotion};
        EquipmentItem[] eIList = new EquipmentItem[System.Enum.GetNames(typeof(EquipType)).Length];
        eIList[(int) vest.equipType] = vest;
        testOne.LoadItems(cIList, eIList);
        testTwo.LoadItems(new List<ConsumableItem>(){healthPotion, manaPotion}, new EquipmentItem[System.Enum.GetNames(typeof(EquipType)).Length]);
        //set enemy locations
        pawnInfoDict[new Vector2(2,2)] = enemyOne;
        pawnInfoDict[new Vector2(-3,-3)] = enemyTwo;
        //set node data for player and enemies MAP TILEMAP POSITION TO NODE POSITION
        tileMap = transform.GetComponentInParent<Tilemap>();
        tileEventManager.Setup(tileMap);
        staticTileHandler.Setup(tileMap);
        foreach (var position in tileMap.cellBounds.allPositionsWithin) {
            Vector2 pos = new Vector2(position.x, position.y);
            PlayerInfo playerInfo = null;
            if (pawnInfoDict.ContainsKey(pos)) {
                playerInfo = pawnInfoDict[pos];
                populateGridWithSprite(pos, pawnInfoDict, position, playerInfo);
            }
            tileMap.SetTileFlags(position, TileFlags.None); //this is so we can change the color freely
            Color originalTileColor = tileMap.GetColor(position);
            nodeDict[position] = new Node(position, playerInfo, originalTileColor);
        }
    }

    void resetPlayerTurnEndedDict() {
        foreach (PlayerInfo pi in pawnInfoDict.Values) {
            if (!pi.getIsEnemy()) {
                if (retreatedPlayers.Contains(pi.getPlayerId())) {
                    playerTurnEndedDict[pi.getPlayerId()] = true;
                } else {
                    playerTurnEndedDict[pi.getPlayerId()] = false;
                    pi.playerAnimator.AnimateRevertSpriteTurnStarted();                
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
        newPawn.setAnimatorMode("game", playerId, playerInfo.getGameControllerPath(), playerInfo.portraitRefPath);
        newPawn.name = playerId;
        pawnInfoDict[pos2D].playerAnimator = newPawn;
        if (newPawn) {
            newPawn.AddSpriteToTile(tileMap, pos3D);
        }        
    }

    void swapNodeInfoOnSpriteMove(Node source, Node dest) {
        PlayerInfo playerInfo = source.getPlayerInfo();
        dest.setPlayerInfo(playerInfo);
        source.setPlayerInfo(null);
        //finally, we update the node dict to reflect this
        nodeDict[source.getPosition()] = source;
        nodeDict[dest.getPosition()] = dest;
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

    public GameState GetCurrentState() {
        return currentState;
    }

    //UI related logic
    Node getNodePositionOnClick() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 globalPosition = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3Int localPosition = tileMap.WorldToCell(globalPosition);
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

    public bool checkIfGameEnded() {
        if (retreatedEnemies.Count == enemyTurnEndedDict.Keys.Count) {
            playerVictory = true;
            ChangeState(GameState.GameEndState);
            return true;
        } else if (retreatedPlayers.Count == playerTurnEndedDict.Keys.Count) {
            playerVictory = false;
            ChangeState(GameState.GameEndState);
            return true;
        }
        return false;
    }

    public Node getNodeAtPosition(Vector3Int pos) {
        return nodeDict.ContainsKey(pos)? nodeDict[pos] : null;
    }

}
