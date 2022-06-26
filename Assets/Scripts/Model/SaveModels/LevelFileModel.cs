using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelFileModel {

    public List<int> playerPlacementRangeX;
    public List<int> playerPlacementRangeY;
    public int maxActivePlayers;
    

    public LevelFileModel(List<int> playerPlacementRangeX, List<int> playerPlacementRangeY, int maxActivePlayers) {
        this.playerPlacementRangeX = playerPlacementRangeX;
        this.playerPlacementRangeY = playerPlacementRangeY;
        this.maxActivePlayers = maxActivePlayers;
    }

    public LevelFileModel() {}
     
}