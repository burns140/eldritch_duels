using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using eldritch;

public class WaitingScript : MonoBehaviour
{
    public string nextSceneName = "DuelScene";

    // Start is called before the first frame update
    void Start()
    {
        // Old asyncronous solution
        // if it worked, it would be better

        /*byte[] data = new byte[256];
        Global.stream.BeginRead(data, 0, data.Length, (IAsyncResult ar) =>
        {
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            if (responseData.StartsWith("match found"))
            {
                try
                {
                    Global.enemyUsername = responseData.Substring(responseData.IndexOf(':') + 2);
                    Debug.Log(global.enemyUsername);
                    SceneManager.LoadScene(nextSceneName);
                    Global.inQueue = false;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            else
                Debug.Log(responseData);
        }, null);*/
    }

    // Update is called once per frame
    void Update()
    {
        // Since async doesn't want to work, just check if there is data every frame
        if (Global.client.Available != 0)
        {
            byte[] data = new byte[256];
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            try
            {
                string[] lines = responseData.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Length == 0)
                        continue;

                    int colonIndex = line.IndexOf(':');
                    if (colonIndex == -1)
                        if(line.Contains("my turn")) {
                            Global.DuelMyTurn = true;
                            Debug.Log("Going first");
                            continue;
                        } else {
                            Debug.Log("Unknown line: " + line);
                            continue;
                        }

                    string cmd = line.Substring(0, colonIndex);
                    string result = line.Substring(colonIndex + 2);

                    Debug.Log(cmd + ": " + result);

                    switch (cmd) {
                        case "MatchID":
                            Global.matchID = result;
                            break;

                        case "match found":
                            Global.enemyUsername = result;
                            break;

                        case "elo":
                            Global.enemyElo = Int32.Parse(result);
                            break;

                        default:
                            Debug.Log("Unknown command: " + cmd);
                            Debug.Log("From line:" + line);
                            break;
                    }
                }
                
                if (Global.matchID != null) {
                    SceneManager.LoadScene(nextSceneName);
                    Global.inQueue = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log(responseData);
                Debug.Log(e);
            }finally{
                SceneManager.LoadScene(nextSceneName);
            }
            /*
            else{
                Debug.Log(responseData);
                if(responseData.Contains("my turn")){
                    Global.DuelMyTurn = true;
                    SceneManager.LoadScene(nextSceneName);
                }
            }*/
        }
    }
}
