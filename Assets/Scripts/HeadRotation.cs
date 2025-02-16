using UnityEngine;

public class Looktarget : MonoBehaviour
{
    public Transform head; // 目标Transform，角色应该看向这里
    private float weight = 0; // IK影响的权重，范围从0到1
    public float turnSpeed=1f;
    public Transform cameraTransform;

    private Animator animator;

    public float Weight { get => weight; 
        set {
            weight = value;
            if (weight < 0) weight = 0;
            if (weight > 1) weight = 1;
                }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }
    private bool IsInCameraView()
    {
       

        // 计算摄像机和目标的方向
        Vector3 toTarget = cameraTransform.position - transform.position;
        // 计算角度
        float angle = Vector3.Angle(transform.forward, toTarget);

        return angle < 63f;
    }
    // 这个函数会在Animator更新IK之前被调用
    private void Update()
    {
        if (!IsInCameraView())
        {
            Weight -= turnSpeed * Time.deltaTime;
        }
        else weight += turnSpeed*Time.deltaTime;
    }
    void OnAnimatorIK(int layerIndex)
    {
        // 确保有一个目标
        if (cameraTransform != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
                // 设置头部看向目标的权重
                animator.SetLookAtWeight(Weight);
                // 设置头部看向的目标位置
                animator.SetLookAtPosition(cameraTransform.position);
            }
        }

    }
}
