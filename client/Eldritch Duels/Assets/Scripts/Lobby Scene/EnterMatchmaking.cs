using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using eldritch;

public class match
{
    public string token;
    public string id;
    public string cmd;
    public bool competetive;

    public match(string cmd, string id, string token, bool competetive)
    {
        this.id = id;
        this.cmd = cmd;
        this.token = token;
        this.competetive = competetive;
    }
}

public class EnterMatchmaking : MonoBehaviour
{

    public UnityEngine.UI.Button enter;
    // Start is called before the first frame update
    public void Start()
    {
        //enter.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        if (!Global.inQueue) // checks if already in queue somehow
        {
            // sends serve request to put user in matchmaking queue
            match user = new match("enterQueue", Global.getID(), Global.getToken(), Global.matchType == MatchType.COMPETETIVE);
            string json = JsonConvert.SerializeObject(user);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[256];

            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            if (String.Equals(responseData, "added to queue"))
            {
                Global.inQueue = true;
                SceneManager.LoadScene(8);
            }
            else
                Debug.Log(responseData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
