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
        lobby = new TypedLobby("tpsLobby", LobbyType.SqlLobby); //1.��������  2.�������ͣ���������
        //�������
        PhotonNetwork.JoinLobby(lobby);
   

    }

  
    public override void OnJoinedLobby()
    {
        Debug.Log("�������...");
        RefreshRoom();
    }

    private void CreateRoom()
    {
        UIManager.Instance.OpenPanel(UIConst.CreateRoom);
    }
    private void RefreshRoom()
    {
        MaskPanel maskPanel=(MaskPanel) UIManager.Instance.OpenPanel(UIConst.MaskUI);
        maskPanel.ShowMessage("ˢ����...");
        PhotonNetwork.GetCustomRoomList(lobby, "1");
    }
    //ˢ�·����Ļص�
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
        Debug.Log("����ˢ��");

        ClearRoomList();
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject obj = Instantiate(roomPrefab, contentTf);
            obj.SetActive(true);
            string roomName = roomList[i].Name;
            int num = roomList[i].PlayerCount;
            obj.transform.Find("roomName").GetComponent<Text>().text = "������"+roomName;
            obj.transform.Find("playerCount").GetComponent<Text>().text = "��ǰ����"+num.ToString();
            obj.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Debug.Log(roomName);
                //���뷿��
                MaskPanel maskPanel = (MaskPanel)UIManager.Instance.OpenPanel(UIConst.MaskUI);
                maskPanel.ShowMessage("������...");
                
                PhotonNetwork.JoinRoom(roomName); //���뷿��
            });
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //���뷿��ʧ��
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
    }
    public override void OnJoinedRoom()
    {
        //���뷿��ص�
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
        UIManager.Instance.OpenPanel(UIConst.RoomUI);
        
    }

    private void CloseUI()
    {
        if (PhotonNetwork.IsConnected)
        {
            // �Ͽ�����
            PhotonNetwork.Disconnect();
        }
        UIManager.Instance.ClosePanel(UIConst.RoomsUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
