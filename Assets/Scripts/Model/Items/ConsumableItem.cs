using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ConsumptionType {HEAL, TEMPBOOST, PERMABOOST}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ConsumableItem")]
[System.Serializable]
public class ConsumableItem : ModdableItem {

    public int baseDuration;
    public int currentDuration;
    public ConsumptionType consumptionType;  

    public override void Use() {
        base.Use();
    }
}
