using UnityEngine;

public class Player_IdleState : PlayerStateBase
{
    public override void Enter()
    {
        // 播放角色待机动画
        Debug.Log("进入IDle");
        player.PlayAnimation("Idle");
    }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float H = player.move.Horizontal;
        float V = player.move.Vertical;
        if (h != 0 || v != 0 || H != 0 || V != 0)
        {
            // 切换到移动状态
            player.ChangeState(PlayerState.Move);
            return;
        }
        // 检测攻击
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }

        // 检测防御
        if (Input.GetMouseButton(1))  // 使用GetMouseButton而不是GetMouseButtonDown来实现持续防御
        {
            player.ChangeState(PlayerState.Defense);
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            player.WalkModel = !player.WalkModel;
        }
            // 检测跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 切换到移动状态
            /*moveStatePower = 0;*/
            player.ChangeState(PlayerState.AirDown);
            return;
        }

        // 检测翻滚
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveStatePower = 0;
            player.ChangeState(PlayerState.Roll);
            return;
        }

        // 检测玩家移动
        if (!player.CharacterController.isGrounded)
        {
            player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        }

        // 检测下落
        //if (player.CharacterController.isGrounded == false)
        //{
        //    player.ChangeState(PlayerState.AirDown);
        //    return;
        //}


     

        // 如果锁定了目标，保持朝向目标
        LockOnSystem lockOnSystem = player.GetComponent<LockOnSystem>();
        if (lockOnSystem != null && lockOnSystem.currentTarget != null)
        {
            Vector3 targetDirection = lockOnSystem.currentTarget.position - player.transform.position;
            targetDirection.y = 0;
            player.transform.rotation = Quaternion.Lerp(
                player.transform.rotation,
                Quaternion.LookRotation(targetDirection),
                player.rotateSpeed * Time.deltaTime
            );
        }
    }
}