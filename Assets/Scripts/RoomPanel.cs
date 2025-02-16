using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using Photon.Realtime;

public class RoomPanel : BasePanel
{
    [SerializeField]private Button returnButton;
    [SerializeField]private Button createRoom;
    [SerializeField]private Button refreshRoom;
    [SerializeField] private Transform contentTf;
    [SerializeField] private GameObject roomPrefab;

    private TypedLobby lobby;
    // Start is called before the first frame update
    void Start()
    {
        returnButton.onClick.AddListener(CloseUI);
        createRoom.onClick.AddListener(CreateRoom);
        refreshRoom.onClick.AddListener(RefreshRoom);
        lobby = new TypedLobby("tpsLobby", LobbyType.SqlLobby); //1.大厅名字  2.大厅类型（可搜索）
        //进入大厅
        PhotonNetwork.JoinLobby(lobby);
   

    }

  
    public override void OnJoinedLobby()
    {
        Debug.Log("进入大厅...");
        RefreshRoom();
    }

    private void CreateRoom()
    {
        UIManager.Instance.OpenPanel(UIConst.CreateRoom);
    }
    private void RefreshRoom()
    {
        MaskPanel maskPanel=(MaskPanel) UIManager.Instance.OpenPanel(UIConst.MaskUI);
        maskPanel.ShowMessage("刷新中...");
        PhotonNetwork.GetCustomRoomList(lobby, "1");
    }
    //刷新房间后的回调
    private void ClearRoomList()
    {
        while (contentTf.childCount != 0)
        {
            DestroyImmediate(contentTf.GetChild(0).gameObject);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
        Debug.Log("房间刷新");

        ClearRoomList();
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject obj = Instantiate(roomPrefab, contentTf);
            obj.SetActive(true);
            string roomName = roomList[i].Name;
            int num = roomList[i].PlayerCount;
            obj.transform.Find("roomName").GetComponent<Text>().text = "房间名"+roomName;
            obj.transform.Find("playerCount").GetComponent<Text>().text = "当前人数"+num.ToString();
            obj.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Debug.Log(roomName);
                //加入房间
                MaskPanel maskPanel = (MaskPanel)UIManager.Instance.OpenPanel(UIConst.MaskUI);
                maskPanel.ShowMessage("加入中...");
                
                PhotonNetwork.JoinRoom(roomName); //加入房间
            });
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //加入房间失败
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
    }
    public override void OnJoinedRoom()
    {
        //加入房间回调
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
        UIManager.Instance.OpenPanel(UIConst.RoomUI);
        
    }

    private void CloseUI()
    {
        if (PhotonNetwork.IsConnected)
        {
            // 断开连接
            PhotonNetwork.Disconnect();
        }
        UIManager.Instance.ClosePanel(UIConst.RoomsUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
