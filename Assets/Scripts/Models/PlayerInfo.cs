using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo {
    
    //Key identifiers
    private string id;
    private string baseSpritePath;
    private string gameControllerPath;
    private string battleControllerPath;
    private bool isEnemy;
    private BattleClass battleClass;

    //Base Stats
    public int level {get; set;}
    public int baseHealth {get; set;}
    public int baseAttack {get; set;}
    public int baseDefense {get; set;}
    public int baseMagicAttack {get; set;}
    public int baseMagicDefense {get; set;}
    public int baseDexterity {get; set;}
    public int baseLuck {get; set;}
    public int baseMov {get; set;}

    //In-battle stats
    public int currentHealth {get; set;}
    public int currentAttack {get; set;}
    public int currentDefense {get; set;}
    public int currentMagicAttack {get; set;}
    public int currentMagicDefense {get; set;}
    public int currentDexterity {get; set;}
    public int currentLuck {get; set;}
    public int currentMov {get; set;}

    //Items
    public EquipmentItemManager equipmentItemManager {get; set;}
    public ConsumableItemManager consumableItemManager {get; set;}

    //Misc
    public int totalExperience {get; set;}
    //private List<Status> statusList {get; set;}
    public string portraitRefPath {get; set;}
    public PlayerAnimator playerAnimator {get; set;}

    public PlayerInfo(string id, bool isEnemy, BattleClass battleClass) {
       this.id = id;
       this.isEnemy = isEnemy;
       this.battleClass = battleClass;
       this.playerAnimator = null;
    }

    public PlayerInfo() {}

    public void setAnimationPaths(string baseSpritePath, string gameAnimatorPath, string battleAnimatorPath) {
        this.baseSpritePath = baseSpritePath;
        this.gameControllerPath = gameAnimatorPath;
        this.battleControllerPath = battleAnimatorPath;
    }

    public void setupBaseStats(List<int> baseStats) {
       this.level = baseStats[0];
       this.baseHealth = baseStats[1];
       this.baseAttack = baseStats[2];
       this.baseDefense = baseStats[3];
       this.baseMagicAttack = baseStats[4];
       this.baseMagicDefense = baseStats[5];
       this.baseDexterity = baseStats[6];
       this.baseLuck = baseStats[7];
       this.baseMov = baseStats[8];
       this.totalExperience = baseStats[9];        
    }

    public void setupBattleStats(bool setHealth) {
        if (setHealth) {
            this.currentHealth = baseHealth;            
        }
        this.currentAttack = baseAttack;
        this.currentDefense = baseDefense;
        this.currentMagicAttack = baseMagicAttack;
        this.currentMagicDefense = currentMagicDefense;
        this.currentDexterity = currentDexterity;
        this.currentLuck = baseLuck;
        this.currentMov = baseMov;
        //this.statusList = new List<Status>();
    }

    public string getPlayerId() {
        return this.id;
    }

    public void setPlayerId(string idToSet) {
        this.id = idToSet;
    }

    public string getBaseSpritePath() {
        return this.baseSpritePath;
    }

    public void setBaseSpritePath(string basePath) {
        this.baseSpritePath = basePath;
    }

    public bool getIsEnemy() {
        return this.isEnemy;
    }

    public void setIsEnemy(bool isE) {
        this.isEnemy = isE;
    }

    public BattleClass getBattleClass() {
        return this.battleClass;
    }

    public void setBattleClass(BattleClass newBattleClass) {
        this.battleClass = newBattleClass;
    }

    public string getGameControllerPath() {
        return this.gameControllerPath;
    }

    public void setGameControllerPath(string newPath) {
        this.gameControllerPath = newPath;
    }

    public string getBattleControllerPath() {
        return this.battleControllerPath;
    }

    public string setBattleControllerPath(string newPath) {
        return this.battleControllerPath = newPath;
    }

    public void UpdateCharacterBaseStats(ModdableItem item) {
        baseHealth += item.healthMod;
        baseAttack += item.atkMod;
        baseDefense += item.defMod;
        baseMagicAttack += item.mAtkMod;
        baseMagicDefense += item.mDefMod;
        baseDexterity += item.dexMod;
        baseLuck += item.luckMod;
        baseMov += item.movMod;        
    }

    public void UpdateBattleStatsInBattle() {
        setupBattleStats(false);
    }

    public void EquipItem(EquipmentItem itemToEquip) {
        EquipmentItem previousItem = equipmentItemManager.Equip(itemToEquip);
        UpdateCharacterBaseStats(itemToEquip);
    }

    public void UnEquipItem(EquipmentItem equippedItem) {
        equipmentItemManager.UnEquip(equippedItem);
        RevertCharacterStatusItemExpired(equippedItem);
    }

    public ConsumableItem GetConsumableItemWithKey(string itemKey) {
        return consumableItemManager.consumableItems.ContainsKey(itemKey) ? consumableItemManager.consumableItems[itemKey] : null;
    }

    public void ConsumeItem(string itemKey) {
        ConsumableItem itemToConsume = GetConsumableItemWithKey(itemKey);
        consumableItemManager.ConsumeItem(itemToConsume);
        if (itemToConsume.consumptionType == ConsumptionType.HEAL || itemToConsume.consumptionType == ConsumptionType.TEMPBOOST) {
            UpdateCharacterItemStatus(itemToConsume, null, true); 
        } else if (itemToConsume.consumptionType == ConsumptionType.PERMABOOST) {
            UpdateCharacterBaseStats(itemToConsume);
        }
    }

    public void AddConsumableItemToPlayerInventory(ConsumableItem newItem) {
        consumableItemManager.AddItem(newItem);
    }

    public void LoadItems(List<ConsumableItem> consumableItems, EquipmentItem[] currentEquippedItems) {
        consumableItemManager.PopulateWithCurrentitems(consumableItems);
        equipmentItemManager.PopulateWithCurrentEquipement(currentEquippedItems);
        //Re-calculate the base stats
        foreach(EquipmentItem item in currentEquippedItems) {
            if (item != null) {
                EquipItem(item);  
            }
        }
    }

    public void UpdateCharacterItemStatus(ModdableItem newItem, ModdableItem oldItem, bool capHealth) {
        if (oldItem != null) {
            subtractPlayerInfoStats(oldItem);
        }
        addPlayerInfoStats(newItem, capHealth); 
    }

    public void RevertCharacterStatusItemExpired(ModdableItem expiredItem) {
        if (expiredItem != null) {
            subtractPlayerInfoStats(expiredItem);            
        }
    }

    private void subtractPlayerInfoStats(ModdableItem item) {
        currentHealth -= item.healthMod;
        currentHealth = Mathf.Max(0, currentHealth);
        currentAttack -= item.atkMod;
        currentDefense -= item.defMod;
        currentMagicAttack -= item.mAtkMod;
        currentMagicDefense -= item.mDefMod;
        currentDexterity -= item.dexMod;
        currentLuck -= item.luckMod; 
        currentMov -= item.movMod;
    }

    private void addPlayerInfoStats(ModdableItem item, bool capHealth) {
        currentHealth += item.healthMod;
        currentHealth = capHealth ? Mathf.Min(currentHealth, baseHealth) : currentHealth;
        currentAttack += item.atkMod;
        currentDefense += item.defMod;
        currentMagicAttack += item.mAtkMod;
        currentMagicDefense += item.mDefMod;
        currentDexterity += item.dexMod;
        currentLuck += item.luckMod;
        currentMov += item.movMod;
    }

    public static void SwitchConsumableItems(PlayerInfo current, PlayerInfo target, string currentItemName, string targetItemName) {
        int currentItemQty = current.consumableItemManager.GetQuantityOfItemInInventory(currentItemName);
        int targetItemQty = target.consumableItemManager.GetQuantityOfItemInInventory(targetItemName);
        ConsumableItem currentItemToSwap = current.consumableItemManager.GetConsumableItem(currentItemName);
        ConsumableItem targetItemToSwap = target.consumableItemManager.GetConsumableItem(targetItemName);

        current.consumableItemManager.RemoveItem(currentItemName);
        target.consumableItemManager.RemoveItem(targetItemName);
        current.consumableItemManager.AddItemWithQuantity(targetItemToSwap, targetItemQty);
        target.consumableItemManager.AddItemWithQuantity(currentItemToSwap, currentItemQty);
    }

    public static PlayerInfo Clone(PlayerInfo playerInfoToClone) {
        PlayerInfo deepCopy = new PlayerInfo();
        deepCopy.setPlayerId(playerInfoToClone.getPlayerId());
        deepCopy.setBaseSpritePath(playerInfoToClone.getBaseSpritePath());
        deepCopy.setGameControllerPath(playerInfoToClone.getGameControllerPath());
        deepCopy.setBattleControllerPath(playerInfoToClone.getBattleControllerPath());
        deepCopy.setIsEnemy(playerInfoToClone.getIsEnemy());
        deepCopy.setBattleClass(playerInfoToClone.getBattleClass());
        deepCopy.level = playerInfoToClone.level;
        deepCopy.baseHealth = playerInfoToClone.baseHealth;
        deepCopy.baseAttack = playerInfoToClone.baseAttack;
        deepCopy.baseDefense = playerInfoToClone.baseDefense;
        deepCopy.baseMagicAttack = playerInfoToClone.baseMagicAttack;
        deepCopy.baseMagicDefense = playerInfoToClone.baseMagicDefense;
        deepCopy.baseDexterity = playerInfoToClone.baseDexterity;
        deepCopy.baseLuck = playerInfoToClone.baseLuck;
        deepCopy.baseMov = playerInfoToClone.baseMov;
        deepCopy.currentHealth = playerInfoToClone.currentHealth;
        deepCopy.currentAttack = playerInfoToClone.currentAttack;
        deepCopy.currentDefense = playerInfoToClone.currentDefense;
        deepCopy.currentMagicAttack = playerInfoToClone.currentMagicAttack;
        deepCopy.currentMagicDefense = playerInfoToClone.currentMagicDefense;
        deepCopy.currentDexterity = playerInfoToClone.currentDexterity;
        deepCopy.currentLuck = playerInfoToClone.currentLuck;
        deepCopy.currentMov = playerInfoToClone.currentMov;
        deepCopy.equipmentItemManager = playerInfoToClone.equipmentItemManager;
        deepCopy.consumableItemManager = playerInfoToClone.consumableItemManager;
        deepCopy.totalExperience = playerInfoToClone.totalExperience;
        deepCopy.portraitRefPath = playerInfoToClone.portraitRefPath;
        deepCopy.playerAnimator = playerInfoToClone.playerAnimator;
        return deepCopy;
    }

    public void gainExp(int gainExp) {   
        List<int> warriorStatBoostInfo = BaseClassConstants.getExperienceDictByClassAndLevel(this.battleClass, this.level);
        int expToLvlUp = warriorStatBoostInfo[0];
        int remExp = totalExperience + gainExp;
        while (remExp > expToLvlUp) {
            levelUp(warriorStatBoostInfo, expToLvlUp);
            remExp -= expToLvlUp;
            //get stat boosts + exp needed for the next level
            warriorStatBoostInfo = BaseClassConstants.getExperienceDictByClassAndLevel(this.battleClass, this.level);
            expToLvlUp = warriorStatBoostInfo[0];
        }
        totalExperience = remExp;
    }

    public List<int> getTotalExpListLevels(int startingLevel, int finalLevel) {
        List<int> expValues = new List<int>();
        for (int lvl = startingLevel; lvl <= finalLevel; lvl++) {
            expValues.Add(BaseClassConstants.getExperienceDictByClassAndLevel(this.battleClass, lvl)[0]);
        }
        return expValues;
    }

    public int getTotalExpNeededToLevelUp() {
        return BaseClassConstants.getExperienceDictByClassAndLevel(this.battleClass, this.level)[0];
    }

    public void levelUp(List<int> warriorStatBoostInfo, int expToLvlUp) {
        int remExp = totalExperience - expToLvlUp;
        List<int> newStats = new List<int> {
            level + 1,
            baseHealth + warriorStatBoostInfo[1],
            baseAttack + warriorStatBoostInfo[2],
            baseDefense + warriorStatBoostInfo[3],
            baseMagicAttack + warriorStatBoostInfo[4],
            baseMagicDefense + warriorStatBoostInfo[5],
            baseDexterity + warriorStatBoostInfo[6],
            baseLuck + warriorStatBoostInfo[7],
            baseMov + warriorStatBoostInfo[8],
            remExp
        };
        setupBaseStats(newStats);
        UpdateBattleStatsInBattle();
    }
}
