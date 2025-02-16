using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomItem : MonoBehaviour
{
    public Image image;
    private string playerName;
    public string imageName;
    private bool isReady=false;
    private Transform readyImage;
    private Text nickName;

    public bool IsReady
    {
        get => isReady;
        set
        {
            isReady = value;
            if (isReady)
            {
                readyImage.gameObject.SetActive(true);
            }
            else
            {
                readyImage.gameObject.SetActive(false);
            }
        }
    }

    public string Name { get => playerName; set { playerName = value;
            Debug.Log(nickName+value);
            nickName.text = value;
        } }
    private void Awake()
    {
        nickName = transform.Find("Bg/PlayerName").GetComponent<Text>();
        readyImage = transform.Find("ready");
        image = transform.Find("Image/Character").GetComponent<Image>();
       
    }
    void Start()
    {
        // ������ȷ�� nickName �Ѿ�����ֵ
        if (nickName == null)
        {
            Debug.LogError("nickName is not assigned in the inspector!");
            // ���������Ӵ������߼���������ýű�����ʾ������Ϣ
        }
    }

    public string ImageName { get => imageName; set { imageName = value;

            Texture2D t; // �� switch �ⲿ���� Texture2D ����

            switch (imageName)
            {
                case "XinYe":
                    t = (Texture2D)Resources.Load("Image/XinYe");
                    if (t != null) // �����Դ�Ƿ���سɹ�
                    {
                        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)); // ʹ�� Vector2(0.5f, 0.5f) ��Ϊ���ĵ�
                        image.sprite = temp;
                    }
                    else
                    {
                        Debug.LogError("Failed to load texture: Image/XinYe");
                    }
                    break;

                case "YouXiang":
                    t = (Texture2D)Resources.Load("Image/YouXiang"); // ������Դ·��
                    if (t != null) // �����Դ�Ƿ���سɹ�
                    {
                        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)); // ʹ�� Vector2(0.5f, 0.5f) ��Ϊ���ĵ�
                        image.sprite = temp;
                    }
                    else
                    {
                        Debug.LogError("Failed to load texture: Image/YouXiang");
                    }
                    break;

                default:
                    Debug.LogWarning("Unknown image name: " + imageName);
                    break;
            }
        }
    }
}