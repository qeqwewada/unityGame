using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ModelBase : MonoBehaviour
{
    public CharacterBase player;
    [SerializeField] protected Animator animator;
    public Animator Animator { get => animator; }
    protected ISkillOwner skillOwner;
    [SerializeField] Weapon_Controller[] weapons;
    public void Init(ISkillOwner skillOwner, List<string> enemeyTagList)
    {
        this.skillOwner = skillOwner;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init(enemeyTagList, skillOwner.OnHit);
        }
    }

    #region 根运动
    protected Action<Vector3, Quaternion> rootMotionAction;

    public void SetRooMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }

    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }

    private void OnAnimatorMove()
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }

    #endregion

    #region 动画事件
    protected void FootStep()
    {
        skillOwner.OnFootStep();
    }
    protected void Move()
    {
        skillOwner.Move();
    }

    protected void StartSkillHit(int weaponIndex)
    {
       

        skillOwner.StartSkillHit(weaponIndex);
        weapons[weaponIndex].StartSkillHit();
    }


    protected void StopSkillHit(int weaponIndex)
    {
        skillOwner.StopSkillHit(weaponIndex);
        weapons[weaponIndex].StopSkillHit();
    }
    protected void SkillCanSwitch()
    {

        skillOwner.SkillCanSwitch();
    }
    public void SwitchWeaponParentObject(int index)
    {
        skillOwner.SwitchWeaponParentObject(index);
    }

    private Coroutine moveForwardCoroutine;
    IEnumerator MoveForwardCoroutine()
    {
        while (true)
        {
            Vector3 vector3 = transform.forward * Time.deltaTime * (1.5f / 0.15f);
            player.CharacterController.Move(vector3);
            yield return null;
        }

    }
    private Coroutine airAttackCoroutine;
    IEnumerator AirAttack()
    {
        while (true)
        {
            Vector3 vector3 = -transform.up * Time.deltaTime * (2 / 0.15f);
            player.CharacterController.Move(vector3);
            yield return null;
        }

    }


    public void MoveForWard()
    {
        moveForwardCoroutine = StartCoroutine(MoveForwardCoroutine());
    }
    public void endForWard()
    {
        if (moveForwardCoroutine != null)
        {
            StopCoroutine(moveForwardCoroutine);
            moveForwardCoroutine = null; // 清除引用，防止再次调用
        }
    }
    public void AirAttackMove()
    {
        airAttackCoroutine= StartCoroutine(AirAttack());
    }
    public void EndAirAttackMove()
    {
        if (airAttackCoroutine != null)
        {
            StopCoroutine(airAttackCoroutine);
            airAttackCoroutine = null; // 清除引用，防止再次调用
        }
    }
    #endregion
}
