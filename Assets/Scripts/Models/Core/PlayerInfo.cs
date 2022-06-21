using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Player", order = 1)]
[System.Serializable]
public class PlayerInfo : ScriptableObject {
    
    //Key identifiers
    public string id;
    public string playerName;
    public string baseSpritePath;
    public string gameControllerPath;
    public string battleControllerPath;
    public bool isEnemy;
    public BattleClass battleClass;
    public string battleClassName;

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
    public EquipmentItemManager equipmentItemManager;
    public ConsumableItemManager consumableItemManager;

    //Misc
    public int totalExperience;
    public List<StatusEffect> statusList;
    public string portraitRefPath;
    public PlayerAnimator playerAnimator;

    public PlayerInfo(string id, string name, bool isEnemy, BattleClass battleClass, string portraitRefPath) {
        this.id = id;
        this.playerName = name;
        this.isEnemy = isEnemy;
        this.battleClass = battleClass;
        this.battleClassName = battleClass.GetType().Name;
        this.playerAnimator = null;
        this.totalExperience = 0;
        this.equipmentItemManager = new EquipmentItemManager();
        this.consumableItemManager = new ConsumableItemManager();
        this.portraitRefPath = portraitRefPath;
        this.statusList = new List<StatusEffect>();
        List<int> baseStats = battleClass.getBaseStats();
        setupBaseStats(baseStats);
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
    }

