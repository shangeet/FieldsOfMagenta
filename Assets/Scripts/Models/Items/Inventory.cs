using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemAction {ADDTOPLAYERINVENTORY, EQUIPTOPLAYER, DROP};
[System.Serializable]
public class Inventory {

    public List<ConsumableItem> consumableItems;
    public List<EquipmentItem> equipmentItems;

    public Inventory() {
        consumableItems = new List<ConsumableItem>();
        equipmentItems = new List<EquipmentItem>();
    }

    public void AddItem(ConsumableItem item) {
        consumableItems.Add(item);
    }

    public void AddItem(EquipmentItem item) {
        equipmentItems.Add(item);
    }
 
    public void RemoveItem(ConsumableItem item) {
        consumableItems.Remove(item);
    }

    public void RemoveItem(EquipmentItem item) {
        equipmentItems.Remove(item);
    }

    public List<Item> GetAllItems() {
        List<Item> allItems = new List<Item>();

        foreach (Item item in consumableItems) {
            allItems.Add(item);
        }

        foreach (Item item in equipmentItems) {
            allItems.Add(item);
        }

        return allItems;  
    }

    public List<ConsumableItem> GetAllConsumableItems() {
        return consumableItems;
    }

    public List<EquipmentItem> GetAllEquipmentItems() {
        return equipmentItems;
    }
}
