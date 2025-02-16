using UnityEngine;
public class Player_MoveState: PlayerStateBase
{
    private float verticalSpeed;
    private float Speed;
    private enum MoveChildState
    { 
        Move,
        Walk
    }
    private MoveChildState moveChild;

    private MoveChildState MoveChild { get => moveChild; set {
            moveChild = value;
            switch (moveChild)
            {
                case MoveChildState.Move:
                    Speed = player.runSpeed;
                    player.PlayAnimation("Move");
                    break;
                case MoveChildState.Walk:
                    Speed = player.walkSpeed;
                    player.PlayAnimation("Walk");
                    break;
            }
        }
    }

    //private float walk2RunTransition; // 0~1
    //private MoveChildState moveState;
    //private MoveChildState MoveState
    //{
    //    get => moveState;
    //    set {
    //        moveState = value;
    //        // 状态进入
    //        switch (moveState)
    //        {
    //            case MoveChildState.Move:
    //                player.PlayAnimation("Move");
    //                break;
    //            case MoveChildState.Stop:
    //                player.PlayAnimation("Stop");
    //                break;

    //        }
    //    }
    //}

    public override void Enter()
    {
        Debug.Log("进入MOve");
        verticalSpeed = 0;
        if (player.WalkModel)
        {
            MoveChild = MoveChildState.Walk;
        }
        else
        {
            MoveChild = MoveChildState.Move;
        }
        // 注册根运动
        player.Model.ClearRootMotionAction();
        /*player.Model.SetRooMotionAction(OnRootMotion);*/
    }

    public override void Update()
    {
        // 检测攻击
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }
        if (Input.GetMouseButton(1))  // 使用GetMouseButton而不是GetMouseButtonDown来实现持续防御
        {
            player.ChangeState(PlayerState.Defense);
            return;
        }
        // 检测翻滚
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }
        //检测跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 切换到移动状态
            //moveStatePower = walk2RunTransition + 1;
            player.ChangeState(PlayerState.AirDown);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            player.WalkModel = !player.WalkModel;
            if (player.WalkModel)
            {
                MoveChild = MoveChildState.Walk;
            }
            else
            {
                MoveChild = MoveChildState.Move;
            }
        }
/*        //检测下落
        if (player.CharacterController.isGrounded == false)
        {
            //moveStatePower = walk2RunTransition + 1;
            player.ChangeState(PlayerState.AirDown);
            return;
        }*/
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical"); 
        float H = player.move.Horizontal;
        float V = player.move.Vertical;

        if (h == 0 && v == 0&&H==0&&V==0)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }
        if (h != 0 || v != 0||H!=0||V!=0)
        {
            Vector3 input;
            if (H == 0 && V == 0)
            {
                // 处理旋转的问题
                 input = new Vector3(h, 0, v);
            }
            else
            {
                input = new Vector3(H, 0, V);
            }
            // 获取相机的旋转值 y
            float y = Camera.main.transform.rotation.eulerAngles.y;
            // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
            player.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
            player.CharacterController.Move(targetDir.normalized*(Speed * Time.deltaTime));
            player.CharacterController.Move(Vector3.up*player.gravity*Time.deltaTime);
        }
        if (!player.CharacterController.isGrounded)
        {
            player.ChangeState(PlayerState.AirDown);
        }
        //if (h != 0 || v != 0)
            //        {
            //            // 处理旋转的问题
            //            Vector3 input = new Vector3(h, 0, v);
            //            // 获取相机的旋转值 y
            //            float y = Camera.main.transform.rotation.eulerAngles.y;
            //            // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
            //            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;

            //            player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
            //        }
            //            return;
            //switch (moveState)
            //{
            //    case MoveChildState.Move:
            //        MoveOnUpdate();
            //        break;
            //    case MoveChildState.Stop:
            //        StopOnUpdate();
            //        break;
            //}
    }

    //private void MoveOnUpdate()
    //{
    //    // h和v用来做实际的移动参考以及判断是否去待机
    //    float h = Input.GetAxis("Horizontal");
    //    float v = Input.GetAxis("Vertical");

    //    // rawH和rawV用来判断急停
    //    float rawH = Input.GetAxisRaw("Horizontal");
    //    float rawV = Input.GetAxisRaw("Vertical");

    //    if (rawH == 0 && rawV == 0)// 玩家松开了按键
    //    {
    //        if (walk2RunTransition > 0.4f) 
    //        {
    //            // 进入急停
    //            MoveState = MoveChildState.Stop;
    //            return;
    //        }
    //        else if(h==0&&v==0)
    //        {
    //            player.ChangeState(PlayerState.Idle);
    //            return;
    //        }

    //    }
    //    else
    //    {
    //        //    处理走到跑的过渡
    //        if (Input.GetKey(KeyCode.LeftShift)) // 走到跑
    //        {
    //            walk2RunTransition = Mathf.Clamp(walk2RunTransition + Time.deltaTime * player.walk2RunTransition, 0, 1);
    //        }
    //        else // 跑到走
    //        {
    //            walk2RunTransition = Mathf.Clamp(walk2RunTransition - Time.deltaTime * player.walk2RunTransition, 0, 1);
    //        }
    //        //player.Model.Animator.SetFloat("Move", walk2RunTransition);
    //        // 通过修改动画播放速度，来达到实际的位移距离变化
    //        player.Model.Animator.speed = Mathf.Lerp(player.walkSpeed, player.runSpeed, walk2RunTransition);

    //        if (h != 0 || v != 0)
    //        {
    //            // 处理旋转的问题
    //            Vector3 input = new Vector3(h, 0, v);
    //            // 获取相机的旋转值 y
    //            float y = Camera.main.transform.rotation.eulerAngles.y;
    //            // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
    //            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;

    //            player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
    //        }
    //    }
    //}

    //private void StopOnUpdate()
    //{
    //    // 检测当前动画的进度，如果播放完毕了，则切换到待机
    //    if (CheckAnimatorStateName("Stop",out float aniamtionTime))
    //    {
    //        if (aniamtionTime >= 1) player.ChangeState(PlayerState.Idle);
    //    }
    //}

    public override void Exit()
    {
        //walk2RunTransition = 0;
        /*player.Model.ClearRootMotionAction();*/
        /*player.Model.Animator.speed = 1;*/
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}