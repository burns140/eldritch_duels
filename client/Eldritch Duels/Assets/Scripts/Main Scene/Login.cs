﻿using System;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using eldritch;

public class User // Used for server requests
{
    public string email;
    public string password;
    public string name;
    public string cmd;

    public User(string cmd, string email, string password, string username)
    {
        this.email = email;
        this.cmd = cmd;
        this.password = password;
        this.name = username;
    }
}

public class login // Used for server requests
{   
    public string email;
    public string password;
    public string cmd;

    public login(string cmd, string email, string password)
    {
        this.email = email;
        this.cmd = cmd;
        this.password = password;
    }
}

public class Login : MonoBehaviour
{
    // Variables to keep track of input fields and the login button
    public static string email = "";
    public static string pass = "";
    public UnityEngine.UI.InputField EmailLoginInput;
    public UnityEngine.UI.InputField PswlLoginInput;
    public UnityEngine.UI.Button login;
    // Calls the login function on click
    public void Start()
    {
        login.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        string result = ServerLogin(email, pass); 
        if(result.Length > 0) // Sets temp file with token and ID if login is successful, as well as global variables
        {
            Debug.Log("Login successful! Temp file is: " + result);
            Global.tokenfile = result;
            string tmp = Global.GetCollection();
            Global.InitUserCards(tmp, 1);
            SceneManager.LoadScene("Lobby");
        } else
        {
            Debug.Log("Login failed!");
        }
    }

    // Updates email and password variables every frame
    public void Update()
    {
        email = EmailLoginInput.text;
        pass = PswlLoginInput.text;
    }

    public static string ServerLogin(string email, string password)
    {
        // server request
        Debug.Log("Inputted: " + email + " | " + password);
        login user = new login("login", email, password);
        string json = JsonConvert.SerializeObject(user);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        if (String.Equals(responseData, "Incorrect password")) // checking for incorrect password response
        {
            return String.Empty;
        }
        string tempFile = "LoginTemp";
        try //make the temp file
        {
            tempFile = Path.GetTempFileName();
            FileInfo fileInfo = new FileInfo(tempFile);
            fileInfo.Attributes = FileAttributes.Temporary;
            Console.WriteLine("TEMP file created at: " + tempFile);
            try
            {
                string[] loginstuff = responseData.Split(':');
                // Write to the temp file.
                StreamWriter streamWriter = File.AppendText(tempFile);
                streamWriter.WriteLine(loginstuff[0]); // token
                streamWriter.WriteLine(loginstuff[1]); // ID
                streamWriter.Flush();
                streamWriter.Close();
                Global.avatar = (loginstuff[2] == null || loginstuff[2] == "")? 0 : Int32.Parse(loginstuff[2]);
                Global.username = loginstuff[3];
                Global.bio = loginstuff[4];
                return tempFile;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Console.WriteLine("Error writing to login file: " + e.Message);
                return String.Empty;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Console.WriteLine("Unable to create login file or set its attributes: " + e.Message);
            return String.Empty;
        }
    }
}
