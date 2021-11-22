using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

//This class handles animations 
public class Player : MonoBehaviour
{
    public string id;
    public string textureRefPath;
    public bool isEnemy;
    public const string LAYER_NAME = "Sprite";
    public int sortingOrder = 3;
    public Texture2D texture;
    private Sprite playerSprite;
    public Sprite playerPortrait {get; set;}
    public SpriteRenderer spriteRenderer;


    void Awake() {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    public void Setup(PlayerInfo info) {
        id = info.getPlayerId();
        textureRefPath = info.getTextureRefPath();
        isEnemy = info.getIsEnemy();
        texture = Resources.Load<Texture2D>(textureRefPath);
        Texture2D portraitTexture = Resources.Load<Texture2D>(info.portraitRefPath);
        playerPortrait = Sprite.Create(portraitTexture, new Rect(0.0f, 0.0f, portraitTexture.width, portraitTexture.height), new Vector2(0.5f, 0.5f));
        print(texture);
        print(playerPortrait);
    }

    public void AddSpriteToTile(Tilemap tileMap, Vector3Int pos) {
        transform.position = tileMap.GetCellCenterWorld(pos);
        spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        playerSprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = playerSprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = LAYER_NAME;
    }

    public void AddPlayerToParallax(Vector3 pos) {
        transform.position = pos;
        spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        playerSprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = playerSprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = LAYER_NAME;
    }

    public void MovePlayerToTile(Tilemap tileMap, List<Node> pathToTake) {
        Debug.Log("MOVED Player");
        foreach(Node node in pathToTake) {
            transform.position = tileMap.GetCellCenterWorld(new Vector3Int(node.getPosition().x, node.getPosition().y, 1));
        }
    }

    public void MoveEnemyNextToPlayer(Tilemap tileMap, List<Node> pathToTake) {
        Debug.Log("Moved Enemy");
        foreach(Node node in pathToTake) {
            transform.position = tileMap.GetCellCenterWorld(new Vector3Int(node.getPosition().x, node.getPosition().y, 1));
        }
    }

}
