using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveDataInfo {

    //In game data
    [System.NonSerialized] public PlayerInfoListData playerInfoData;
    [System.NonSerialized] public Inventory playerInventory;

    //Serialized Save file data
    public SerializableInventory playerSerializedInventory;
    public SerializablePlayerInfoListData playerSerializedInfoData;

    //Misc
    public QuestListData playerQuestData;
    public int playerGold;

    public SaveDataInfo(QuestListData playerQuestData, PlayerInfoListData playerInfoData, Inventory playerInventory, int playerGold) {
        this.playerQuestData = playerQuestData;
        this.playerInfoData = playerInfoData;
        this.playerInventory = playerInventory;
        this.playerGold = playerGold;
    }

    public void Serialize() {
        //serialize player info data
        List<SerializablePlayerInfo> serPI = new List<SerializablePlayerInfo>();
        foreach (PlayerInfo pInfo in playerInfoData.playerInfoDataList) {
            serPI.Add(SerializablePlayerInfo.ConvertFromPlayerInfo(pInfo));
        }
        //serialize player inventory
        playerSerializedInfoData = new SerializablePlayerInfoListData(serPI);
        playerSerializedInventory = SerializableInventory.ConvertFromInventory(playerInventory);
        playerInfoData = null;
        playerInventory = null;
    }

    public void Deserialize() {
        List<PlayerInfo> pInfoList = new List<PlayerInfo>();
        foreach(SerializablePlayerInfo sPI in playerSerializedInfoData.serializablePlayerInfoList) {
            pInfoList.Add(SerializablePlayerInfo.ConvertToPlayerInfo(sPI));
        }
        playerInfoData = new PlayerInfoListData(pInfoList);
        playerInventory = SerializableInventory.ConvertToInventory(playerSerializedInventory);
        playerSerializedInventory = null;
        playerSerializedInfoData = null;
    }
}