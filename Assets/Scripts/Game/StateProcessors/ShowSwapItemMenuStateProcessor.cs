using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSwapItemMenuStateProcessor : StateProcessor {
    
    public override bool Process() {
        SwapItemsMenu swapItemsMenu = uiHandler.GetSwapItemMenu();
        if (!swapItemsMenu.IsSwapItemMenuDisplayed()) {
            bool highlightedPossibleSwapPartner = sharedResourceBus.HighlightedPossibleSwapPartner();
            Node clickedNode = sharedResourceBus.GetClickedNode();
            Dictionary<Vector3Int, Node> nodeDict = sharedResourceBus.GetNodeDict();
            if (!highlightedPossibleSwapPartner) {
                //Look for valid partners
                if (clickedNode != null) {
                    List<Node> nearbyPlayerNodes = NodeUtils.GetAdjacentNodes(clickedNode, nodeDict, FindNodeFunctions.NEARBY_PLAYER);
                    if (nearbyPlayerNodes.Count == 0) { //no nodes exist. go back to battle state
                        ChangeState(GameState.ShowBattleMenuState);
                        return true;
                    } else {
                        foreach (Node n in nearbyPlayerNodes) {
                            if (NodeUtils.nodeClickedIsPlayer(n)) {
                                StaticTileHandler staticTileHandler = sharedResourceBus.GetStaticTileHandler();
                                staticTileHandler.SpawnTile(n.getPosition(), "YellowTile");
                            }
                        }  
                        sharedResourceBus.SetHighlightedPossibleSwapPartner(true); //toggle to true                      
                    }
                }
            } else { //highlighted. open menu if valid node click
                Node currentClickedNode = sharedResourceBus.GetCurrentClickedNode();
                if (currentClickedNode != null) {
                    List<Node> nearbyPlayerNodes = NodeUtils.GetAdjacentNodes(clickedNode, nodeDict, FindNodeFunctions.NEARBY_PLAYER);
                    if (NodeUtils.nodeClickedIsPlayer(currentClickedNode) && nearbyPlayerNodes.Contains(currentClickedNode)) {
                        swapItemsMenu.OpenSwapItemMenu(clickedNode.getPlayerInfo(), currentClickedNode.getPlayerInfo());
                    } else { //invalid node. go back to show battle menu state
                        endShowSwapItemMenuStateToShowBattleMenuState(nearbyPlayerNodes);
                        return true;
                    }                    
                }
            }
        }
        return false;        
    }

    private void endShowSwapItemMenuStateToShowBattleMenuState(List<Node> nearbyPlayerNodes) {
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

}
