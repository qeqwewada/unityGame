using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TPSCameraController : MonoBehaviour
{
    public float horizontalSpeed = 100f;  // ˮƽ��ת�ٶ�
    public float verticalSpeed = 80f;     // ��ֱ��ת�ٶ�
    public float verticalAngleLimit = 80f; // ��ֱ�ӽ����ƣ���ֹ��ת��

    private CinemachineVirtualCamera vCam;
    private Cinemachine3rdPersonFollow follow;
    private float currentHorizontalAngle; // ��ǰˮƽ��ת�Ƕ�
    private float currentVerticalAngle;   // ��ǰ��ֱ��ת�Ƕ�

    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        follow = vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        currentHorizontalAngle = transform.eulerAngles.y;
        currentVerticalAngle = follow.VerticalArmLength;
    }

    void Update()
    {
        // ��ȡ����
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // ˮƽ��ת����ɫ�����һ��ת��
        currentHorizontalAngle += horizontalInput * horizontalSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, currentHorizontalAngle, 0);

        // ��ֱ��ת������������߶ȣ�
        currentVerticalAngle -= verticalInput * verticalSpeed * Time.deltaTime;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalAngleLimit, verticalAngleLimit);
        follow.VerticalArmLength = currentVerticalAngle;
    }
}