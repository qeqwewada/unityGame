using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPackage : MonoBehaviour
{
    public static void OpenPackagePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            UIManager.Instance.OpenPanel(UIConst.PackagePanel);
        }
    }
}
