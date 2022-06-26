using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class QuestsDatabase {

    private static string questsPath = "Quests/QuestAssets";

    public static List<Quest> GetAllQuests() {
        return FileUtils.GetAssetsAtPath<Quest>(questsPath); 
    }

    public static List<Quest> GetAllActiveQuests(List<int> completedQuests) {
        List<Quest> activeQuests = new List<Quest>();
        foreach (Quest quest in GetAllQuests()) {
            if (quest.meetsPrerequisites(completedQuests)) {
                activeQuests.Add(quest);
            }
        }
        return activeQuests;
    }

}