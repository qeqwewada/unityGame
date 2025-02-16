using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Xingye : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image Bg;
    [SerializeField] Image Character;
      bool isSelect=false;
    [SerializeField] YouXiang youXiang;

    public bool IsSelect { get => isSelect; set { isSelect = value;
            if (isSelect)
            {
                Bg.color = Color.green;
            }
            else
            {
                Bg.color = Color.blue;
            }
        } }
    void MaintainAspectRatio(Image image, Texture2D texture)
    {
        RectTransform rectTransform = image.rectTransform;

        // 重置 RectTransform 的大小为初始值
        rectTransform.sizeDelta = new Vector2(800, 800); // 这里的初始值可以根据你的需求设置

        // 计算原始纹理的宽高比
        float textureAspect = (float)texture.width / texture.height;

        // 获取当前 Image 的宽高比
        float imageAspect = rectTransform.rect.width / rectTransform.rect.height;

        // 调整 Image 的大小以保持原始宽高比
        if (imageAspect > textureAspect)
        {
            // 如果 Image 的宽高比大于纹理的宽高比，调整高度
            float newHeight = rectTransform.rect.width / textureAspect;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
        }
        else if (imageAspect < textureAspect)
        {
            // 如果 Image 的宽高比小于纹理的宽高比，调整宽度
            float newWidth = rectTransform.rect.height * textureAspect;
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table.Add("Character", "XinYe");

        PhotonNetwork.LocalPlayer.SetCustomProperties(table); //设置自定义参数
        Texture2D t= (Texture2D)Resources.Load("Image/BigXinYe");
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        Character.sprite = temp;
        MaintainAspectRatio(Character, t);
        // 获取 SpriteRenderer 组件

        IsSelect = true;
        youXiang.IsSelect = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelect)
        {
            Bg.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelect)
        {
            Bg.color = Color.blue;
        }
    }

    
}
