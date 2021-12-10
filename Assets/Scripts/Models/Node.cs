using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Vector3Int position;
    private bool hasObstacle;
    private PlayerInfo playerInfo;

    private Color originalColor;

    public Node(Vector3Int position, bool hasObstacle, PlayerInfo playerInfo, Color originalColor) {
        this.position = position;
        this.hasObstacle = hasObstacle;
        this.playerInfo = playerInfo;
        this.originalColor = originalColor;
    }

    public void setHasObstacle(bool isBlocked) {
        this.hasObstacle = isBlocked;
    }

    public bool getHasObstacle() {
        return this.hasObstacle;
    }

    public void setPlayerInfo(PlayerInfo playerInfo) {
        this.playerInfo = playerInfo;
    }

    public PlayerInfo getPlayerInfo() {
        return this.playerInfo;
    }

    public string getPlayerId() {
        return playerInfo == null ? null : playerInfo.getPlayerId();
    }

    public Vector3Int getPosition() {
        return this.position;
    }

    public bool isOccupied() {
        return this.playerInfo != null;
    }

    public Color getOriginalColor() {
        return this.originalColor;
    }
}
