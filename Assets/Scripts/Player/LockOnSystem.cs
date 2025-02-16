using UnityEngine;
using System.Collections.Generic;

public class LockOnSystem : MonoBehaviour
{
    public float lockOnRange = 10f; // 锁定范围
    public LayerMask targetLayer; // 目标层级
    public Transform currentTarget; // 当前锁定的目标
    public float sphereCastRadius = 0.5f; // 球形检测半径
    public Transform cameraTransform; // 相机Transform
    
    private Player_Controller playerController;

    private void Start()
    {
        playerController = transform.GetComponent<Player_Controller>();
        if (playerController == null)
        {
            Debug.LogError("LockOnSystem: Player_Controller component not found!");
        }
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("LockOnSystem: Camera Transform not set, using Camera.main");
        }
    }

    private void Update()
    {
        if (playerController == null) return;

        // 按下鼠标中键切换锁定状态
        if (Input.GetMouseButtonDown(2))
        {
            if (currentTarget == null)
            {
                FindAndLockTarget();
            }
            else
            {
                UnlockTarget();
            }
        }

        // 如果有锁定目标，让角色朝向目标
        if (currentTarget != null)
        {
            // 计算到目标的方向
            Vector3 targetDirection = currentTarget.position - transform.position;
            targetDirection.y = 0; // 保持y轴不变

            // 如果目标距离过远，解除锁定
            if (targetDirection.magnitude > lockOnRange)
            {
                UnlockTarget();
                return;
            }

            if (targetDirection != Vector3.zero)  // 添加零向量检查
            {
                // 平滑旋转朝向目标
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    playerController.rotateSpeed * Time.deltaTime
                );
            }
        }
    }

    public void FindAndLockTarget()
    {
        // 获取相机前方的射线
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
        
        // 在锁定范围内搜索可锁定目标
        RaycastHit[] hits = Physics.SphereCastAll(
            ray.origin,
            sphereCastRadius,
            ray.direction,
            lockOnRange,
            targetLayer
        );

        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        foreach (RaycastHit hit in hits)
        {
            // 检查是否在视野范围内
            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(cameraTransform.forward, directionToTarget);

            // 如果目标在视野前方（点积大于0）
            if (dotProduct > 0)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hit.transform;
                }
            }
        }

        if (closestTarget != null)
        {
            LockTarget(closestTarget);
        }
    }

    private void LockTarget(Transform target)
    {
        currentTarget = target;
        // 这里可以添加锁定目标时的视觉效果，比如UI指示器等
    }

    public void UnlockTarget()
    {
        currentTarget = null;
        // 清除锁定目标时的视觉效果
    }

    // 在Scene视图中绘制锁定范围（仅在编辑器中可见）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }
} 