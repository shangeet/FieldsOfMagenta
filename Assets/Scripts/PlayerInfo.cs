using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    //Key identifiers
    private string id;
    private string textureRefPath;
    private bool isEnemy;
    private BattleClass battleClass;

    //Base Stats
    public int level {get; set;}
    public int baseHealth {get; set;}
    public int baseAttack {get; set;}
    public int baseDefense {get; set;}
    public int baseMagicAttack {get; set;}
    public int baseMagicDefense {get; set;}
    public int dexterity {get; set;}
    public int luck {get; set;}
    public int mov {get; set;}
    public int totalExperience {get; set;}
    //In-game statuses
    public int currentHealth {get; set;}
    //private List<Status> statusList {get; set;}
    public string portraitRefPath {get; set;}

    public PlayerInfo(string id, string textureRefPath, bool isEnemy, BattleClass battleClass) {
       this.id = id;
       this.textureRefPath = textureRefPath;
       this.isEnemy = isEnemy;
       this.battleClass = battleClass;
    }

    public void setupBaseStats() {
       List<int> baseStats = BaseClassConstants.getBaseStatsForBattleClass(this.battleClass);
       this.level = 1;
       this.baseHealth = baseStats[0];
       this.baseAttack = baseStats[1];
       this.baseDefense = baseStats[2];
       this.baseMagicAttack = baseStats[3];
       this.baseMagicDefense = baseStats[4];
       this.dexterity = baseStats[5];
       this.luck = baseStats[6];
       this.mov = baseStats[7];
       this.totalExperience = 0;
    }

    public void setupBattleStats() {
        this.currentHealth = baseHealth;
        //this.statusList = new List<Status>();
    }

    public string getPlayerId() {
        return this.id;
    }

    public string getTextureRefPath() {
        return this.textureRefPath;
    }

    public bool getIsEnemy() {
        return this.isEnemy;
    }

    public BattleClass getBattleClass() {
        return this.battleClass;
    }
   
}
