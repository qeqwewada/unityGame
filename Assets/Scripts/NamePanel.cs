using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NamePanel : BasePanel
{
    [SerializeField] InputField nickName;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button quit;
    // Start is called before the first frame update
    void Start()
    {
        confirmBtn.onClick.AddListener(OnCofirm);
        quit.onClick.AddListener(OnQuit);
    }

    private void OnQuit()
    {
        UIManager.Instance.ClosePanel(UIConst.NameUI);
    }

    private void OnCofirm()
    {
        Debug.Log(nickName.text+"qwq");
        PhotonNetwork.LocalPlayer.NickName = nickName.text;
        UIManager.Instance.OpenPanel(UIConst.MaskUI);
        PhotonNetwork.ConnectUsingSettings();
        UIManager.Instance.ClosePanel(UIConst.NameUI);
    }

  
}
