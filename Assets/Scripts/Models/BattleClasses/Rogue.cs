using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : BattleClass
{
    public Rogue() {
        this.baseStats = new List<int> {1, 12, 8, 8, 6, 6, 15, 12, 3, 1};
        this.expDict = new Dictionary<int, List<int>> {
            {1, new List<int>(){1000, 1, 1, 1, 0, 0, 1, 0, 1}},
            {2, new List<int>(){2000, 0, 1, 0, 1, 1, 0, 1, 0}}
        };
    }
}
