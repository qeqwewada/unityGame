using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasePanel : MonoBehaviourPunCallbacks
{
    protected bool isRemove = false;
    protected new string name;
    protected virtual void Awake()
    {
    }

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void OpenPanel(string name)
    {
        GameObject player = GameObject.Find("GameManager");
        GameObject controller = GameObject.Find("Player");
   
        if (player != null&&controller!=null)
        {
           
            MonoManager manager = player.GetComponent<MonoManager>();
            Animator animator = controller.GetComponentInChildren<Animator>();
            
         
            if (manager != null&&animator!=null)
            {
                animator.speed = 0;
                manager.enabled = false;
            }
        }
        else
        {
            Debug.Log("xxx");
        }
        this.name = name;
        SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        

    }

    public virtual void ClosePanel()
    {
        GameObject player = GameObject.Find("GameManager");
        GameObject controller = GameObject.Find("Player");
        // ȷ���ҵ�����Ҷ���
        if (player != null && controller != null)
        {
            // ��ȡplay_Controller���
            MonoManager manager = player.GetComponent<MonoManager>();
            Animator animator = controller.GetComponentInChildren<Animator>();

            // ����ҵ���������������
            if (manager != null && animator != null)
            {
                animator.speed = 1;
                Debug.Log(animator.speed);
                manager.enabled = true;
            }
        }
        else
        {
            Debug.Log("xxx");
        }
        isRemove = true;
        
        SetActive(false);
        Destroy(gameObject);
        if (UIManager.Instance.panelDict.ContainsKey(name))
        {
            UIManager.Instance.panelDict.Remove(name);
        }
      

    }

}

