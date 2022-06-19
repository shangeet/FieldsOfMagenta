using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerAnimatorInterface {

    IEnumerator AttackEnemy(GameObject defenderPlayer, Vector3 defenderPos, bool isAttacker);

}
