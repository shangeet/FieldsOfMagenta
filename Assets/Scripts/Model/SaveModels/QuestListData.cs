using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestListData {
    
    public List<Quest> availableQuests;
    public List<int> completedQuestIds;

    public QuestListData() {
        availableQuests = new List<Quest>();
        completedQuestIds = new List<int>();
    }

}