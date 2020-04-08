using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GoToLeaderboard : MonoBehaviour
{
    public void moveToLeaderboard()
    {
       SceneManager.LoadScene("Leaderboard"); 
    }

}
