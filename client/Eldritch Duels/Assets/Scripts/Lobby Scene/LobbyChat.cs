using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using eldritch;
using UnityEngine.UI;

public class LobbyChat : MonoBehaviour
{

    public GameObject friendButtonPrefab; // Button prefab for friends buttons in UI
    public GameObject friendsPanel; // panel to display friends list in UI
    public Text chatText;
    public InputField TextEntry;
    public string TextEntryText;
    public string currentChat = "-1";
    public Button sendText;
    public List<ChatLog> chatlogs = new List<ChatLog>();

    // Start is called before the first frame update
    void Start()
    {
        populateFriendsList();
        sendText.onClick.AddListener(sendMessage);
    }

    // Update is called once per frame
    void Update()
    {
        TextEntryText = TextEntry.text;
        // Since async doesn't want to work, just check if there is data every frame
        if (Global.client.Available != 0)
        {
            byte[] data = new byte[256];
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            const string expectedBeginning = "message:";
            Debug.Log("Received Message: " + responseData);
            if (responseData.StartsWith(expectedBeginning))
            {
                try
                {
                    string[] parsed = responseData.Split(':');
                    string prework = parsed[1];
                    string[] broken = prework.Split('|');
                    string result = broken[0] + ": ";
                    string actual = broken[1];
                    if (Global.profanityFilter)
                    {
                        actual = Global.filterText(actual);
                    }
                    if (currentChat.Equals(broken[0]))
                    {
                        chatText.text += result + actual + "\n";
                    }
                    foreach (ChatLog t in chatlogs)
                    {
                        if (t.getUser().Equals(broken[0]))
                        {
                            t.addMessage(result + actual + "\n");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

    public void populateFriendsList()
    {
        Debug.Log("populating friends list");
        getCollection getfriends = new getCollection("getAllFriends", Global.getID(), Global.getToken());
        string responseData = Global.NetworkRequest(getfriends);

        List<string> friendsList = new List<string>();

        friendsList = responseData.Split(',').ToList();

        foreach (string value in friendsList)
        {
            chatlogs.Add(new ChatLog(value));

            Debug.Log("Attempting to add " + value + " to friends list");
            if (value == "nofriends")
            {
                continue;
            }
            GameObject friendObject = (GameObject)Instantiate(friendButtonPrefab); // Create friend user button
            friendObject.GetComponentInChildren<Text>().text = value; // Set text to the friend username 
            friendObject.SetActive(true);
            friendObject.transform.SetParent(friendsPanel.transform, false); // Add friend username buttons to friends panel
            Button friendbutt = friendObject.GetComponent<Button>();
            friendbutt.onClick.AddListener(() => friendSelected(value));
        }
    }

    public void friendSelected(string friend)
    {
        Debug.Log(friend + " was selected");
        currentChat = friend;

        foreach (ChatLog t in chatlogs)
        {
            if (t.getUser().Equals(friend))
            {
                chatText.text = t.getLogs();
            }
        }

        /*FriendChatRequest getChatLogs = new FriendChatRequest("getChatHistory", Global.getID(), Global.getToken(), friend);
        string responsedata = Global.NetworkRequest(getChatLogs);*/
        //TODO: PARSE RESPONSE STRING INTO CHAT LOGS
        //CONSIDER DELIMITING MESSAGES WITH |
        //OUTPUT INTO AN ARRAY, THEN PUT ARRAY INTO CHAT TEXT
    }

    public void sendMessage()
    {

        if (!currentChat.Equals("-1"))
        {
            sendMessage mess = new sendMessage("sendMessage", Global.getEmail(), Global.getID(), Global.getToken(), currentChat, TextEntryText);
            string responsedata = Global.NetworkRequest(mess);
            if (responsedata.Equals("notloggedin"))
            {
                chatText.text += "That user is not logged in." + "\n";
            }
            else
            {
                chatText.text += Global.username + ": " + TextEntryText + "\n";
                foreach (ChatLog t in chatlogs)
                {
                    if (t.getUser().Equals(currentChat))
                    {
                        t.addMessage(Global.username + ": " + TextEntryText + "\n");
                    }
                }
            }
        }
    }
}

public class ChatLog
{
    string user;
    string logs;
    public ChatLog(string user)
    {
        this.user = user;
        logs = "";
    }
    public void updateLog(string log)
    {
        logs = log;
    }
    public string getLogs()
    {
        return logs;
    }
    public string getUser()
    {
        return user;
    }
    public void addMessage(string m)
    {
        logs += m;
    }
}