using System;
using Newtonsoft.Json;
using UnityEngine;
using eldritch;

public class match
{
    public string token;
    public string id;
    public string cmd;

    public match(string cmd, string id, string token)
    {
        this.id = id;
        this.cmd = cmd;
        this.token = token;
    }
}

public class EnterMatchmaking : MonoBehaviour
{

    public UnityEngine.UI.Button enter;
    // Start is called before the first frame update
    public void Start()
    {
        enter.onClick.AddListener(clicked);
    }

    void clicked()
    {
        if (!Global.inQueue)
        {
            match user = new match("enterQueue", Global.getID(), Global.getToken());
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
