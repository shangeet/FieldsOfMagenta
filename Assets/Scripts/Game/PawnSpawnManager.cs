using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PawnSpawnManager : MonoBehaviour {
    
    public List<PlayerInfo> enemiesToSpawn;
    public List<Vector2Int> enemyLocations;
    public GameObject healthBarGo;
    public GameObject statusContainerGo;
    public GameObject statusContainerSlotGo;
    public List<EffectType> statusSpriteKeys;
    public List<Sprite> statusSpriteValues;
    public Dictionary<EffectType, Sprite> effectTypeSpriteDict = new Dictionary<EffectType, Sprite>();

    public void Setup(Tilemap tileMap, Dictionary<Vector2, PlayerInfo> pawnInfoDict, Dictionary<Vector3Int, Node> nodeDict, MasterGameStateController gameStateInstance, List<Vector2> validPawnPlacements) {       

        /*
            This is the temporary setup for now. Once all features are added, we can extract this and move it out
        */
        PlayerInfo newPlayer = new PlayerInfo("fakeid", "Shan", false, new Warrior(), "images/portraits/test_face");
        newPlayer.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        gameStateInstance.CreateNewSaveInstance(newPlayer);
        //add one more player
        PlayerInfo newPlayerTwo = new PlayerInfo("fakeid2", "Bobby", false, new Warrior(), "images/portraits/test_face");
        newPlayerTwo.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        gameStateInstance.AddNewPlayer(newPlayerTwo);
        PlayerInfo testOne = gameStateInstance.GetPlayerInfoById("fakeid");
        PlayerInfo testTwo = gameStateInstance.GetPlayerInfoById("fakeid2");
        testOne.setupBattleStats(true);
        testTwo.setupBattleStats(true);
        pawnInfoDict[validPawnPlacements[0]] = testOne;
        pawnInfoDict[validPawnPlacements[1]] = testTwo;

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
        /*
            END OF TEST CODE
        */

        //setup sprite dictionary
        for (int i = 0; i < statusSpriteKeys.Count; i++) {
            if (i < statusSpriteValues.Count) {
                effectTypeSpriteDict[statusSpriteKeys[i]] = statusSpriteValues[i];  
            }
        }

        //create enemies and place them in the dict
        int idx = 0;

        foreach (PlayerInfo info in enemiesToSpawn) {
            pawnInfoDict[enemyLocations[idx]] = PlayerInfo.Clone(info);
            idx++;
        }

        foreach (var position in tileMap.cellBounds.allPositionsWithin) {
            Vector2 pos = new Vector2(position.x, position.y);
            PlayerInfo playerInfo = null;
            if (pawnInfoDict.ContainsKey(pos)) {
                playerInfo = pawnInfoDict[pos];
                PopulateGridWithSprite(tileMap, pos, pawnInfoDict, position, playerInfo);
            }
            tileMap.SetTileFlags(position, TileFlags.None); //this is so we can change the color freely
            Color originalTileColor = tileMap.GetColor(position);
            nodeDict[position] = new Node(position, playerInfo, originalTileColor);
        }
    }

    public void PopulateGridWithSprite(Tilemap tileMap, Vector2 pos2D, Dictionary<Vector2, PlayerInfo> pawnInfoDict, Vector3Int pos3D, PlayerInfo playerInfo) {
        string playerId = playerInfo.getPlayerId();
        GameObject objToSpawn = new GameObject(playerId);
        PlayerAnimator newPawn = objToSpawn.AddComponent<PlayerAnimator>() as PlayerAnimator;
        newPawn.setAnimatorMode("game", playerId, playerInfo.getGameControllerPath(), playerInfo.portraitRefPath);
        newPawn.name = playerId;
        pawnInfoDict[pos2D].playerAnimator = newPawn;
        if (newPawn) {
            newPawn.AddSpriteToTile(tileMap, pos3D);
        }
        // add health bar ui
        GameObject hbGo = Instantiate<GameObject>(healthBarGo);        
        hbGo.name = "HB-" + playerInfo.id;
        MiniHealthBar mhb = hbGo.AddComponent<MiniHealthBar>();
        // add status effect ui
        GameObject statContainerGo = Instantiate<GameObject>(statusContainerGo);
        statContainerGo.name = "SC-" + playerInfo.id;
        StatusIconContainer statusIconContainer = statContainerGo.AddComponent<StatusIconContainer>();
        mhb.Initialize(playerInfo, newPawn);
        statusIconContainer.Initialize(playerInfo, newPawn, effectTypeSpriteDict, statusContainerSlotGo);
    }

    public PlayerInfo AddSpriteToGrid(Tilemap tileMap, Vector3Int pos3D, PlayerInfo playerInfo) {
        string playerId = playerInfo.getPlayerId();
        GameObject objToSpawn = new GameObject(playerId);
        PlayerAnimator newPawn = objToSpawn.AddComponent<PlayerAnimator>() as PlayerAnimator;
        newPawn.setAnimatorMode("game", playerId, playerInfo.getGameControllerPath(), playerInfo.portraitRefPath);
        newPawn.name = playerId;
        playerInfo.playerAnimator = newPawn;
        if (newPawn) {
            newPawn.AddSpriteToTile(tileMap, pos3D);
        }
        // add health bar ui
        GameObject hbGo = Instantiate<GameObject>(healthBarGo);        
        hbGo.name = "HB-" + playerInfo.id;
        MiniHealthBar mhb = hbGo.AddComponent<MiniHealthBar>();
        // add status effect ui
        GameObject statContainerGo = Instantiate<GameObject>(statusContainerGo);
        statContainerGo.name = "SC-" + playerInfo.id;
        StatusIconContainer statusIconContainer = statContainerGo.AddComponent<StatusIconContainer>();
        mhb.Initialize(playerInfo, newPawn);
        statusIconContainer.Initialize(playerInfo, newPawn, effectTypeSpriteDict, statusContainerSlotGo);
        return playerInfo;
    }

    public void DeletePlayerSpriteOnGrid(PlayerInfo info) {
        string playerId = info.getPlayerId();
        GameObject spawnedObject = GameObject.Find(playerId);
        if (spawnedObject != null) {
            GameObject hbGo = GameObject.Find("HB-" + playerId);            
            GameObject statContainerGo = GameObject.Find("SC-" + playerId);      
            Destroy(hbGo);
            Destroy(statContainerGo);
            Destroy(spawnedObject);
        }
    }

}
