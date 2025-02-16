using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Cinemachine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }
    
    [Header("房间设置")]
    public string gameVersion = "1.0";
    public byte maxPlayersPerRoom = 4;
    public string roomName = "GameRoom";

    [Header("相机配置")]
    public GameObject virtualCameraPrefab;  // 虚拟相机预制体
    public float cameraDistance = 10f;      // 相机距离
    public Vector3 cameraOffset = new Vector3(0, 2, 0);  // 相机偏移
    
    private void Awake()
    {
        SpawnPlayer();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
          
        }
    }


 



    public override void OnJoinedRoom()
    {
        Debug.Log($"加入房间: {PhotonNetwork.CurrentRoom.Name}, 当前人数: {PhotonNetwork.CurrentRoom.PlayerCount}");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // 随机生成位置
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        GameObject player = PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);

        // 只为本地玩家创建相机
        if (player.GetComponent<PhotonView>().IsMine)
        {
            SpawnPlayerCamera(player);
        }
    }

    private void SpawnPlayerCamera(GameObject player)
    {
        // 实例化虚拟相机
        GameObject vcamObj = Instantiate(virtualCameraPrefab);
        CinemachineVirtualCamera virtualCamera = vcamObj.GetComponent<CinemachineVirtualCamera>();
        
        // 配置虚拟相机
        virtualCamera.Follow = player.transform.Find("Kalie/Armature/Hips/LooKTarget");
        virtualCamera.LookAt = player.transform.Find("Kalie/Armature/Hips/LooKTarget");
        
        // 获取或添加相机组件
        var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_TrackedObjectOffset = cameraOffset;
        }

        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_FollowOffset = new Vector3(0, cameraOffset.y, -cameraDistance);
        }

        // 设置相机优先级
        virtualCamera.Priority = 10;
        
        // 将相机引用传递给玩家控制器
        CharacterBase characterBase = player.GetComponent<CharacterBase>();
        if (characterBase != null)
        {
            characterBase.SetVirtualCamera(virtualCamera);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"断开连接: {cause}");
    }
} 