using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoProfile : MonoBehaviour
{
    public void MovetoProfile()
    {
        SceneManager.LoadScene(3);
    }
}
