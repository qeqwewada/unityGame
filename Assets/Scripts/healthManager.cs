using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthManager : MonoBehaviour
{

    public Transform targetCamera; // 绑定的主摄像机
    public Vector3 offset = new Vector3(0, 0, 0); // 血条位置偏移

    void LateUpdate()
    {
        if (targetCamera == null)
        {
            // 自动查找主摄像机（如果未手动绑定）
            targetCamera = Camera.main.transform;
        }

        // 更新血条位置
        transform.position = transform.parent.position + offset;

        // 让血条始终面向摄像机
        transform.rotation = targetCamera.rotation;
    }

}
