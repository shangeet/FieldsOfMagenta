using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectType { POISONED, BURNED, PARALYZED, BUFF, DEBUFF }
[System.Serializable]
public class StatusEffect  {
    public EffectType effectType;
    
    public int remainingActiveTurns;

    public int maxTurns;

    public StatusEffect(EffectType effectType, int maxTurns) {
        this.effectType = effectType;
        this.maxTurns = maxTurns;
        this.remainingActiveTurns = maxTurns;
    }

    public StatusEffect(EffectType effectType, int maxTurns, int remainingActiveTurns) {
        this.effectType = effectType;
        this.maxTurns = maxTurns;
        this.remainingActiveTurns = remainingActiveTurns;        
    }

}
