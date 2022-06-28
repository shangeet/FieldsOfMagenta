using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatusEffect : ItemEffect {

    private EffectType effectType;
    private float procChance;

    public ItemStatusEffect(string effectName, string effectDesc, EffectType effectType, float procChance) : base(effectName, effectDesc) {
        this.effectType = effectType;
        this.procChance = procChance;
    }
    
}
