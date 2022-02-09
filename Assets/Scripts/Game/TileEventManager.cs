using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {OBSTACLE, LEVER, TRAP, TREASURE}
public class TileEventManager : MonoBehaviour {

    public int sortingOrder = 2;

    [System.Serializable]
    public struct ObstacleTileEvent {
        [System.NonSerialized]
        public int id;
        public Vector3Int tilePos;
    }

    [System.Serializable]
    public struct LeverTileEvent {
        [System.NonSerialized]
        public int id;
        public Vector3Int tilePos;
        public Vector3Int tileToTriggerLocation;
        public Sprite triggerSpriteBefore;
        public Sprite triggerSpriteAfter;
        public Sprite triggerLocSpriteBefore;
        public Sprite triggerLocSpriteAfter;
    }

    [System.Serializable] 
    public struct TrapTileEvent {
        [System.NonSerialized]
        public int id;
        public Vector3Int tilePos;
        public Sprite trapSprite;
        public int damage;
    }

    [System.Serializable]
    public struct TreasureTileEvent {
        [System.NonSerialized]
        public int id;
        public Vector3Int tilePos;
        public Sprite treasureSpriteBefore;
        public Sprite treasureSpriteAfter;
        public ConsumableItem consumableTreasure;
        public EquipmentItem equipmentTreasure;
        public int gold;      
    }

    [SerializeField] 
    public List<ObstacleTileEvent> obstacleTileEvents;

    [SerializeField]
    public List<LeverTileEvent> leverTileEvents;

    [SerializeField]
    public List<TrapTileEvent> trapTileEvents;

    [SerializeField]
    public List<TreasureTileEvent> treasureTileEvents;

    private Dictionary<Vector3Int, int> tileIdDict = new Dictionary<Vector3Int, int>(); 
    private Dictionary<Vector3Int, TileType> tileEventsDict = new Dictionary<Vector3Int, TileType>();

    public void Setup(Tilemap tileMap) {
        //setup tiles
        int id = 0;

        for (int i = 0; i < obstacleTileEvents.Count; i++) {
            ObstacleTileEvent tileEvent  = obstacleTileEvents[i];
            tileEvent.id = id;
            setupEvent(tileMap, tileEvent, id);
            tileIdDict[tileEvent.tilePos] = id;
            tileEventsDict[tileEvent.tilePos] = TileType.OBSTACLE;
            obstacleTileEvents[i] = tileEvent;
            id++;
        }

        for (int i = 0; i < leverTileEvents.Count; i++) {
            LeverTileEvent tileEvent = leverTileEvents[i];
            tileEvent.id = id;
            setupEvent(tileMap, tileEvent, id);
            tileIdDict[tileEvent.tilePos] = id;
            tileEventsDict[tileEvent.tilePos] = TileType.LEVER; 
            tileEventsDict[tileEvent.tileToTriggerLocation] = TileType.OBSTACLE;
            leverTileEvents[i] = tileEvent; 
            id++;          
        }

        for (int i = 0; i < trapTileEvents.Count; i++) {
            TrapTileEvent tileEvent = trapTileEvents[i];
            tileEvent.id = id;
            setupEvent(tileMap, tileEvent, id);
            tileIdDict[tileEvent.tilePos] = id;
            tileEventsDict[tileEvent.tilePos] = TileType.TRAP;
            trapTileEvents[i] = tileEvent; 
            id++;           
        }

        for (int i = 0; i < treasureTileEvents.Count; i++) {
            TreasureTileEvent tileEvent = treasureTileEvents[i];
            tileEvent.id = id;
            setupEvent(tileMap, tileEvent, id);
            tileIdDict[tileEvent.tilePos] = id;
            tileEventsDict[tileEvent.tilePos] = TileType.TREASURE;  
            treasureTileEvents[i] = tileEvent;
            id++;          
        }
    }

    public void ProcessTile(Node node, PlayerInfo player, MasterGameStateController instance) {
        Vector3Int nodePos = node.getPosition();
        if (IsLeverTile(node)) {
            int id = tileIdDict[nodePos];
            LeverTileEvent leverTile = getLeverEventById(id);
            processEvent(leverTile, id);
            removeTileAtPos(nodePos); //no longer want to trigger the lever tile
            removeTileAtPos(leverTile.tileToTriggerLocation); //this tile is no longer an obstacle
        } else if (IsTrapTile(node)) {
            int id = tileIdDict[nodePos];
            TrapTileEvent trapTile = getTrapEventById(id);
            processEvent(player, trapTile, id);
        } else if (IsTreasureTile(node)) {
            int id = tileIdDict[nodePos];
            TreasureTileEvent treasureTile = getTreasureEventById(id);
            processEvent(instance, player, treasureTile, id);
            removeTileAtPos(nodePos); //no longer want to trigger the treasure tile
        }
    }

    private LeverTileEvent getLeverEventById(int id) {
        foreach(LeverTileEvent tileEvent in leverTileEvents) {
            if (tileEvent.id == id) {
                return tileEvent;
            }
        }
        return new LeverTileEvent();
    }

    private TrapTileEvent getTrapEventById(int id) {
        foreach(TrapTileEvent tileEvent in trapTileEvents) {
            if (tileEvent.id == id) {
                return tileEvent;
            }
        }
        return new TrapTileEvent();
    }

    private TreasureTileEvent getTreasureEventById(int id) {
        foreach(TreasureTileEvent tileEvent in treasureTileEvents) {
            if (tileEvent.id == id) {
                return tileEvent;
            }
        }
        return new TreasureTileEvent();
    }

