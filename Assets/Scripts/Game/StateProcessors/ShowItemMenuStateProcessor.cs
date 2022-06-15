using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowItemMenuStateProcessor : StateProcessor
{
    public override bool Process() {
        ItemMenu itemMenu = uiHandler.GetItemMenu();
        if (!itemMenu.IsItemMenuDisplayed()) {
            itemMenu.openPlayerItemMenu();
        }

        if (Input.GetMouseButtonDown(1)) { //cancel movement and track back
            itemMenu.closePlayerItemMenu();
            ChangeState(GameState.ShowBattleMenuState);
            return true;
        } 
        return false;       
    }
    
}
