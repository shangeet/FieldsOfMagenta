using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoDatabase {
    
    public static Sprite GetPlayerSpriteFromPath(string path) {
        return Resources.Load<Sprite>(path);
    }

}
