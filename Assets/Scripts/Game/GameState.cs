using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlayerSetupState,
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
    HealState,
    BuffState,
    HandleTileState,
    TurnEndState,
    EnemyTurnState,
    GameEndState
}
