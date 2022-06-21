using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBattleMenuStateProcessor : StateProcessor {

    public override bool Process() {
        PlayerBattleMenu playerBattleMenu = uiHandler.GetPlayerBattleMenu();
        if (!playerBattleMenu.IsPlayerBattleMenuDisplayed() && !sharedResourceBus.PlayerCurrentlyMoving()) {
            playerBattleMenu.openPlayerBattleMenu();
        }

        if (Input.GetMouseButtonDown(1)) { //cancel movement and track back
            CancelPlayerMove();
            playerBattleMenu.closePlayerBattleMenu();
            ResetTurnStateData();
            ChangeState(GameState.PlayerTurnStart);
            return true;
        }        
        return false;
    }
    
}
