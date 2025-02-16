using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_Controller : CharacterBase
{
    public Player_Controller targetPlayer;
    public NavMeshAgent navMeshAgent;
    public float walkRange = 5;
    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed = 5f;

    public float standAttackRange;
    public float attackRange;

    public float SPAttackdistance = 5;
    public float vigilantTime = 10;
    public float vigilantRange = 6;
    public float vigilantSpeed = 2.5f;

    public float attackTime = 5;
    public bool anger;

    public float walkTime = 2;
    public float idleTime;
    public float patrolTime;
    public float patrolRadius;

    // 防御相关参数
    private float lastHurtTime = 0f;  // 上次受伤时间
    private int hurtCount = 0;        // 连续受伤次数
    private float hurtCountResetTime = 3f;  // 重置连续受伤计数的时间
    private float defenseThreshold = 0.4f;  // 血量低于这个值时更容易防御
    private float lastDefenseTime = 0f;     // 上次防御的时间
    private float defenseCooldown = 10f;     // 防御冷却时间

    private void Start()
    {
        Init();
        ChangeState(BossState.Walk);
    }

    public void ChangeState(BossState bossState, bool reCurrstate = false)
    {
        switch (bossState)
        {
            case BossState.Idle:
                stateMachine.ChangeState<Boss_IdleState>(reCurrstate);
                break;
            case BossState.Walk:
                stateMachine.ChangeState<Boss_WalkState>(reCurrstate);
                break;
            case BossState.Run:
                stateMachine.ChangeState<Boss_RunState>(reCurrstate);
                break;
            case BossState.Hurt:
                stateMachine.ChangeState<Boss_HurtState>(reCurrstate);
                break;
            case BossState.Attack:
                stateMachine.ChangeState<Boss_AttackState>(reCurrstate);
                break;
            case BossState.Patrol:
                stateMachine.ChangeState<Boss_PatrolState>(reCurrstate);
                break;
            case BossState.Defense:
                stateMachine.ChangeState<Boss_DefenseState>(reCurrstate);
                break;
        }
    }

    // 判断是否应该进入防御状态
    public bool ShouldEnterDefense()
    {
        if (targetPlayer == null) return false;
        if (stateMachine.currentState is Boss_DefenseState) return false;

        // 检查防御冷却时间
        if (Time.time - lastDefenseTime < defenseCooldown)
        {
            return false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
        bool shouldDefense = false;
        
        // 1. 血量低时更容易防御
        if (CurrentHP < maxHP * defenseThreshold)
        {
            shouldDefense = UnityEngine.Random.value < 0.6f;  // 60%概率防御
        }
        // 2. 玩家在攻击范围内时可能防御
    /*    else if (distanceToPlayer <= attackRange * 1.5f)
        {
            Debug.Log(UnityEngine.Random.value);
            shouldDefense = UnityEngine.Random.value < 0.1f;  // 30%概率防御
        }
        // 3. 连续受到多次攻击时更容易防御*/
        else if (hurtCount >= 3 && Time.time - lastHurtTime < hurtCountResetTime)
        {
            shouldDefense = UnityEngine.Random.value < 0.7f;  // 70%概率防御
        }

        if (shouldDefense)
        {
            lastDefenseTime = Time.time; // 记录本次防御时间
        }

        return shouldDefense;
    }

    public override void Hurt(Skill_HitData hitData, ISkillOwner hurtSource)
    {
        // 记录原始伤害值
        float originalDamage = hitData.DamgeValue;

        // 如果在防御状态下
        if (stateMachine.currentState is Boss_DefenseState defenseState)
        {
            defenseState.OnBlock();
            hitData.DamgeValue = 0f;  // 格挡时将伤害设为0
            lastDefenseTime = Time.time;
        }
        else
        {
            // 更新连续受伤计数
            if (Time.time - lastHurtTime < hurtCountResetTime)
            {
                hurtCount++;
            }
            else
            {
                hurtCount = 1;
            }
            lastHurtTime = Time.time;

            // 如果连续受伤次数达到阈值，考虑进入防御状态
            if (hurtCount >= 3 && ShouldEnterDefense())
            {
                ChangeState(BossState.Defense);
                return;
            }
        }

       
        // 更新HP并处理状态变化
        if (!(stateMachine.currentState is Boss_DefenseState))
        {
            UpdateHP(hitData);
            // 无论是否在防御状态，都调用基类的Hurt方法
        
            ChangeState(BossState.Hurt, true);
        }
        base.Hurt(hitData, hurtSource);

        // 恢复原始伤害值，避免影响其他逻辑
        hitData.DamgeValue = originalDamage;
    }

    #region UnityEditor
    #if UNITY_EDITOR
    [ContextMenu("SetHurtCollider")]
    private void SetHurtCollider()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Weapon_Controller>() == null)
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("HurtCollider");
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
    #endif
    #endregion
}
