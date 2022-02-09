using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

//This class handles animations 
public class PlayerAnimator : MonoBehaviour {

    public string id;
    public string textureRefPath;
    public const string LAYER_NAME = "Sprite";
    public int sortingOrder = 3;
    public Texture2D texture;
    private Sprite basePlayerSprite;
    private Sprite basePlayerBattleSprite;
    public Animator animator;
    public Sprite playerPortrait {get; set;}
    public SpriteRenderer spriteRenderer;
    public GameMaster gameMaster;
    private Vector3 spriteGameScale = new Vector3(3.0f, 3.0f, 1.0f);

    void Awake() {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        animator = gameObject.AddComponent<Animator>() as Animator;
        gameMaster = GameObject.Find("Grid").gameObject.transform.GetChild(0).GetComponent<GameMaster>();
    }

    public void setAnimatorMode(string mode, string pId, string controllerPath, string portraitRefPath) {
        id = pId;
        if (mode == "battle") {
            textureRefPath = controllerPath;
            sortingOrder = 5;
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(controllerPath);
        } else if (mode == "game") {
            textureRefPath = controllerPath;
            sortingOrder = 3;
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(controllerPath);
        }
        //texture = Resources.Load<Texture2D>(textureRefPath);
        if (portraitRefPath != null) {
            Texture2D portraitTexture = Resources.Load<Texture2D>(portraitRefPath);
            playerPortrait = Sprite.Create(portraitTexture, new Rect(0.0f, 0.0f, portraitTexture.width, portraitTexture.height), new Vector2(0.5f, 0.5f));            
        }
    }

    public void AddSpriteToTile(Tilemap tileMap, Vector3Int pos) {
        transform.position = tileMap.GetCellCenterWorld(pos);
        transform.localScale = spriteGameScale;
        spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        //basePlayerSprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //spriteRenderer.sprite = basePlayerSprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = LAYER_NAME;
    }

    public void AddPlayerToParallax(Vector3 pos, bool isAttacker) {
        transform.position = pos;
        if (isAttacker) {
           spriteRenderer.flipX = true; 
        }
        spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        //basePlayerSprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //spriteRenderer.sprite = basePlayerSprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = LAYER_NAME;
    }

    public IEnumerator AttackEnemy(GameObject defenderPlayer, Vector3 defenderPos, bool isAttacker) {
        Vector3 attackerOriginalPos = transform.position;
        //move close to enemy position
        Vector3 targetPosition;
        if (isAttacker) {
            targetPosition = new Vector3(defenderPos.x - 0.5f, defenderPos.y, defenderPos.z);
        } else {
            targetPosition = new Vector3(defenderPos.x + 0.5f, defenderPos.y, defenderPos.z);
        }
        print("Moving to target position");
        animator.SetBool("IsMove", true);
        float time = 0.0f;
        float step = 0.5f;
        float totalTime = 5.0f;
        while (time != totalTime) {
            transform.position = Vector3.Lerp(transform.position, targetPosition, time/totalTime);
            time += step;
            yield return new WaitForSeconds(0.01f);
        }
        print("Attacking...");
        animator.SetBool("IsMove",false);
        animator.SetBool("IsAttack", true);
        defenderPlayer.GetComponent<PlayerAnimator>().animator.SetBool("IsTakeDamage", true);
        yield return new WaitForSeconds(0.5f);
        print("Moving back");
        animator.SetBool("IsAttack", false);
        defenderPlayer.GetComponent<PlayerAnimator>().animator.SetBool("IsTakeDamage", false);
        yield return new WaitForSeconds(0.8f);
        spriteRenderer.flipX = !spriteRenderer.flipX;
        animator.SetBool("IsMove", true);
        time = 0.0f;
        step = 0.5f;
        totalTime = 5.0f;
        while (time != totalTime) {
            transform.position = Vector3.Lerp(transform.position, attackerOriginalPos, time/totalTime);
            time += step;
            yield return new WaitForSeconds(0.01f);
        }
        spriteRenderer.flipX = !spriteRenderer.flipX;
        ResetToIdleAnimation();        
    }

    public IEnumerator AnimateFaint() {
        animator.SetBool("IsFaint", true);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator MovePlayerToTile(Tilemap tileMap, List<Node> pathToTake) {
        gameMaster.playerCurrentlyMoving = true;
        Node previousNode = null;
        foreach(Node node in pathToTake) {
            transform.position = tileMap.GetCellCenterWorld(new Vector3Int(node.getPosition().x, node.getPosition().y, 1));
            AnimateDirection(previousNode, node);
            previousNode = node;
            yield return new WaitForSeconds(0.05f);
        }
        ResetToIdleAnimation();
        gameMaster.playerCurrentlyMoving = false;
    }

    public void PlayerReturnToTile(Tilemap tileMap, Node nodeToReturn) {
        transform.position = tileMap.GetCellCenterWorld(new Vector3Int(nodeToReturn.getPosition().x, nodeToReturn.getPosition().y, 1));
    }

    public void MoveEnemyNextToPlayer(Tilemap tileMap, List<Node> pathToTake) {
        Debug.Log("Moved Enemy");
        foreach(Node node in pathToTake) {
            transform.position = tileMap.GetCellCenterWorld(new Vector3Int(node.getPosition().x, node.getPosition().y, 1));
        }
    }

    public void AnimateDirection(Node previousNode, Node currentNode) {
        foreach(AnimatorControllerParameter parameter in animator.parameters) {            
                animator.SetBool(parameter.name, false);            
        }
        if (previousNode != null && currentNode != null) {
            int xDiff = currentNode.getPosition().x - previousNode.getPosition().x;
            int yDiff = currentNode.getPosition().y - previousNode.getPosition().y;
            if (xDiff > 0) { //move right
                animator.SetBool("IsWalkRight", true);
            } else if (xDiff < 0) { //move left
                animator.SetBool("IsWalkLeft", true);
            } else if (yDiff > 0) {
                animator.SetBool("IsWalkUp", true);
            } else if (yDiff < 0) {
                animator.SetBool("IsWalkDown", true);
            }
        } else {
            ResetToIdleAnimation();
        }
    }

    public void ResetToIdleAnimation() {
        foreach(AnimatorControllerParameter parameter in animator.parameters) {            
                animator.SetBool(parameter.name, false);            
        }        
    }

    public void AnimateSpriteTurnEnded() {
        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    public void AnimateRevertSpriteTurnStarted() {
        spriteRenderer.color = new Color(1,1,1,1);
    }

}
