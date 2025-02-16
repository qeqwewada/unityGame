using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

using Photon.Pun;
public class LoginPanel : BasePanel, IConnectionCallbacks
{

   
    [SerializeField] private Transform SinglePlayer;
    [SerializeField] private Transform MultiPlayer;
    [SerializeField] private Transform Settings;
    [SerializeField] private Transform LoginOut;
    [SerializeField] private Transform game;
    override protected void Awake()
    {
        base.Awake();
        InitUI();
        UIManager.Instance.panelDict.Clear();
    }
    void Start()
    {
       
    }
    public void setName()
    {
        photonView.RPC("setCharacterName", RpcTarget.All);
    }
    [PunRPC]
    public void setCharacterName()
    {
        if (Game.Instance == null)
        {
            Debug.LogError("Game.Instance is null");
            return;
        }

        if (Game.Instance.selectPanel == null)
        {
            Debug.LogError("Game.Instance.selectPanel is null");
            return;
        }

        if (Game.Instance.selectPanel.roomItems == null || Game.Instance.selectPanel.roomItems.Count == 0)
        {
            Debug.LogError("roomItems is null or empty");
            return;
        }
        RoomItem item = Game.Instance.selectPanel.roomItems.Find((RoomItem _item) => { return PhotonNetwork.NickName == _item.Name; });
        Game.Instance.characterName = item.imageName;
        Debug.Log(Game.Instance.characterName);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    private void InitUI()
    {
        
        SinglePlayer.GetComponent<Button>().onClick.AddListener(OnSinglePlayer);
        MultiPlayer.GetComponent<Button>().onClick.AddListener(OnMultiPlayer);
        Settings.GetComponent<Button>().onClick.AddListener(OnSet);
        LoginOut.GetComponent<Button>().onClick.AddListener(OnLoginOut);
    }

    private void OnLoginOut()
    {
        Application.Quit();
    }

    private void OnSet()
    {
        
    }

    private void OnMultiPlayer()
    {
        UIManager.Instance.OpenPanel(UIConst.NameUI);
        
    }

    private void OnSinglePlayer()
    {
        
        SceneLoader.Instance.LoadSceneAsync("Game");
    }

    public void OnConnected()
    {
        
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnConnectedToMaster()
    {
        
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
        Debug.Log("连接成功");
        UIManager.Instance.OpenPanel(UIConst.RoomsUI);

    }

    public void OnDisconnected(DisconnectCause cause)
    {
        
    }
/*    IEnumerator ShowMessage(float waitTime)
    {
        MaskPanel maskPanel = (MaskPanel)UIManager.Instance.OpenPanel(UIConst.MaskUI);
        maskPanel.ShowMessage("连接超时，自动断开,3s回到主页面");
        yield return new WaitForSeconds(waitTime);
        UIManager.Instance.CloseAllPanel();
    }*/

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
       
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {

    }
}
