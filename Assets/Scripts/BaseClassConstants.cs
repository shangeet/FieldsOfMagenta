using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClassConstants
{

    // Reference base stats dicts
    public static List<int> warriorBaseStats = new List<int> {10, 10, 8, 10, 10, 10, 10, 2};


    public static List<int> getBaseStatsForBattleClass(BattleClass battleClass) {
        switch(battleClass) {
            case BattleClass.Warrior:
                return warriorBaseStats;
            default:
                return warriorBaseStats;
        }
    }

    // Experience dicts
    //List [ExpToNextLevel, health+, attack+, defense+, magAtk+, magDef+, dex+, luck+, mov+]
    public static Dictionary<int, List<int>> warriorExpDict = new Dictionary<int, List<int>> {
        {1, new List<int>(){1000, 1, 1, 1, 0, 0, 1, 0, 1}},
        {2, new List<int>(){2000, 0, 1, 0, 1, 1, 0, 1, 0}}
    };

    public static List<int> getExperienceDictByClassAndLevel(string battleClass, int level) {
        switch(battleClass) {
            case "Warrior":
                return warriorExpDict[level];
            default:
                return new List<int> {int.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0};
        }
    }

}
