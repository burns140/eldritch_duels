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

public class User
{
    public string email;
    public string password;
    public string name;
    public string cmd;

    public User(string cmd)
    {
        this.email = "testemail@email.edu";
        this.cmd = cmd;
        this.password = "password";
        this.name = "username";
    }

    public User(string cmd, string email, string password, string username)
    {
        this.email = email;
        this.cmd = cmd;
        this.password = password;
        this.name = username;
    }
}

public class login
{   
    public string email;
    public string password;
    public string cmd;

    public login(string cmd)
    {
        this.email = "testemail@email.edu";
        this.cmd = cmd;
        this.password = "password";
    }

    public login(string cmd, string email, string password)
    {
        this.email = email;
        this.cmd = cmd;
        this.password = password;
    }
}

public class Login : MonoBehaviour
{
    public static string email = "";
    public static string pass = "";
    public UnityEngine.UI.InputField EmailLoginInput;
    public UnityEngine.UI.InputField PswlLoginInput;
    public UnityEngine.UI.Button login;
    // Start is called before the first frame update
    public void Start()
    {
        login.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        string result = ServerLogin(email, pass);
        if(result.Length > 0)
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

    // Update is called once per frame
    public void Update()
    {
        email = EmailLoginInput.text;
        pass = PswlLoginInput.text;
    }

    public static string ServerLogin(string email, string password)
    {
        Debug.Log("Inputted: " + email + " | " + password);
        login user = new login("login", email, password);
        //Debug.Log(user);
        string json = JsonConvert.SerializeObject(user);
        //Debug.Log(json);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        //Debug.Log(data);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        if (String.Equals(responseData, "Incorrect password"))
        {
            return String.Empty;
        }
        string tempFile = "LoginTemp";
        try
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

                Global.avatar = Int32.Parse(loginstuff[2]);
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
