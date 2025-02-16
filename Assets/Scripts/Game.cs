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
            DontDestroyOnLoad(gameObject); // �����˶���ʹ���ڼ����³���ʱ��������
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ����Ѿ�����һ��ʵ������������´�����ʵ��
            return;
        } // �����˶���ʹ���ڼ����³���ʱ��������

        /* DontDestroyOnLoad(gameObject);*/
        //���÷���  ������ϢƵ�� �����ӳ�
        PhotonNetwork.SendRate = 200;
        PhotonNetwork.SerializationRate = 200;

    }
   

    // Start is called before the first frame update
    void Start()
    {
        //��ʾ��¼����
       
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
        maskPanel.ShowMessage("���ӳ�ʱ���Զ��Ͽ�,3s�ص���ҳ��");
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