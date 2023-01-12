using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    
    public float kickForce = 25;
    public GameObject rockPrefab;
    public Transform handPos;

    //动画事件
    public void KickOff()
    {
        var targetStats = attackTarget.GetComponent<CharactersStats>();

        Vector3 direction = attackTarget.transform.position - transform.position;
        direction.Normalize();

        targetStats.GetComponent<NavMeshAgent>().isStopped = true;
        targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;

        targetStats.GetComponent<Animator>().SetTrigger("Dizzy");

        targetStats.TakeDamage(charactersStats, targetStats);
    }

    //动画事件
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
