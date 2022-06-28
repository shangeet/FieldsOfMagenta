using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect {
    
    private string effectName;
    private string effectDesc;

    public ItemEffect(string effectName, string effectDesc) {
        this.effectName = effectName;
        this.effectDesc = effectDesc;
    }

    public string getEffectName() {
        return effectName;
    }

    public string getEffectDescription() {
        return effectDesc;
    }

}
