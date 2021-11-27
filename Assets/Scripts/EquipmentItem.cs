using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { HEAD, NECK, BODY, HELD, FEET }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/EquipmentItem")]
public class EquipmentItem : Item
{
    public int atkMod {get; set;}
    public int defMod {get; set;}
    public int mAtkMod {get; set;}
    public int mDefMod {get; set;}
    public int luckMod {get; set;}
    public int movMod {get; set;}
    public int healthMod {get; set;}
    public int manaMod {get; set;}
    public EquipType equipType {get; set;}
    
    public override void Use() {
        base.Use();
        EquipmentManager.instance.Equip(this);
        Inventory.instance.RemoveItem(this);
    }

}
