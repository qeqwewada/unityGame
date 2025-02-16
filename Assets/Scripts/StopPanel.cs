using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StopPanel : BasePanel
{
    [SerializeField] private Button exit;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button goHomeBtn;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        exit.onClick.AddListener(OnExit);
        continueBtn.onClick.AddListener(OnContinue);
        goHomeBtn.onClick.AddListener(OnGoHome);
    }

    private void OnGoHome()
    {
        if (PhotonNetwork.IsConnected)
        {
            // ¶Ï¿ªÁ¬½Ó
            PhotonNetwork.Disconnect();
        }
        UIManager.Instance.ClosePanel(UIConst.StopPanel);
        SceneLoader.Instance.LoadSceneAsync("Login");
    }

    private void OnContinue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        UIManager.Instance.ClosePanel(UIConst.StopPanel);
    }

    private void OnExit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
