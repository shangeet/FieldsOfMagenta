using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTileRenderer : MonoBehaviour {
    
    public const string LAYER_NAME = "Sprite";

    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    public void SetAttributes(Vector3 pos, int sortingOrder) {
        transform.position = pos;
        transform.localScale = new Vector3(6.0f, 6.0f, 1.0f);
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = LAYER_NAME;
    }

    public void SpawnTile(Sprite tileToRender) {
        spriteRenderer.sprite = tileToRender;     
    }

    public void DespawnTile() {
        spriteRenderer.sprite = null;
    }

}
