using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using eldritch;

public class ForgotPassword : MonoBehaviour
{
    public static string email = "";
    public UnityEngine.UI.InputField EmailLoginInput;
    public UnityEngine.UI.Button login;
    // Start is called before the first frame update
    public void Start()
    {
        login.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        ResetPass(email);
    }

    // Update is called once per frame
    public void Update()
    {
        email = EmailLoginInput.text;
    }

    public static void ResetPass(string email)
    {
        login user = new login("tempPass", email, "asdf");
        string json = JsonConvert.SerializeObject(user);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //TODO: maybe add display message to show message, not needed
        Thread.Sleep(2500);
    }
}
