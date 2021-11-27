using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerInfo playerInfo;
 
    #region Singleton
    public static PlayerManager instance;
 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
 
    #endregion
 
    public void UpdateCharacterStatus(EquipmentItem newItem, EquipmentItem oldItem)
    {
        if(oldItem != null) {} 
    }
}
