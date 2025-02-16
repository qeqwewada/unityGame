using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Chat : MonoBehaviourPunCallbacks
{
    public ChatManager chatManager;
    public ScrollRect scrollRect;
    [PunRPC]
    void SendMsg(string message,string name)
    {
        Debug.Log("Received message: " + message);
        GameObject obj = Instantiate(chatManager.msg, chatManager.content);
        obj.SetActive(true);
        obj.transform.Find("msgContent").GetComponent<Text>().text =name+":"+ message;
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatManager.content.GetComponent<RectTransform>());
        ScrollToBottom();
        // ���������Ӹ�����߼����������UI��ʾ��Ϣ
    }
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // ȷ�����в��ָ������
        // �������λ�ã�0�ǵײ���1�Ƕ���
        float verticalPosition = 0f;
        // ���ù�����ͼ��λ��
        scrollRect.verticalNormalizedPosition = verticalPosition;
   
    }

    // ������Ϣ���������
    public void SendChatMessage()
    {
        photonView.RPC("SendMsg", RpcTarget.All, chatManager.inputField.text,PhotonNetwork.LocalPlayer.NickName);
    }


}
