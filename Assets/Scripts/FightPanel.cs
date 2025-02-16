using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class FightPanel : BasePanel
{
    public Transform currentPlayer;
    public Text bulletCount; 
    public Image character;
    public Image Weapon;
    public Image HP;
    public Image shieldValue;
    public Button stopBtn;
    public Button JumpBtn;
    public Button aimBtn;

    private void Start()
    {
        stopBtn.onClick.AddListener(OnStop);
        JumpBtn.onClick.AddListener(OnJump);
        aimBtn.onClick.AddListener(OnAim);
    }


    private void OnAim()
    {
        if(currentPlayer.TryGetComponent(out StarterAssetsInputs starterAssets))
        {
            starterAssets.aim =! starterAssets.aim;
        }
    }

    private void OnJump()
    {
        if (currentPlayer.TryGetComponent(out StarterAssetsInputs starterAssets))
        {
            starterAssets.jump = true;
        }
    }

    private void OnStop()
    {
        UIManager.Instance.OpenPanel(UIConst.StopPanel);
    }
}
