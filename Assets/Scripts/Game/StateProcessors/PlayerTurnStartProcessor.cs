using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnStartProcessor : StateProcessor
{
    
    public override bool Process() {

        if (CheckIfGameEnded()) {
            return true;
        }

        Node currentClickedNode = sharedResourceBus.GetCurrentClickedNode();
        if (currentClickedNode != null) {
            Node clickedNode = currentClickedNode;  
            sharedResourceBus.SetClickedNode(clickedNode);
            Dictionary<string, bool> playerTurnEndedDict = sharedResourceBus.GetPlayerTurnEndedDict();
            if (NodeUtils.nodeClickedIsPlayer(clickedNode) && playerTurnEndedDict[clickedNode.getPlayerInfo().getPlayerId()] == false) {
                TileEventManager tileEventManager = sharedResourceBus.GetTileEventManager();
                Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
                List<Node> clickedNodePath = NodeUtils.getViableNodesPaths(clickedNode, nodeDict, tileEventManager);
                sharedResourceBus.SetClickedPlayerNode(clickedNode);
                sharedResourceBus.SetClickedNodePath(clickedNodePath);
                StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
                foreach (Node node in clickedNodePath) {
                    staticTileHandler.SpawnTile(node.getPosition(), "BlueTile");
                }
                ChangeState(GameState.MovePlayerStartState);
                return true;
            }
        }
        return false;
    }
}
