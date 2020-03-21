using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoLobby : MonoBehaviour
{
    public void MoveToLobby()
    {
        SceneManager.LoadScene("Lobby");  // duel scene
    }
}
