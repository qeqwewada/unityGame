using UnityEngine;

public class Boss_DefenseState : BossStateBase
{
    private bool isBlocking = false; // 是否正在播放格挡动画
    private float blockTime = 0f;    // 格挡持续时间
    private float maxBlockTime = 3f;  // 最大格挡时间

    public override void Enter()
    {
        // 播放防御动画
        boss.PlayAnimation("Defense");
        blockTime = 0f;
        boss.Model.SetRooMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        // 如果正在播放格挡动画，检查动画是否播放完毕
        if (isBlocking)
        {
            if (CheckAnimatorStateName("DefenseBlock", out float normalizedTime))
            {
                if (normalizedTime >= 0.9f)
                {
                    isBlocking = false;
                    boss.PlayAnimation("Defense");
                }
                return;
            }
        }

        // 更新格挡时间
        blockTime += Time.deltaTime;
        if (blockTime >= maxBlockTime)
        {
            // 格挡时间结束，切换到其他状态
            boss.ChangeState(BossState.Walk);
            return;
        }

        // 始终面向玩家
        Vector3 targetDirection = boss.targetPlayer.transform.position - boss.transform.position;
        targetDirection.y = 0;
        boss.transform.rotation = Quaternion.Lerp(
            boss.transform.rotation,
            Quaternion.LookRotation(targetDirection),
            boss.rotateSpeed * Time.deltaTime
        );

        // 处理重力
        if (!boss.CharacterController.isGrounded)
        {
            boss.CharacterController.Move(new Vector3(0, boss.gravity * Time.deltaTime, 0));
        }
    }

    // 在Boss_Controller中的Hurt方法中调用这个方法
    public void OnBlock()
    {
        if (!isBlocking)
        {
            isBlocking = true;
            boss.PlayAnimation("DefenseBlock");
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
    }
} 