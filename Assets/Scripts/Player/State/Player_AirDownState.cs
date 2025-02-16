using UnityEngine;

public class Player_AirDownState : PlayerStateBase
{
    float speed;
    float VerticalSpeed;
    private enum AirDownChildState
    {
        //Loop,
        //End
        JumpStart,
        InAir,
        DoubleJump,
        JumpDown,
        AirDodge,
        TakeUnbrella

    }

    //private float playEndAnimationHeight = 1.5f;   // 如果空中检测到距离地面有2米则启动翻滚
    //private float endAnimationHeight = 1.5f;     // End动画播放需要的高度，从这个高度开始播放

    bool hasJump = false;
    bool hasDodge = false;
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
    //private bool needEndAnimation;
    private AirDownChildState airDownState;
    private AirDownChildState AirDownState
    {
        get => airDownState;
        set
        {
            airDownState = value;
            switch (airDownState)
            {
                case AirDownChildState.InAir:
                    player.Model.Animator.CrossFade("InAir", 0.1f);
                    break;
                case AirDownChildState.DoubleJump:
                    player.Model.Animator.CrossFade("DoubleJump", 0.1f);
                    break;
                case AirDownChildState.JumpDown:
                    player.Model.Animator.CrossFade("JumpDown", 0.1f);
                    break;
                case AirDownChildState.JumpStart:
                    player.Model.Animator.CrossFadeInFixedTime("JumpStart", 0.1f);
                    break;
                case AirDownChildState.AirDodge:
                    player.Model.Animator.CrossFade("Air_Dodge_Front", 0.1f);
                    break;
                case AirDownChildState.TakeUnbrella:
                    player.Unbrella.SetActive(true);
                    player.Model.Animator.CrossFade("TakeUmbrella", 0.1f);
                    break;
                    //    case AirDownChildState.Loop:
                    //        player.PlayAnimation("JumpLoop");
                    //        break;
                    //    case AirDownChildState.End:
                    //        player.PlayAnimation("JumpEnd");
                    //        break;
                    //}
            }
        }
    }

    public override void Enter()
    {
        player.Model.ClearRootMotionAction();
        if (player.CharacterController.isGrounded)
        {
            VerticalSpeed = Mathf.Sqrt(-player.jumpHeight * player.gravity * 2);
            //AirDownState = AirDownChildState.Loop;
            //// 判断当前角色的高度是否有可能切换到End
            //needEndAnimation = !Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.up * -1, playEndAnimationHeight + 0.5f, groundLayerMask);

            AirDownState = AirDownChildState.JumpStart;
        }
        else
        {
            AirDownState = AirDownChildState.InAir;
        }
        AirControll();
    }

