using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowExpGainStateProcessor : StateProcessor {
    
    public override bool Process() {
        BattleEventScreen battleEventScreen = uiHandler.GetBattleEventScreen();
        PlayerExpScreen playerExpScreen = uiHandler.GetPlayerExpScreen();
        if (!sharedResourceBus.ShowExpGainBar() && !battleEventScreen.IsBattleEventScreenDisplayed()) {
                ChangeState(GameState.HandleTileState);  
                return true;
        } else {
            if (!battleEventScreen.IsBattleEventScreenDisplayed() && !playerExpScreen.IsExperienceScreenProcessing()) {
                Vector3 playerPos = sharedResourceBus.GetClickedPlayerNode().getPosition();
                playerExpScreen.ShowPlayerGainExpScreen(sharedResourceBus.GetOldBattlePI(), sharedResourceBus.GetNewBattlePI(),
                 sharedResourceBus.GetTotalExpToLevelUp(), sharedResourceBus.GetTimesLeveledUp(), playerPos);
                sharedResourceBus.SetShowExpGainBar(false);
                sharedResourceBus.SetOldBattlePI(null);
                sharedResourceBus.SetNewBattlePI(null);
                ChangeState(GameState.HandleTileState);
                return true;
            }
        }
        return false;
    }
}
