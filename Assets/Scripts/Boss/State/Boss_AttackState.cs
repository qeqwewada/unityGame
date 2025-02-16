using UnityEngine;

public class Boss_AttackState : BossStateBase
{
    // 当前时第几次攻击
    private int currentAttackIndex;
    private float currentAttackTime;
/*    private int CurrentAttackIndex
    {
        get => currentAttackIndex;
        set
        {
            if (value >= boss.standAttackConfigs.Length) currentAttackIndex = 0;
            else currentAttackIndex = value;
        }
    }*/

    public override void Enter()
    {
        currentAttackTime = 0f;
        boss.Model.SetRooMotionAction(OnRootMotion);
        currentAttackIndex = -1;
        // 播放技能

        StandAttack();

    }

    public override void Exit()
    {
        boss.Model.ClearRootMotionAction();
        boss.OnSkillOver();
    }

    private void StandAttack()
    {
        currentAttackIndex += 1;
        // 注册根运动
        boss.transform.LookAt(new Vector3(boss.targetPlayer.transform.position.x, boss.transform.position.y, boss.targetPlayer.transform.position.z));
        boss.StartAttack(boss.standAttackConfigs[currentAttackIndex]);
       
        
    }

    public override void Update()
    {
        currentAttackTime += Time.deltaTime;
        //待机检测
        //if (CheckAnimatorStateName(boss.standAttackConfigs[CurrentAttackIndex].AnimationName, out float aniamtionTime) && aniamtionTime >= 1)
        //{
        //    // 回到待机
        //    boss.ChangeState(BossState.Idle);
        //    return;
        //}

        if (boss.CanSwitchSkill)
        {
            if (currentAttackIndex>= boss.standAttackConfigs.Length-1)
            {
                
                boss.ChangeState(BossState.Walk);
                return;
                
            }
            else
            {
                /*Debug.Log(currentAttackIndex);*/
                StandAttack();
                return;
            }
        }
        //else
        //{
        //    Debug.Log(2);
        //    boss.ChangeState(BossState.Patrol);
        //    return;
        //}
    }


    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = boss.gravity * Time.deltaTime;
        boss.CharacterController.Move(deltaPosition);
    }
}