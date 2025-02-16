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

    // ���ô˷�������ʼ�첽���س���
    public void LoadSceneAsync(string sceneName)
    {
        UIManager.Instance.OpenPanel(UIConst.LoadUI);
        // ��ʼ�첽���س���������ȡAsyncOperation����
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        
    }

    // ��Update�����м����ؽ���
    public float GetProgress()
    {
        if (asyncOperation != null)
        {
            // ��ȡ���ؽ��ȣ�����ֵ��Χ��0��1
            float progress = asyncOperation.progress;
            return progress;
            // ��ӡ����ֵ�����߸��ݽ��ȸ���UI��
            Debug.Log("Loading Progress: " + (progress * 100) + "%");

            // ����������
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
