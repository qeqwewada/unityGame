using System;
using UnityEngine;

public class Player_RunState : PlayerStateBase
{
    private float turnTime = 0;
    private enum RunChildState
    {
        Run,
        Stop,
        TurnBack
    }

    private RunChildState childState;

    private RunChildState ChildState 
    { 
        get => childState; 
        set
        {
            childState = value;
            switch (childState)
            {
                case RunChildState.Run:
                    player.Model.Animator.CrossFade("Run", 0.15f);
                    break;
                case RunChildState.Stop:
                    if(player.Model.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime<0.5f)
                    player.Model.Animator.CrossFade("Stop1", 0.1f);
                    else
                    {
                        player.Model.Animator.CrossFade("Stop2", 0.1f);
                    }
                    break;
                case RunChildState.TurnBack:
                    player.Model.Animator.CrossFade("Turn", 0f);
                    break;
            }
        }
    }

    public override void Enter()
    {
        ChildState = RunChildState.Run;
        player.Model.SetRooMotionAction(OnRootMotion);
    }

    public override void Update()
    {
       
        // 检测攻击
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }
        // 检测翻滚
        if (Input.GetMouseButtonDown(1))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        //检测跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 切换到移动状态
            /*moveStatePower =  2;*/
            player.ChangeState(PlayerState.AirDown);
            return;
        }
        //检测下落
        if (player.CharacterController.isGrounded == false)
        {
            moveStatePower =  2;
            player.ChangeState(PlayerState.AirDown);
            return;
        }
        switch (childState)
        {
            case RunChildState.Run:
                RunOnUpdate();
                break;
            case RunChildState.Stop:
                StopOnUpdate();
                break;
            case RunChildState.TurnBack:
                TurnOnUpdate();
                break;
        }
    }

    private void TurnOnUpdate()
    {
        if (CheckAnimatorStateName("Turn", out float aniamtionTime))
        {
          
            player.transform.rotation = player.Model.Animator.deltaRotation * player.transform.rotation;
            if (aniamtionTime >= 0.75)
            {
                
                 ChildState = RunChildState.Run;
                       
            }
                
        }
    }

    private void StopOnUpdate()
    {
        bool temp = CheckAnimatorStateName("Stop1", out float aniamtionTime) ;
        bool temp2 = CheckAnimatorStateName("Stop2", out float aniamtionTime1);
        if (temp||temp2)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (h != 0 || v != 0 )
            {
               
                turnTime = 0;
                player.ChangeState(PlayerState.Move);
                
                return;
            }
          
            if (aniamtionTime > 0.95)
            {
                Debug.Log("qwqwq");
                player.ChangeState(PlayerState.Idle);
               
            }
            else if (aniamtionTime1 > 0.95)
            {
                Debug.Log("qwq");
                player.ChangeState(PlayerState.Idle);
            }
        }

    }

    private void RunOnUpdate()
    {
        // h和v用来做实际的移动参考以及判断是否去待机
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // rawH和rawV用来判断急停
        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");
   
        if (rawH == 0&& rawV == 0)
        {
            turnTime += Time.deltaTime;
            if (turnTime > 0.3f)
            {
                ChildState = RunChildState.Stop;
                turnTime = 0;
                return;
            }
        }
        else
        {
            if (h != 0 || v != 0)
            {

                // 处理旋转的问题
                Vector3 input = new Vector3(h, 0, v);
                // 获取相机的旋转值 y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                float turnBack = Vector3.Angle(targetDir, player.transform.forward);

                if (turnBack > 120)
                {
                    ChildState = RunChildState.TurnBack;
                }
                player.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
            }
        }



    }

    public override void Exit()
    {
      
        player.Model.ClearRootMotionAction();
        player.Model.Animator.speed = 1;
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
        

    }
}
