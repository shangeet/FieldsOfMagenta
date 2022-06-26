using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemsDatabase {

    private static string consumableItemsPath = "Items/Stock/ConsumableItems";
    private static string equipmentItemsPath = "Items/Stock/EquipmentItems";

    public static List<ConsumableItem> GetAllConsumableItems() {
        return FileUtils.GetAssetsAtPath<ConsumableItem>(consumableItemsPath);
    }

    public static List<EquipmentItem> GetAllEquipmentItems() {
        return FileUtils.GetAssetsAtPath<EquipmentItem>(equipmentItemsPath);
    }

    public static List<ConsumableItem> GetAllUnlockedConsumableItems(List<int> completedQuests) {
        List<ConsumableItem> consumableItems = new List<ConsumableItem>();
        foreach (ConsumableItem item in GetAllConsumableItems()) {
            if (item.meetsPrerequisites(completedQuests)) {
                consumableItems.Add(item);
            }
        }
        return consumableItems;
    }

    public static List<EquipmentItem> GetAllUnlockedEquipmentItems(List<int> completedQuests) {
        List<EquipmentItem> equipmentItems = new List<EquipmentItem>();
        foreach (EquipmentItem item in GetAllEquipmentItems()) {
            if (item.meetsPrerequisites(completedQuests)) {
                equipmentItems.Add(item);
            }
        }
        return equipmentItems;
    }

    public static ConsumableItem GetConsumableItemByName(string name) {
        foreach (ConsumableItem item in GetAllConsumableItems()) {
            if (item.itemName.Equals(name)) {
                return item;
            }
        }
        return null;
    }

    public static EquipmentItem GetEquipmentItemByName(string name) {
        foreach (EquipmentItem item in GetAllEquipmentItems()) {
            if (item.itemName.Equals(name)) {
                return item;
            }
        }
        return null;
    }

}
