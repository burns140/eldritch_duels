using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using eldritch;
using System;


//TODO: LINK UP TO DUEL SCREEN UI
public class Chat : MonoBehaviour
{

    public UnityEngine.UI.Button connectbutton;
    public UnityEngine.UI.Button sendbutton;
    public UnityEngine.UI.InputField sendtext;
    public UnityEngine.UI.Text textbox;

    private string channel;
    private string messagetext;

    public void onMessageGet(string message)
    {
        Debug.Log(message);
        textbox.text = textbox.text + message + "\n";
    }

    public void sendMessage()
    {
        Debug.Log("Attempting to send message...");
        string message = "chat:" + Global.username + ": " + messagetext;
        string format = Global.username + ": " + messagetext;
        textbox.text = textbox.text + format + "\n";
        sendtext.text = "";
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
        Global.stream.Write(data, 0, data.Length);
        
    }

    // Start is called before the first frame update
    void Start() // REWORK TO CONNECT WHEN DUEL SCENE IS OPENED
    {
        textbox.text = "";
        sendbutton.onClick.AddListener(sendMessage);
    }

    // Update is called once per frame
    void Update()
    {
        messagetext = sendtext.text;
    }

    
}
