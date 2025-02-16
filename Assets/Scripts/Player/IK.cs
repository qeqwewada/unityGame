using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{


    public Transform leftFootTarget;
    public Transform rightFootTarget;
    public float ikWeight = 1.0f;
    public float distanceToGround = 0.1f;
    public float sphereRadius;
    private Animator animator;
    public float yOffset;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // ��Animator��Update�е���
    void OnAnimatorIK(int layerIndex)
    {
        RaycastHit hit;
        if (Physics.Raycast(leftFootTarget.position, -Vector3.up, out hit, distanceToGround + 1f))
        {
            leftFootTarget.position = hit.point;
            // 根据地面法线设置左脚旋转
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            leftFootTarget.rotation = targetRotation;
        }
        if (Physics.Raycast(rightFootTarget.position, -Vector3.up, out hit, distanceToGround + 1f))
        {
            rightFootTarget.position = hit.point;
            // 根据地面法线设置右脚旋转
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            rightFootTarget.rotation = targetRotation;
        }
        if (animator && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            // ������ŵ�IKλ�ú�Ȩ��
            Debug.DrawLine(leftFootTarget.position - Vector3.right * sphereRadius, leftFootTarget.position + Vector3.right * sphereRadius, Color.blue);
            Debug.DrawLine(leftFootTarget.position - Vector3.forward * sphereRadius, leftFootTarget.position + Vector3.forward * sphereRadius, Color.blue);
            Debug.DrawLine(leftFootTarget.position - Vector3.up * sphereRadius, leftFootTarget.position + Vector3.up * sphereRadius, Color.blue);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position+Vector3.up * yOffset);
            // �����ҽŵ�IKλ�ú�Ȩ��
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position+Vector3.up * yOffset);

         /*   // 启用IK旋转并应用旋转
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ikWeight);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, ikWeight);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);*/
        }
    }

 /*   void Update()
    {
        // ʹ�����߼��Ų�λ��
        RaycastHit hit;
        if (Physics.Raycast(leftFootTarget.position, -Vector3.up, out hit, distanceToGround + 1f))
        {
            Debug.Log(yOffset);
            Debug.DrawRay(leftFootTarget.position, -Vector3.up * (hit.distance), Color.red);
            Debug.DrawLine(hit.point - Vector3.right * sphereRadius, hit.point + Vector3.right * sphereRadius, Color.red);
            Debug.DrawLine(hit.point - Vector3.forward * sphereRadius, hit.point + Vector3.forward * sphereRadius, Color.red);
            Debug.DrawLine(hit.point - Vector3.up * sphereRadius, hit.point + Vector3.up * sphereRadius, Color.red);
            Debug.Log(leftFootTarget.position);
            leftFootTarget.position = hit.point;
            
        }
        if (Physics.Raycast(rightFootTarget.position, -Vector3.up, out hit, distanceToGround + 1f))
        {
            rightFootTarget.position = hit.point;
        }
    }
*/

}
