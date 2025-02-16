using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_PatrolState : BossStateBase
{
    private float currentTime;
    public override void Enter()
    {
        boss.PlayAnimation("Run");
        boss.navMeshAgent.enabled = true;
        boss.navMeshAgent.speed = boss.walkSpeed;
        currentTime = 0f;
    }
    public override void Update()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        currentTime += Time.deltaTime;
        if (currentTime <Random.Range(3f,boss.patrolTime))
        {
            
             PatrolState();
        }
        else
        {
            if (distance < boss.walkRange)
            {
                boss.ChangeState(BossState.Walk);
                return;
            }
            boss.ChangeState(BossState.Idle);
            return;
        }
       
    }
    private void PatrolState()
    {
        Vector3 randomDirection = Random.insideUnitSphere * boss.patrolRadius;
        //将生成的随机方向向量randomDirection移动到怪物当前的位置
        randomDirection += boss.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, boss.patrolRadius, NavMesh.AllAreas);

        Vector3 targetPos = hit.position;
        boss.navMeshAgent.SetDestination(targetPos);
        Debug.Log("Patrol");
    }
    public override void Exit()
    {
        boss.navMeshAgent.enabled = false;
        currentTime = 0;
    }

}
