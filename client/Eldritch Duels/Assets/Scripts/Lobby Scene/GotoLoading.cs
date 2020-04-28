using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using eldritch;

public class GotoLoading : MonoBehaviour
{
    public void MovetoLoading()
    {
        SceneManager.LoadScene(8);
    }

    public void PlayAI(){
        SceneManager.LoadScene("AIDuelScene");
    }

    public void SetEasy(){
       AIScript.Difficulty = AIDifficulty.EASY;
    }
    public void SetNormal(){
       AIScript.Difficulty = AIDifficulty.NORMAL;
    }
    public void SetHard(){
       AIScript.Difficulty = AIDifficulty.HARD;
    }
    public void SetExtreme(){
       AIScript.Difficulty = AIDifficulty.EXTREME;
    }
}
