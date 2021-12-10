using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject {

    public string itemName = "New Name";
    public Sprite itemSprite;
    public string itemDescription = "";
    public string assetPath = "";

    public virtual void Use() {} //Override in child classes

    public virtual void Drop() {
        Inventory.instance.RemoveItem(this);
    }
    
    public override bool Equals(System.Object obj) {
      //Check for null and compare run-time types.
      if ((obj == null) || ! this.GetType().Equals(obj.GetType())) {
         return false;
      } else {
         Item item = (Item) obj;
         return (itemName.Equals(item.itemName));
      }
   }
    
    public bool Equals(Item item) {
     if ((object)item == null)
         return false;
     return itemName.Equals(item.itemName);
    }
}
