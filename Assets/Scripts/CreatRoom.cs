using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class CreatRoom : BasePanel
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private InputField inputField;
   

    void Start()
    {
        closeBtn.onClick.AddListener(Onclose);
        confirmBtn.onClick.AddListener(OnConfirm);
        inputField.text = "room_" + Random.Range(1, 999);
    }

    private void OnConfirm()
    {
        MaskPanel maskPanel = (MaskPanel)UIManager.Instance.OpenPanel(UIConst.MaskUI);
        maskPanel.ShowMessage("������...");
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 8;  //������������
        PhotonNetwork.CreateRoom(inputField.text, room);
    }

    private void Onclose()
    {
        UIManager.Instance.ClosePanel(UIConst.CreateRoom);
    }
     public override void OnCreatedRoom()
    {
        UIManager.Instance.ClosePanel(UIConst.MaskUI);
        UIManager.Instance.ClosePanel(UIConst.CreateRoom);
        UIManager.Instance.OpenPanel(UIConst.RoomUI);
        //��ʾ����UI
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
