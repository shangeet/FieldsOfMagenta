using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "FieldsOfMagenta/Item", order = 1)]
public class Item : ScriptableObject {
    public string itemName = "New Name";
    public Sprite itemSprite {get; set;}
    public string itemDescription = "";

    public virtual void Use() {} //Override in child classes
    public virtual void Drop() {
        Inventory.instance.RemoveItem(this);
    }
}
