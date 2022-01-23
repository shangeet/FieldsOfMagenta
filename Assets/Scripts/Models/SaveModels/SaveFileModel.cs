using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveFileModel {

    public string saveFileName;
    public SaveDataInfo saveData;
    public string saveTimestamp;

    public SaveFileModel(SaveDataInfo saveData) {
        this.saveData = saveData;
    }
}