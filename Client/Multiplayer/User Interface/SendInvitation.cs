
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using UnityEngine.UI;

public class SendInvitation : MonoBehaviour
{
    private SocketIOComponent socket;
    [SerializeField]
    private Sprite spriteSend;
    [SerializeField]
    private Sprite spriteAdd;
    private void Start()
    {
        LobbyState.isdeny = false;
    }
    public void OnClickSendInvitation()
    {
        User user = new User();
        Text name = GameObject.Find("Text").GetComponent<Text>();
        user.name = name.text.ToString();
        user.id = MainMenuManager.invitations[user.name].id;
        socket = NetworkClient.users[NetworkClient.clientID].socket;
        socket.Emit("sendInvitation", new JSONObject(JsonUtility.ToJson(user)));
        if (LobbyState.isdeny)
        {
            LobbyState.isdeny = false;
        }
        this.transform.GetComponent<Image>().sprite = spriteSend;
        Debug.Log("Send invitation player(" + user.name + ")");
        
    }
    private void Update()
    {
        if (LobbyState.isdeny)
        {
            this.transform.GetComponent<Image>().sprite = spriteAdd;
        }
    }
}