using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 1)]
[System.Serializable]
public class Item : ScriptableObject {

    public string itemName;
    [System.NonSerialized] public Sprite itemSprite;
    public string itemDescription;
    public string assetPath;
    public string spritePath;
    public List<int> unlockableQuestIds = new List<int>();
    public int goldValue;

    public virtual void Use() {} //Override in child classes
    
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

    public bool meetsPrerequisites(List<int> currentCompletedQuestIds) {
        foreach(int questId in unlockableQuestIds) {
            if (!currentCompletedQuestIds.Contains(questId)) {
                return false;
            }
        }
        return true;
    }

    public Sprite GetSpriteFromSpritePath() {
        return Resources.Load<Sprite>(spritePath);
    }
}
