using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleClass {

    public List<int> baseStats;

    public Dictionary<int, List<int>> expDict;

    public List<int> getBaseStats() {
        return baseStats;
    }

    public List<int> getExperienceDictByClassAndLevel(int level) {
        if (expDict.ContainsKey(level)) {
            return expDict[level];
        }
        return new List<int> {int.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0};
    }
}
