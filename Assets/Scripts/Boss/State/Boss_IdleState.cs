using UnityEngine;

public class Boss_IdleState : BossStateBase
{
    //private float currentTime;
    public override void Enter()
    {
        //currentTime=0f;
        boss.PlayAnimation("moveTree");
    }

   
    public override void Update()
    {
        //currentTime += Time.deltaTime;
        //Debug.Log(currentTime);
        //boss.CharacterController.Move(new Vector3(0, boss.gravity * Time.deltaTime, 0));

        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        //if (currentTime > boss.idleTime)
        //{
        //    boss.ChangeState(BossState.Patrol);
        //    return;
        //}
        if (distance < boss.walkRange)
        {
            boss.ChangeState(BossState.Walk);
            return;
        }
        else
        {
            boss.ChangeState(BossState.Run);
            return;
        }

        // 检查是否需要进入防御状态
        if (boss.ShouldEnterDefense())
        {
            boss.ChangeState(BossState.Defense);
            return;
        }
    }

    //public override void Exit()
    //{
    //    currentTime = 0f;
    //}
}