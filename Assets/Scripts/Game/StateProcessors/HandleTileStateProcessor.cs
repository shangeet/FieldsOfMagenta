using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTileStateProcessor : StateProcessor {

    public override bool Process() {
        TileEventManager tileEventManager = sharedResourceBus.GetTileEventManager();
        Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
        if (clickedPlayerNode != null) {
            PlayerInfo currentPI = clickedPlayerNode.getPlayerInfo();
            if (currentPI.currentHealth > 0) {
                MasterGameStateController gameStateInstance = sharedResourceBus.GetMasterGameStateController();
                tileEventManager.ProcessTile(clickedPlayerNode, currentPI, gameStateInstance);
                CheckIfPlayerDied(currentPI);                
            }
        }
        ChangeState(GameState.TurnEndState);
        return true;
    }
}
