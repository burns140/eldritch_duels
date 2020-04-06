using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoAchievement : MonoBehaviour
{
    public void MoveToAchievement()
    {
        SceneManager.LoadScene("Achievement");  
    }
}
