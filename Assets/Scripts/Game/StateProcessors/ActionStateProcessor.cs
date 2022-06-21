using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStateProcessor : StateProcessor {

    public override bool Process() {

        StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
        int playerActionRange = 1; //TODO Implement this based on player's class
        Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
        Node currentClickedNode = sharedResourceBus.GetCurrentClickedNode();
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        bool isAttackAction = sharedResourceBus.GetCurrentGameState() == GameState.AttackState;
        List<Node> validActionNodes = NodeUtils.getViableActionNodes(playerActionRange, clickedPlayerNode, nodeDict, isAttackAction);
        BattleEventScreen battleEventScreen = uiHandler.GetBattleEventScreen();

        //preprocess highlight tiles red if attack action else green
        foreach (Node node in validActionNodes) {
            //tileMap.SetColor(node.getPosition(), Color.red);
            if (sharedResourceBus.GetCurrentGameState() == GameState.AttackState) {
                staticTileHandler.SpawnTile(node.getPosition(), "RedTile");    
            } else {
                staticTileHandler.SpawnTile(node.getPosition(), "GreenTile");
            }
        }   

        if (currentClickedNode != null && validActionNodes.Contains(currentClickedNode)) { //clicked on a valid action node. Time to act
            //Remove the tile highlight
            foreach (Node node in validActionNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }

            Node otherNode = currentClickedNode;
            Node playerActing = clickedPlayerNode;
            if (sharedResourceBus.GetCurrentGameState() == GameState.AttackState) {
                // display battle
                CalculateBattleEventDisplayBattleUI(battleEventScreen, playerActing, otherNode, nodeDict);                
            } else if (sharedResourceBus.GetCurrentGameState() == GameState.AttackState) {
                // display heal
                CalculateHealEventDisplayBattleUI(battleEventScreen, playerActing, otherNode, nodeDict);
            } else if (sharedResourceBus.GetCurrentGameState() == GameState.BuffState) {
                CalculateBuffEventDisplayBattleUI(battleEventScreen, playerActing, otherNode, nodeDict);
            }
            //turn is over for player
            ChangeState(GameState.ShowExpGainState);
            return true;
        } else if (currentClickedNode != null && !validActionNodes.Contains(currentClickedNode)) {
            //Remove the red highlight and go back to battle menu state
            foreach (Node node in validActionNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }
            ChangeState(GameState.ShowBattleMenuState);  
            return true;     
        } else if (Input.GetMouseButtonDown(1) && !battleEventScreen.IsBattleEventScreenDisplayed()) { //cancel movement and track back
            //Remove the red highlight
            foreach (Node node in validActionNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }
            CancelPlayerMove();
            PlayerBattleMenu playerBattleMenu = uiHandler.GetPlayerBattleMenu();
            playerBattleMenu.closePlayerBattleMenu();
            ResetTurnStateData();
            ChangeState(GameState.PlayerTurnStart);
            return true;
        }
        //Didn't click on anything. Stay in state
        return false;
    }

}
