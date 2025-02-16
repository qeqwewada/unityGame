using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : BasePanel
{
    public Image progressBar;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneLoader.Instance.asyncOperation != null)
        {
            
            progressBar.fillAmount = SceneLoader.Instance.GetProgress();
            Debug.Log("Loading Progress: " + (SceneLoader.Instance.asyncOperation.progress * 100) + "%");
            if (SceneLoader.Instance.asyncOperation.progress>0.99f)
            {
                Debug.Log("╪стьмЙЁи");
                
               /* UIManager.Instance.ClosePanel(UIConst.LoadUI);*/
            }
        }
       

    }
}
