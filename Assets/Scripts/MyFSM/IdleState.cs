using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFSM
{
    public  class IdleState : StateBase,IState
    {
        public IdleState(Player kalie) : base(kalie)
        {
            
        }


       
        public override void Enter()
        {
            kalie.playerParameter.animator.CrossFadeInFixedTime("Idle", 0.15f);
        }

        
        public override void Exit()
        {

        }

        public override void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (h != 0 || v != 0 )
            {
                //看不到
                kalie.fSM.ChangeState(StateType.walk);
                return;
            }
        }
    }
}