using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumableItemManager {

    public Dictionary<string,int> currentConsumableItemsInventory;
    [SerializeReference] public Dictionary<string, ConsumableItem> consumableItems;
    public static int numMaxSlots = 6;
    public static int numMaxItemsPerSlot = 3;

    public ConsumableItemManager() {
        currentConsumableItemsInventory = new Dictionary<string, int>() {};
        consumableItems = new Dictionary<string, ConsumableItem>();
    }

    public void PopulateWithCurrentitems(List<ConsumableItem> itemList) {
        foreach(ConsumableItem item in itemList) {
            consumableItems[item.itemName] = item;
            if (currentConsumableItemsInventory.ContainsKey(item.itemName)) {
                currentConsumableItemsInventory[item.itemName]+=1;
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
                currentConsumableItemsInventory[newItem.itemName] = currentConsumableItemsInventory.ContainsKey(newItem.itemName) ? currentConsumableItemsInventory[newItem.itemName] + 1 : 1;
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
        Debug.Log(consumableItems);
        if (itemName == null) {return 0;}
        return currentConsumableItemsInventory.ContainsKey(itemName) ? currentConsumableItemsInventory[itemName] : 0;
    }

    public int GetQuantityOfItemInInventory(ConsumableItem item) {
        Debug.Log(consumableItems);
        string itemName = item.itemName;
        if (itemName == null) {return 0;}
        return currentConsumableItemsInventory.ContainsKey(itemName) ? currentConsumableItemsInventory[itemName] : 0;
    }

    public void RemoveItem(string itemKey) {
        if (itemKey != null) {
            currentConsumableItemsInventory.Remove(itemKey);
            consumableItems.Remove(itemKey);            
        }
    }

    public void RemoveItem(ConsumableItem item) {
        string itemKey = item.itemName;
        if (itemKey != null) {
            currentConsumableItemsInventory.Remove(itemKey);
            consumableItems.Remove(itemKey);               
        }
    }

    public ConsumableItem GetConsumableItem(string itemName) {
        if (itemName == null) {return null;}
        return consumableItems.ContainsKey(itemName) ? consumableItems[itemName] : null;
    }

    public Dictionary<string,int> GetCurrentConsumableItemsInventory() {
        return currentConsumableItemsInventory;
    }

    public int GetMaxItemsPerSlot() {
        return numMaxItemsPerSlot;
    }

    public int GetMaxSlots() {
        return numMaxSlots;
    }

    public bool CanAddItem(ConsumableItem item, int quantity) {
        Debug.Log("Key: " + item.itemName);
        Debug.Log("Exists? " + currentConsumableItemsInventory.ContainsKey(item.itemName).ToString());
        if (item != null) {
            int currentUsedSlots = currentConsumableItemsInventory.Keys.Count;
            int currentItemsInSlot = currentConsumableItemsInventory.ContainsKey(item.itemName) ? currentConsumableItemsInventory[item.itemName] : 0;    
            Debug.Log("Current Items in slot: " + currentItemsInSlot.ToString());
            Debug.Log("Current Used Slots: " + currentUsedSlots.ToString());
            if (!currentConsumableItemsInventory.ContainsKey(item.itemName) && currentUsedSlots < numMaxSlots && quantity <= numMaxItemsPerSlot) {
                return true;
            } else if (currentConsumableItemsInventory.ContainsKey(item.itemName) && (currentItemsInSlot + quantity) <= numMaxItemsPerSlot) {
                return true;
            } 
            return false;       
        }
        return false;
    }
}
