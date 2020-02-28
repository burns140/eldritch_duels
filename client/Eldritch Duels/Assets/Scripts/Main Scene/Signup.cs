using System;
using Newtonsoft.Json;
using UnityEngine;
using eldritch;

public class Signup : MonoBehaviour
{
    public static string email = "";
    public static string pass = "";
    public static string user = "";
    public UnityEngine.UI.InputField Email;
    public UnityEngine.UI.InputField Password;
    public UnityEngine.UI.InputField Username;
    public UnityEngine.UI.Image UserPanel;
    public UnityEngine.UI.Button quitbutton;
    public UnityEngine.UI.Button loginbutton;
    public UnityEngine.UI.Button signupbutton;
    public UnityEngine.UI.Button signup;
    // Start is called before the first frame update
    public void Start()
    {
        signup.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        string result = ServerSignup(email, pass, user);
        //if(String.Equals(result,"User with that email already exists"))
        //{
        //    Debug.Log(result);
        //} else
        //{
        //    string[] temp = result.Split(' ');

        //}
        UserPanel.gameObject.SetActive(false);
        quitbutton.gameObject.SetActive(true);
        loginbutton.gameObject.SetActive(true);
        signupbutton.gameObject.SetActive(true);
        Debug.Log(result);

    }

    // Update is called once per frame
    public void Update()
    {
        email = Email.text;
        pass = Password.text;
        user = Username.text;
    }

    static string ServerSignup(string email, string password, string username)
    {
        User user = new User("signup", email, password, username);
        string json = JsonConvert.SerializeObject(user);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        return responseData;
    }
}
