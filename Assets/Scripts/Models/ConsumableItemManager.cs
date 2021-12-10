using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemManager {

    public Dictionary<string,int> currentConsumableItemsInventory = new Dictionary<string, int>() {};
    public Dictionary<string, ConsumableItem> consumableItems = new Dictionary<string, ConsumableItem>();
    public static int numMaxSlots = 6;
    public static int numMaxItemsPerSlot = 3;

    public void PopulateWithCurrentitems(List<ConsumableItem> itemList) {
        foreach(ConsumableItem item in itemList) {
            consumableItems[item.itemName] = item;
            if (currentConsumableItemsInventory.ContainsKey(item.itemName)) {
                currentConsumableItemsInventory[item.itemName]++;
            } else {
                currentConsumableItemsInventory[item.itemName] = 1;
            }
        }
    }

    public void AddItem(ConsumableItem newItem) {
        if (newItem != null) {
            int currentUsedSlots = currentConsumableItemsInventory.Keys.Count;
            int currentItemsInSlot = currentConsumableItemsInventory.ContainsKey(newItem.itemName) ? currentConsumableItemsInventory[newItem.itemName] : 0;
        
            if ((currentConsumableItemsInventory.ContainsKey(newItem.itemName) && currentItemsInSlot < numMaxItemsPerSlot) ||
                (!currentConsumableItemsInventory.ContainsKey(newItem.itemName) && currentUsedSlots <= numMaxSlots)) {
                consumableItems[newItem.itemName] = newItem;
                currentConsumableItemsInventory[newItem.itemName] = currentConsumableItemsInventory.ContainsKey(newItem.itemName) ? currentConsumableItemsInventory[newItem.itemName]++ : 1;
            }              
        }
    }

    public void AddItemWithQuantity(ConsumableItem newItem, int qty) {
        if (newItem != null && qty != 0) {
            for (int i = 0; i < qty; i++) {
                AddItem(newItem);
            }
        }
    }

    public void ConsumeItem(ConsumableItem itemToConsume) {

        if (currentConsumableItemsInventory.ContainsKey(itemToConsume.itemName)) {
            int currentItemsInSlot = currentConsumableItemsInventory[itemToConsume.itemName];
            if (currentItemsInSlot == 1) {
                currentConsumableItemsInventory.Remove(itemToConsume.itemName);
                consumableItems.Remove(itemToConsume.itemName);
            } else {
                currentConsumableItemsInventory[itemToConsume.itemName]--;
            }
        }
    }

    public int GetQuantityOfItemInInventory(string itemName) {
        if (itemName == null) {return 0;}
        return currentConsumableItemsInventory.ContainsKey(itemName) ? currentConsumableItemsInventory[itemName] : 0;
    }

    public void RemoveItem(string itemKey) {
        if (itemKey != null) {
            currentConsumableItemsInventory.Remove(itemKey);
            consumableItems.Remove(itemKey);            
        }
    }

    public ConsumableItem GetConsumableItem(string itemName) {
        if (itemName == null) {return null;}
        return consumableItems.ContainsKey(itemName) ? consumableItems[itemName] : null;
    }
}
