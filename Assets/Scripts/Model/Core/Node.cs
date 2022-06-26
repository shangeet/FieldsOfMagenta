using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Vector3Int position;
    private PlayerInfo playerInfo;
    private Color originalColor;

    public Node(Vector3Int position, PlayerInfo playerInfo, Color originalColor) {
        this.position = position;
        this.playerInfo = playerInfo;
        this.originalColor = originalColor;
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
