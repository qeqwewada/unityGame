using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;

public abstract class CharacterBase : MonoBehaviourPunCallbacks, IStateMachineOwner, ISkillOwner, IHurt,IPunObservable
{
    [SerializeField] protected ModelBase model;
    [SerializeField] protected PhotonAnimatorView photonAnimatorView;
    public ModelBase Model { get => model; }
    public Transform ModelTransform => Model.transform;
    [SerializeField] protected CharacterController characterController;
    public CharacterController CharacterController { get => characterController; }
    [SerializeField] protected AudioSource audioSource;
    protected StateMachine stateMachine;

    public AudioClip[] footStepAudioClips;
    public List<string> enemeyTagList;
    public SkillConfig[] standAttackConfigs;
    public SkillConfig fallAttack;
    public SkillConfig SPAttack;
    public Image HPFillName;
    [SerializeField]protected float maxHP;
    private float currentHP;

    public float CurrentHP
    { 
        get => currentHP;
        set 
        { 
            currentHP = value;
            if (currentHP <0)
            {
                currentHP = 0;
                HPFillName.fillAmount = currentHP / maxHP;
            }
            else
                HPFillName.fillAmount = currentHP / maxHP;

        }
    }
   
    public float gravity = -9.8f;
    [SerializeField] protected WeaponSystem weaponSystem;  // 替换原有的weapons数组

   

    protected CinemachineVirtualCamera virtualCamera;

    public virtual void Init()
    {
        if (photonAnimatorView == null)
        {
            photonAnimatorView = GetComponentInChildren<PhotonAnimatorView>();
        }
        
        // 设置 Animator 的同步模式
        if (model && model.Animator)
        {
            model.Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
        
        CurrentHP = maxHP;
        Model.Init(this, enemeyTagList);
        stateMachine = new StateMachine();
        stateMachine.Init(this);
        CanSwitchSkill = true;
    }

    #region 技能相关

    public Skill_HitData hitData { get; protected set; }
    public ISkillOwner hurtSource { get; protected set; }
    public SkillConfig CurrentSkillConfig { get; set; }
    public int currentHitIndex = 0;
    // 可以切换技能，主要用于判定前摇和后摇
    public bool CanSwitchSkill { get; private set; }
    public bool CanMove { get; private set; }
    

    public void StartAttack(SkillConfig skillConfig)
    {
        CanMove = false;
        CanSwitchSkill = false; // 防止玩家立刻播放下一个技能
        CurrentSkillConfig = skillConfig;
        currentHitIndex = 0;
        // 播放技能动画
        PlayAnimation(CurrentSkillConfig.AnimationName);
        // 技能释放音效
        SpawnSkillObject(skillConfig.ReleaseData.SpawnObj);
        // 技能释放物体
        PlayAudio(CurrentSkillConfig.ReleaseData.AudioClip);
    }
    public void setfallAttack()
    {
        CurrentSkillConfig = fallAttack;
        currentHitIndex = 0;
    }
    public void SetSPAttack()
    {
        CurrentSkillConfig = SPAttack;
        currentHitIndex = 0;
    }
    public void StartRoll()
    {
        CanSwitchSkill = false;
    }
    public void StartSkillHit(int weaponIndex)
    {
        
        if (currentHitIndex >= 2)
        {
            currentHitIndex = 1;
        }
        // 技能释放音效
        SpawnSkillObject(CurrentSkillConfig.AttackData[currentHitIndex].SpawnObj);
        // 技能释放物体
        PlayAudio(CurrentSkillConfig.AttackData[currentHitIndex].AudioClip);
    }

    public void StopSkillHit(int weaponIndex)
    {   
        currentHitIndex += 1;
    }

    public void SkillCanSwitch()
    {
        CanSwitchSkill = true;
    }
    public void Move()
    {
        CanMove = true;
    }

    public void SpawnSkillObject(Skill_SpawnObj spawnObj)
    {
        if (spawnObj != null && spawnObj.Prefab != null)
        {
            StartCoroutine(DoSpawnObject(spawnObj));
        }
    }

    protected IEnumerator DoSpawnObject(Skill_SpawnObj spawnObj)
    {
        // 延迟时间
        yield return new WaitForSeconds(spawnObj.Time);
        GameObject skillObj = GameObject.Instantiate(spawnObj.Prefab, null);
        // 一般特效的生成位置是相对于主角的
        skillObj.transform.position = Model.transform.position + Model.transform.TransformDirection(spawnObj.Position);
        skillObj.transform.localScale = spawnObj.Scale;
        skillObj.transform.eulerAngles = Model.transform.eulerAngles + spawnObj.Rotation;
        PlayAudio(spawnObj.AudioClip);
    }

    public virtual void OnHit(IHurt target, Vector3 hitPosition)
    {
        
        
        Skill_AttackData attackData = CurrentSkillConfig.AttackData[currentHitIndex];

        // 通过RPC同步特效和伤害
        if (CurrentSkillConfig == null) return;

        attackData = CurrentSkillConfig.AttackData[currentHitIndex];
        StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig, hitPosition));
        StartFreezeFrame(attackData.FreezeFrameTime);
        StartFreezeTime(attackData.FreezeGameTime);

