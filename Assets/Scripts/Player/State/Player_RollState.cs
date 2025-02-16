using UnityEngine;
using System.Collections;

public class Player_RollState : PlayerStateBase
{
    //private Coroutine rotateCoroutine;
    //private bool isRotate = false;
    string rollState;
    public override void Enter()
    {

        player.Model.Animator.speed = 1;
        // 检测玩家的输入方向
        player.StartRoll();
        player.PlayAnimation("Evade_Front");
        player.Model.SetRooMotionAction(OnRootMotion);
        //    float h = Input.GetAxisRaw("Horizontal");
        //    float v = Input.GetAxisRaw("Vertical");

        //    //
        //    //if (h != 0 || v != 0)
        //    //{
        //    //    Vector3 input = new Vector3(h, 0, v);
        //    //    // 获取相机的旋转值 y
        //    //    float y = Camera.main.transform.rotation.eulerAngles.y;
        //    //    // 让四元数和向量相乘：表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量

        //    if (h != 0 || v != 0)
        //    {
        //        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        //        Debug.Log("input:" + inputDir);

        //        float y = Camera.main.transform.rotation.eulerAngles.y;
        //        Vector3 targetDir = Quaternion.Euler(0, y, 0) * inputDir;
        //        //Debug.Log(targetDir);
        //        float angel = Vector3.Angle(targetDir, player.transform.forward);
        //        Debug.Log(player.transform.forward);
        //        Debug.Log(angel);
        //        if (angel < 10)
        //        {

        //            player.PlayAnimation("Evade_Front");
        //            rollState = "Evade_Front";
        //        }
        //        if (angel > 170)
        //        {
        //            player.PlayAnimation("Evade_Back");
        //            rollState = "Evade_Back";

        //        }
        //        if (angel > 10 && angel < 90&&h<0 ) { player.Model.Animator.CrossFade("Evade_Front_Left",0);  rollState = "Evade_Front_Left"; }
        //        if (angel > 10 && angel < 90&&h>0 ) { player.PlayAnimation("Evade_Front_Right"); rollState = "Evade_Front_Right"; }

        //        if (angel > 90 && angel < 170&&h>0 ) { player.PlayAnimation("Evade_Back_Right"); rollState = "Evade_Back_Right"; }
        //        if (angel > 90 && angel < 170&&h<0 ) { player.PlayAnimation("Evade_Back_Left"); rollState = "Evade_Back_Left"; }

        //        player.Model.SetRooMotionAction(OnRootMotion);
        //        Debug.Log("qwq");



        //        //    player.Model.SetRooMotionAction(OnRootMotion);
        //        //}
        //        //else
        //        //{
        //        //    player.PlayAnimation("Evade_Front");
        //        //    player.Model.SetRooMotionAction(OnRootMotion);
        //        //}


        //        //if (h != 0 || v != 0)
        //        //{
        //        //    Vector3 inputDir = new Vector3(h, 0, v).normalized;
        //        //    rotateCoroutine = MonoManager.Instance.StartCoroutine(DoRotate(inputDir));
        //        //}
        //        //else
        //        //{   

        //        //}
        //    }
        //    else
        //    {


        //        player.PlayAnimation("Evade_Front");
        //        rollState = "Evade_Front";
        //        player.Model.SetRooMotionAction(OnRootMotion);

        //    }
        //}
        //private IEnumerator DoRotate(Vector3 dir)
        //{
        //    isRotate = true;
        //    float y = Camera.main.transform.rotation.eulerAngles.y;
        //    Vector3 targetDir = Quaternion.Euler(0, y, 0) * dir;
        //    Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        //    float rate = 0;
        //    while (rate < 1)
        //    {
        //        rate += Time.deltaTime * 10; // 10倍速旋转
        //        player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, targetRotation, rate);
        //        yield return null;
        //    }
        //    isRotate = false;
        //    player.PlayAnimation("Roll");
        //    player.Model.SetRooMotionAction(OnRootMotion);
        //}
    }
    public override void Update()
    {
        //if (isRotate) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if ((h != 0 || v != 0)&&player.CanSwitchSkill)
        {
            
            // 切换到移动状态
            player.ChangeState(PlayerState.Run);
            return;
        }
        if (CheckAnimatorStateName("Evade_Front", out float animationTime))
        {
            if (animationTime > 0.8f)
            {
               
                player.ChangeState(PlayerState.Idle);
            }
        }
    }

    public override void Exit()
    {
        moveStatePower = 0;
        player.Model.ClearRootMotionAction();
        
        //if (rotateCoroutine != null) MonoManager.Instance.StopCoroutine(rotateCoroutine);
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        //Debug.Log(player.Model.Animator.speed);
        //deltaPosition *= Mathf.Clamp(moveStatePower, 1, 1.5f);
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}