using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    private string id;
    private int health;
    private int mov;
    private string textureRefPath;
    private bool isEnemy;
    
   public PlayerInfo(string id, string textureRefPath, bool isEnemy, int mov) {
       this.id = id;
       this.textureRefPath = textureRefPath;
       this.isEnemy = isEnemy;
       this.mov = mov;
       this.health = 50;
   }

    public string getPlayerId() {
        return this.id;
    }

    public string getTextureRefPath() {
        return this.textureRefPath;
    }

    public bool getIsEnemy() {
        return this.isEnemy;
    }

    public int getMov() {
        return this.mov;
    }

    public int getHealth() {
        return this.health;
    }
   
}
