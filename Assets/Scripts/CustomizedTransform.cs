using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//需调优，使用vector2,来排查Y轴的影响
//使用方法，在动画帧事件中调用MoveForward方法，数据格式：dis#time，定时变速
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

    //检测障碍
    public void MoveForward(string param)
    {
  
/*            StopCoroutine(MoveCoroutine());
            isMoving = false;

        if (!isMoving)
        {
            // 解析参数*/
            string[] stringArray = param.Split('#');
            float distance = float.Parse(stringArray[0]);
            //得出碰撞检测方向
            if (distance > 0)
                obstacleDetectionDir = 1;
            else
                obstacleDetectionDir = -1;

            moveTime = float.Parse(stringArray[1]);

            // 计算速度和目标位置
            moveSpeed = distance / moveTime;
            startPosition = transform.position;
            endPosition = transform.position + transform.forward * distance;

            // 启动协程
            StartCoroutine(MoveCoroutine());
   /*     }*/
    }

    //遇到障碍继续位移
    public void MoveForwardIgnoreObstacle(string param)
    {
        if (isMoving == true)
        {
            StopCoroutine(MoveCoroutine());
            isMoving = false;
        }

        if (!isMoving)
        {
            // 解析参数
            string[] stringArray = param.Split('#');
            float distance = float.Parse(stringArray[0]);
            //得出碰撞检测方向
            if (distance > 0)
                obstacleDetectionDir = 1;
            else
                obstacleDetectionDir = -1;

            moveTime = float.Parse(stringArray[1]);

            // 计算速度和目标位置
            moveSpeed = distance / moveTime;
            startPosition = transform.position;
            endPosition = transform.position + transform.forward * distance;

            // 启动协程
            StartCoroutine(MoveCoroutineIgnoreObstacle());
        }
    }


    //测试，测试通过
    private IEnumerator MoveCoroutine()
    {
        isMoving = true;
        float currentTime = 0f;

        float actualMoveTime = 0f;

        while (currentTime < moveTime)
        {
            currentTime += Time.deltaTime;

            // 进行碰撞检测，检测前方0.3米是否有障碍物
            if (CheckObstacle())
            {
                //有障碍物不做操作
            }
            else
            {
                // 没有障碍物，继续移动
                //实际移动时间计时
                actualMoveTime += Time.deltaTime;

                // 计算插值系数
                float t = actualMoveTime / moveTime;
                // 使用插值来平滑移动
                Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, t);
                transform.position = nextPosition;
                Debug.Log("1");
            }
            yield return null;
        }

        // 移动完成，重置参数
        isMoving = false;
    }

    private IEnumerator MoveCoroutineIgnoreObstacle()
    {
        isMoving = true;
        float currentTime = 0f;


        while (currentTime < moveTime)
        {
            currentTime += Time.deltaTime;


            // 计算插值系数
            float t = currentTime / moveTime;
            // 使用插值来平滑移动
            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, t);
            transform.position = nextPosition;
            yield return null;
        }

        // 移动完成，重置参数
        isMoving = false;
    }


    private bool CheckObstacle()
    {
        /*        if (Physics.Raycast(transform.position, obstacleDetectionDir * transform.forward, out hit, 1f)
                    || Physics.Raycast(collisionDetectionRay.position, obstacleDetectionDir * collisionDetectionRay.forward, out hit, 1f))*/

        // 进行碰撞检测，检测前方0.2米是否有障碍物
        RaycastHit hit;
        if (Physics.Raycast(transform.position, obstacleDetectionDir * transform.forward, out hit, 1f)
            || Physics.Raycast(collisionDetectionRay.position, obstacleDetectionDir * collisionDetectionRay.forward, out hit, 1f))
        {

            Debug.Log("障碍物名：" + hit.collider.name);
            // 如果有障碍物，返回 true
            return true;
        }

        // 没有障碍物，返回 false
        return false;
    }


}
