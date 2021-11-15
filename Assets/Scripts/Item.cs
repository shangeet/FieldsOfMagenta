using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

    private string itemName;
    private int atkMod = 0;
    private int defMod = 0;
    private int  mAtkMod = 0;
    private int mDefMod = 0;
    private int luckMod = 0;
    private int movMod = 0;
    private int healthMod = 0;
    private int manaMod = 0;
    private int cost; 
    private bool isConsumable;


    public Item(string itemName, Dictionary<string,int> modifiers, int cost, bool isConsumable) {
        this.itemName = itemName;

        this.atkMod = modifiers.ContainsKey("atkMod") ? modifiers["atkMod"] : atkMod;
        this.defMod = modifiers.ContainsKey("defMod") ? modifiers["defMod"] : defMod;
        this.mAtkMod = modifiers.ContainsKey("mAtkMod") ? modifiers["mAtkMod"] : mAtkMod;
        this.mDefMod = modifiers.ContainsKey("mDefMod") ? modifiers["mDefMod"] : mDefMod;
        this.luckMod = modifiers.ContainsKey("luckMod") ? modifiers["luckMod"] : luckMod;
        this.movMod = modifiers.ContainsKey("movMod") ? modifiers["movMod"] : movMod;
        this.healthMod = modifiers.ContainsKey("healthMod") ? modifiers["healthMod"] : healthMod;
        this.manaMod = modifiers.ContainsKey("manaMod") ? modifiers["manaMod"] : manaMod;

        this.cost = cost;
        this.isConsumable = isConsumable;
    }
    
}
