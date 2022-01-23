using System.Collections;
using System.Collections.Generic;

public class SerializablePlayerInfo {

    //Key identifiers
    public string id;
    public string name;
    public string baseSpritePath;
    public string gameControllerPath;
    public string battleControllerPath;
    public bool isEnemy;
    public BattleClass battleClass;

    //Base Stats
    public int level;
    public int baseHealth;
    public int baseAttack;
    public int baseDefense;
    public int baseMagicAttack;
    public int baseMagicDefense;
    public int baseDexterity;
    public int baseLuck;
    public int baseMov;

    //In-battle stats
    public int currentHealth;
    public int currentAttack;
    public int currentDefense;
    public int currentMagicAttack;
    public int currentMagicDefense;
    public int currentDexterity;
    public int currentLuck;
    public int currentMov;

    //Items
    public SerializableItemManager itemManager;

    //Misc
    public int totalExperience;
    public string portraitRefPath;
    
    public SerializablePlayerInfo(string id, string name, string baseSpritePath, string gameControllerPath, string battleControllerPath,
        bool isEnemy, BattleClass battleClass, int level, int baseHealth, int baseAttack, int baseDefense, int baseMagicAttack, int baseMagicDefense,
        int baseDexterity, int baseLuck, int baseMov, int currentHealth, int currentAttack, int currentDefense, int currentMagicAttack, int currentMagicDefense,
        int currentDexterity, int currentLuck, int currentMov, SerializableItemManager itemManager, int totalExperience, string portraitRefPath) {

        this.id = id;
        this.name = name;
        this.baseSpritePath = baseSpritePath;
        this.gameControllerPath = gameControllerPath;
        this.battleControllerPath = battleControllerPath;
        this.isEnemy = isEnemy;
        this.battleClass = battleClass;
        this.level = level;
        this.baseHealth = baseHealth;
        this.baseAttack = baseAttack;
        this.baseDefense = baseDefense;
        this.baseMagicAttack = baseMagicAttack;
        this.baseMagicDefense = baseMagicDefense;
        this.baseDexterity = baseDexterity;
        this.baseLuck = baseLuck;
        this.baseMov = baseMov;
        this.currentHealth = currentHealth;
        this.currentAttack = currentAttack;
        this.currentDefense = currentDefense;
        this.currentMagicAttack = currentMagicAttack;
        this.currentMagicDefense = currentMagicDefense;
        this.currentDexterity = currentDexterity;
        this.currentLuck = currentLuck;
        this.currentMov = currentMov;
        this.itemManager = itemManager;
        this.totalExperience = totalExperience;
        this.portraitRefPath = portraitRefPath;
    }

    public static PlayerInfo ConvertToPlayerInfo(SerializablePlayerInfo sPI) {
        PlayerInfo newPI = new PlayerInfo();
        newPI.id = sPI.id;
        newPI.name = sPI.name;
        newPI.baseSpritePath = sPI.baseSpritePath;
        newPI.gameControllerPath = sPI.gameControllerPath;
        newPI.battleControllerPath = sPI.battleControllerPath;
        newPI.isEnemy = sPI.isEnemy;
        newPI.battleClass = sPI.battleClass;
        newPI.level = sPI.level;
        newPI.baseHealth = sPI.baseHealth;
        newPI.baseAttack = sPI.baseAttack;
        newPI.baseDefense = sPI.baseDefense;
        newPI.baseMagicAttack = sPI.baseMagicAttack;
        newPI.baseMagicDefense = sPI.baseMagicDefense;
        newPI.baseDexterity = sPI.baseDexterity;
        newPI.baseLuck = sPI.baseLuck;
        newPI.baseMov = sPI.baseMov;
        newPI.currentHealth = sPI.currentHealth;
        newPI.currentAttack = sPI.currentAttack;
        newPI.currentDefense = sPI.currentDefense;
        newPI.currentMagicAttack = sPI.currentMagicAttack;
        newPI.currentMagicDefense = sPI.currentMagicDefense;
        newPI.currentDexterity = sPI.currentDexterity;
        newPI.currentLuck = sPI.currentLuck;
        newPI.currentMov = sPI.currentMov;
        newPI.totalExperience = sPI.totalExperience;
        newPI.portraitRefPath = sPI.portraitRefPath;
        newPI.consumableItemManager = SerializableItemManager.ConvertToConsumableItemManager(sPI.itemManager);
        newPI.equipmentItemManager = SerializableItemManager.ConvertToEquipmentItemManager(sPI.itemManager);
        return newPI;
    }

    public static SerializablePlayerInfo ConvertFromPlayerInfo(PlayerInfo pI) {
        SerializableItemManager newItemManager = SerializableItemManager.ConvertFromItemManager(pI.consumableItemManager, pI.equipmentItemManager);
        SerializablePlayerInfo sPI = new SerializablePlayerInfo(
            pI.id, pI.name, pI.baseSpritePath, pI.gameControllerPath, pI.battleControllerPath,
        pI.isEnemy, pI.battleClass, pI.level, pI.baseHealth, pI.baseAttack, pI.baseDefense, pI.baseMagicAttack, pI.baseMagicDefense,
        pI.baseDexterity, pI.baseLuck, pI.baseMov, pI.currentHealth, pI.currentAttack, pI.currentDefense, pI.currentMagicAttack, pI.currentMagicDefense,
        pI.currentDexterity, pI.currentLuck, pI.currentMov, newItemManager, pI.totalExperience, pI.portraitRefPath);
        return sPI;
    }

}