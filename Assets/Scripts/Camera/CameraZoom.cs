using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] [Range(0f,10f)]private float defaultDistance=2f;
    [SerializeField] [Range(0f, 10f)] private float minDistance=1f;
    [SerializeField] [Range(0f, 10f)] private float maxDistance=6f;

    [SerializeField] [Range(0f, 10f)] private float smoothing = 4f;
    [SerializeField] [Range(0f, 10f)] private float zoomSensitivity= 1f;
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    private CinemachineFramingTransposer transposer;

    private float currentTargetDistance;
    private void Awake()
    {
        transposer = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();
        currentTargetDistance = defaultDistance;
    }
    private void Update()
    {
        Zoom();
    }

    private  void Zoom()
    {
        Debug.Log("qwq");
        float zoomValue = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue,minDistance,maxDistance);
        float currentDistance = transposer.m_CameraDistance;
        if (currentDistance == currentTargetDistance) return;
        float lerpedValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);
        transposer.m_CameraDistance = lerpedValue;
    }
}
