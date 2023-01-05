using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharactersStats charactersStats;

    //攻击目标
    private GameObject attackTarget;

    //攻击冷却时间
    private float lastAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        charactersStats = GetComponent<CharactersStats>();
    }

    private void Start()
    {
        //将MoveToTarget这个方法添加进OnMouseClicked中
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }

    private void Update()
    {
        SwitchAnimation();

        //冷却时间缩减
        lastAttackTime -= Time.deltaTime;
    }

    //切换动画
    private void SwitchAnimation()
    {
        //agent.velocity.sqrMagnitude可以获得速度的浮点类型
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }

    //移动到点击对象，用来添加进OnMouseClicked的事件，因为事件创建时带有Vector3，所以这里要格式一致
    public void MoveToTarget(Vector3 target)
    {
        //终止所有协程以执行当前指令
        StopAllCoroutines();

        //还原到可以移动的状态
        agent.isStopped = false;

        agent.destination = target;
    }

    //攻击事件，用来添加进OnMouseClicked的事件，因为事件创建时带有GameObject，所以这里要格式一致
    private void EventAttack(GameObject target)
    {
        //攻击目标不为空时执行
        if(target != null)
        {
            attackTarget = target;

            //执行协程
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //协程
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        transform.LookAt(attackTarget.transform);

        //与攻击对象的距离大于攻击距离
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > charactersStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            //return null代表在下一帧再次执行上面的命令
            yield return null;
        }

        //到达目标，停止移动
        agent.isStopped = true;

        //跳出循环，执行攻击
        //攻击CD结束
        if(lastAttackTime < 0)
        {
            //播放攻击动画
            anim.SetTrigger("Attack");

            //重置冷却时间
            lastAttackTime = 0.5f;
        }
    }
}
