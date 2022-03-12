using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 0)]
public class Quest : ScriptableObject {

    public int questId;
    public List<int> prereqQuestIds;
    public string questName;
    public string questDescription;
    public string naninovelQuestScenePath;
    public QuestType questType;
    public bool isQuestCompleted = false;

    public Quest(int questId, List<int> prereqQuestIds, string questName, string questDescription, string naninovelQuestPath, QuestType questType, bool isQuestCompleted) {
        this.questId = questId;
        this.prereqQuestIds = prereqQuestIds;
        this.questName = questName;
        this.questDescription = questDescription;
        this.naninovelQuestScenePath = naninovelQuestPath;
        this.questType = questType;
        this.isQuestCompleted = isQuestCompleted;
    }

    public bool meetsPrerequisites(List<int> currentCompletedQuestIds) {
        foreach(int questId in prereqQuestIds) {
            if (!currentCompletedQuestIds.Contains(questId)) {
                return false;
            }
        }
        return true;
    }
    
}