    public void setupBattleStats(bool setHealth) {
        if (setHealth) {
            this.currentHealth = baseHealth;            
        }
        this.currentAttack = baseAttack;
        this.currentDefense = baseDefense;
        this.currentMagicAttack = baseMagicAttack;
        this.currentMagicDefense = baseMagicDefense;
        this.currentDexterity = baseDexterity;
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

    public void AddCharacterBaseStats(ModdableItem item) {
        baseHealth += item.healthMod;
        baseAttack += item.atkMod;
        baseDefense += item.defMod;
        baseMagicAttack += item.mAtkMod;
        baseMagicDefense += item.mDefMod;
        baseDexterity += item.dexMod;
        baseLuck += item.luckMod;
        baseMov += item.movMod;        
    }

    public void SubtractCharacterBaseStats(ModdableItem item) {
        baseHealth -= item.healthMod;
        baseAttack -= item.atkMod;
        baseDefense -= item.defMod;
        baseMagicAttack -= item.mAtkMod;
        baseMagicDefense -= item.mDefMod;
        baseDexterity -= item.dexMod;
        baseLuck -= item.luckMod;
        baseMov -= item.movMod; 
    }

    public void UpdateBattleStatsInBattle() {
        setupBattleStats(false);
    }

    public void EquipItem(EquipmentItem itemToEquip) {
        equipmentItemManager.Equip(itemToEquip);
        AddCharacterBaseStats(itemToEquip);
        setupBattleStats(true);
    }

    public void UnEquipItem(EquipmentItem equippedItem) {
        equipmentItemManager.UnEquip(equippedItem);
        RevertCharacterStatusItemExpired(equippedItem);
        setupBattleStats(true);
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
            AddCharacterBaseStats(itemToConsume);
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

    public ConsumableItemManager GetConsumableItemManager() {
        return consumableItemManager;
    }

    public void SetConsumableItemManager(ConsumableItemManager updatedManager) {
        consumableItemManager = updatedManager;
    }

    public EquipmentItemManager GetEquipmentItemManager() {
        return equipmentItemManager;
    }

    public void UpdateCharacterItemStatus(ModdableItem newItem, ModdableItem oldItem, bool capHealth) {
        if (oldItem != null) {
            subtractPlayerInfoStats(oldItem);
        }
        addPlayerInfoStats(newItem, capHealth); 
    }

    public void RevertCharacterStatusItemExpired(ModdableItem expiredItem) {
        if (expiredItem != null) {
            SubtractCharacterBaseStats(expiredItem);            
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
        deepCopy.battleClassName = playerInfoToClone.battleClassName;
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
        deepCopy.statusList = new List<StatusEffect>();
        foreach (StatusEffect effect in playerInfoToClone.statusList) {
            StatusEffect effectClone = new StatusEffect(effect.effectType, effect.maxTurns, effect.remainingActiveTurns);
            deepCopy.statusList.Add(effectClone);   
        }
        return deepCopy;
    }

    public void gainExp(int gainExp) {   
        List<int> warriorStatBoostInfo = battleClass.getExperienceDictByClassAndLevel(this.level);
        int expToLvlUp = warriorStatBoostInfo[0];
        int remExp = totalExperience + gainExp;
        while (remExp > expToLvlUp) {
            levelUp(warriorStatBoostInfo, expToLvlUp);
            remExp -= expToLvlUp;
            //get stat boosts + exp needed for the next level
            warriorStatBoostInfo = battleClass.getExperienceDictByClassAndLevel(this.level);
            expToLvlUp = warriorStatBoostInfo[0];
        }
        totalExperience = remExp;
    }

    public int GetExpNeededToLevelUp() {
        return battleClass.getExperienceDictByClassAndLevel(this.level)[0];
    }

    public List<int> getTotalExpListLevels(int startingLevel, int finalLevel) {
        List<int> expValues = new List<int>();
        for (int lvl = startingLevel; lvl <= finalLevel; lvl++) {
            expValues.Add(battleClass.getExperienceDictByClassAndLevel(lvl)[0]);
        }
        return expValues;
    }

    public int getTotalExpNeededToLevelUp() {
        return battleClass.getExperienceDictByClassAndLevel(this.level)[0];
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

    public bool IsHealer() {
        return battleClassName.Equals("Healer");
    }

    public bool IsBard() {
        return battleClassName.Equals("Bard");
    }

    public void AddStatusEffect(StatusEffect effect) {

        bool statusExists = false;

        for (int i = 0; i < statusList.Count; i++) {

            StatusEffect currEffect = statusList[i];

            if (currEffect is BuffStatusEffect && effect is BuffStatusEffect) {

                BuffStatusEffect buffStatusEffectCurr = currEffect as BuffStatusEffect;
                BuffStatusEffect buffStatusEffect = effect as BuffStatusEffect;

                if (buffStatusEffectCurr.GetBuffAttr() == buffStatusEffect.GetBuffAttr()) {
                    buffStatusEffectCurr.remainingActiveTurns = buffStatusEffectCurr.maxTurns;
                    statusList[i] = currEffect;
                    statusExists = true;
                }

            } else if (currEffect.effectType == effect.effectType) {
                currEffect.remainingActiveTurns = currEffect.maxTurns; 
                statusList[i] = currEffect;
                statusExists = true;
            }
        }

        if (!statusExists) {
            statusList.Add(effect);
            if (effect is BuffStatusEffect) {
                BuffStatusEffect buffEffect = effect as BuffStatusEffect;
                AddBuff(buffEffect);
            }
        }

    }

    public override bool Equals(System.Object obj) {
        if ((obj == null) || ! this.GetType().Equals(obj.GetType())) {
            return false;
        } else {
            PlayerInfo p = obj as PlayerInfo;
            return p.id.Equals(this.id);
        }
    }

    public void AddBuff(BuffStatusEffect effect) {
        BuffAttr buffAttr = effect.GetBuffAttr();
        int buffAmt = effect.GetBuffAmt();
        switch (buffAttr) {
            case BuffAttr.ATK:
                currentAttack += buffAmt;
                break;
            case BuffAttr.DEF:
                currentDefense += buffAmt;
                break;
            case BuffAttr.MATK:
                currentMagicAttack += buffAmt;
                break;
            case BuffAttr.MDEF:
                currentMagicDefense += buffAmt;
                break;
            case BuffAttr.DEX:
                currentDexterity += buffAmt;
                break;
            case BuffAttr.LUK:
                currentLuck += buffAmt;
                break;
            case BuffAttr.MOV:
                currentMov += buffAmt;
                break;
            default:
                break;
        }
    }

    public void RemoveBuff(BuffStatusEffect effect) {
        BuffAttr buffAttr = effect.GetBuffAttr();
        int buffAmt = effect.GetBuffAmt();
        switch (buffAttr) {
            case BuffAttr.ATK:
                currentAttack -= buffAmt;
                break;
            case BuffAttr.DEF:
                currentDefense -= buffAmt;
                break;
            case BuffAttr.MATK:
                currentMagicAttack -= buffAmt;
                break;
            case BuffAttr.MDEF:
                currentMagicDefense -= buffAmt;
                break;
            case BuffAttr.DEX:
                currentDexterity -= buffAmt;
                break;
            case BuffAttr.LUK:
                currentLuck -= buffAmt;
                break;
            case BuffAttr.MOV:
                currentMov -= buffAmt;
                break;
            default:
                break;
        }
    }
}
