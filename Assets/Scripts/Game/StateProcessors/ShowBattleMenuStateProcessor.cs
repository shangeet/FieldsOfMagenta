using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBattleMenuStateProcessor : StateProcessor {

    public override bool Process() {
        PlayerBattleMenu playerBattleMenu = uiHandler.GetPlayerBattleMenu();
        if (!playerBattleMenu.IsPlayerBattleMenuDisplayed() && !sharedResourceBus.PlayerCurrentlyMoving()) {
            bool isHealingClass = sharedResourceBus.GetClickedPlayerNode().getPlayerInfo().IsHealer();
            playerBattleMenu.openPlayerBattleMenu(isHealingClass);
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
