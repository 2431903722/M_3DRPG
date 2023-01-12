using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    public RockStates rockStates;

    public enum RockStates { HitPlayer, HitEnemy, HitNoting }

    [Header("Basic Settings")]
    public float force;
    public GameObject target;
    private Vector3 direction;
    public int damage;

    public GameObject breakEffect;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //给一个初速，防止将其默认为静止状态
        rb.velocity = Vector3.one;

        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        //速度小于1则认为石头静止
        if(rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNoting;
        }
    }

    public void FlyToTarget()
    {
        if(target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
      
        direction = target.transform.position - transform.position + Vector3.up;
        direction.Normalize();

        //力的模式为冲击力
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharactersStats>().TakeDamage(damage, other.gameObject.GetComponent<CharactersStats>());

                    rockStates = RockStates.HitNoting;
                }
                break;

            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharactersStats>();
                    otherStats.TakeDamage(damage, otherStats);

                    //销毁前生产破碎效果
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
