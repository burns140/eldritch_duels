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

public class EditProfileCancelButton : MonoBehaviour
{
    private const string EMAIL_PREF_KEY = "email"; // EMAIL PREF KEY to store user email

    // Handle Cancel button click
    public void onSubmit()
    {
        PlayerPrefs.SetString(EMAIL_PREF_KEY,Global.getEmail()); // Get the user email from PLAYER PREFS;
        SceneManager.LoadScene("ProfileScene"); // Don't save profile changes and go back to Lobby
    }
}
