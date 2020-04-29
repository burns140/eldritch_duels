using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using eldritch;


public class GetStats : MonoBehaviour
{
    public GameObject statLabels, winLabels, lossLabels, totalLabels, eloLabel;
    
    class GetStatsReq
    {
        public string cmd, id, token;

        public GetStatsReq()
        {
            this.cmd = "getStats";
            this.id = Global.getID();
            this.token = Global.getToken();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetStatsReq statsReq = new GetStatsReq();
        string json = JsonConvert.SerializeObject(statsReq);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);

        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        string resp = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        
        string[] lines = resp.Split('\n');
        int len = lines.Length;

        string[] labels = new string[len];
        string[] wins = new string[len];
        string[] losses = new string[len];
        string[] totals = new string[len];
        string elo = "not found";

        int index = 0;
        
        for (int i = 0; i < len; i++)
        {
            string line = lines[i];
            if (line.Length == 0)
                continue;

            int equalIndex = line.IndexOf('=');

            if (equalIndex == -1)
            {
                Debug.Log("Unknown line: " + line);
                continue;
            }

            string label = line.Substring(0, equalIndex);
            string values = line.Substring(equalIndex + 1);

            int colonIndex = values.IndexOf(':');

            if (colonIndex == -1)
            {
                if (label == "Elo")
                {
                    elo = values;
                    continue;
                }

                Debug.Log("Unknown value in line: " + line);
                continue;
            }

            try
            {
                int win = Int32.Parse(values.Substring(0, colonIndex));
                int loss = Int32.Parse(values.Substring(colonIndex + 1));
                int total = win + loss;

                labels[index] = label;
                wins[index] = "" + win;
                losses[index] = "" + loss;
                totals[index] = "" + total;
                index++;
            } catch (Exception e)
            {
                Debug.Log("Parsing error");
                Debug.Log(e);
            }
        }
        
        statLabels.GetComponent<Text>().text = String.Join("\n", labels);
        winLabels.GetComponent<Text>().text = String.Join("\n", wins);
        lossLabels.GetComponent<Text>().text = String.Join("\n", losses);
        totalLabels.GetComponent<Text>().text = String.Join("\n", totals);
        eloLabel.GetComponent<Text>().text = "Elo: " + elo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