        RPC_TakeDamage(attackData.HitData.DamgeValue);

        target.Hurt(attackData.HitData, this);
        // 只有攻击者才发送伤害
        /*   if (target is CharacterBase targetCharacter)
           {
               targetCharacter.photonView.RPC("RPC_TakeDamage", RpcTarget.All, attackData.HitData.DamgeValue);
           }*/
    }

 
    protected virtual void RPC_OnHit(Vector3 hitPosition, int hitIndex)
    {

    }

    /*[PunRPC]*/
    protected virtual void RPC_TakeDamage(float damage)
    {
        CurrentHP -= damage;
        
    }

    [PunRPC]
    protected virtual void RPC_OnDeath()
    {
        // 处理死亡逻辑
        gameObject.SetActive(false);
    }

    protected void StartFreezeFrame(float time)
    {
        if (time > 0) StartCoroutine(DoFreezeFrame(time));
    }

    protected IEnumerator DoFreezeFrame(float time)
    {
        Model.Animator.speed = 0;
        yield return new WaitForSeconds(time);
        Model.Animator.speed = 1;
    }

    protected void StartFreezeTime(float time)
    {
        if (time > 0) StartCoroutine(DoFreezeTime(time));
    }

    protected IEnumerator DoFreezeTime(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }


    protected IEnumerator DoSkillHitEF(SkillHitEFConfig hitEFConfig, Vector3 spawnPoint)
    {
        if (hitEFConfig == null) yield break;
        // 延迟时间
        PlayAudio(hitEFConfig.AudioClip);
        if (hitEFConfig.SpawnObject != null && hitEFConfig.SpawnObject.Prefab != null)
        {
            yield return new WaitForSeconds(hitEFConfig.SpawnObject.Time);
            GameObject temp = Instantiate(hitEFConfig.SpawnObject.Prefab);
            temp.transform.position = spawnPoint + hitEFConfig.SpawnObject.Position;
            temp.transform.LookAt(Camera.main.transform);
            temp.transform.eulerAngles += hitEFConfig.SpawnObject.Rotation;
            temp.transform.localScale += hitEFConfig.SpawnObject.Scale;
            PlayAudio(hitEFConfig.SpawnObject.AudioClip);
        }
    }

    public void OnSkillOver()
    {
        CanSwitchSkill = true;
    }


    #endregion

    //public void PlayAnimation(string animationName, float fixedTransitionDuration = 0.15f)
    //{
    //    model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    //}
    private string currentAnimationName;
    public void PlayAnimation(string animationName, bool reState = true, float fixedTransitionDuration = 0.15f)
    {
        if (currentAnimationName == animationName && !reState)
        {
            return;
        }
        currentAnimationName = animationName;
        model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }

    //public void PlayAnimationt(string animationName, float fixedTransitionDuration = 0f)
    //{
    //    model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    //}

    public void OnFootStep()
    {
        audioSource.PlayOneShot(footStepAudioClips[Random.Range(0, footStepAudioClips.Length)]);
    }
    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null) audioSource.PlayOneShot(audioClip);
    }

    public virtual void Hurt(Skill_HitData hitData, ISkillOwner hurtSource)
    {
        this.hitData = hitData;
        this.hurtSource = hurtSource;
    }
    public virtual void UpdateHP(Skill_HitData hitData)
    {
        CurrentHP -= hitData.DamgeValue;
     
    }

    public virtual void SwitchWeaponParentObject(int index)
    {
        
    }

    public void SetVirtualCamera(CinemachineVirtualCamera camera)
    {
        virtualCamera = camera;
        
        // 如果需要可以在这里进行额外的相机配置
        if (!photonView.IsMine)
        {
            // 非本地玩家的相机优先级设为较低
            virtualCamera.Priority = 0;
        }
    }

    // 可以添加相机控制方法
    public void UpdateCameraOffset(Vector3 offset)
    {
        if (virtualCamera != null)
        {
            var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null)
            {
                composer.m_TrackedObjectOffset = offset;
            }
        }
    }
    private int currentAnimationHash;
    public void UpdateCameraDistance(float distance)
    {
        if (virtualCamera != null)
        {
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                Vector3 offset = transposer.m_FollowOffset;
                offset.z = -distance;
                transposer.m_FollowOffset = offset;
            }
        }
    }
    public Vector3 currentPos;
    public Quaternion currentRotation;
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        //发送数据
        stream.SendNext(transform.position);
        stream.SendNext(transform.rotation);
        stream.SendNext(Model.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        Debug.Log(Model.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
    }
    else
    {
        //接收数据
        currentPos = (Vector3)stream.ReceiveNext();
        currentRotation = (Quaternion)stream.ReceiveNext();
        int newAnimHash = (int)stream.ReceiveNext();

        // 只在动画哈希值改变时更新动画
        if (currentAnimationHash != newAnimHash)
        {
                Debug.Log(newAnimHash);
            currentAnimationHash = newAnimHash;
            model.Animator.CrossFadeInFixedTime(newAnimHash, 0.1f);
        }
    }
}


}
