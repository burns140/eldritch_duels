using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using eldritch;

public class deleted
{
    public string token;
    public string id;
    public string cmd;


    public deleted(string cmd, string token, string id)
    {
        this.token = token;
        this.cmd = cmd;
        this.id = id;
    }
}

public class Delete : MonoBehaviour
{
    public UnityEngine.UI.Button deletebutton;
    // Start is called before the first frame update
    public void Start()
    {
        deletebutton.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        Debug.Log("in clicked");
        deleted user = new deleted("deleteAccount", Global.getToken(), Global.getID());
        string json = JsonConvert.SerializeObject(user);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log(responseData);
        SceneManager.LoadScene("MainScene");
        Global.username = "";
        Global.userID = 0;
        Global.userCards.Clear();
        Global.usercredits = 0;
        Global.selectedDeck = new eldritch.cards.Deck();
        Global.userDecks.Clear();
        Global.avatar = 0;
        Global.bio = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