    private void removeTileAtPos(Vector3Int pos) {
        if (tileIdDict.ContainsKey(pos)) {
            tileIdDict.Remove(pos);
        }
        if (tileEventsDict.ContainsKey(pos)) {
            tileEventsDict.Remove(pos);
        }
    }

    private void setupEvent(Tilemap tilemap, ObstacleTileEvent tileEvent, int id) {
        GameObject obstacleGo = new GameObject();
        obstacleGo.name = "Obstacle-" + id.ToString();
        obstacleGo.transform.position = tilemap.GetCellCenterWorld(tileEvent.tilePos);
    }

    private void setupEvent(Tilemap tilemap, LeverTileEvent tileEvent, int id) {
        GameObject triggerSpriteGo = new GameObject();
        triggerSpriteGo.name = "Lever-" + id.ToString() + "-a";
        triggerSpriteGo.transform.position = tilemap.GetCellCenterWorld(tileEvent.tilePos);
        SpriteRenderer triggerRenderer = triggerSpriteGo.AddComponent<SpriteRenderer>();
        triggerRenderer.sprite = tileEvent.triggerSpriteBefore;
        triggerRenderer.sortingOrder= sortingOrder;
        GameObject triggerObjSpriteGo = new GameObject();
        triggerObjSpriteGo.name = "Lever-" + id.ToString() + "-b";
        triggerObjSpriteGo.transform.position = tilemap.GetCellCenterWorld(tileEvent.tileToTriggerLocation);
        SpriteRenderer triggerObjRenderer = triggerObjSpriteGo.AddComponent<SpriteRenderer>();
        triggerObjRenderer.sprite = tileEvent.triggerLocSpriteBefore;
        triggerObjRenderer.sortingOrder = sortingOrder;
    }

    private void processEvent(LeverTileEvent tileEvent, int id) {
        GameObject triggerSpriteGo = GameObject.Find("Lever-" + id.ToString() + "-a");
        SpriteRenderer renderer = triggerSpriteGo.GetComponent<SpriteRenderer>();
        renderer.sprite = tileEvent.triggerSpriteAfter;
        GameObject triggerLocSpriteGo = GameObject.Find("Lever-" + id.ToString() + "-b");
        SpriteRenderer locRenderer = triggerLocSpriteGo.GetComponent<SpriteRenderer>();
        locRenderer.sprite = tileEvent.triggerLocSpriteAfter;
    }

    private void setupEvent(Tilemap tilemap, TrapTileEvent tileEvent, int id) {
        GameObject trapSpriteGo = new GameObject();
        trapSpriteGo.name = "Trap-" + id.ToString();
        trapSpriteGo.transform.position = tilemap.GetCellCenterWorld(tileEvent.tilePos);
        SpriteRenderer renderer = trapSpriteGo.AddComponent<SpriteRenderer>();
        renderer.sprite = null;
        renderer.sortingOrder= sortingOrder;        
    }

    private void processEvent(PlayerInfo player, TrapTileEvent tileEvent, int id) {
        //reveal the tile
        GameObject trapSpriteGo = GameObject.Find("Trap-" + id.ToString());
        SpriteRenderer renderer = trapSpriteGo.GetComponent<SpriteRenderer>();
        renderer.sprite = tileEvent.trapSprite;
        renderer.sortingOrder= sortingOrder;         
        player.currentHealth -= tileEvent.damage;
        player.currentHealth = Mathf.Max(0, player.currentHealth);        
    }

    private void setupEvent(Tilemap tilemap, TreasureTileEvent tileEvent, int id) {
        GameObject treasureSpriteGo = new GameObject();
        treasureSpriteGo.name = "Treasure-" + id.ToString();
        SpriteRenderer renderer = treasureSpriteGo.AddComponent<SpriteRenderer>();
        renderer.sprite = tileEvent.treasureSpriteBefore;
        treasureSpriteGo.transform.position = tilemap.GetCellCenterWorld(tileEvent.tilePos);
        renderer.sortingOrder = sortingOrder;         
    }

    private void processEvent(MasterGameStateController instance, PlayerInfo player, TreasureTileEvent tileEvent, int id) {
        
        if (tileEvent.gold > 0) {
            instance.AddPlayerGold(tileEvent.gold);
        }

        if (tileEvent.consumableTreasure != null && player.consumableItemManager.CanAddItem(tileEvent.consumableTreasure, 1)) {
            player.consumableItemManager.AddItemWithQuantity(tileEvent.consumableTreasure, 1);
        }

        if (tileEvent.equipmentTreasure != null) {
            instance.AddItemToInventory(tileEvent.equipmentTreasure);
        } 

        GameObject treasureSpriteGo = GameObject.Find("Treasure-" + id.ToString());
        SpriteRenderer renderer = treasureSpriteGo.GetComponent<SpriteRenderer>();
        renderer.sprite = tileEvent.treasureSpriteAfter;
    }

    public bool IsEventTile(Node node) {
        return tileEventsDict.ContainsKey(node.getPosition());
    }

    public bool IsObstacleTile(Node node) {
        return IsEventTile(node) && tileEventsDict[node.getPosition()] == TileType.OBSTACLE;
    }

    public bool IsLeverTile(Node node) {
        return IsEventTile(node) && tileEventsDict[node.getPosition()] == TileType.LEVER;
    }

    public bool IsTrapTile(Node node) {
        return IsEventTile(node) && tileEventsDict[node.getPosition()] == TileType.TRAP;
    }

    public bool IsTreasureTile(Node node) {
        return IsEventTile(node) && tileEventsDict[node.getPosition()] == TileType.TREASURE;
    }

}
