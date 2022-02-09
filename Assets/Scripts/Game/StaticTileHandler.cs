using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StaticTileHandler : MonoBehaviour {

    [SerializeField]
    private int sortingOrder;
    
    [SerializeField] List<string> tileSpriteDictKeys;
    [SerializeField] List<Sprite> tileSpriteDictValues;
    private Dictionary<string, Sprite> tilesToSpriteDict = new Dictionary<string, Sprite>();
    private Dictionary<Vector3Int, GameObject> posToRendererDict = new Dictionary<Vector3Int, GameObject>();

    void Start() {
        for (int i = 0; i < tileSpriteDictKeys.Count; i++) {
            tilesToSpriteDict[tileSpriteDictKeys[i]] = tileSpriteDictValues[i];
        }
    }

    public void Setup(Tilemap tilemap) {
        foreach (var position in tilemap.cellBounds.allPositionsWithin) {
            GameObject tileRendererGo = new GameObject(position.ToString());
            StaticTileRenderer tileRenderer = tileRendererGo.AddComponent<StaticTileRenderer>() as StaticTileRenderer;
            tileRenderer.SetAttributes(tilemap.GetCellCenterWorld(position), sortingOrder);
            posToRendererDict[position] = tileRendererGo;
        }
    }

    public void SpawnTile(Vector3Int position, string tileId) {
        if (posToRendererDict.ContainsKey(position) && tilesToSpriteDict.ContainsKey(tileId)) {
            GameObject tileRendererGo = posToRendererDict[position];
            tileRendererGo.GetComponent<StaticTileRenderer>().SpawnTile(tilesToSpriteDict[tileId]);
        }
    }   

    public void DespawnTile(Vector3Int position) {
        if (posToRendererDict.ContainsKey(position)) {
            GameObject tileRendererGo = posToRendererDict[position];
            tileRendererGo.GetComponent<StaticTileRenderer>().DespawnTile();
        }
    } 
}
