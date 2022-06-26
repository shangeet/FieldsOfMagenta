using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    [SerializeField] private Sprite cursor;
    [SerializeField] private RuntimeAnimatorController animatorController;
    private SpriteRenderer spriteRenderer; 
    private Animator animator;
    private UIHandler uiHandler;
    private SharedResourceBus sharedResourceBus;
    private const string LAYER_NAME = "Sprite";
    private UnitInfoMenu unitInfoMenu;
    private Camera gameCamera;
    private static List<GameState> validCursorMovementStates = new List<GameState>() {
        GameState.PlayerSetupState,
        GameState.PlayerTurnStart,
        GameState.MovePlayerStartState,
        GameState.ShowBattleMenuState,
        GameState.AttackState,
        GameState.HealState        
    };

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cursor;
        spriteRenderer.sortingOrder = 3;
        spriteRenderer.sortingLayerName = LAYER_NAME;
        animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        transform.localScale = new Vector3(6.0f, 6.0f, 1.0f);
        uiHandler = GameObject.Find("UIHandler").GetComponent<UIHandler>();
        sharedResourceBus = GameObject.Find("SharedResourceBus").GetComponent<SharedResourceBus>();
        gameCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        unitInfoMenu = uiHandler.GetUnitInfoMenu();
    }

    // Update is called once per frame
    void Update() {
        if (uiHandler.IsInputEnabled() && gameCamera.isActiveAndEnabled) {
            Vector3Int pos = getNodePositionOnHover();
            GameState currentGameState = sharedResourceBus.GetCurrentGameState();
            bool cursorAllowed = validCursorMovementStates.Contains(currentGameState);
            if (pos != null && cursorAllowed) {
                transform.position = sharedResourceBus.GetTileMap().GetCellCenterWorld(pos);  
                Node currentClickedNode = sharedResourceBus.getNodeAtPosition(pos);
                processShowUnitInfo(currentClickedNode);
            }            
        }
    }

    Vector3Int getNodePositionOnHover() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 globalPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int localPosition = sharedResourceBus.GetTileMap().WorldToCell(globalPosition);
        return localPosition;
    }

    void processShowUnitInfo(Node currentClickedNode) {
        if (NodeUtils.nodeClickedIsPlayer(currentClickedNode) && !uiHandler.GetPlayerBattleMenu().IsPlayerBattleMenuDisplayed()) {
            unitInfoMenu.openUnitInfoMenu(currentClickedNode.getPlayerInfo());
        } else {
            unitInfoMenu.closeUnitInfoMenu();
        }
    }
}
