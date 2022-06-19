using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnEndProcessor : StateProcessor {

    public override bool Process() {

        //check if game over before processing starts
        if (CheckIfGameEnded()) {
            return true;
        }

        //record end of unit's turn in dict, but only if it exists (if it died, we ignore since that node is now null)
        if (!sharedResourceBus.IsEnemyTurn()) {

            Node clickedNode = sharedResourceBus.GetClickedNode();
            Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
            Dictionary<string, bool> playerTurnEndedDict = sharedResourceBus.GetPlayerTurnEndedDict();
            PlayerExpScreen playerExpScreen = uiHandler.GetPlayerExpScreen();
            PhaseTransitionUIHandler phaseTransitionUIHandler = uiHandler.GetPhaseTransitionUIHandler();

            if (clickedNode.getPlayerId() != null) {
                playerTurnEndedDict[clickedPlayerNode.getPlayerInfo().getPlayerId()] = true;
                clickedPlayerNode.getPlayerInfo().playerAnimator.AnimateSpriteTurnEnded();
            }    

            //auto turn-end if all units are done
            if (!playerTurnEndedDict.ContainsValue(false) && !phaseTransitionUIHandler.IsPhaseTransitionRunning() && !playerExpScreen.IsExperienceScreenProcessing()) {
                ProcessStatusEffects();
                sharedResourceBus.ResetPlayerTurnEndedDict();
                sharedResourceBus.ResetEnemyTurnEndedDict();
                StartCoroutine(phaseTransitionUIHandler.translatePhaseImage("EnemyPhase"));
                //check if all enemies or players have died
                CheckIfGameEnded(); 
                ChangeState(GameState.EnemyTurnState);
                ResetTurnStateData();
                return true;
            } else if (!phaseTransitionUIHandler.IsPhaseTransitionRunning() && !playerExpScreen.IsExperienceScreenProcessing()) { //else we go back to the player turn start state, but wait for transition to not run
                //check if all enemies or players have died
                CheckIfGameEnded(); 
                ChangeState(GameState.PlayerTurnStart);         
                //reset turn state data
                ResetTurnStateData();
                return true;
            }        
        } else {
            //check if all enemies or players have died
            CheckIfGameEnded(); 
            ChangeState(GameState.EnemyTurnState);
            return true;
        }    
        return false;    
    }
    
}
