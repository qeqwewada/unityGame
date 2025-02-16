using UnityEngine;
using System.Collections;

public class Boss_HurtState : BossStateBase
{
    private Skill_HitData hitData => boss.hitData;
    private ISkillOwner source => boss.hurtSource;

    private Coroutine repelCoroutine;

    enum HurtChildState
    {
        // 普通受伤
        NoramlHurt,
        // 击倒-Down
        Down,
        // 击倒-Rise
        Rise
    }

    private HurtChildState hurtState;
    private HurtChildState HurtState
    {
        get => hurtState;
        set
        {
            hurtState = value;
            switch (hurtState)
            {
                case HurtChildState.NoramlHurt:
                    boss.OnFootStep();
                    if (source != null && source.ModelTransform != null)
                    {
                        boss.transform.LookAt(source.ModelTransform);
                    }
                    boss.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    boss.OnFootStep();
                    if (source != null && source.ModelTransform != null)
                    {
                        boss.transform.LookAt(source.ModelTransform);
                    }
                    boss.PlayAnimation("Down");
                    break;
                case HurtChildState.Rise:
                    boss.OnFootStep();
                    boss.PlayAnimation("Rise");
                    break;
            }
        }
    }

    public override void Enter()
    {
        currHardTime = 0;
        if (hitData != null && hitData.Down)
        {
            HurtState = HurtChildState.Down;
        }
        else
        {
            HurtState = HurtChildState.NoramlHurt;
        }

        // 击退击飞
        if (hitData != null && hitData.RepelVelocity != Vector3.zero)
        {
            repelCoroutine = MonoManager.Instance.StartCoroutine(DoRepel(hitData.RepelTime, hitData.RepelVelocity));
        }
    }

    private float currHardTime = 0;
    public override void Update()
    {
        // 没有在击飞击退模拟中，就可以有重力
        if (repelCoroutine == null)
        {
            boss.CharacterController.Move(new Vector3(0, boss.gravity * Time.deltaTime, 0));
        }

        currHardTime += Time.deltaTime;
        switch (HurtState)
        {
            case HurtChildState.NoramlHurt:
                // 硬直时间到了 && 击飞击退效果也结束了 
                if (hitData != null && currHardTime >= hitData.HardTime && repelCoroutine == null)
                {
                    Debug.Log("1");
                    boss.ChangeState(BossState.Idle);
                }
                break;
            case HurtChildState.Down:
                // 硬直时间到了 && 击飞击退效果也结束了 
                if (hitData != null && currHardTime >= hitData.HardTime && repelCoroutine == null)
                {
                   boss.ChangeState(BossState.Idle);
                }
                break;
            case HurtChildState.Rise:

                // 检测起身动画播放完毕
                if (CheckAnimatorStateName("Rise", out float time) && time >= 0.99f)
                {
                    boss.ChangeState(BossState.Idle);
                }
                break;
        }
    }

    private IEnumerator DoRepel(float time, Vector3 velocity)
    {
        if (source == null || source.ModelTransform == null)
        {
            repelCoroutine = null;
            yield break;
        }

        float currTime = 0;
        // 避免分母为0
        time = time == 0 ? 0.0001f : time;
        // 将位移方向修改为针对对手的方向
        Vector3 veloFromSource = source.ModelTransform.TransformDirection(velocity);
        while (currTime < time)
        {
            boss.CharacterController.Move(veloFromSource / time * Time.deltaTime);
            currTime += Time.deltaTime;
            yield return null;
        }
        repelCoroutine = null;
    }

    public override void Exit()
    {
        if (repelCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(repelCoroutine);
            repelCoroutine = null;
        }
    }

}
