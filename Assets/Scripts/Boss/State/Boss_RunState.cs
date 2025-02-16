using System.Diagnostics;
using UnityEngine;

public class Boss_RunState : BossStateBase
{
    Vector3 playerPos;
    private enum RunChildState
    {
        Run,
        SPAttack1
    }
    private RunChildState childState;

    private RunChildState ChildState { get => childState; 
        set {
            childState = value;
            switch (childState)
            {
                case RunChildState.Run:
                    boss.PlayAnimation("Run");
                    break;
                case RunChildState.SPAttack1:
                    boss.navMeshAgent.enabled = false;
                    boss.transform.LookAt(new Vector3(playerPos.x, boss.transform.position.y, playerPos.z));
                    boss.Model.SetRooMotionAction(OnRootMotion);
                    boss.SetSPAttack();
                    boss.PlayAnimation("SPAttack1");
                    break;
            }
        }
    }

    public override void Enter()
    {
        playerPos = boss.targetPlayer.transform.position;
        ChildState = RunChildState.Run;
        boss.navMeshAgent.enabled = true;
        boss.navMeshAgent.speed = boss.runSpeed;
    }
    public override void Update()
    {
        switch (childState)
        {
            case RunChildState.Run:
                UpdateOnRun();
                break;
            case RunChildState.SPAttack1:
                UpdateOnAttack();
                break;
        }

    }

    private void UpdateOnRun()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        if (distance <= boss.SPAttackdistance)
        {
            ChildState = RunChildState.SPAttack1;
        }
        else
        {
            boss.navMeshAgent.SetDestination(boss.targetPlayer.transform.position);
        }
    }

    private void UpdateOnAttack()
    {
        
        if(CheckAnimatorStateName("SPAttack1",out float time))
        {
            if (time > 0.99)
            {
                boss.ChangeState(BossState.Attack);
            }
        }
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = boss.gravity * Time.deltaTime;
        boss.CharacterController.Move(deltaPosition);
    }
    public override void Exit()
    {
        boss.Model.ClearRootMotionAction();
        boss.navMeshAgent.enabled = false;
    }
}