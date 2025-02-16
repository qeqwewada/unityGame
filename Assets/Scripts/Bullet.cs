using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject holePrefab;
    private Rigidbody bulletRigidbody;
    [SerializeField]private  float speed = 20f;
    public float lifeTime = 2f;
    private Vector3 lastPosition;
    public float decalOffset = 0.01f;
    void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }
    void SpawnBulletHole(RaycastHit hit)
    {
        // 生成弹孔并调整位置和旋转
        GameObject bulletHole = Instantiate(holePrefab,
            hit.point + hit.normal * decalOffset,
            Quaternion.LookRotation(hit.normal));

        // 附加到物体表面并避免缩放影响
       
        

        // 可选：旋转修正（视预制体方向调整）
        bulletHole.transform.Rotate(Vector3.right, 90);
        Destroy(bulletHole, lifeTime);
    }
    private void Start()
    {
        Destroy(gameObject, lifeTime);
        bulletRigidbody.velocity = transform.forward * speed;
    }
    /*
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.collider.name);
            TPSFire tPS;
            if (collision.collider.TryGetComponent<TPSFire>(out tPS))
            {

                tPS.GetHit();
            }
            else
            {
                // 计算射线方向和距离
                Vector3 direction = transform.position - lastPosition;
                float distance = direction.magnitude;
                direction.Normalize();

                RaycastHit hit;
                if (Physics.Raycast(lastPosition, direction, out hit, distance))
                {
                    // 确保命中同一物体
                    if (hit.collider == collision.collider)
                    {
                        SpawnBulletHole(hit);
                    }
                }
            }
            Destroy(gameObject, 0.01f);
        }
    */
    void OnTriggerEnter(Collider other)
    {
        // 检查是否为可击中层
/*        if (((1 << other.gameObject.layer) & shootableLayer) != 0)
        {*/
            TPSFire tPS;
            if (other.TryGetComponent<TPSFire>(out tPS))
            {

                tPS.GetHit();
            }
            else
            {
                // 计算射线方向和距离
                Vector3 direction = transform.position - lastPosition;
                float distance = direction.magnitude;
                direction.Normalize();

                RaycastHit hit;
                if (Physics.Raycast(lastPosition, direction, out hit, distance))
                {
                    // 确保命中同一物体
                    if (hit.collider == other)
                    {
                        SpawnBulletHole(hit);
                    }
                }
            }
            Destroy(gameObject);

        }


    }
