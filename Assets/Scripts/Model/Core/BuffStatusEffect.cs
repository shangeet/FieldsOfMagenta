using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffAttr { ATK, DEF, MATK, MDEF, DEX, LUK, MOV }
[System.Serializable]
public class BuffStatusEffect : StatusEffect {
    
    private BuffAttr buffAttr;
    private int buffAmt;

    public BuffStatusEffect(EffectType effectType, BuffAttr buffAttr, int buffAmt, int maxTurns) : base(effectType, maxTurns) {
       this.buffAttr = buffAttr;
       this.buffAmt = buffAmt;
    }

    public BuffAttr GetBuffAttr() {
        return buffAttr;
    }

    public int GetBuffAmt() {
        return buffAmt;
    }

}
