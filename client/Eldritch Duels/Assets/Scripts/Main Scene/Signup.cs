using System;
using Newtonsoft.Json;
using UnityEngine;
using eldritch;

public class Signup : MonoBehaviour
{
    // Keeps track of all relevant fields and variables
    public static string email = "";
    public static string pass = "";
    public static string user = "";
    public UnityEngine.UI.InputField Email;
    public UnityEngine.UI.InputField Password;
    public UnityEngine.UI.InputField Username;
    public UnityEngine.UI.Image UserPanel;
    public UnityEngine.UI.Image ErrorPanel;
    public UnityEngine.UI.Text ErrorText;
    // These are tracked to open/close buttons and panels when signup is successful.
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
        if (String.Equals(result, "email error"))
        {
            ErrorPanel.gameObject.SetActive(true);
            ErrorText.text = "Invalid Email";
            ErrorText.gameObject.SetActive(true);
            
        }
        else if (String.Equals(result, "User with that email already exists"))
        {
            ErrorPanel.gameObject.SetActive(true);
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = "Email is already in use";
        }
        else
        {
            UserPanel.gameObject.SetActive(false);
            quitbutton.gameObject.SetActive(true);
            loginbutton.gameObject.SetActive(true);
            signupbutton.gameObject.SetActive(true);
            if (ErrorPanel.IsActive())
            {
                ErrorPanel.gameObject.SetActive(false);
                ErrorText.gameObject.SetActive(false);
            }
        }
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
        // server query
        /*try
        {
            //regex checker
            System.Net.Mail.MailAddress emailtest = new System.Net.Mail.MailAddress(email);
            if (String.Equals(emailtest.Address, email))
            {*/
                User user = new User("signup", email, password, username);
                string json = JsonConvert.SerializeObject(user);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                

                return responseData;
            /*}
            else
            {
                return "email error";
            }
        } catch (Exception e)
        {
            Debug.Log(e.Message);
            return "email error";
        }*/
    }
}
