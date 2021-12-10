using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo {
    
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
    public PlayerAnimator animator {get; set;}

    public PlayerInfo(string id, string textureRefPath, bool isEnemy, BattleClass battleClass) {
       this.id = id;
       this.textureRefPath = textureRefPath;
       this.isEnemy = isEnemy;
       this.battleClass = battleClass;
       this.animator = null;
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

    public void setupBattleStats() {
        this.currentHealth = baseHealth;
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

    public string getTextureRefPath() {
        return this.textureRefPath;
    }

    public bool getIsEnemy() {
        return this.isEnemy;
    }

    public BattleClass getBattleClass() {
        return this.battleClass;
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
}
