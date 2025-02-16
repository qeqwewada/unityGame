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
       
        // ��⹥��
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }
        // ��ⷭ��
        if (Input.GetMouseButtonDown(1))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        //�����Ծ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �л����ƶ�״̬
            /*moveStatePower =  2;*/
            player.ChangeState(PlayerState.AirDown);
            return;
        }
        //�������
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
        // h��v������ʵ�ʵ��ƶ��ο��Լ��ж��Ƿ�ȥ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // rawH��rawV�����жϼ�ͣ
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

                // ������ת������
                Vector3 input = new Vector3(h, 0, v);
                // ��ȡ�������תֵ y
                float y = Camera.main.transform.rotation.eulerAngles.y;
                // ����Ԫ����������ˣ���ʾ������������������Ԫ�������ĽǶȽ�����ת��õ��µ�����
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
