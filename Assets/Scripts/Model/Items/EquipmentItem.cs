using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EquipType { HEAD, NECK, BODY, HELD, FEET }
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/EquipmentItem")]
[System.Serializable]
public class EquipmentItem : ModdableItem {

    public EquipType equipType;
    public List<ItemEffect> itemEffects;

    public override void Use() {
        base.Use();
    }

    public EquipType GetEquipType() {
        return equipType;
    }

}
