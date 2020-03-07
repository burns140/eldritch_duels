using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditProfileCancelButton : MonoBehaviour
{
    // Handle Cancel button click
    public void onSubmit()
    {
        SceneManager.LoadScene(1); // Don't save profile changes and go back to Lobby
    }
}
