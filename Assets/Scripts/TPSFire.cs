using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using StarterAssets;
using Photon.Pun;
using UnityEngine.UI;


public class TPSFire : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera followCamera;
    private StarterAssetsInputs starterAssets;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float normalSensitivity;
    private ThirdPersonController TPSController;
    public LayerMask aimColliderMask;
    Vector2 screenCenterPoint;
    [SerializeField] private float shootDistance = 200f;
  /*  [SerializeField] private Transform debugTransform;*/
    [SerializeField] private Transform bullet;
    [SerializeField] private Transform spawnBulletPos;
    [SerializeField] private Animator animator;
    private float currentHP;
    [SerializeField] private float maxHP=100; 
    private float shieldValue;
    [SerializeField] private float maxshieldValue=100;
    public int bulletCount;
    public int maxBulletCount=5;
    public float shootCD=0.1f;
    private float currentTime;
    public AudioClip reloadClip;
    public AudioClip shootClip;
    public TwoBoneIKConstraint constraint;
    public TwoBoneIKConstraint rightHandConstraint;
    public Avatar humanAvatar;
    public Avatar avatar;
    public Transform LeftHandle;
    public Transform rightHandle;
    public Transform weapon;
    public FightPanel fightPanel;
    public string name;
    public CinemachineImpulseSource impulseSource;
    private bool isDie = false;
    private bool isLoad = false;
    [SerializeField] private Transform canvas;
    [SerializeField] private Image sheid3d;
    [SerializeField] private Image HP3d;

    public int BulletCount { get => bulletCount; set { bulletCount = value;
            fightPanel.bulletCount.text = bulletCount.ToString() + "/" + maxBulletCount;
            if (bulletCount == 0)
            {   
                animator.SetTrigger("load");
                AudioSource.PlayClipAtPoint(reloadClip, transform.position);
            
                BulletCount = maxBulletCount;
            }
        } }

    public float CurrentHP { get => currentHP; set { currentHP = value;
   
            if (photonView.IsMine)
            {
                fightPanel.HP.fillAmount = currentHP / maxHP;
            }
            else
            {
                HP3d .fillAmount= currentHP / maxHP;
            }
        
        } }
    public float ShieldValue { get => shieldValue; set { shieldValue = value;
            if (photonView.IsMine)
            {
                fightPanel.shieldValue.fillAmount = shieldValue / maxshieldValue;
            }
            else
            {
                sheid3d.fillAmount = shieldValue / maxshieldValue;
            }
        
        } }

    // Start is called before the first frame update
    private void Awake()
    {
      
    }
    void Start()
    {
       
        if (photonView.IsMine)
        {
            GameObject aimCamera = GameObject.Find("aimCamera");
            aimVirtualCamera = aimCamera.GetComponent<CinemachineVirtualCamera>();
            followCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();

            aimVirtualCamera.Follow = transform.Find("PlayerCameraRoot");
            followCamera.Follow = transform.Find("PlayerCameraRoot");
            aimCamera.SetActive(false);
        }
        currentTime = shootCD;
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        TPSController = GetComponent<ThirdPersonController>();
        starterAssets = GetComponent<StarterAssetsInputs>();
        fightPanel = GameObject.Find("FightUI").GetComponent<FightPanel>();
        fightPanel.currentPlayer = gameObject.transform;
        BulletCount = maxBulletCount;
        currentPos = transform.position;
        currentRotation = transform.rotation;
        CurrentHP = maxHP;
        ShieldValue = maxshieldValue;
        if (photonView.IsMine)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        AnimatorStateInfo currentAnim = animator.GetCurrentAnimatorStateInfo(1);
   /*     if (animator.GetCurrentAnimatorStateInfo(1).IsName("load") && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.9f)
        {
            rightHandConstraint.weight = 0;
            *//*weapon.SetParent(rightHandle);*//*

        }*/
 


        Vector3 mouseWorldPosition = Vector3.zero;
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, shootDistance, aimColliderMask))
        {
            mouseWorldPosition = raycastHit.point;
            /*debugTransform.position = raycastHit.point;*/
        }
        else
        {
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenCenterPoint) + ray.direction * shootDistance;
            /*debugTransform.position = Camera.main.ScreenToWorldPoint(screenCenterPoint) + ray.direction * shootDistance;*/
        }
        if (starterAssets.aim)
        {
            
            animator.SetBool("Aim", true);
            Rotate(mouseWorldPosition);
            aimVirtualCamera.gameObject.SetActive(true);
            TPSController.SetSensitivity(aimSensitivity);
        }
        else
        {
            animator.SetBool("Aim", false);
            TPSController.SetRotateOnMove(true);
            aimVirtualCamera.gameObject.SetActive(false);
            TPSController.SetSensitivity(normalSensitivity);
        }
        currentTime += Time.deltaTime;
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("load"))
        {
            constraint.weight = 0;
            rightHandConstraint.weight = 0;
        }
        else
        {
            constraint.weight = 1;
            rightHandConstraint.weight = 1;
        }

        if (starterAssets.shoot)
        {

            if (BulletCount > 0)
            {
                //如果正在播放填充子弹的动作不能开枪
                if (animator.GetCurrentAnimatorStateInfo(1).IsName("load"))
                {
                    return;
                }
                
                if (constraint != null)
                   
                animator.SetBool("Shoot", true);
              /*  if (name == "YouXiang")
                {
                    constraint.weight = 0;
                }*/
                if(currentTime>=shootCD)
                {
                    impulseSource.GenerateImpulse(0.1f);
                    Vector3 aimDir = (mouseWorldPosition - spawnBulletPos.position).normalized;
                    BulletCount--;
                    photonView.RPC("Shoot", RpcTarget.All, aimDir);
                    currentTime = 0;
                }
                Rotate(mouseWorldPosition);
                /* Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);*/
                //播放开火动画
                /*animator.Play("Fire", 1, 0);*/

            }


/*            Vector3 aimDir = (mouseWorldPosition - spawnBulletPos.position).normalized;
            Instantiate(bullet, spawnBulletPos.position, Quaternion.LookRotation(aimDir, Vector3.up));*/
            

        }
        else
        {
            /*if(constraint!=null&&name=="YouXiang")
                constraint.weight = 1f;*/
            if (!starterAssets.aim) TPSController.SetRotateOnMove(true);

            animator.SetBool("Shoot", false);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("load");
            BulletCount = maxBulletCount;
        }
        
    }

    public void GetHit()
    {
        if (isDie == true)
        {
            return;
        }

        //同步所有角色受伤
        photonView.RPC("GetHitRPC", RpcTarget.All);
    }


    public void OnReset()
    {
        //隐藏鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        photonView.RPC("OnResetRPC", RpcTarget.All);
    }

    [PunRPC]
    public void OnResetRPC()
    {
        isDie = false;
        CurrentHP = maxHP;
        if (photonView.IsMine)
        {
            /*Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);*/
        }
    }


    [PunRPC]
    public void GetHitRPC()
    {
        if (shieldValue > 0)
        {
            shieldValue -= 5;
        }
        else
        {
            CurrentHP -= 10;
        }
        
        //扣一滴血
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            animator.SetBool("IsDie", true);
            isDie = true;
        }
        else
        {
            animator.SetBool("GetHit", true);
            impulseSource.GenerateImpulse(0.2f);
        }

        if (photonView.IsMine)
        {
 

            if (CurrentHP == 0)
            {
                Invoke("gameOver", 3);     
            }
        }
    }

    private void gameOver()
    {
        //显示鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //显示失败界面
       /* Game.uiManager.ShowUI<LossUI>("LossUI").onClickCallBack = OnReset;*/
    }



    [PunRPC]
    private void Shoot(Vector3 aimDir)
    {
        Instantiate(bullet, spawnBulletPos.position, Quaternion.LookRotation(aimDir, Vector3.up));
        
        AudioSource.PlayClipAtPoint(shootClip, spawnBulletPos.position, 0.5f);
    }
    public void Rotate(Vector3 mouseWorldPosition)
    {
        TPSController.SetRotateOnMove(false);
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }
    public Vector3 currentPos;
    public Quaternion currentRotation;
    public void UpdateLogic()
    {
        
        transform.position = Vector3.Lerp(transform.position, currentPos, 0.1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, 0.1f);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //发送数据
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            Debug.Log("send:" + transform.position);

        }
        else
        {
       
            currentPos = (Vector3)stream.ReceiveNext();
            currentRotation = (Quaternion)stream.ReceiveNext();
            Debug.Log("Receive:" + currentPos);

        }
    }
}                                                       
