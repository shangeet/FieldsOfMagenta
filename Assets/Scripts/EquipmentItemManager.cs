using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemManager {
    
    public int numSlots = System.Enum.GetNames(typeof(EquipType)).Length;
    public EquipmentItem[] currentEquipment;

    public EquipmentItemManager() {
        currentEquipment = new EquipmentItem[numSlots];
    }

    public void PopulateWithCurrentEquipement(EquipmentItem[] savedEquipment) {
        currentEquipment = savedEquipment;
    }

    public EquipmentItem Equip(EquipmentItem newItem) {

        int equipSlot = (int) newItem.equipType;
    
        EquipmentItem oldItem = null;
    
        if(currentEquipment[equipSlot] != null) {
            oldItem = currentEquipment[equipSlot];
        }
    
        currentEquipment[equipSlot] = newItem;

        return oldItem;  
       
    }

    public void UnEquip(EquipmentItem equippedItem) {
        
        int equipSlot = (int) equippedItem.equipType;

        currentEquipment[equipSlot] = null;
    }

}
