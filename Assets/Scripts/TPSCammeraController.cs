using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TPSCameraController : MonoBehaviour
{
    public float horizontalSpeed = 100f;  // 水平旋转速度
    public float verticalSpeed = 80f;     // 垂直旋转速度
    public float verticalAngleLimit = 80f; // 垂直视角限制（防止翻转）

    private CinemachineVirtualCamera vCam;
    private Cinemachine3rdPersonFollow follow;
    private float currentHorizontalAngle; // 当前水平旋转角度
    private float currentVerticalAngle;   // 当前垂直旋转角度

    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        follow = vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        currentHorizontalAngle = transform.eulerAngles.y;
        currentVerticalAngle = follow.VerticalArmLength;
    }

    void Update()
    {
        // 获取输入
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // 水平旋转（角色和相机一起转）
        currentHorizontalAngle += horizontalInput * horizontalSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, currentHorizontalAngle, 0);

        // 垂直旋转（仅调整相机高度）
        currentVerticalAngle -= verticalInput * verticalSpeed * Time.deltaTime;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalAngleLimit, verticalAngleLimit);
        follow.VerticalArmLength = currentVerticalAngle;
    }
}