using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumptionType {HEAL, TEMPBOOST, PERMABOOST}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ConsumableItem")]
public class ConsumableItem : ModdableItem {

    public int baseDuration;
    public int currentDuration;
    public ConsumptionType consumptionType;  

    public override void Use() {
        base.Use();
    }
}
