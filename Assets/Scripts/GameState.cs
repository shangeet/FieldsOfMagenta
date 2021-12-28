using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlayerTurnStart,
    ShowUnitInfoState,
    ShowEnemyInfoState,
    MovePlayerStartState,
    ShowBattleMenuState,
    ShowExpGainState,
    ShowItemMenuState,
    UseItemState,
    SwapItemState,
    AttackState,
    TurnEndState,
    EnemyTurnState,
    GameEndState
}
