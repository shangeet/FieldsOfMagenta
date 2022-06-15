using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMenu : MonoBehaviour {
    

    public SharedResourceBus sharedResourceBus;

    protected virtual void Awake() {
        sharedResourceBus = GameObject.Find("SharedResourceBus").GetComponent<SharedResourceBus>();
    }

    public void ChangeState(GameState gameState) {
        sharedResourceBus.SetCurrentGameState(gameState);
    }

    public void EndShowSwapItemMenuStateToShowBattleMenuState(List<Node> nearbyPlayerNodes) {
        StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
        foreach (Node n in nearbyPlayerNodes) {
            if (NodeUtils.nodeClickedIsPlayer(n)) {
                //tileMap.SetColor(n.getPosition(), n.getOriginalColor());
                staticTileHandler.DespawnTile(n.getPosition());
            }
        }
        ChangeState(GameState.ShowBattleMenuState);
        sharedResourceBus.SetHighlightedPossibleSwapPartner(false); // toggle to false
    }

    public void EndShowSwapItemMenuStateToHandleTileState(PlayerInfo currentPlayerInfo) {
        List<Node> nearbyPlayerNodes = NodeUtils.getNearbyPlayerNodes(sharedResourceBus.GetClickedNode(), sharedResourceBus.GetNodeDict());
        StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
        foreach (Node n in nearbyPlayerNodes) {
            if (NodeUtils.nodeClickedIsPlayer(n)) {
                //tileMap.SetColor(n.getPosition(), n.getOriginalColor());
                staticTileHandler.DespawnTile(n.getPosition());
            }
        }
        //playerTurnEndedDict[currentPlayerInfo.getPlayerId()] = true;
        sharedResourceBus.SetHighlightedPossibleSwapPartner(false);
        ChangeState(GameState.HandleTileState);
    }
}
