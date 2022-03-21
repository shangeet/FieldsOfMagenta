using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClassConstants
{

    // Reference base stats dicts
    public static List<int> defaultBaseStats = new List<int> {1, 10, 10, 10, 10, 10, 10, 10, 2, 1};
    public static List<int> warriorBaseStats = new List<int> {1, 10, 10, 10, 10, 10, 10, 10, 2, 1};
    public static List<int> paladinBaseStats = new List<int> {1, 15, 10, 6,	10,	8, 10, 10, 1, 1};
    public static List<int> healerBaseStats = new List<int> {1, 8, 5, 5, 10, 20, 10, 10, 2,	2};
    public static List<int> rogueBaseStats = new List<int> {1, 12, 8, 8, 6, 6, 15, 12, 3, 1};
    public static List<int> rangerBaseStats = new List<int> {1, 8, 10, 8, 10, 8, 14, 10, 2, 2};
    public static List<int> mageBaseStats = new List<int> {1, 12, 6, 5, 15, 10, 10, 10, 2};
    public static List<int> bardBaseStats = new List<int> {1, 11, 6, 7, 6, 7, 15, 15, 3, 2};


    public static List<int> getBaseStatsForBattleClass(BattleClass battleClass) {
        switch(battleClass) {
            case BattleClass.Warrior:
                return warriorBaseStats;
            case BattleClass.Paladin:
                return paladinBaseStats;
            case BattleClass.Healer:
                return healerBaseStats;
            case BattleClass.Mage:
                return mageBaseStats;
            case BattleClass.Rogue:
                return rogueBaseStats;
            case BattleClass.Ranger:
                return rangerBaseStats;
            case BattleClass.Bard:
                return bardBaseStats;
            default:
                return defaultBaseStats;
        }
    }

    // Experience dicts
    //List [ExpToNextLevel, health+, attack+, defense+, magAtk+, magDef+, dex+, luck+, mov+]
    public static Dictionary<int, List<int>> warriorExpDict = new Dictionary<int, List<int>> {
        {1, new List<int>(){1000, 1, 1, 1, 0, 0, 1, 0, 1}},
        {2, new List<int>(){2000, 0, 1, 0, 1, 1, 0, 1, 0}}
    };

    public static List<int> getExperienceDictByClassAndLevel(BattleClass battleClass, int level) {
        switch(battleClass) {
            case BattleClass.Warrior:
                return warriorExpDict[level];
            default:
                return new List<int> {int.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0};
        }
    }

}
