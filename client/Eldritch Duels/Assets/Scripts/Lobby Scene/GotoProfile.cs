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

public class GotoProfile : MonoBehaviour
{
    private const string EMAIL_PREF_KEY = "email"; // EMAIL PREF KEY to store user email
    public void MovetoProfile()
    {
        PlayerPrefs.SetString(EMAIL_PREF_KEY,Global.getEmail()); // Save the my email to EMAIL PREF KEY
        SceneManager.LoadScene("ProfileScene");
    }
}
