using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerPlayerAnimator : PlayerAnimator
{
    public override IEnumerator AttackEnemy(GameObject defenderPlayer, Vector3 defenderPos, bool isAttacker) {
        Vector3 attackerOriginalPos = transform.position;
        //move close to enemy position
        Vector3 targetPosition;
        if (isAttacker) {
            targetPosition = new Vector3(defenderPos.x - 0.5f, defenderPos.y, defenderPos.z);
        } else {
            targetPosition = new Vector3(defenderPos.x + 0.5f, defenderPos.y, defenderPos.z);
        }
        print("Moving to target position");
        animator.SetBool("IsMove", true);
        float time = 0.0f;
        float step = 0.5f;
        float totalTime = 5.0f;
        while (time != totalTime) {
            transform.position = Vector3.Lerp(transform.position, targetPosition, time/totalTime);
            time += step;
            yield return new WaitForSeconds(0.01f);
        }
        print("Attacking...");
        animator.SetBool("IsMove",false);
        animator.SetBool("IsAttack", true);
        defenderPlayer.GetComponent<PlayerAnimator>().animator.SetBool("IsTakeDamage", true);
        yield return new WaitForSeconds(0.5f);
        print("Moving back");
        animator.SetBool("IsAttack", false);
        defenderPlayer.GetComponent<PlayerAnimator>().animator.SetBool("IsTakeDamage", false);
        yield return new WaitForSeconds(0.8f);
        spriteRenderer.flipX = !spriteRenderer.flipX;
        animator.SetBool("IsMove", true);
        time = 0.0f;
        step = 0.5f;
        totalTime = 5.0f;
        while (time != totalTime) {
            transform.position = Vector3.Lerp(transform.position, attackerOriginalPos, time/totalTime);
            time += step;
            yield return new WaitForSeconds(0.01f);
        }
        spriteRenderer.flipX = !spriteRenderer.flipX;
        ResetToIdleAnimation();        
    }
}
