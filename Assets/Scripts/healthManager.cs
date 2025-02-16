using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthManager : MonoBehaviour
{

    public Transform targetCamera; // �󶨵��������
    public Vector3 offset = new Vector3(0, 0, 0); // Ѫ��λ��ƫ��

    void LateUpdate()
    {
        if (targetCamera == null)
        {
            // �Զ�����������������δ�ֶ��󶨣�
            targetCamera = Camera.main.transform;
        }

        // ����Ѫ��λ��
        transform.position = transform.parent.position + offset;

        // ��Ѫ��ʼ�����������
        transform.rotation = targetCamera.rotation;
    }

}
