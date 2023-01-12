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

        //��һ�����٣���ֹ����Ĭ��Ϊ��ֹ״̬
        rb.velocity = Vector3.one;

        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        //�ٶ�С��1����Ϊʯͷ��ֹ
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

        //����ģʽΪ�����
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

                    //����ǰ��������Ч��
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
