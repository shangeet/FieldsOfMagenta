using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : BattleClass
{
    public Ranger() {
        this.baseStats = new List<int> {1, 8, 10, 8, 10, 8, 14, 10, 2, 2};
        this.expDict = new Dictionary<int, List<int>> {
            {1, new List<int>(){1000, 1, 1, 1, 0, 0, 1, 0, 1}},
            {2, new List<int>(){2000, 0, 1, 0, 1, 1, 0, 1, 0}}
        };
    }    
}
