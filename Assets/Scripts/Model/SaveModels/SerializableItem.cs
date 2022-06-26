using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType {CONSUMABLE, EQUIPMENT}
[System.Serializable]
public class SerializableItem {
    
    public ItemType itemType;
    public string itemName;
    public string itemDescription;
    public string assetPath;
    public string spritePath;
    public List<int> unlockableQuestIds = new List<int>();
    public int goldValue;
    public EquipType equipType;
    public int baseDuration;
    public int currentDuration;
    public ConsumptionType consumptionType;

    public SerializableItem() {

    }
    
    public SerializableItem(string itemName, string itemDescription, string assetPath, string spritePath,
     List<int> unlockableQuestIds, int goldValue, int baseDuration, int currentDuration, ConsumptionType consumptionType,
      EquipType equipType, ItemType itemType) {
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.assetPath = assetPath;
        this.spritePath = spritePath;
        this.unlockableQuestIds = unlockableQuestIds;
        this.goldValue = goldValue;
        this.baseDuration = baseDuration;
        this.currentDuration = currentDuration;
        this.consumptionType = consumptionType;
        this.equipType = equipType;
        this.itemType = itemType;          
    }

    public SerializableItem(string itemName, string itemDescription, string assetPath, string spritePath, List<int> unlockableQuestIds, int goldValue, int baseDuration, int currentDuration, ConsumptionType consumptionType) {
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.assetPath = assetPath;
        this.spritePath = spritePath;
        this.unlockableQuestIds = unlockableQuestIds;
        this.goldValue = goldValue;
        this.baseDuration = baseDuration;
        this.currentDuration = currentDuration;
        this.consumptionType = consumptionType;
        this.itemType = ItemType.CONSUMABLE;
    }

    public SerializableItem(string itemName, string itemDescription, string assetPath, string spritePath, List<int> unlockableQuestIds, int goldValue, EquipType equipType) {
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.assetPath = assetPath;
        this.spritePath = spritePath;
        this.unlockableQuestIds = unlockableQuestIds;
        this.goldValue = goldValue;
        this.equipType = equipType;
        this.itemType = ItemType.EQUIPMENT;
    }

    public static SerializableItem ConvertFromConsumableItem(ConsumableItem item) {
        
        SerializableItem serItem = new SerializableItem(
            item.itemName,
            item.itemDescription,
            item.assetPath,
            item.spritePath,
            item.unlockableQuestIds,
            item.goldValue,
            item.baseDuration,
            item.currentDuration,
            item.consumptionType
        );
        return serItem;
    } 

    public static SerializableItem ConvertFromEquipmentItem(EquipmentItem item) {
        SerializableItem serItem = new SerializableItem(
            item.itemName,
            item.itemDescription,
            item.assetPath,
            item.spritePath,
            item.unlockableQuestIds,
            item.goldValue,
            item.equipType
        );
        return serItem;
    }

    public static ConsumableItem ConvertToConsumableItem(SerializableItem item) {
        ConsumableItem cItem = ScriptableObject.CreateInstance<ConsumableItem>();
        cItem.itemName = item.itemName;
        cItem.itemDescription = item.itemDescription;
        cItem.assetPath = item.assetPath;
        cItem.spritePath = item.spritePath;
        cItem.unlockableQuestIds = item.unlockableQuestIds;
        cItem.goldValue = item.goldValue;
        cItem.baseDuration = item.baseDuration;
        cItem.currentDuration = item.currentDuration;
        cItem.consumptionType = item.consumptionType;
        return cItem;
    }

    public static EquipmentItem ConvertToEquipmentItem(SerializableItem item) {
        EquipmentItem eItem = ScriptableObject.CreateInstance<EquipmentItem>();
        eItem.itemName = item.itemName;
        eItem.itemDescription = item.itemDescription;
        eItem.assetPath = item.assetPath;
        eItem.spritePath = item.spritePath;
        eItem.unlockableQuestIds = item.unlockableQuestIds;
        eItem.goldValue = item.goldValue;
        eItem.equipType = item.equipType;
        return eItem;
    }
}
