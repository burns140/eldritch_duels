using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using eldritch;


//TODO: LINK UP TO DUEL SCREEN UI
public class Chat : MonoBehaviour
{

    public UnityEngine.UI.Button connectbutton;
    public UnityEngine.UI.Button sendbutton;
    public UnityEngine.UI.InputField sendtext;
    public UnityEngine.UI.Text textbox;
    private string channel;
    private string messagetext;

    public void onMessageGet(string sender, string message)
    {
        Debug.Log(sender + ": " + message);
        textbox.text = textbox.text + sender + ": " + message + "\n";

    }

    public void sendMessage()
    {
        Debug.Log("Attempting to send message...");
        DuelScript.
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
