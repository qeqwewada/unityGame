using UnityEngine;

public class Player_DefenseState : PlayerStateBase
{
    private LockOnSystem lockOnSystem;
    private bool isBlocking = false; // 是否正在播放格挡动画

    public override void Enter()
    {
        // 播放防御动画
        player.PlayAnimation("Defense");
        
        // 获取锁定系统组件
        lockOnSystem = player.GetComponent<LockOnSystem>();
        if (lockOnSystem != null && lockOnSystem.currentTarget == null)
        {
            // 进入防御状态时自动寻找目标
            lockOnSystem.FindAndLockTarget();
        }
        player.SwitchWeaponParentObject(0);
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
                    player.PlayAnimation("Defense");
                }
                return;
            }
        }

        // 如果松开鼠标右键，退出防御状态
        if (!Input.GetMouseButton(1))
        {
            if (lockOnSystem != null)
            {
                lockOnSystem.UnlockTarget();
            }
            player.ChangeState(PlayerState.Idle);
            return;
        }

        // 在防御状态下移动
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (h != 0 || v != 0)
        {
            Vector3 movement;
            
            // 如果有锁定目标，使用目标相对方向移动和动画
            if (lockOnSystem != null && lockOnSystem.currentTarget != null)
            {
                // 设置动画混合树参数
                player.Model.Animator.SetFloat("x", h, 0.1f, Time.deltaTime);
                player.Model.Animator.SetFloat("y", v, 0.1f, Time.deltaTime);

                Vector3 targetDirection = lockOnSystem.currentTarget.position - player.transform.position;
                targetDirection.y = 0;
                player.transform.rotation = Quaternion.Lerp(
                    player.transform.rotation,
                    Quaternion.LookRotation(targetDirection),
                    player.rotateSpeed * Time.deltaTime
                );

                // 相对于锁定目标的方向移动
                movement = new Vector3(h, 0, v);
                movement = Quaternion.Euler(0, player.transform.eulerAngles.y, 0) * movement;
            }
            else
            {
                // 没有目标时，始终设置y值为1表示向前移动
                player.Model.Animator.SetFloat("x", 0, 0.1f, Time.deltaTime);
                player.Model.Animator.SetFloat("y", 1, 0.1f, Time.deltaTime);

                // 使用相机方向作为移动方向
                movement = new Vector3(h, 0, v);
                movement = Camera.main.transform.TransformDirection(movement);
                movement.y = 0;

                // 让角色朝向移动方向
                if (movement != Vector3.zero)
                {
                    player.transform.rotation = Quaternion.Lerp(
                        player.transform.rotation,
                        Quaternion.LookRotation(movement),
                        player.rotateSpeed * Time.deltaTime
                    );
                }
            }

            movement.Normalize();
            // 移动速度降为正常速度的一半
            player.CharacterController.Move(movement * player.walkSpeed * 0.5f * Time.deltaTime);
        }
        else
        {
            // 没有移动输入时，将动画参数逐渐归零
            player.Model.Animator.SetFloat("x", 0, 0.1f, Time.deltaTime);
            player.Model.Animator.SetFloat("y", 0, 0.1f, Time.deltaTime);
        }

        // 处理重力
        if (!player.CharacterController.isGrounded)
        {
            player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        }
    }

    // 在Player_Controller中的Hurt方法中调用这个方法
    public void OnBlock()
    {
        if (!isBlocking)
        {
            isBlocking = true;
            player.PlayAnimation("DefenseBlock");
        }
    }

    public override void Exit()
    {
        player.Model.Animator.SetFloat("x", 0);
        player.Model.Animator.SetFloat("y", 0);
        player.SwitchWeaponParentObject(1);
    }
} 