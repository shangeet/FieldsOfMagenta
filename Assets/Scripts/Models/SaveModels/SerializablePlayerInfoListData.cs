using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SerializablePlayerInfoListData {
    
    public List<SerializablePlayerInfo> serializablePlayerInfoList;

    public SerializablePlayerInfoListData(List<SerializablePlayerInfo> infoList) {
        this.serializablePlayerInfoList = infoList;
    }
    
}