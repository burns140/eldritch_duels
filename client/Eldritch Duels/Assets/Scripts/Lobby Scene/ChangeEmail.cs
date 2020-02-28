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

public class ChangeEmail : MonoBehaviour
{
    public GameObject Panel;

    public InputField newEmailInput;

    public void UpdateEmail()
    {
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }

        var newEmail = newEmailInput.GetComponent<InputField>().text;
        ChangeEmailRequest req = new ChangeEmailRequest("changeEmail", Global.getID(), Global.getToken(), newEmail);
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;

        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    }

    public class ChangeEmailRequest {
        public string cmd;
        public string id;
        public string token;
        public string email;

        public ChangeEmailRequest (string cmd, string id, string token, string email) {
            this.cmd = cmd;
            this.id = id;
            this.token = token;
            this.email = email;
        }
    }
}


