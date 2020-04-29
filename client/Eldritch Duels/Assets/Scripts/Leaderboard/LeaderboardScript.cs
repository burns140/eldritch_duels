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
    private List<string> winsList = new List<string>(); // To store wins for users

    public class genericRequest {
        public string id;
        public string token;
        public string cmd;

        public genericRequest(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        genericRequest req = new genericRequest(Global.getID(), Global.getToken(), "getLeaderboard");
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[1024];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log("RESPONSE DATA"+responseData);
        string[] info = responseData.Split(',');
        Debug.Log("INFO:"+info.ToString());
        string firstusername = info[0].Split('_')[0];
        Debug.Log("First USERNAME:"+firstusername);
        string firstwins = info[0].Split('_')[1]; 
        Debug.Log("First WINS:"+firstwins);
        
        // @TODO add the usernames to leaderboardList
        for(int i=0; i<info.Length; i++){
            int j = i+1;
            leaderboardList.Add(j+"    "+info[i].Split('_')[0]);
            winsList.Add(info[i].Split('_')[1]);
        }
        
        loadLeaderboard();
    }

    private void loadLeaderboard(){
        Button[] gameObjects = leaderboardPanel.GetComponentsInChildren<Button>(); // Get previous leaderboard items
        foreach(Button o in gameObjects){ 
            Destroy(o.gameObject); // Destroy all previous leaderboard items
        }
        int i=0;
        foreach(string value in leaderboardList){
            GameObject valueButoon = (GameObject)Instantiate(buttonPrefab); // Create leaderboard item button
            GameObject winButton = (GameObject)Instantiate(buttonPrefab);
            valueButoon.GetComponentInChildren<Text>().text = value; // Set text to the leaderboard item 
            winButton.GetComponentInChildren<Text>().text = winsList[i];
            i++;
            valueButoon.GetComponentInChildren<Button>().interactable = false;
            winButton.GetComponentInChildren<Button>().interactable = false;
            valueButoon.SetActive(true);
            winButton.SetActive(true);
            valueButoon.transform.SetParent(leaderboardPanel.transform, false); // Add item buttons to leaderboard panel
            winButton.transform.SetParent(leaderboardPanel.transform, false);
        }
    }

    public void goBack(){
        SceneManager.LoadScene("Lobby"); // Load lobby scene
    }

}
