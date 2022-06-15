using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndStateProcessor : StateProcessor {
    
    public override bool Process() {
        NovelEventManager novelEventManager = sharedResourceBus.GetNovelEventManager();
        PhaseTransitionUIHandler phaseTransitionUIHandler = uiHandler.GetPhaseTransitionUIHandler();
        PlayerExpScreen playerExpScreen = uiHandler.GetPlayerExpScreen();
        if (!novelEventManager.IsEventRunning()) {
            if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !playerExpScreen.IsExperienceScreenProcessing() && novelEventManager.AllEventsPlayed()) {
                MasterGameStateController gameStateInstance = sharedResourceBus.GetMasterGameStateController();
                if (sharedResourceBus.PlayerVictory()) {
                    GameObject playerVictoryScreen = uiHandler.GetPlayerVictoryScreen();
                    playerVictoryScreen.SetActive(true);
                    gameStateInstance.ClearInfoBeforeBattleData();
                    return true;
                } else {
                    GameObject playerDefeatScreen = uiHandler.GetPlayerDefeatScreen();
                    playerDefeatScreen.SetActive(true);
                    gameStateInstance.RevertDataToBeforeBattle();
                    return true;
                }
            }
        }
        return false;
    }
}
