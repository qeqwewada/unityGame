using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFSM
{
    public class MoveState :StateBase 
    {
        public MoveState(Player kalie) : base(kalie)
        {

        }

        public override void Enter()
        {
            kalie.playerParameter.animator.CrossFadeInFixedTime("Move", 0.15f);
        }

        
        public override void Exit()
        {
            
        }


        public override void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
       

            if (h == 0 && v == 0 )
            {
                kalie.fSM.ChangeState(StateType.Idle);
                return;
            }
            if (h != 0 || v != 0 )
            {
                Vector3 input;
                
                    
                    input = new Vector3(h, 0, v);
              
               
                float y = Camera.main.transform.rotation.eulerAngles.y;
                
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                kalie.transform.rotation = Quaternion.Slerp(kalie.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * kalie.playerParameter.rotateSpeed);
                kalie.playerParameter.characterController.Move(targetDir.normalized * (kalie.playerParameter.walkSpeed * Time.deltaTime));
                kalie.playerParameter.characterController.Move(Vector3.up * kalie.playerParameter.gravity * Time.deltaTime);
            }
        }
    }
}