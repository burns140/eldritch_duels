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
using eldritch.cards;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LeaderboardScript : MonoBehaviour
{
    public GameObject buttonPrefab; // Button prefab in UI
    public GameObject leaderboardPanel; // Leaderboard panel in UI

    private List<string> leaderboardList = new List<string>(); // To store leaderboard content

    // Start is called before the first frame update
    void Start()
    {
        leaderboardList.Add("   1    Amigo");
        leaderboardList.Add("   2    Que Paso");
        leaderboardList.Add("   3    HolaAmigo");
        loadLeaderboard();
    }

    private void loadLeaderboard(){
        Button[] gameObjects = leaderboardPanel.GetComponentsInChildren<Button>(); // Get previous leaderboard items
        foreach(Button o in gameObjects){ 
            Destroy(o.gameObject); // Destroy all previous leaderboard items
        }

        foreach(string value in leaderboardList){
            GameObject valueButoon = (GameObject)Instantiate(buttonPrefab); // Create leaderboard item button
            valueButoon.GetComponentInChildren<Text>().text = value; // Set text to the leaderboard item 
            valueButoon.SetActive(true);
            valueButoon.transform.SetParent(leaderboardPanel.transform, false); // Add item buttons to leaderboard panel
        }
    }

    public void goBack(){
        SceneManager.LoadScene("Lobby"); // Load lobby scene
    }

}
