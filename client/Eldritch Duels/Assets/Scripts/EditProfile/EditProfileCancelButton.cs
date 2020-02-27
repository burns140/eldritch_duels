using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditProfileCancelButton : MonoBehaviour
{
    public void onSubmit()
    {
        SceneManager.LoadScene(1);
    }
}
