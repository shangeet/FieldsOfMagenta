using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventStatus { NULL, HEALTH, PLAYER_POSITION }
public enum EventConditional{ NULL, LESS_THAN, EQUAL_TO, GREATER_THAN, AT}
[System.Serializable]
public class NovelEventCondition {

    public string playerId;
    public EventStatus playerStatus;
    public EventConditional conditional;
    public int intValue;
    public Vector3Int vectorValue;

    public NovelEventCondition() {
        this.playerId = "";
        this.playerStatus = EventStatus.NULL;
        this.conditional = EventConditional.NULL;
        this.intValue = int.MinValue;
        this.vectorValue = Vector3Int.zero;
    }

    public NovelEventCondition(string playerId, EventStatus playerStatus, EventConditional conditional, int intValue, Vector3Int vectorValue) {
        this.playerId = playerId;
        this.playerStatus = playerStatus;
        this.conditional = conditional;
        this.intValue = intValue;
        this.vectorValue = vectorValue;
    }

    public bool MeetsCondition(PlayerInfo playerInfo, Vector3Int playerCurrentPosition) {
        
        if (playerInfo.id != playerId) {
            return false;
        }

        if (playerStatus == EventStatus.HEALTH) {
            var condition = new PlayerStatNovelCondition(playerStatus, conditional, intValue, playerInfo);
            return condition.IsValid();
        } else if (playerStatus == EventStatus.PLAYER_POSITION) {
            var condition = new PlayerPositionNovelCondition(conditional, vectorValue, playerCurrentPosition);
            return condition.IsValid();
        }
        return false;
    }

}
