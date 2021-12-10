using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { HEAD, NECK, BODY, HELD, FEET }
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/EquipmentItem")]
public class EquipmentItem : ModdableItem {

    public EquipType equipType;
    
    public override void Use() {
        base.Use();
    }

}
