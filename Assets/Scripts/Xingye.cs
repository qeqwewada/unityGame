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

        // ���� RectTransform �Ĵ�СΪ��ʼֵ
        rectTransform.sizeDelta = new Vector2(800, 800); // ����ĳ�ʼֵ���Ը��������������

        // ����ԭʼ����Ŀ�߱�
        float textureAspect = (float)texture.width / texture.height;

        // ��ȡ��ǰ Image �Ŀ�߱�
        float imageAspect = rectTransform.rect.width / rectTransform.rect.height;

        // ���� Image �Ĵ�С�Ա���ԭʼ��߱�
        if (imageAspect > textureAspect)
        {
            // ��� Image �Ŀ�߱ȴ�������Ŀ�߱ȣ������߶�
            float newHeight = rectTransform.rect.width / textureAspect;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
        }
        else if (imageAspect < textureAspect)
        {
            // ��� Image �Ŀ�߱�С������Ŀ�߱ȣ��������
            float newWidth = rectTransform.rect.height * textureAspect;
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table.Add("Character", "XinYe");

        PhotonNetwork.LocalPlayer.SetCustomProperties(table); //�����Զ������
        Texture2D t= (Texture2D)Resources.Load("Image/BigXinYe");
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        Character.sprite = temp;
        MaintainAspectRatio(Character, t);
        // ��ȡ SpriteRenderer ���

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
