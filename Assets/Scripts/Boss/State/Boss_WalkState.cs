using System.Diagnostics;
using UnityEngine;
using System.Collections;
public class Boss_WalkState : BossStateBase
{
    private bool isVigilant;
    private float currentTime;
    Vector3 playerPos ;
    private enum ChildWalkState
    {
        walkL,
        walkR
    }
    private ChildWalkState childState;
    private ChildWalkState ChildState
    {
        get => childState; 
        set
        {
            childState = value;
            switch (childState)
            {
                case ChildWalkState.walkL:
                    boss.PlayAnimation("walkL");
                    break;
                case ChildWalkState.walkR:
                    boss.PlayAnimation("walkR");
                    break;
            }
        }
    }

    public override void Enter()
    {
        
        ChildState = ChildWalkState.walkL;
        boss.navMeshAgent.enabled = false;
        boss.Model.SetRooMotionAction(OnRootMotion);
      /*  if (boss.anger) isVigilant = false;
        else isVigilant = Random.Range(0, 2) >= 1;
        if (isVigilant)
        {
            *//*boss.navMeshAgent.updateRotation = true;
            boss.navMeshAgent.speed = boss.vigilantSpeed;*//*
            stopVigilantCoroutine = MonoManager.Instance.StartCoroutine(StopVigilant());
        }*/
        /*else boss.navMeshAgent.speed = boss.walkSpeed;*/
    }


    
public override void Update()
{
        if (boss.targetPlayer == null) return;
        playerPos = boss.targetPlayer.transform.position;
        boss.transform.LookAt(new Vector3(playerPos.x, boss.transform.position.y, playerPos.z));
        /*    Vector3 direction = boss.targetPlayer.transform.position - boss.transform.position;
            direction.y = 0;
            boss.transform.rotation = Quaternion.LookRotation(direction);*/
        currentTime += Time.deltaTime;
    if(CheckAnimatorStateName("walkL",out float time1)&&currentTime>boss.walkTime)
        {
            ChildState = ChildWalkState.walkR;
            currentTime = 0;
        }
        if (CheckAnimatorStateName("walkR", out float time2) && currentTime > boss.walkTime)
        {
            ChildState = ChildWalkState.walkR;
            currentTime = 0;
            if (Vector3.Distance(playerPos, boss.transform.position) < boss.attackRange)
            {
                boss.ChangeState(BossState.Attack);
                return;
            }
            else
            {
                boss.ChangeState(BossState.Run);
            }
        }

    // 检查是否需要进入防御状态
    if (boss.ShouldEnterDefense())
    {
        boss.ChangeState(BossState.Defense);
        return;
    }

      
        //    if (distance <= boss.standAttackRange)
        //{
        //    boss.ChangeState(BossState.Attack);
        //}
        //else if (distance > boss.walkRange)
        //{
        //    boss.ChangeState(BossState.Run);
        //}
        //else
        //{
        //    boss.navMeshAgent.SetDestination(boss.targetPlayer.transform.position);
        //}
        //if (distance > boss.walkRange)
        //{
        //    boss.ChangeState(BossState.Run);
        //    return;
        //}
        
     /*   
        if (isVigilant) // 朝向玩家，但是保持一个距离
        {
        
        Vector3 targetPos = (boss.transform.position - playerPos).normalized * boss.vigilantRange + playerPos;
        if (Vector3.Distance(playerPos, boss.transform.position) == 2.5f)
        {
                
      
                
        }
        else if(Vector3.Distance(playerPos, boss.transform.position) > 2.5f)
        {
               
                boss.Model.Animator.SetFloat("x", 0);
                boss.Model.Animator.SetFloat("y", 1);
                //boss.navMeshAgent.enabled = true;
                *//*boss.navMeshAgent.SetDestination(targetPos);*//*
        }
        else
            {
                UnityEngine.Debug.Log("2");
                if (changeTime > 2f)
                {
                    UnityEngine.Debug.Log("3");
                    boss.Model.Animator.SetFloat("x", Random.Range(-1, 2));
                    boss.Model.Animator.SetFloat("y", 0);
                    changeTime = 0;
                }
                if(Vector3.Distance(playerPos, boss.transform.position)<1.5)
                {
                    boss.Model.Animator.SetFloat("x", 0);
                    boss.Model.Animator.SetFloat("y", -1);
                }
            }
    }
        else // 常规追玩家的逻辑，追到就攻击
        {
            if (distance <= boss.attackRange)
            {
                
                boss.ChangeState(BossState.Attack);
            }
            else
            {
                UnityEngine.Debug.Log("2");
                boss.Model.Animator.SetFloat("x", 0);
                boss.Model.Animator.SetFloat("y", 1);
            }
        }*/
        //if (distance <= boss.attackRange)
        //{
        //    boss.ChangeState(BossState.Attack);
        //    return;
        //}
        //else
        //{
        //    boss.navMeshAgent.SetDestination(boss.targetPlayer.transform.position);

        //}
        //else // 常规追玩家的逻辑，追到就攻击
        //{

        //}
    }


    public override void Exit()
    {
        boss.navMeshAgent.enabled = false;
/*        if (stopVigilantCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(stopVigilantCoroutine);
            stopVigilantCoroutine = null;
        }*/
        boss.Model.ClearRootMotionAction();
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = boss.gravity * Time.deltaTime;
        boss.CharacterController.Move(deltaPosition);
    }
}