using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public  class SceneLoader
{
    private static SceneLoader instance;

    public AsyncOperation asyncOperation;

    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneLoader();
            }
            return instance;
        }
    }

    // 调用此方法来开始异步加载场景
    public void LoadSceneAsync(string sceneName)
    {
        UIManager.Instance.OpenPanel(UIConst.LoadUI);
        // 开始异步加载场景，并获取AsyncOperation对象
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        
    }

    // 在Update方法中检查加载进度
    public float GetProgress()
    {
        if (asyncOperation != null)
        {
            // 获取加载进度，进度值范围是0到1
            float progress = asyncOperation.progress;
            return progress;
            // 打印进度值，或者根据进度更新UI等
            Debug.Log("Loading Progress: " + (progress * 100) + "%");

            // 如果加载完成
            if (asyncOperation.isDone)
            {
                Debug.Log("Scene loaded successfully.");
                UIManager.Instance.ClosePanel(UIConst.LoadUI);
                asyncOperation = null;
            }
        }
        return 0;
    }
}
