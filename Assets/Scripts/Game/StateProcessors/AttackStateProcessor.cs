using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateProcessor : StateProcessor
{
    public override bool Process() {

        StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
        int playerAttackRange = 1; //TODO Implement this based on player's class
        Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
        Node currentClickedNode = sharedResourceBus.GetCurrentClickedNode();
        Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
        List<Node> validAttackNodes = NodeUtils.getViableAttackNodes(playerAttackRange, clickedPlayerNode, nodeDict);
        BattleEventScreen battleEventScreen = uiHandler.GetBattleEventScreen();

        //preprocess highlight tiles red
        foreach (Node node in validAttackNodes) {
            //tileMap.SetColor(node.getPosition(), Color.red);
            staticTileHandler.SpawnTile(node.getPosition(), "RedTile");
        }   

        if (currentClickedNode != null && validAttackNodes.Contains(currentClickedNode)) { //clicked on a valid attack node. Time to attack
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }

            Node enemyNodeToAttack = currentClickedNode;
            Node playerAttacking = clickedPlayerNode;
            //display battle
            CalculateBattleEventDisplayBattleUI(battleEventScreen, playerAttacking, enemyNodeToAttack, nodeDict);
            //turn is over for player
            ChangeState(GameState.ShowExpGainState);
            return true;
        } else if (currentClickedNode != null && !validAttackNodes.Contains(currentClickedNode)) {
            //Remove the red highlight and go back to battle menu state
            foreach (Node node in validAttackNodes) {
                //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                staticTileHandler.DespawnTile(node.getPosition());
            }
            ChangeState(GameState.ShowBattleMenuState);  
            return true;     
        } else if (Input.GetMouseButtonDown(1) && !battleEventScreen.IsBattleEventScreenDisplayed()) { //cancel movement and track back
            //Remove the red highlight
            foreach (Node node in validAttackNodes) {
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
