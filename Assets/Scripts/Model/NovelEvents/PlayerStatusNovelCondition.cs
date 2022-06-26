using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusNovelCondition : NovelEventCondition, NovelEventInterface {

    private PlayerInfo playerInfo;

    public PlayerStatusNovelCondition(EventStatus playerStatus, EventConditional conditional, int intValue, PlayerInfo info) {
        this.playerInfo = info; 
        this.playerStatus = playerStatus;
        this.conditional = conditional;
        this.intValue = intValue;
    }

    public bool IsValid() {
        return false;
    }

    
}
