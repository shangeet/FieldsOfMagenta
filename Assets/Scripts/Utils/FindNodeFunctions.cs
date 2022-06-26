using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNodeFunctions {

    public static bool CONDITION_OPEN_NODE(Node node, TileEventManager tileEventManager, List<Node> visitedNodes) {
        //node is not an obstacle, other player, and hasn't been visited
        return (!tileEventManager.IsObstacleTile(node) &&
                !visitedNodes.Contains(node) &&
                !node.isOccupied());
    }

    public static bool CONDITION_VALID_NODE(Node node, TileEventManager tileEventManager, List<Node> visitedNodes) {
        return (!tileEventManager.IsObstacleTile(node) && !visitedNodes.Contains(node));
    }

    public static bool CONDITION_FOUND_ALWAYS_FALSE(Node currentNode, Node startNode) {
        return false;
    }

    public static bool CONDITION_FOUND_ENEMY_NODE(Node currentNode, Node startNode) {
        return (currentNode.isOccupied() && currentNode.getPlayerInfo().getIsEnemy() && currentNode != startNode);
    }

    public static bool CONDITION_FOUND_PLAYER_NODE(Node currentNode, Node startNode) {
        return currentNode.isOccupied() && !currentNode.getPlayerInfo().getIsEnemy();
    }
    
    public static bool NEARBY_PLAYER(Dictionary<Vector3Int, Node> nodeDict, Vector3Int pos) {
        return (nodeDict.ContainsKey(pos) && NodeUtils.nodeClickedIsPlayer(nodeDict[pos]));
    }

    public static bool NEARBY_NODE_NO_RESTRICTIONS(Dictionary<Vector3Int, Node> nodeDict, Vector3Int pos) {
        return nodeDict.ContainsKey(pos);
    }

}