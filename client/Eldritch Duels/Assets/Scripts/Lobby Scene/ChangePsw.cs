using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ChangePsw : MonoBehaviour
{

    public GameObject Panel;

    public InputField newPassInput;


    public void ChangePassword()
    {
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }

        var newPass = newPassInput.GetComponent<InputField>().text;
        ChangePassRequest req = new ChangePassRequest("changePassword", Global.getID(), Global.getToken(), newPass);
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;

        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    }

    public class ChangePassRequest {
        public string cmd;
        public string id;
        public string token;
        public string pass;

        public ChangePassRequest (string cmd, string id, string token, string pass) {
            this.cmd = cmd;
            this.id = id;
            this.token = token;
            this.pass = pass;
        }
    }
}
