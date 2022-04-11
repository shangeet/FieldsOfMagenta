using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleHandler {
    
    public static void ProcessBattle(PlayerInfo attackingPlayer, PlayerInfo defendingPlayer) {}

    public static PlayerInfo HandlePlayerStatuses(PlayerInfo info) {

        if (info.statusList.Count == 0) {
            return info;
        }

        List<StatusEffect> updatedStatusList = new List<StatusEffect>();

        foreach(StatusEffect effect in info.statusList) {

            if (effect.remainingActiveTurns != 0) {
                if (effect.effectType == EffectType.BURNED && effect.maxTurns == effect.remainingActiveTurns) { //applied only once
                    info.currentAttack = Mathf.RoundToInt((info.baseAttack * (0.5f)));
                    effect.remainingActiveTurns--;
                    updatedStatusList.Add(effect);
                } else if (effect.effectType == EffectType.POISONED) {
                    info.currentHealth = Mathf.RoundToInt((info.currentHealth * (0.75f)));
                    effect.remainingActiveTurns--;
                    updatedStatusList.Add(effect);
                } else if (effect.effectType == EffectType.PARALYZED && effect.maxTurns == effect.remainingActiveTurns) {
                    info.currentMov = info.currentMov == 1 ? 0 : Mathf.RoundToInt(info.currentMov * 0.5f);
                    effect.remainingActiveTurns--;
                    updatedStatusList.Add(effect);
                } //applied only once                       
            }
        }

        info.statusList = updatedStatusList;
        return info;
    }
}
