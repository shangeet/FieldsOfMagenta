using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public PlayerInfo playerInfo;
    public ConsumableItemManager consumableItemManager;
    public EquipmentItemManager equipmentItemManager;

    public void Setup(PlayerInfo pI, ConsumableItemManager cIMgr, EquipmentItemManager eQPMgr) {
        playerInfo = pI;
        consumableItemManager = cIMgr;
        equipmentItemManager = eQPMgr;
    }
}
