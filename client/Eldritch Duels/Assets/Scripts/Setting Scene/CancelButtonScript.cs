using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CancelButtonScript : MonoBehaviour
{
    // Handle Cancel button click
    public void onSubmit(){
        SceneManager.LoadScene("Lobby"); // Don't save settings and go back to Lobby
    }
}
