using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatNovelCondition : NovelEventCondition, NovelEventInterface {

    private PlayerInfo playerInfo;
    public PlayerStatNovelCondition(EventStatus playerStatus, EventConditional conditional, int value, PlayerInfo info) {
        this.playerStatus = playerStatus;
        this.conditional = conditional;
        this.intValue = value;
        this.playerInfo = info;
    }

    public bool IsValid() {
        try {
            if (conditional == EventConditional.LESS_THAN) {
                return getGoalStat() - intValue < 0;
            } else if (conditional == EventConditional.GREATER_THAN) {
                return getGoalStat() - intValue > 0;
            } else if (conditional == EventConditional.EQUAL_TO) {
                return (getGoalStat() - intValue) == 0;
            }            
        } catch(System.Exception e) {
            Debug.Log(e.ToString());
            return false;
        }
        return false;
    }

    private int getGoalStat() {
        if (playerStatus == EventStatus.HEALTH) {
            return playerInfo.currentHealth;
        }
        throw new System.NotImplementedException();
    }

    
}
