using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains all heuristics for various pathfinding or AI approaches to decision-making
public class Heuristic
{

    public delegate int AllPathsOneHeuristicDelegate();
    public static int AllPathsOneHeuristic() {
        return 0;
    }

    public delegate int NodeDistanceHeuristicDelegate(Node a, Node b);
    public static int NodeDistanceHeuristic(Node a, Node b) {
        return 2 * ((int) (Mathf.Abs(a.getPosition().x - b.getPosition().x) + Mathf.Abs(a.getPosition().y - b.getPosition().y)));
    }

    public static int BasicEnemyAIPlayerChoiceHeuristic(Node enemy, Node player) {
        PlayerInfo playerInfo = player.getPlayerInfo();
        return NodeDistanceHeuristic(enemy, player) + playerInfo.currentHealth;
    }

}
