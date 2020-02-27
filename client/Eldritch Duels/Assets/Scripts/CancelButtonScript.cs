using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CancelButtonScript : MonoBehaviour
{
    public void onSubmit(){
        SceneManager.LoadScene(1);
    }
}
