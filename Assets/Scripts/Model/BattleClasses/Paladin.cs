using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : BattleClass
{
    public Paladin() {
        this.baseStats = new List<int> {1, 15, 10, 6, 10, 8, 10, 10, 1, 1};
        this.expDict = new Dictionary<int, List<int>> {
            {1, new List<int>(){1000, 1, 1, 1, 0, 0, 1, 0, 1}},
            {2, new List<int>(){2000, 0, 1, 0, 1, 1, 0, 1, 0}}
        };
    }
}
