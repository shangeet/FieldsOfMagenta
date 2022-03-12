using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionNovelCondition : NovelEventCondition, NovelEventInterface {
    
    private Vector3Int playerCurrentPosition;
    public PlayerPositionNovelCondition(EventConditional conditional, Vector3Int vectorValue, Vector3Int playerCurrentPosition) {
        this.conditional = conditional;
        this.vectorValue = vectorValue;
        this.playerCurrentPosition = playerCurrentPosition;
    }

    public bool IsValid() {
        if (conditional == EventConditional.EQUAL_TO) {
            return vectorValue == playerCurrentPosition;
        }
        return false;
    }
}
