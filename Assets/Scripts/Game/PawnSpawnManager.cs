using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PawnSpawnManager : MonoBehaviour {
    
    public List<PlayerInfo> enemiesToSpawn;
    public List<Vector2Int> enemyLocations;
    public GameObject healthBarGo;

    public void Setup(Tilemap tileMap, Dictionary<Vector2, PlayerInfo> pawnInfoDict, Dictionary<Vector3Int, Node> nodeDict, MasterGameStateController gameStateInstance) {       

        /*
            This is the temporary setup for now. Once all features are added, we can extract this and move it out
        */
        PlayerInfo newPlayer = new PlayerInfo("fakeid", "Shan", false, BattleClass.Warrior, "images/portraits/test_face");
        newPlayer.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
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

        //create enemies and place them in the dict
        int idx = 0;

        foreach (PlayerInfo info in enemiesToSpawn) {
            pawnInfoDict[enemyLocations[idx]] = info;
            idx++;
        }

        foreach (var position in tileMap.cellBounds.allPositionsWithin) {
            Vector2 pos = new Vector2(position.x, position.y);
            PlayerInfo playerInfo = null;
            if (pawnInfoDict.ContainsKey(pos)) {
                playerInfo = pawnInfoDict[pos];
                populateGridWithSprite(tileMap, pos, pawnInfoDict, position, playerInfo);
            }
            tileMap.SetTileFlags(position, TileFlags.None); //this is so we can change the color freely
            Color originalTileColor = tileMap.GetColor(position);
            nodeDict[position] = new Node(position, playerInfo, originalTileColor);
        }
    }

    void populateGridWithSprite(Tilemap tileMap, Vector2 pos2D, Dictionary<Vector2, PlayerInfo> pawnInfoDict, Vector3Int pos3D, PlayerInfo playerInfo) {
        string playerId = playerInfo.getPlayerId();
        GameObject objToSpawn = new GameObject(playerId);
        PlayerAnimator newPawn = objToSpawn.AddComponent<PlayerAnimator>() as PlayerAnimator;
        newPawn.setAnimatorMode("game", playerId, playerInfo.getGameControllerPath(), playerInfo.portraitRefPath);
        newPawn.name = playerId;
        pawnInfoDict[pos2D].playerAnimator = newPawn;
        if (newPawn) {
            newPawn.AddSpriteToTile(tileMap, pos3D);
        }
        //add health bar
        GameObject hbGo = Instantiate<GameObject>(healthBarGo);        
        hbGo.name = "HB-" + playerInfo.id;
        MiniHealthBar mhb = hbGo.AddComponent<MiniHealthBar>();
        mhb.Initialize(playerInfo, newPawn);
    }

}