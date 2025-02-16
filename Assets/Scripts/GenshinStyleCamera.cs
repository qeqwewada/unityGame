using UnityEngine;

public class GenshinStyleCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;          // 玩家角色
    public Transform cameraPivot;      // 相机旋转支点
    public LayerMask collisionLayers;  // 碰撞检测层

    [Header("Settings")]
    public float distance = 5f;        // 默认距离
    public float minDistance = 1f;     // 最小镜头距离
    public float maxDistance = 10f;    // 最大镜头距离
    public float zoomSpeed = 5f;       // 缩放速度
    public float rotationSpeed = 200f; // 旋转速度
    public float verticalAngleLimit = 80f; // 垂直角度限制

    [Header("Smoothing")]
    public float positionSmoothTime = 0.1f; // 位置平滑时间
    public float rotationSmoothTime = 0.1f; // 旋转平滑时间

    private Vector3 _currentRotation;
    private float _currentZoom;
    private Vector3 _positionVelocity;
    private float _rotationVelocityX;
    private float _rotationVelocityY;

    private void Start()
    {
        _currentRotation = cameraPivot.eulerAngles;
        _currentZoom = distance;
    }

    private void LateUpdate()
    {
        if (!target) return;

        HandleInput();
        UpdateCameraPosition();
        HandleCollision();
    }

    private void HandleInput()
    {
        // 鼠标控制旋转
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            _currentRotation.x -= mouseY;
            _currentRotation.y += mouseX;

            // 限制垂直角度
            _currentRotation.x = Mathf.Clamp(_currentRotation.x, -verticalAngleLimit, verticalAngleLimit);
        }

        // 鼠标滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _currentZoom = Mathf.Clamp(_currentZoom - scroll * zoomSpeed, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        // 平滑旋转
        Quaternion targetRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
        cameraPivot.rotation = Quaternion.Slerp(
            cameraPivot.rotation,
            targetRotation,
            rotationSmoothTime * Time.deltaTime
        );

        // 计算目标位置
        Vector3 targetPosition = target.position -
                                cameraPivot.forward * _currentZoom;

        // 平滑移动
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _positionVelocity,
            positionSmoothTime
        );

        // 始终面向目标
        transform.LookAt(target.position);
    }

    private void HandleCollision()
    {
        RaycastHit hit;
        Vector3 dir = (transform.position - target.position).normalized;
        float targetDistance = _currentZoom;

        if (Physics.SphereCast(
            target.position,
            0.3f,
            dir,
            out hit,
            _currentZoom,
            collisionLayers))
        {
            targetDistance = hit.distance - 0.2f; // 保持微小距离防止穿模
        }

        // 应用碰撞后的实际距离
        transform.position = target.position - dir * targetDistance;
    }

    // 可视化调试
    private void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.position, transform.position);
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}