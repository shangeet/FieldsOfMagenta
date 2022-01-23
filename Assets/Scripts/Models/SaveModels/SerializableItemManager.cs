using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum ItemManagerType {CONSUMABLE_MANAGER, EQUIPMENT_MANAGER}
[System.Serializable]
public class SerializableItemManager {

    public SerializableItem[] currentEquipment;
    public Dictionary<string,int> currentConsumableItemsInventory;
    public Dictionary<string, SerializableItem> consumableItems;
    public ItemManagerType itemManagerType;

    public SerializableItemManager() {}
    
    public SerializableItemManager(SerializableItem[] currentEquipment, Dictionary<string,int> currentConsumableItemsInventory,
     Dictionary<string, SerializableItem> consumableItems, ItemManagerType itemManagerType) {

         this.currentEquipment = currentEquipment;
         this.currentConsumableItemsInventory = currentConsumableItemsInventory;
         this.consumableItems = consumableItems;
         this.itemManagerType = itemManagerType; 
    }

    public SerializableItemManager(EquipmentItem[] equipmentItemArr) {
        this.currentEquipment = new SerializableItem[equipmentItemArr.Length];
        for(int i = 0; i < equipmentItemArr.Length; i++) {
            if (equipmentItemArr[i] != null) {
                this.currentEquipment[i] = SerializableItem.ConvertFromEquipmentItem(equipmentItemArr[i]);
            }
        }
        this.itemManagerType = ItemManagerType.EQUIPMENT_MANAGER;
    }

    public SerializableItemManager(Dictionary<string, int> currentConsumableItemsInventory, Dictionary<string, ConsumableItem> consumableItemDict) {
        this.currentConsumableItemsInventory = currentConsumableItemsInventory;
        this.consumableItems = new Dictionary<string, SerializableItem>();
        foreach(KeyValuePair<string, ConsumableItem> entry in consumableItemDict) {
            this.consumableItems[entry.Key] = SerializableItem.ConvertFromConsumableItem(entry.Value);
        }
        this.itemManagerType = ItemManagerType.CONSUMABLE_MANAGER;
    }


    public SerializableItemManager(EquipmentItem[] equipmentItemArr, Dictionary<string, int> currentConsumableItemsInventory, Dictionary<string, ConsumableItem> consumableItemDict) {
        this.currentEquipment = new SerializableItem[equipmentItemArr.Length];
        for(int i = 0; i < equipmentItemArr.Length; i++) {
            if (equipmentItemArr[i] != null) {
                this.currentEquipment[i] = SerializableItem.ConvertFromEquipmentItem(equipmentItemArr[i]);
            }
        }
        this.currentConsumableItemsInventory = currentConsumableItemsInventory;
        this.consumableItems = new Dictionary<string, SerializableItem>();
        foreach(KeyValuePair<string, ConsumableItem> entry in consumableItemDict) {
            this.consumableItems[entry.Key] = SerializableItem.ConvertFromConsumableItem(entry.Value);
        }        
    }

    public static SerializableItemManager ConvertFromItemManager(ConsumableItemManager cItemManager, EquipmentItemManager eItemManager) {
        return new SerializableItemManager(eItemManager.currentEquipment, cItemManager.currentConsumableItemsInventory, cItemManager.consumableItems);
    }

    public static SerializableItemManager ConvertFromConsumableItemManager(ConsumableItemManager cItemManager) {
        return new SerializableItemManager(cItemManager.currentConsumableItemsInventory, cItemManager.consumableItems);
    }

    public static SerializableItemManager ConvertFromEquipmentItemManager(EquipmentItemManager eItemManager) {
        return new SerializableItemManager(eItemManager.currentEquipment);
    }

    public static EquipmentItemManager ConvertToEquipmentItemManager(SerializableItemManager sItemManager) {
        EquipmentItemManager eItemManager = new EquipmentItemManager();
        for(int i = 0; i < sItemManager.currentEquipment.Length; i++) {
            if (sItemManager.currentEquipment[i] != null) {
                eItemManager.currentEquipment[i] = SerializableItem.ConvertToEquipmentItem(sItemManager.currentEquipment[i]);
            }
        }
        return eItemManager;        
    }

    public static ConsumableItemManager ConvertToConsumableItemManager(SerializableItemManager sItemManager) {
        ConsumableItemManager cItemManager = new ConsumableItemManager();
        cItemManager.currentConsumableItemsInventory = sItemManager.currentConsumableItemsInventory;
        foreach(KeyValuePair<string, SerializableItem> entry in sItemManager.consumableItems) {
            cItemManager.consumableItems[entry.Key] = SerializableItem.ConvertToConsumableItem(entry.Value);
        }
        return cItemManager;        
    }

}