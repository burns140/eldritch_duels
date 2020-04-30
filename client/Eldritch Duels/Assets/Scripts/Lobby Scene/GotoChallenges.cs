using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoChallenges : MonoBehaviour
{
    public void MoveToChallenge() {
        SceneManager.LoadScene("Challenge");
    }
}
