using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInfoListData {

    public List<PlayerInfo> playerInfoDataList;

    public PlayerInfoListData() {
        playerInfoDataList = new List<PlayerInfo>();
    }

    public PlayerInfoListData(List<PlayerInfo> playerInfoDataList) {
        this.playerInfoDataList = playerInfoDataList;
    }

}