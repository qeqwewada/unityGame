using System;
using UnityEngine;

public class Player_StandAttackState : PlayerStateBase
{
    private enum AttackChildState
    {
        Attack,
        end
    }
    //当前时第几次攻击
    private int currentAttackIndex;

    private int CurrentAttackIndex
    {
        get => currentAttackIndex;
        set
        {
            if (value >= player.standAttackConfigs.Length) currentAttackIndex = 0;
            else currentAttackIndex = value;
        }
    }
    private AttackChildState attackState;
    private AttackChildState AttackState
    {
        get => attackState;
        set
        {
            attackState = value;
            // 状态进入
            switch (attackState)
            {
                case AttackChildState.Attack:
                    StandAttack();
                    break;
                case AttackChildState.end:
                    player.PlayAnimation(player.CurrentSkillConfig.AttackEnd);
                    break;

            }
        }
    }

    public override void Enter()
    {

        // 注册根运动
        player.Model.SetRooMotionAction(OnRootMotion);
        CurrentAttackIndex = -1;
        // 播放技能
        //StandAttack();
        AttackState = AttackChildState.Attack;
    }

    public override void Exit()
    {
        player.OnSkillOver();
    }

    private void StandAttack()
    {
        player.SwitchWeaponParentObject(0);
        CurrentAttackIndex += 1;
        player.StartAttack(player.standAttackConfigs[CurrentAttackIndex]);
        AmendAttack.instance.AmendAttackMethod();
    }

    public override void Update()
    {
        // 待机检测
        //if (CheckAnimatorStateName(player.standAttackConfigs[CurrentAttackIndex].AnimationName, out float aniamtionTime) && aniamtionTime >= 1)
        //{
        //    //Debug.Log("end");
        //    //player.PlayAnimationt(player.CurrentSkillConfig.AttackEnd);
        //    //AnimatorStateInfo stateInfo = player.Model.Animator.GetCurrentAnimatorStateInfo(0);
        //    //string name = stateInfo.IsName("Attack_01") ? "0" : "Unknown";
        //    //Debug.Log(name);
        //    //// 回到待机 
        //    if (CheckAnimatorStateName(player.standAttackConfigs[CurrentAttackIndex].AttackEnd, out float time))
        //    {
        //        Debug.Log(time);
        //        if (time >= 1) 
        //        {
        //            player.ChangeState(PlayerState.Idle);
        //            return;
        //        }


        //    }

        if (player.CurrentSkillConfig.ReleaseData.CanRotate && CheckAnimatorStateName(player.standAttackConfigs[CurrentAttackIndex].AnimationName, out float aniamtionTime) && aniamtionTime < 0.3)
        {
            if (AmendAttack.instance.GetNearestMonster() == null)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                if (h != 0 || v != 0)
                {
                    // 处理旋转的问题
                    Vector3 input = new Vector3(h, 0, v);
                    // 获取相机的旋转值 y
                    float y = Camera.main.transform.rotation.eulerAngles.y;
                    // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
                    Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                    player.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeedForAttack);
                }
            }
        }
        //}
        switch (attackState)
        {
            case AttackChildState.Attack:
                AttackOnUpdate();
                break;
            case AttackChildState.end:
                EndOnUpdate();
                break;
        }
        //// 攻击检测
        //if (CheckStandAttack())
        //{
        //    StandAttack();
        //    return;
        //}

        // 旋转逻辑
        //if (player.CurrentSkillConfig.ReleaseData.CanRotate/*&& player.CanSwitchSkill*/)
        //{
        //    float h = Input.GetAxis("Horizontal");
        //    float v = Input.GetAxis("Vertical");
        //    if (h != 0 || v != 0)
        //    {
        //        // 处理旋转的问题
        //        Vector3 input = new Vector3(h, 0, v);
        //        // 获取相机的旋转值 y
        //        float y = Camera.main.transform.rotation.eulerAngles.y;
        //        // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
        //        Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
        //        player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeedForAttack);
        //    }
        //}

        if (player.CanMove)
        {
            // 检测跳跃
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 切换到移动状态
                moveStatePower = 0;
               player.ChangeState(PlayerState.AirDown);
               return;
            }
            // 检测翻滚
            if (Input.GetMouseButtonDown(1))
            {
                moveStatePower = 0;
                player.ChangeState(PlayerState.Roll);
                return;
            }
        }
    }

    private void EndOnUpdate()
    {

        if (CheckAnimatorStateName(player.standAttackConfigs[CurrentAttackIndex].AttackEnd, out float aniamtionTime) && aniamtionTime >= 1)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }
    }

    private void AttackOnUpdate()
    {
        if (CheckAnimatorStateName(player.standAttackConfigs[CurrentAttackIndex].AnimationName, out float aniamtionTime) && aniamtionTime >= 1)
        {
            player.ChangeState(PlayerState.Idle);
            player.PlayAnimation("Unequip");
        }
        if (CheckStandAttack())
        {
            StandAttack();
            return;
        }
        else if (player.CanMove)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (h != 0 || v != 0)
            {
                player.ChangeState(PlayerState.Move);
                player.PlayAnimation("Unequip");
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(PlayerState.AirDown);
            player.PlayAnimation("Unequip");
        }
        if (Input.GetMouseButtonDown(1))
        {
            player.ChangeState(PlayerState.Roll);
            player.PlayAnimation("Unequip");
        }
    }

    public bool CheckStandAttack()
    {
        return Input.GetMouseButtonDown(0) && player.CanSwitchSkill;
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}