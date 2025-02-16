using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class UIManager
{
    private static UIManager _instance;
    private Transform _uiRoot;
    // 路径配置字典
    private Dictionary<string, string> pathDict;
    // 预制件缓存字典
    private Dictionary<string, GameObject> prefabDict;
    // 已打开界面的缓存字典
    public Dictionary<string, BasePanel> panelDict;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    _uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    _uiRoot = new GameObject("Canvas").transform;
                }
            };
            return _uiRoot;
        }
    }

    private UIManager()
    {
        InitDicts();
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();

        pathDict = new Dictionary<string, string>()
        {
            {UIConst.PackagePanel, "Package/PackagePanel"},
            { UIConst.LoginUI,"Login/LoginUI"},
            {UIConst.LoadUI,"Load/LoadUI" },
            {UIConst.MaskUI,"Mask/MaskUI" },
            {UIConst.RoomsUI,"Room/RoomsUI" },
            {UIConst.CreateRoom,"Room/CreateRoom" },
            {UIConst.RoomUI,"Room/RoomUI" },
            {UIConst.NameUI,"Name/NamePanel" },
            {UIConst.StopPanel,"Stop/StopPanel" },
        };
    }

    public BasePanel GetPanel(string name)
    {
        BasePanel panel = null;
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out panel))
        {
            return panel;
        }
        return null;
    }
  
    public BasePanel OpenPanel(string name)
    {
       
      
        BasePanel panel = null;
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面已打开: " + name);
            return null;
        }

        // 检查路径是否配置
        string path = "";
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.Log("界面名称错误，或未配置路径: " + name);
            return null;
        }

        // 使用缓存预制件
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefab/" + path;

            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }

        // 打开界面
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        Debug.Log(panel);
        panel.OpenPanel(name);
        return panel;
    }

    public bool ClosePanel(string name)
    {
       

        BasePanel panel = null;
        if (!panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面未打开: " + name);
            return false;
        }

        panel.ClosePanel();
        // panelDict.Remove(name);
        return true;
    }

    public void CloseAllPanel()
    {


        // 创建一个副本
        var panelDictCopy = new Dictionary<string, BasePanel>(panelDict);

        // 遍历副本
        foreach (KeyValuePair<string, BasePanel> panel in panelDictCopy)
        {
            panel.Value.ClosePanel();
        }


    }


}

public class UIConst
{
    // menu panels

    public const string PackagePanel = "PackagePanel";
    public const string LoginUI = "LoginUI";
    public const string LoadUI = "LoadUI";
    public const string MaskUI = "MaskUI";
    public const string RoomsUI = "RoomsUI";
    public const string CreateRoom = "CreateRoomUI";
    public const string RoomUI = "RoomUI";
    public const string NameUI = "NameUI";
    public const string StopPanel = "StopPanel";
}

