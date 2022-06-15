using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerStartStateProcessor : StateProcessor {


    public override bool Process() {

        //check if game over before processing starts
        if (CheckIfGameEnded()) {
            return true;
        }
        
        if (!sharedResourceBus.PlayerCurrentlyMoving()) {
            StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
            List<Node> clickedNodePath = sharedResourceBus.GetClickedNodePath();
            Node currentClickedNode = sharedResourceBus.GetCurrentClickedNode(); // this is the new node that the player clicked (aka the destination node)
            Node clickedNode = sharedResourceBus.GetClickedNode(); // this node was the first clicked node in the movement phase (aka the node the player is on)
            if (Input.GetMouseButtonDown(1)) { //track back
                foreach (Node node in clickedNodePath) {
                    //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                    staticTileHandler.DespawnTile(node.getPosition());
                }
                ChangeState(GameState.PlayerTurnStart);
                return true;
            } else if (currentClickedNode == clickedNode) { //player node clicked on again, open battle menu and reset colors
                foreach (Node node in clickedNodePath) {
                    //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                    staticTileHandler.DespawnTile(node.getPosition());
                }
                ChangeState(GameState.ShowBattleMenuState);
                return true;
            } else if (currentClickedNode != null) {
                sharedResourceBus.SetPreviousClickedNode(clickedNode);
                sharedResourceBus.SetClickedNode(currentClickedNode);
                if(clickedNodePath != null && clickedNodePath.Contains(currentClickedNode)) {
                    Node clickedPlayerNode = sharedResourceBus.GetClickedPlayerNode();
                    PlayerAnimator playerToAnimate = clickedPlayerNode.getPlayerInfo().playerAnimator;
                    //move player
                    List<Node> pathToTake = NodeUtils.getShortestPathNodes(clickedPlayerNode, currentClickedNode, clickedNodePath,
                     Heuristic.NodeDistanceHeuristic, sharedResourceBus.GetNodeDict());
                    
                    StartCoroutine(playerToAnimate.MovePlayerToTile(sharedResourceBus.GetTileMap(), pathToTake));
                    
                    //update player information in nodes
                    SwapNodeInfoOnSpriteMove(pathToTake[0], pathToTake[pathToTake.Count - 1]);
                    sharedResourceBus.SetClickedPlayerNode(pathToTake[pathToTake.Count - 1]);

                    //reset state after moving 
                    foreach (Node node in clickedNodePath) {
                        //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                        staticTileHandler.DespawnTile(node.getPosition());
                    }
                    ChangeState(GameState.ShowBattleMenuState); 
                    return true;
                } else { //outside node. go back to turn start state and reset info
                    foreach (Node node in clickedNodePath) {
                        //tileMap.SetColor(node.getPosition(), node.getOriginalColor());
                        staticTileHandler.DespawnTile(node.getPosition());
                    }            
                    ChangeState(GameState.PlayerTurnStart);
                    ResetTurnStateData();
                    return true;
                }
            }
        }

        return false;        
    }
    
}
