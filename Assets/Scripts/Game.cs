using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Game : MonoBehaviourPun,IConnectionCallbacks
{
    public static Game Instance { get; private set; }

    public string characterName;

    public SelectPanel selectPanel;
 


    private void Awake()
    {

        Application.targetFrameRate = 120;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保留此对象，使其在加载新场景时不被销毁
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 如果已经存在一个实例，销毁这个新创建的实例
            return;
        } // 保留此对象，使其在加载新场景时不被销毁

        /* DontDestroyOnLoad(gameObject);*/
        //设置发送  接收消息频率 降低延迟
        PhotonNetwork.SendRate = 200;
        PhotonNetwork.SerializationRate = 200;

    }
   

    // Start is called before the first frame update
    void Start()
    {
        //显示登录界面
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnectedToMaster()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        StartCoroutine(ShowMessage(3f));
    }
    IEnumerator ShowMessage(float waitTime)
    {
        MaskPanel maskPanel = (MaskPanel)UIManager.Instance.OpenPanel(UIConst.MaskUI);
        maskPanel.ShowMessage("连接超时，自动断开,3s回到主页面");
        yield return new WaitForSeconds(waitTime);
        UIManager.Instance.CloseAllPanel();
        SceneLoader.Instance.LoadSceneAsync("Login");
    }
    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        throw new System.NotImplementedException();
    }
}