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
        // 在这里确保 nickName 已经被赋值
        if (nickName == null)
        {
            Debug.LogError("nickName is not assigned in the inspector!");
            // 这里可以添加错误处理逻辑，比如禁用脚本或显示错误消息
        }
    }

    public string ImageName { get => imageName; set { imageName = value;

            Texture2D t; // 在 switch 外部声明 Texture2D 变量

            switch (imageName)
            {
                case "XinYe":
                    t = (Texture2D)Resources.Load("Image/XinYe");
                    if (t != null) // 检查资源是否加载成功
                    {
                        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)); // 使用 Vector2(0.5f, 0.5f) 作为中心点
                        image.sprite = temp;
                    }
                    else
                    {
                        Debug.LogError("Failed to load texture: Image/XinYe");
                    }
                    break;

                case "YouXiang":
                    t = (Texture2D)Resources.Load("Image/YouXiang"); // 修正资源路径
                    if (t != null) // 检查资源是否加载成功
                    {
                        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)); // 使用 Vector2(0.5f, 0.5f) 作为中心点
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