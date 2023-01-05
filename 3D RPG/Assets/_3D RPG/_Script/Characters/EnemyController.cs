﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//枚举变量，用于敌人状态机
public enum EnmeyStates { GUARD, PATROL, CHASE , DEAD }

//当脚本所挂载的物体上没有对应组件时则自动添加相应组件
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnmeyStates enmeyState;
    private Animator anim;
    private CharactersStats charactersStats;

    [Header("Basic Settings")]
    //敌人可视范围
    public float sightRadius;
    //攻击对象
    private GameObject attackTarget;
    //敌人属性
    //动画bool
    public bool isGuard;
    bool isWalk;
    bool isChase;
    bool isFollow;
    //一般状态下的速度
    private float originalSpeed;
    //在某个点等待的时间
    public float lookAtTime;
    private float remainLookAtTime;
    //攻击计时器
    private float lastAttackTime;

    [Header("Patrol State")]
    //巡逻范围
    public float patrolRange;
    //巡逻点
    private Vector3 wayPoint;
    //在某个点范围内巡逻
    private Vector3 guardPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        charactersStats = GetComponent<CharactersStats>();
        originalSpeed = agent.speed;
        guardPosition = transform.position;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        //根据敌人类型设置状态
        if (isGuard)
        {
            enmeyState = EnmeyStates.GUARD;
        }
        else
        {
            enmeyState = EnmeyStates.PATROL;
            //提供初始巡逻点
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        SwitchStates();
        SwitchAnimation();
        
        //攻击计时器计时
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", charactersStats.isCritical);
    }

    //切换敌人行动状态
    void SwitchStates()
    {
        //如果发现Player，切换到CHASE
        if (FoundPlayer())
        {
            enmeyState = EnmeyStates.CHASE;
        }

        switch (enmeyState)
        {
            case EnmeyStates.GUARD:
                break;
            case EnmeyStates.PATROL:
                Patrol();
                break;
            case EnmeyStates.CHASE:
                Chase();
                break;
            case EnmeyStates.DEAD:
                break;
        }
    }

    //判断敌人是否找到Player
    bool FoundPlayer()
    {
        //用OverlapSphere来获取球形范围内所有的collider
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        //对于每个collider和Player做对比，找到就返回true,并给attackTarget赋值
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                attackTarget = collider.gameObject;
                return true;
            }
        }
        //脱离范围
        attackTarget = null;
        return false;
    }

    //CHASE状态行为
    void Chase()
    {
        //改变动画状态
        isWalk = false;
        isChase = true;

        //追击时改变速度
        agent.speed = originalSpeed;

        //玩家脱离视野
        if (!FoundPlayer())
        {
            //停留在当前位置
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }
            //如果是守卫状态的敌人则继续守卫
            else if (isGuard)
            {
                enmeyState = EnmeyStates.GUARD;
            }
            //巡逻状态的敌人继续巡逻
            else
            {
                enmeyState = EnmeyStates.PATROL;
            }
        }
        else
        {
            //将目标点设为attackTarget
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
        }

        //目标处于攻击范围内
        if (TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow = false;
            agent.isStopped = true;

            //攻击冷却完成
            if (lastAttackTime <= 0)
            {
                //重置攻击计时器
                lastAttackTime = charactersStats.attackData.coolDown;

                //暴击判断
                charactersStats.isCritical = Random.value < charactersStats.attackData.criticalChance;

                //攻击
                Attack();
            }
        }
    }

    //攻击
    void Attack()
    {
        //转向攻击目标
        transform.LookAt(attackTarget.transform);
        
        if (TargetInAttackRange())
        {
            //攻击
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //释放技能
            anim.SetTrigger("Skill");
        }

    }

    //目标是否在攻击范围内
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= charactersStats.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }
    
    //目标是否在技能范围内
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= charactersStats.attackData.skillRange;
            
        }
        else
        {
            return false;
        }
    }

    //PATROL状态行为
    void Patrol()
    {
        isChase = false;

        //巡逻时速度减半
        agent.speed = originalSpeed * 0.5f;

        //判断是否走到随机点
        if(Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)
        {
            isWalk = false;

            //执行等待
            if (remainLookAtTime > 0)
            {
                remainLookAtTime -= Time.deltaTime;
            }
            else
            {
                GetNewWayPoint();
            }
        }
        else
        {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }

    //随机获取一个新的路线点
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ= Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPosition.x + randomX, transform.position.y, guardPosition.z + randomZ);

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1))
        {
            wayPoint = hit.position;
        }
        else
        {
            wayPoint = transform.position;
        }

        wayPoint = randomPoint;
    }

    //在场景中绘制一些图形以供判断怪物视野
    private void OnDrawGizmosSelected()
    {
        //设置颜色
        Gizmos.color = Color.blue;
        //画一个线球
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
