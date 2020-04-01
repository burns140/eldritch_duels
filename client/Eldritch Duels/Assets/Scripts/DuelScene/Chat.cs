using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using eldritch;


//TODO: LINK UP TO DUEL SCREEN UI
public class Chat : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;

    public UnityEngine.UI.Button connectbutton;
    public UnityEngine.UI.Button sendbutton;
    public UnityEngine.UI.InputField sendtext;
    public UnityEngine.UI.Text textbox;
    private string channel;
    private string messagetext;

    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state.ToString());
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { channel });
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        int msgCount = messages.Length;
        for (int i = 0; i < msgCount; i++)
        { //go through each received msg
            string sender = senders[i];
            string msg = (string)(messages[i]);
            Debug.Log(sender + ": " + msg);
            textbox.text = textbox.text + sender + ": " + msg + "\n";
            Debug.Log(textbox.text.Length);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) // We aren't planning on having private messages for now.
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) // we don't have friends right now so this isn't helpful
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results) 
    {
        Debug.Log("Subscribed to a new channel!");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Unsubscribed from a channel!");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log("New " + user + " joined " + channel + "!");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log(user + " left the channel " + channel);
    }

    public void OnApplicationQuit()
    {
        if (chatClient != null) { chatClient.Disconnect(); }
    }

    public void connect()
    {
        Debug.Log("Connecting...");
        ExitGames.Client.Photon.ConnectionProtocol connectProtocol = ExitGames.Client.Photon.ConnectionProtocol.Tcp;
        this.chatClient = new ChatClient(this, connectProtocol) //Creates a chat client, and sets it to the specified region.
        {
            ChatRegion = "US" //SET CHAT REGION
        };
        Debug.Log("Attempted to create chat client.");

        Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues
        {
            UserId = Global.username,
            AuthType = Photon.Chat.CustomAuthenticationType.None
        };

        Debug.Log("Attempted to authenticate.");

        chatClient.Connect("fb39f16f-bcce-4d9d-9910-db8c9abfd911", "0.01", authValues); //DO NOT CHANGE THIS

        Debug.Log("Attempted to connect.");
    }

    public void sendMessage()
    {
        Debug.Log("Attempting to send message...");
        chatClient.PublishMessage(channel, messagetext);
    }

    // Start is called before the first frame update
    void Start() // REWORK TO CONNECT WHEN DUEL SCENE IS OPENED
    {
        textbox.text = "";
        //channel = "Test";
        channel = Global.matchID;
        Debug.Log(channel);
        connect();
        sendbutton.onClick.AddListener(sendMessage);
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null) {
            chatClient.Service();
        }
        messagetext = sendtext.text;
    }

    
}
