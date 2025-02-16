using UnityEngine;

public class Looktarget : MonoBehaviour
{
    public Transform head; // Ŀ��Transform����ɫӦ�ÿ�������
    private float weight = 0; // IKӰ���Ȩ�أ���Χ��0��1
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
       

        // �����������Ŀ��ķ���
        Vector3 toTarget = cameraTransform.position - transform.position;
        // ����Ƕ�
        float angle = Vector3.Angle(transform.forward, toTarget);

        return angle < 63f;
    }
    // �����������Animator����IK֮ǰ������
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
        // ȷ����һ��Ŀ��
        if (cameraTransform != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
                // ����ͷ������Ŀ���Ȩ��
                animator.SetLookAtWeight(Weight);
                // ����ͷ�������Ŀ��λ��
                animator.SetLookAtPosition(cameraTransform.position);
            }
        }

    }
}
