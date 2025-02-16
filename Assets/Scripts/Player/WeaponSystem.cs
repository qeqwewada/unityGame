using UnityEngine;
using System;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;

public class WeaponSystem : MonoBehaviourPunCallbacks
{
    [Header("攻击检测参数")]
    public float attackRadius = 1f;           // 攻击检测半径
    public float attackDistance = 2f;         // 攻击距离
    public LayerMask targetLayer;             // 目标层级
    public Transform attackOrigin;            // 攻击判定起始点
    public Transform effectSpawnPoint;        // 特效生成点
    
    private CharacterBase owner;              // 武器拥有者
    private List<IHurt> hitEnemies = new List<IHurt>();  // 已命中的敌人列表

    private void Start()
    {

        owner = GetComponent<CharacterBase>();
        if (attackOrigin == null)
        {
            attackOrigin = transform;
        }
        if (effectSpawnPoint == null)
        {
            effectSpawnPoint = attackOrigin;
        }
    }

    // 由动画事件调用的攻击检测
    public void CheckAttackHit()
    {
        if(owner.CurrentSkillConfig == null) return;
        
        hitEnemies.Clear();
        Vector3 attackPoint = attackOrigin.position + attackOrigin.forward * attackDistance;
        
        // 球形范围检测
        Collider[] colliders = Physics.OverlapSphere(attackPoint, attackRadius, targetLayer);
        
        foreach (Collider col in colliders)
        {
            IHurt target = col.GetComponentInParent<IHurt>();
            if (target != null && !hitEnemies.Contains(target))
            {
                hitEnemies.Add(target);
                Vector3 hitPoint = col.ClosestPoint(attackPoint);
                owner.OnHit(target, hitPoint);
            }
        }
    }

   

    // 在编辑器中可视化攻击范围
    private void OnDrawGizmosSelected()
    {
        if (attackOrigin != null)
        {
            Vector3 attackPoint = attackOrigin.position + attackOrigin.forward * attackDistance;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint, attackRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(attackOrigin.position, attackPoint);
        }
    }
} 