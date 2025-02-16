using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SelectPanel : BasePanel, IInRoomCallbacks
{
    [SerializeField] private Button exitRoom;
    [SerializeField] private Button ready;
    [SerializeField] private Text readyText;
    [SerializeField] private Button startBtn;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject roomItem;
   
    private AsyncOperation asyncOperation;
    public List<RoomItem> roomItems;
    protected  override void Awake()
    {
        gameObject.AddComponent<PhotonView>();
       
        roomItems = new List<RoomItem>();
        exitRoom.onClick.AddListener(OnExit);
        ready.onClick.AddListener(OnReady);
        startBtn.onClick.AddListener(OnStart);
        PhotonNetwork.AutomaticallySyncScene = true;
        GameObject.Find("Game").GetComponent<Game>().selectPanel = this;
    }
    void Start()
    {
        
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            CreateRoomItem(p);
        }
    }

 
    private void OnStart()
    {
        /*  // 事件的唯一代码
          PhotonNetwork.RaiseEvent(eventCode, null, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);*/


        LoginPanel loginPanel = GameObject.Find("LoginUI").GetComponent<LoginPanel>();
        loginPanel.setName();
        PhotonNetwork.LoadLevel("testScene");
        

    }
  
  

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return null; // 等待一帧，确保Photon完成其加载过程

        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false; // 暂时阻止场景激活

        while (!asyncOperation.isDone)
        {
            // 更新加载进度
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // 当加载完成时，可以设置 allowSceneActivation 为 true 来激活场景
            if (progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    private void OnReady()
    {
        RoomItem item = roomItems.Find((RoomItem _item) => { return PhotonNetwork.NickName == _item.Name; });
        item.IsReady =!item.IsReady;
        if (item.IsReady)
        {
            readyText.text = "已准备";
        }
        else
        {
            readyText.text = "未准备";
        }

        
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();

        table.Add("IsReady", item.IsReady);

        PhotonNetwork.LocalPlayer.SetCustomProperties(table);

    }

    private void OnExit()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            TypedLobby lobby;
            lobby = new TypedLobby("tpsLobby", LobbyType.SqlLobby);
            PhotonNetwork.JoinLobby(lobby);
        }
        UIManager.Instance.ClosePanel(UIConst.RoomUI);

    }

  
    public void DeleteRoomItem(Player p)
    {
        RoomItem item = roomItems.Find((RoomItem _item) => { return p.NickName == _item.Name; });
        if (item != null)
        {
            Destroy(item.gameObject);
            roomItems.Remove(item);
        }
    }
    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        RoomItem item = roomItems.Find((_item) => { return _item.Name == targetPlayer.NickName; });
        if (item != null && changedProps != null)
        {
            if (changedProps.ContainsKey("IsReady") && changedProps["IsReady"] != null)
            {
               
                if (changedProps["IsReady"] is bool)
                {
                    item.IsReady = (bool)changedProps["IsReady"];
                    Debug.Log("propertiesChanged");
                }
                else
                {
                    Debug.LogError("changedProps['IsReady'] is not a boolean value.");
                }
               
            }
          
            if (changedProps.ContainsKey("Character") && changedProps["Character"] != null)
            {
                item.ImageName = (string)changedProps["Character"];
            }
        }

        //如果是主机玩家判断所有玩家的准备状态
        if (PhotonNetwork.IsMasterClient)
        {
            bool isAllReady = true;
            for (int i = 0; i < roomItems.Count; i++)
            {
                if (roomItems[i].IsReady == false)
                {
                    isAllReady = false;
                    break;
                }
            }
            startBtn.gameObject.SetActive(isAllReady); //开始按钮是否显示
        }
    }



    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreateRoomItem(newPlayer);
    }
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        DeleteRoomItem(otherPlayer);
    }
    private void OnEnable()
    {

        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void CreateRoomItem(Player p)
    {
        GameObject obj = Instantiate(roomItem, content);
        obj.SetActive(true);
        RoomItem item = obj.AddComponent<RoomItem>();
        Debug.Log(p.NickName);
        item.Name = p.NickName;  
        roomItems.Add(item);
        Debug.Log(roomItems.Count);

        object val;
        if (p.CustomProperties.TryGetValue("IsReady", out val))
        {
            item.IsReady = (bool)val;
        }
        if (p.CustomProperties.TryGetValue("Character", out val))
        {
            item.ImageName = (string)val;
        }
    }


  
}
