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
    public UnityEngine.UI.Button login; //was too lazy to change this, this points to the forgot password button
    // Start is called before the first frame update

    // Calls ResetPass when forget password is clicked
    public void Start()
    {
        login.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        ResetPass(email);
    }

    // Constantly updates email variable with text in email field
    public void Update()
    {
        email = EmailLoginInput.text;
    }

    public static void ResetPass(string email)
    {
        if (email == "") {
            return;
        }
        login user = new login("tempPass", email, "asdf"); //uses Login class to format the right JSON request
        //server query
        string json = JsonConvert.SerializeObject(user); 
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log(responseData);
        //TODO: add display message to show message
    }
}
