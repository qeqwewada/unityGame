using UnityEngine;

public class GenshinStyleCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;          // ��ҽ�ɫ
    public Transform cameraPivot;      // �����ת֧��
    public LayerMask collisionLayers;  // ��ײ����

    [Header("Settings")]
    public float distance = 5f;        // Ĭ�Ͼ���
    public float minDistance = 1f;     // ��С��ͷ����
    public float maxDistance = 10f;    // ���ͷ����
    public float zoomSpeed = 5f;       // �����ٶ�
    public float rotationSpeed = 200f; // ��ת�ٶ�
    public float verticalAngleLimit = 80f; // ��ֱ�Ƕ�����

    [Header("Smoothing")]
    public float positionSmoothTime = 0.1f; // λ��ƽ��ʱ��
    public float rotationSmoothTime = 0.1f; // ��תƽ��ʱ��

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
        // ��������ת
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            _currentRotation.x -= mouseY;
            _currentRotation.y += mouseX;

            // ���ƴ�ֱ�Ƕ�
            _currentRotation.x = Mathf.Clamp(_currentRotation.x, -verticalAngleLimit, verticalAngleLimit);
        }

        // ����������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _currentZoom = Mathf.Clamp(_currentZoom - scroll * zoomSpeed, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        // ƽ����ת
        Quaternion targetRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
        cameraPivot.rotation = Quaternion.Slerp(
            cameraPivot.rotation,
            targetRotation,
            rotationSmoothTime * Time.deltaTime
        );

        // ����Ŀ��λ��
        Vector3 targetPosition = target.position -
                                cameraPivot.forward * _currentZoom;

        // ƽ���ƶ�
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _positionVelocity,
            positionSmoothTime
        );

        // ʼ������Ŀ��
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
            targetDistance = hit.distance - 0.2f; // ����΢С�����ֹ��ģ
        }

        // Ӧ����ײ���ʵ�ʾ���
        transform.position = target.position - dir * targetDistance;
    }

    // ���ӻ�����
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