    public void CaculateGravity()
    {

        if (!CheckAnimatorStateName("Air_Dodge_Front", out float time))
        {
            if (VerticalSpeed >= 0)
            {
                VerticalSpeed += player.gravity * Time.deltaTime;
            }
            else
            {
                VerticalSpeed += player.gravity * Time.deltaTime * 1.4f;
            }
        }

    }
    public override void Update()
    {


        CaculateGravity();
        
        switch (airDownState)
        {
            case AirDownChildState.InAir:
                if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDodge)
                {
                    VerticalSpeed = 0;
                    AirDownState = AirDownChildState.AirDodge;
                    hasDodge = true;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    player.ChangeState(PlayerState.AirAttack);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    VerticalSpeed = player.UnbrellaVerticalSpeed;
                    AirDownState = AirDownChildState.TakeUnbrella;
                }

                else if (Input.GetKeyDown(KeyCode.Space) && !hasJump)
                {
                    VerticalSpeed = Mathf.Sqrt(-player.jumpHeight * player.gravity * 2);
                    AirDownState = AirDownChildState.DoubleJump;
                    hasJump = true;
                }
                else
                {
                    if (player.CharacterController.isGrounded)
                    {
                        player.OnFootStep();
                        hasJump = false;
                        hasDodge = false;
                        float h = Input.GetAxis("Horizontal");
                        float v = Input.GetAxis("Vertical");
                        if (h != 0 || v != 0)
                        {

                            player.ChangeState(PlayerState.Move);
                            return;
                        }
                        AirDownState = AirDownChildState.JumpDown;
                        //return;
                    }
                }

                break;
            case AirDownChildState.JumpDown:
                if (CheckAnimatorStateName("JumpDown", out float animationTime))
                {
                    if (animationTime >= 0.5f)
                    {
                        player.ChangeState(PlayerState.Idle);
                        hasJump = false;
                        hasDodge = false;
                        return;
                    }
                }

                break;
            case AirDownChildState.DoubleJump:

                if (player.CharacterController.isGrounded)
                {
                    player.OnFootStep();
                    //player.CharacterController.Move(de)
                    if (CheckAnimatorStateName("DoubleJump", out float animationTime2))
                    {
                        if (animationTime2 > 0.8)
                        {
                            AirDownState = AirDownChildState.JumpDown;
                        }
                    }

                    //return;
                }
                else
                {
                    if (CheckAnimatorStateName("DoubleJump", out float animationTime2))
                    {
                        if (animationTime2 > 0.8)
                        {
                            AirDownState = AirDownChildState.InAir;
                            /*                            player.Model.ClearRootMotionAction();*/
                        }
                    }
                }
                //AirControll();
                break;
            case AirDownChildState.JumpStart:
                if (CheckAnimatorStateName("JumpStart", out float animationTime3))
                {
                    AirControll();
                    if (animationTime3 > 0.3)
                    {
                        AirDownState = AirDownChildState.InAir;
                    }
                }
                break;
            case AirDownChildState.AirDodge:
                VerticalSpeed = 0;
                if (CheckAnimatorStateName("Air_Dodge_Front", out float animationTime4))
                {
                    if (animationTime4 > 1)
                    {
                        AirDownState = AirDownChildState.InAir;
                    }
                }
                break;
            case AirDownChildState.TakeUnbrella:
                VerticalSpeed = player.UnbrellaVerticalSpeed;
                if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDodge)
                {
                    player.Unbrella.SetActive(false);
                    VerticalSpeed = 0;
                    AirDownState = AirDownChildState.AirDodge;
                    hasDodge = true;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    player.Unbrella.SetActive(false);
                    player.ChangeState(PlayerState.AirAttack);
                }
                else if (Input.GetKeyDown(KeyCode.Space) && !hasJump)
                {
                    player.Unbrella.SetActive(false);
                    VerticalSpeed = Mathf.Sqrt(-player.jumpHeight * player.gravity * 2);
                    AirDownState = AirDownChildState.DoubleJump;
                    hasJump = true;
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    player.Unbrella.SetActive(false);
                    AirDownState = AirDownChildState.InAir;
                }
                else
                {
                    if (player.CharacterController.isGrounded)
                    {
                        player.Unbrella.SetActive(false);
                        player.OnFootStep();
                        hasJump = false;
                        hasDodge = false;
                        float h = Input.GetAxis("Horizontal");
                        float v = Input.GetAxis("Vertical");
                        if (h != 0 || v != 0)
                        {

                            player.ChangeState(PlayerState.Move);
                            return;
                        }
                        AirDownState = AirDownChildState.JumpDown;
                        //return;
                    }
                }
                break;
        }
        AirControll();
    }
    public override void Exit()
    {

    }

    private void AirControll()
    {

        // 处理控制位移
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 motion = new Vector3(0, VerticalSpeed * Time.deltaTime, 0);

        if (h != 0 || v != 0)
        {
            Vector3 input = new Vector3(h, 0, v);
            /* Vector3 dir = Camera.main.transform.TransformDirection(input);*/
            float y = Camera.main.transform.rotation.eulerAngles.y;
            // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
            motion.x = player.moveSpeedForAirDown * Time.deltaTime * player.transform.forward.x;
            motion.z = player.moveSpeedForAirDown * Time.deltaTime * player.transform.forward.z;

            player.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
        }
        if (!CheckAnimatorStateName("Air_Dodge_Front", out float time))
        {
            player.CharacterController.Move(motion);
        }

       
    }
/*    public void setUnbrella(GameObject unbrella,Transform UnbrellaPosition)
    {
        GameObject Obj = GameObject.Instantiate(unbrella, null);
        Obj.transform.SetParent(UnbrellaPosition);
        Obj.transform.localPosition = Vector3.zero;
        Obj.transform.localRotation = Quaternion.identity;
    }*/
    /* private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
     {
         float h = Input.GetAxis("Horizontal");
         float v = Input.GetAxis("Vertical");
         if (h != 0 || v != 0)
         {
             Vector3 input = new Vector3(h, 0, v);
             Vector3 dir = Camera.main.transform.TransformDirection(input);
             float y = Camera.main.transform.rotation.eulerAngles.y;
             // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
             Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
             deltaPosition.x = player.moveSpeedForAirDown * Time.deltaTime * -dir.x;
             deltaPosition.z = player.moveSpeedForAirDown * Time.deltaTime * -dir.z;
             // 处理旋转
             // 获取相机的旋转值 y

             player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
         }


     }*/
}
