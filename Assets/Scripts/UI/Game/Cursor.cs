using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    [SerializeField] private Sprite cursor;
    [SerializeField] private RuntimeAnimatorController animatorController;
    private SpriteRenderer spriteRenderer; 
    private Animator animator;
    private GameMaster gameMaster;
    private const string LAYER_NAME = "Sprite";
    private UnitInfoMenu unitInfoMenu;
    private Camera gameCamera;

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cursor;
        spriteRenderer.sortingOrder = 3;
        spriteRenderer.sortingLayerName = LAYER_NAME;
        animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        transform.localScale = new Vector3(6.0f, 6.0f, 1.0f);
        gameMaster = GameObject.Find("Grid").gameObject.transform.GetChild(0).gameObject.GetComponent<GameMaster>();
        gameCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (gameMaster.IsInputEnabled() && gameCamera.isActiveAndEnabled) {
            Vector3Int pos = getNodePositionOnHover();
            bool cursorAllowed = (gameMaster.currentState == GameState.PlayerTurnStart ||
                                gameMaster.currentState == GameState.MovePlayerStartState ||
                                gameMaster.currentState == GameState.ShowBattleMenuState ||
                                gameMaster.currentState == GameState.AttackState);
            if (pos != null && cursorAllowed) {
                transform.position = gameMaster.tileMap.GetCellCenterWorld(pos);  
                Node currentClickedNode = gameMaster.getNodeAtPosition(pos);
                processShowUnitInfo(currentClickedNode);
            }            
        }
    }

    Vector3Int getNodePositionOnHover() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 globalPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int localPosition = gameMaster.tileMap.WorldToCell(globalPosition);
        return localPosition;
    }

    void processShowUnitInfo(Node currentClickedNode) {
        unitInfoMenu = gameMaster.unitInfoMenu;
        if (NodeUtils.nodeClickedIsPlayer(currentClickedNode) && !gameMaster.playerBattleMenu.IsPlayerBattleMenuDisplayed()) {
            unitInfoMenu.openUnitInfoMenu(currentClickedNode.getPlayerInfo());
        } else {
            unitInfoMenu.closeUnitInfoMenu();
        }
    }
}
