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
        // 这里可以添加更多的逻辑，比如更新UI显示消息
    }
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // 确保所有布局更新完成
        // 计算滚动位置，0是底部，1是顶部
        float verticalPosition = 0f;
        // 设置滚动视图的位置
        scrollRect.verticalNormalizedPosition = verticalPosition;
   
    }

    // 发送消息给所有玩家
    public void SendChatMessage()
    {
        photonView.RPC("SendMsg", RpcTarget.All, chatManager.inputField.text,PhotonNetwork.LocalPlayer.NickName);
    }


}
