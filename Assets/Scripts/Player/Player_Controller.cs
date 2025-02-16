using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player_Controller : CharacterBase
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    public Joystick move;
    #region 配置类型的信息
    [Header("配置")]
    public Text alertText; // 指向提示框的Text组件
    public float displayTime = 2.0f; // 提示框显示的时间
    public float fadeOutTime = 1.0f; // 淡出时间
    public float rotateSpeed = 5;
    public float rotateSpeedForAttack = 4;
    public float walk2RunTransition = 1;
    public float walkSpeed = 1;
    public float runSpeed = 5;
    public float sprintSpeed = 4;
    public float jumpHeight;
    public float moveSpeedForJump;
    public float moveSpeedForAirDown;
    public float DodgeDistance = 2;
    public float UnbrellaVerticalSpeed = 0.5f;
    public GameObject Unbrella;
    private bool walkModel = false;
    private bool isEquipWeapon;
    private int currentWeaponPointIndex = 1;

    [Header("_剑武器挂点")]
    public Transform swordWeaponPos;
    public Transform handSwordWeaponPoint;
    public Transform backSwordWeaponPoint;

    public bool WalkModel 
    { 
        get => walkModel; 
        set 
        { 
            walkModel = value;
            if (walkModel == false)
            {
                SwitchToRunMode();
            }
            else
            {
                SwitchToWalkMode();
            }
        } 
    }
    #endregion
    /*    [Header("伞挂点")]
        public Transform unbrellaPosition;*/
    private void Awake()
    {
        // 确保基础组件已经赋值
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        if (model == null)
            model = GetComponentInChildren<ModelBase>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
     
    }
 
    private void Start()
    {
        currentPos = transform.position;
        currentRotation = transform.rotation;
        /*alertText.gameObject.SetActive(false);*/
        Init();
        ChangeState(PlayerState.Idle); // 默认进入待机状态
    }


    public void ChangeState(PlayerState playerState, bool reCurrstate = false)
    {
        /*if (!photonView.IsMine) return;*/

/*
        // 通过RPC同步状态改变

        photonView.RPC("RPC_ChangeState", RpcTarget.All, (int)playerState, reCurrstate);

    }



    [PunRPC]
    private void RPC_ChangeState(int playerStateInt, bool reCurrstate)

    {

        PlayerState playerState = (PlayerState)playerStateInt;*/


        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<Player_IdleState>(reCurrstate);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<Player_MoveState>(reCurrstate);
                break;
            /*case PlayerState.Jump:
                stateMachine.ChangeState<Player_JumpState>(reCurrstate);
                break;*/
            case PlayerState.AirDown:
                stateMachine.ChangeState<Player_AirDownState>(reCurrstate);
                break;
            case PlayerState.Roll:
                stateMachine.ChangeState<Player_RollState>(reCurrstate);
                break;
            case PlayerState.Hurt:
                stateMachine.ChangeState<Player_HurtState>(reCurrstate);
                break;
            case PlayerState.StandAttack:
                stateMachine.ChangeState<Player_StandAttackState>(reCurrstate);
                break;
            case PlayerState.Run:
                stateMachine.ChangeState<Player_RunState>(reCurrstate);
                break;
            case PlayerState.AirAttack:
                stateMachine.ChangeState<Player_AirAttack>(reCurrstate);
                break;
            case PlayerState.Defense:
                stateMachine.ChangeState<Player_DefenseState>(reCurrstate);
                break;
        }
    }

    private void SwitchToRunMode()
    {
        StartCoroutine(ShowAlert("切换到奔跑模式!"));
    }

    private void SwitchToWalkMode()
    {
        StartCoroutine(ShowAlert("切换到行走模式!"));
    }

    // 显示提示框的协程
    IEnumerator ShowAlert(string message)
    {
        alertText.text = message; // 设置提示信息
        alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, 1); // 确保是完全不透明的

        // 显示提示框
        alertText.gameObject.SetActive(true);

        // 等待一段时间
        yield return new WaitForSeconds(displayTime);

        // 开始淡出
        float elapsedTime = 0;
        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / fadeOutTime);
            alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, alpha);
            yield return null;
        }

        // 完全淡出后，隐藏提示框
        alertText.gameObject.SetActive(false);
    }

    public void ScreenImpulse(float force)
    {
        impulseSource.GenerateImpulse(force * 2); // 默认2倍
    }

    public override void OnHit(IHurt target, Vector3 hitPostion)
    {
        // 拿到这一段攻击的数据
        Skill_AttackData attackData = CurrentSkillConfig.AttackData[currentHitIndex];
        // 生成基于命中配置的效果
        StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig, hitPostion));
        // 播放效果类
        if (attackData.ScreenImpulseValue != 0) ScreenImpulse(attackData.ScreenImpulseValue);
        if (attackData.ChromaticAberrationValue != 0) PostProcessManager.Instance.ChromaticAberrationEF(attackData.ChromaticAberrationValue);
        StartFreezeFrame(attackData.FreezeFrameTime);
        StartFreezeTime(attackData.FreezeGameTime);
        // 传递伤害数据
        target.Hurt(attackData.HitData, this);
    }

    public override void Hurt(Skill_HitData hitData, ISkillOwner hurtSource)
    {
        // 如果在防御状态下
        if (stateMachine.currentState is Player_DefenseState defenseState)
        {
            // 播放格挡动画
            defenseState.OnBlock();

            // 可以在这里添加格挡音效或特效

            // 可以减少一定的伤害
            hitData.DamgeValue= 0f; // 格挡时只受到20%的伤害
        }

        UpdateHP(hitData);
        base.Hurt(hitData, hurtSource);
        
        // 如果不在防御状态，才切换到受伤状态
        if (!(stateMachine.currentState is Player_DefenseState))
        {
            ChangeState(PlayerState.Hurt, true);
        }
    }
  
    public override void SwitchWeaponParentObject(int index)
    {
       /* if (!photonView.IsMine) return;*/

        //index0为右手，1为背部
        if (index == 0)
        {
            currentWeaponPointIndex = 0;
            //打断收回武器动画
            Model.Animator.CrossFade("Null", 0f);
            RPC_SetWeaponPos(0);
            /*photonView.RPC("RPC_SetWeaponPos", RpcTarget.All, 0);*/
            isEquipWeapon = true;
        }
        else if (index == 1)
        {
            currentWeaponPointIndex = 1;
            
            RPC_SetWeaponPos(1);
           /* photonView.RPC("RPC_SetWeaponPos", RpcTarget.All, 1);*/
            isEquipWeapon = false;
        }
    }

   /* [PunRPC]*/
    private void RPC_SetWeaponPos(int pointIndex)
    {
        Transform targetPoint = pointIndex == 0 ? handSwordWeaponPoint : backSwordWeaponPoint;
        
        if (swordWeaponPos != null && targetPoint != null)
        {
            swordWeaponPos.SetParent(targetPoint);
            swordWeaponPos.localPosition = Vector3.zero;
            swordWeaponPos.localRotation = Quaternion.identity;
        }
    }



  
}
