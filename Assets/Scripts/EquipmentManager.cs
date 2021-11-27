using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public EquipmentItem[] currentEquipment;
    public delegate void OnEquipmentItemChangedCallback();
    public OnEquipmentItemChangedCallback onEquipmentChangedCallback;
    public static EquipmentManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        int numSlots = System.Enum.GetNames(typeof(EquipType)).Length;
        currentEquipment = new EquipmentItem[numSlots];
    }

    public void Equip(EquipmentItem newItem) {

        int equipSlot = (int) newItem.equipType;
    
        EquipmentItem oldItem = null;
    
        if(currentEquipment[equipSlot] != null) {
            oldItem = currentEquipment[equipSlot];
            Inventory.instance.AddItem(oldItem);
        }
    
        currentEquipment[equipSlot] = newItem;
    
        PlayerManager.instance.UpdateCharacterStatus(newItem, oldItem);  
    
        onEquipmentChangedCallback.Invoke();    
    }
    #endregion Singleton
}
