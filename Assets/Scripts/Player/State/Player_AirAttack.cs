using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AirAttack : PlayerStateBase
{
    private enum AirAttackChild
    {
        //Loop,
        //End
        Start,
        Loop,
        End,
    }

    private AirAttackChild attackChild;

    private AirAttackChild AttackChild {
        get => attackChild;

        set { attackChild = value;
            switch (attackChild)
            {
                case AirAttackChild.Start:
                    player.setfallAttack();
                    AmendAttack.instance.AmendAttackMethod();
                    player.Model.Animator.CrossFade("SwordFallingAttackStart", 0.1f);
                    break;
                case AirAttackChild.Loop:
                    player.Model.Animator.CrossFade("SwordFallingAttackLoop", 0f);
                    break;
                case AirAttackChild.End:
                    player.Model.Animator.CrossFade("SwordFallingAttackEnd", 0f);
                    break;
            }
        }
    }

    public override void Enter()
    {
        player.SwitchWeaponParentObject(0);
        AttackChild = AirAttackChild.Start;
    }

    public override void Update()
    {
        if(CheckAnimatorStateName("SwordFallingAttackStart",out float time))
        {
            if (time > 0.6)
            {   
                AttackChild = AirAttackChild.Loop;
            }
        }
        else if(CheckAnimatorStateName("SwordFallingAttackLoop", out float time1))
        {
            if (player.CharacterController.isGrounded)
            {
                AttackChild = AirAttackChild.End;
            }
        }
        if(CheckAnimatorStateName("SwordFallingAttackEnd", out float time2))
        {
            if (time2 > 0.6)
            {
                player.PlayAnimation("Unequip");
                player.ChangeState(PlayerState.Idle);
                
            }
        }
    }
}
