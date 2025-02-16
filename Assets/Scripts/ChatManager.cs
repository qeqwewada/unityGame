using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class ChatManager : MonoBehaviourPun
{
    private Chat chat;
    [SerializeField] private Button sendBtn;
    [SerializeField] public InputField inputField;
    [SerializeField] public Transform content;
    [SerializeField] public GameObject msg;
    [SerializeField] public ScrollRect scroll;

   
    private void Start()
    {
        
        
        GameObject game=  GameObject.Find("LoginUI");
        chat= game.GetComponent<Chat>();
        chat.chatManager = this;
        chat.scrollRect = this.scroll;
        sendBtn.onClick.AddListener(Send);
    }

    private void Send()
    {
        chat.SendChatMessage();
    }
}
