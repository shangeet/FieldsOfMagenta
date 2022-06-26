using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUtils {

    public delegate bool IsValidFindConditionFunc(Node currentNode, Node startNode);
    public delegate bool IsValidChildFunc(Node node, TileEventManager tileEventManager, List<Node> visitedNodes);
    public delegate bool NearbyNodeAlgorithmFunc(Dictionary<Vector3Int, Node> nodeDict, Vector3Int pos);

    public static List<Node> getViableNodesPaths(Node startNode, Dictionary<Vector3Int, Node> nodeDict, TileEventManager tileManager) {
        int mov = startNode.getPlayerInfo().currentMov;
        List<Node> visitedNodes = new List<Node>();
        List<Node> validNodes = new List<Node>();
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, int> distFromStart = new Dictionary<Node, int>();

        distFromStart[startNode] = 0;
        queue.Enqueue(startNode);
        int iterations = 0;
        while (queue.Count != 0) {
            iterations += 1;
            if (iterations > 100) {
                //Debug.Log("Detected Problem. Infinite loop");
                return null;
            }
            Node currentNode = queue.Dequeue();
            List<Node> nearbyNodes = GetAdjacentNodes(currentNode, nodeDict, FindNodeFunctions.NEARBY_NODE_NO_RESTRICTIONS);
            foreach (Node childNode in nearbyNodes) {
                if (!tileManager.IsObstacleTile(childNode) &&  !visitedNodes.Contains(childNode) && !childNode.isOccupied()) { //node is not an obstacle, other player, and hasn't been visited
                    int newDist = distFromStart[currentNode] + 1;
                    if (newDist <= mov) {
                        distFromStart[childNode] = newDist;
                        if (!validNodes.Contains(childNode)) {
                            validNodes.Add(childNode);
                            queue.Enqueue(childNode);
                        }
                    }
                }
            }
            visitedNodes.Add(currentNode);
        }
        //Debug.Log(validNodes.Count);
        return validNodes;
    }    

    public static List<Node> getViableActionNodes(int actionRange, Node startNode, Dictionary<Vector3Int, Node> nodeDict, bool isActionOnEnemy) {
        List<Node> visitedNodes = new List<Node>();
        List<Node> validNodes = new List<Node>();
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, int> distFromStart = new Dictionary<Node, int>();

        distFromStart[startNode] = 0;
        queue.Enqueue(startNode);
        int iterations = 0;
        while (queue.Count != 0) {
            iterations += 1;
            if (iterations > 100) {
                return null;
            }
            Node currentNode = queue.Dequeue();
            List<Node> nearbyNodes = GetAdjacentNodes(currentNode, nodeDict, FindNodeFunctions.NEARBY_NODE_NO_RESTRICTIONS);
            foreach (Node childNode in nearbyNodes) {
                if (!visitedNodes.Contains(childNode) && childNode.isOccupied() && (childNode.getPlayerInfo().getIsEnemy() == isActionOnEnemy)) { //node is not visited, occupied, check if it's an enemy action or not
                    int newDist = distFromStart[currentNode] + 1;
                    if (newDist <= actionRange) {
                        distFromStart[childNode] = newDist;
                        if (!validNodes.Contains(childNode)) {
                            validNodes.Add(childNode);
                            queue.Enqueue(childNode);
                        }
                    }
                }
            }
            visitedNodes.Add(currentNode);
        }
        return validNodes;
    }

        //Use A* for shortest path
    public static List<Node> getShortestPathNodes(Node start, Node end, List<Node> viableNodes,  Heuristic.NodeDistanceHeuristicDelegate H, Dictionary<Vector3Int, Node> nodeDict) {
        List<Node> openList = new List<Node> {start};
        List<Node> closedList = new List<Node>();
        Dictionary<Node, int> costMap = new Dictionary<Node, int>();
        int iterations = 0;

        //calculate initial costs    
        costMap[start] = 0 + H(start, end);

        while(openList.Count > 0) {
            iterations += 1;
            Node currentNode = getLowestCostNodeMap(costMap, openList);
            if (iterations >= 100) {
                //Debug.Log("Detected Problem. Infinite loop");
                return null;
            }
            if (currentNode == end) {
                //Debug.Log("Got path!");
                //printDictionaryToConsole(costMap);          
                List<Node> optimalPath = NodeUtils.derivePath(costMap, start, end, nodeDict);
                // string pathToPrint = "Path: ";
                // foreach(Node node in optimalPath) {
                //     pathToPrint += "(" + node.getPosition().x + "," + node.getPosition().y + ") ";
                // }
                // Debug.Log(pathToPrint); 
                return optimalPath;
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (Node child in GetAdjacentNodes(currentNode, nodeDict, FindNodeFunctions.NEARBY_NODE_NO_RESTRICTIONS)) {
                if(closedList.Contains(child)) { //second check performed since we don't know if child is necessarily viable
                    continue; //skip this iteration 
                }
                if (viableNodes.Contains(child) || child == end) {
                    int gStartToCurrentNode = getDistanceBetweenNodes(start, currentNode);
                    int distCurrentNodeToChild = getDistanceBetweenNodes(currentNode, child);
                    int newCost = gStartToCurrentNode + distCurrentNodeToChild;
                    int gStartToChild = getDistanceBetweenNodes(start, child);
                    //Debug.Log("Start->Child " + gStartToChildNode + " CostStartCurr->CurrChild " + newCost);
                    if (openList.Contains(child) && newCost < gStartToChild) {
                        //Debug.Log("Removed to openlist: (" + child.getPosition().x + "," + child.getPosition().y + ")");
                        openList.Remove(child);
                    }
                    if (closedList.Contains(child) && newCost < gStartToChild) {
                        closedList.Remove(child);
                    }
                    if (!openList.Contains(child) && !(closedList.Contains(child))) {
                        openList.Add(child);
                        costMap[child] = gStartToChild + H(child, end);
                        //Debug.Log("Added to openlist: (" + child.getPosition().x + "," + child.getPosition().y + ")");
                        //Debug.Log("Costmap updated..." + newCost + " " + H(child, end));
                        //printDictionaryToConsole(costMap);
                    }
                }
            }
        }
        return openList;
    }

    public static List<Node> GetAdjacentNodes(Node currentNode, Dictionary<Vector3Int, Node> nodeDict, NearbyNodeAlgorithmFunc isValidFunc) {
        List<Node> neighboringNodes = new List<Node>();
        Vector3Int[] positions = {
            new Vector3Int(currentNode.getPosition().x, currentNode.getPosition().y + 1, currentNode.getPosition().z),
            new Vector3Int(currentNode.getPosition().x, currentNode.getPosition().y - 1, currentNode.getPosition().z),
            new Vector3Int(currentNode.getPosition().x + 1, currentNode.getPosition().y, currentNode.getPosition().z),
            new Vector3Int(currentNode.getPosition().x - 1, currentNode.getPosition().y, currentNode.getPosition().z)
        };
        foreach (Vector3Int pos in positions) {
            if (isValidFunc(nodeDict, pos)) {
                neighboringNodes.Add(nodeDict[pos]);
            }
        }
        return neighboringNodes;
    }

    //TODO optimize this
    public static Node getLowestCostNodeMap(Dictionary<Node, int> nodeMap, List<Node> openList) {
        int smallest = int.MaxValue;
        Node lowestCostNode = null;
        foreach(KeyValuePair<Node, int> entry in nodeMap) {
            int newDist = entry.Value;
            if (newDist < smallest && openList.Contains(entry.Key)) {
                lowestCostNode = entry.Key;
                smallest = newDist;
            }
        }
        return lowestCostNode;
    }

    public static bool nodeClickedIsPlayer(Node node) {
        return node != null && node.isOccupied() && !node.getPlayerInfo().getIsEnemy();
    }

    public static bool nodeClickedIsEnemy(Node node) {
        return node != null && node.isOccupied() && node.getPlayerInfo().getIsEnemy();
    }

    public static int getDistanceBetweenNodes(Node a, Node b) {
        return (int) Mathf.Abs(a.getPosition().x - b.getPosition().x) + Mathf.Abs(a.getPosition().y - b.getPosition().y);
    }

    public static Node findEnemyNode(string playerId, Dictionary<Vector3Int, Node> nodeDict) { //TODO optimize this using a class dictionary var
        //Debug.Log("Looking for id: " + playerId);
        foreach(Node n in nodeDict.Values) {
            if (n.getPlayerInfo() != null && n.getPlayerInfo().getPlayerId() == playerId) {
                //Debug.Log("Returned a node!");
                return n;
            }
        }
        //Debug.Log("returned null...");
        return null;
    }

    public static List<Node> derivePath(Dictionary<Node, int> fMap, Node startNode, Node goalNode, Dictionary<Vector3Int, Node> nodeDict) {
        List<Node> optimalPath = new List<Node>(){startNode};
        Node lastNode = startNode;
        int iterations = 0;
       //Debug.Log("START NODE: " + "(" + startNode.getPosition().x + "," + startNode.getPosition().y + ")");
       //Debug.Log("GOAL NODE: " + "(" + goalNode.getPosition().x + "," + goalNode.getPosition().y + ")");
        while(!optimalPath.Contains(goalNode)) {
            iterations += 1;
            //Debug.Log("LAST NODE: " + "(" + lastNode.getPosition().x + "," + lastNode.getPosition().y + ")");
            List<Node> neighbors = GetAdjacentNodes(lastNode, nodeDict, FindNodeFunctions.NEARBY_NODE_NO_RESTRICTIONS);
            if (neighbors.Contains(goalNode)) {
                //Debug.Log("Adding goal node...");
                optimalPath.Add(goalNode);
            } else {
                //Debug.Log("Getting neighbors...");
                // foreach(Node n in neighbors) {
                //     Debug.Log("NEIGHBOR: " + "(" + n.getPosition().x + "," + n.getPosition().y + ")");
                // }
                int lowestDist = int.MaxValue;
                Node bestNode = null;
                foreach (Node n in neighbors) {
                    if (fMap.ContainsKey(n) && fMap[n] < lowestDist && !optimalPath.Contains(n)) {
                        lowestDist = fMap[n];
                        bestNode = n;
                    }
                }
                optimalPath.Add(bestNode);
                lastNode = bestNode;
            }
            if (iterations > 100) {
                return null;
            }
        }
        return optimalPath;
    }

    public static Node FindNode(Node startNode, Dictionary<Vector3Int, Node> nodeDict, TileEventManager tileManager, IsValidFindConditionFunc isValidFindConditionFunc, IsValidChildFunc isValidChildFunc) {
        int mov = startNode.getPlayerInfo().currentMov;
        List<Node> visitedNodes = new List<Node>();
        List<Node> validNodes = new List<Node>();
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, int> distFromStart = new Dictionary<Node, int>();

        distFromStart[startNode] = 0;
        queue.Enqueue(startNode);
        int iterations = 0;
        while (queue.Count != 0) {
            iterations += 1;
            if (iterations > 100) {
                return null;
            }
            Node currentNode = queue.Dequeue();
            if (isValidFindConditionFunc(currentNode, startNode)) { //found player node in range
                return currentNode;
            }
            List<Node> nearbyNodes = GetAdjacentNodes(currentNode, nodeDict, FindNodeFunctions.NEARBY_NODE_NO_RESTRICTIONS);
            foreach (Node childNode in nearbyNodes) {
                if (isValidChildFunc(childNode, tileManager, visitedNodes)) { //node is not an obstacle, other enemy, and hasn't been visited
                    int newDist = distFromStart[currentNode] + 1;
                    if (newDist <= mov) {
                        distFromStart[childNode] = newDist;
                        if (!validNodes.Contains(childNode)) {
                            validNodes.Add(childNode);
                            queue.Enqueue(childNode);
                        }
                    }
                }
            }
            visitedNodes.Add(currentNode);
        }
        return null;        
    }

}
