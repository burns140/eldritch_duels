using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoDeckBuilder : MonoBehaviour
{
    public void MoveToDeckBuilder()
    {
        SceneManager.LoadScene("Decks");
    }
}
