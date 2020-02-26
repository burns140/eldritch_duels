﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class Signup : MonoBehaviour
{
    public static string email = "";
    public static string pass = "";
    public UnityEngine.UI.Text EmailLoginInput;
    public UnityEngine.UI.Text PswlLoginInput;
    public UnityEngine.UI.Button signup;
    // Start is called before the first frame update
    public void Start()
    {
        signup.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        string result = ServerSignup(email, pass);
        //if(String.Equals(result,"User with that email already exists"))
        //{
        //    Debug.Log(result);
        //} else
        //{
        //    string[] temp = result.Split(' ');

        //}
        Debug.Log(result);
    }

    // Update is called once per frame
    public void Update()
    {
        email = EmailLoginInput.text;
        pass = PswlLoginInput.text;
    }

    static string ServerSignup(string email, string password)
    {
        User user = new User("signup", email, password, "temp");
        string json = JsonConvert.SerializeObject(user);
        Int32 port = 8000;
        TcpClient client = new TcpClient("localhost", port);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        NetworkStream stream = client.GetStream();
        stream.Write(data, 0, data.Length);
        Console.WriteLine("Sent");
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Console.WriteLine("Received: {0}", responseData);
        Thread.Sleep(2500);
        
        client.Close();

        return responseData;
    }
}