using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����ţ�ʹ��vector2,���Ų�Y���Ӱ��
//ʹ�÷������ڶ���֡�¼��е���MoveForward���������ݸ�ʽ��dis#time����ʱ����
public class CustomizedTransform : MonoBehaviour
{
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float moveSpeed;
    private float moveTime;

    public static CustomizedTransform instance;

    private float obstacleDetectionDir = 0;


    public Transform collisionDetectionRay;


    


    private void Start()
    {
        instance = this;
       
    }

    //����ϰ�
    public void MoveForward(string param)
    {
  
/*            StopCoroutine(MoveCoroutine());
            isMoving = false;

        if (!isMoving)
        {
            // ��������*/
            string[] stringArray = param.Split('#');
            float distance = float.Parse(stringArray[0]);
            //�ó���ײ��ⷽ��
            if (distance > 0)
                obstacleDetectionDir = 1;
            else
                obstacleDetectionDir = -1;

            moveTime = float.Parse(stringArray[1]);

            // �����ٶȺ�Ŀ��λ��
            moveSpeed = distance / moveTime;
            startPosition = transform.position;
            endPosition = transform.position + transform.forward * distance;

            // ����Э��
            StartCoroutine(MoveCoroutine());
   /*     }*/
    }

    //�����ϰ�����λ��
    public void MoveForwardIgnoreObstacle(string param)
    {
        if (isMoving == true)
        {
            StopCoroutine(MoveCoroutine());
            isMoving = false;
        }

        if (!isMoving)
        {
            // ��������
            string[] stringArray = param.Split('#');
            float distance = float.Parse(stringArray[0]);
            //�ó���ײ��ⷽ��
            if (distance > 0)
                obstacleDetectionDir = 1;
            else
                obstacleDetectionDir = -1;

            moveTime = float.Parse(stringArray[1]);

            // �����ٶȺ�Ŀ��λ��
            moveSpeed = distance / moveTime;
            startPosition = transform.position;
            endPosition = transform.position + transform.forward * distance;

            // ����Э��
            StartCoroutine(MoveCoroutineIgnoreObstacle());
        }
    }


    //���ԣ�����ͨ��
    private IEnumerator MoveCoroutine()
    {
        isMoving = true;
        float currentTime = 0f;

        float actualMoveTime = 0f;

        while (currentTime < moveTime)
        {
            currentTime += Time.deltaTime;

            // ������ײ��⣬���ǰ��0.3���Ƿ����ϰ���
            if (CheckObstacle())
            {
                //���ϰ��ﲻ������
            }
            else
            {
                // û���ϰ�������ƶ�
                //ʵ���ƶ�ʱ���ʱ
                actualMoveTime += Time.deltaTime;

                // �����ֵϵ��
                float t = actualMoveTime / moveTime;
                // ʹ�ò�ֵ��ƽ���ƶ�
                Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, t);
                transform.position = nextPosition;
                Debug.Log("1");
            }
            yield return null;
        }

        // �ƶ���ɣ����ò���
        isMoving = false;
    }

    private IEnumerator MoveCoroutineIgnoreObstacle()
    {
        isMoving = true;
        float currentTime = 0f;


        while (currentTime < moveTime)
        {
            currentTime += Time.deltaTime;


            // �����ֵϵ��
            float t = currentTime / moveTime;
            // ʹ�ò�ֵ��ƽ���ƶ�
            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, t);
            transform.position = nextPosition;
            yield return null;
        }

        // �ƶ���ɣ����ò���
        isMoving = false;
    }


    private bool CheckObstacle()
    {
        /*        if (Physics.Raycast(transform.position, obstacleDetectionDir * transform.forward, out hit, 1f)
                    || Physics.Raycast(collisionDetectionRay.position, obstacleDetectionDir * collisionDetectionRay.forward, out hit, 1f))*/

        // ������ײ��⣬���ǰ��0.2���Ƿ����ϰ���
        RaycastHit hit;
        if (Physics.Raycast(transform.position, obstacleDetectionDir * transform.forward, out hit, 1f)
            || Physics.Raycast(collisionDetectionRay.position, obstacleDetectionDir * collisionDetectionRay.forward, out hit, 1f))
        {

            Debug.Log("�ϰ�������" + hit.collider.name);
            // ������ϰ������ true
            return true;
        }

        // û���ϰ������ false
        return false;
    }


}
