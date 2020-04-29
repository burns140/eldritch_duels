using eldritch;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementScene : MonoBehaviour
{

    public class Achievement {
        public string name;
        public string desc;

        public Achievement(string name, string desc) {
            this.name = name;
            this.desc = desc;
        }
    }

    List<Achievement> earned = new List<Achievement>();
    List<Achievement> all = new List<Achievement>();

    public GameObject AchievementPanel_1;
    public GameObject AchievementPanel_2;
    public GameObject AchievementPanel_3;
    public GameObject AchievementPanel_4;

    public GameObject desc;
    public GameObject locked;


    // Start is called before the first frame update
    void Start()
    {
        setAllAchievements();
        setAchievementsEarned();
        setASchievementFromList_UI(earned);
        setASchievementFromList_UI(all);
    }

    void setAchievementsEarned() {
        AchievementRequest req = new AchievementRequest(Global.getID(), Global.getToken(), "getAchievements", "array");
        string res = Global.NetworkRequest(req);

        string[] vals = res.Split(','); // Array of numerical values that represent the index of that achievement in the total list of achievements

        for (int i = 0; i < vals.Length; i++) {
            Request req2 = new Request(vals[i], Global.getToken(), "getOneAchievement");
            string res2 = Global.NetworkRequest(req2);

            string[] format = res2.Split(';');  // Array with format [name, description] of each achievement that matched a value queried above.
            Achievement temp = new Achievement(format[0], format[1]);
            earned.Add(temp);
        }
    }

    void setAllAchievements() {
        string res = "";
        int i = 0;
        while (!res.Contains("no achievement with that id")) {
            Request req = new Request(i.ToString(), Global.getToken(), "getOneAchievement");
            res = Global.NetworkRequest(req);

            string[] format = res.Split(';');
            Achievement temp = new Achievement(format[0], format[1]);
            all.Add(temp);
        }
    }


    public void setASchievementFromList_UI(List<Achievement> list)
    {
        foreach (Achievement earnedAch in list)
        {
            if (earnedAch.name.Equals("1"))
                setAchievementUI(AchievementPanel_1, earnedAch);
            else if (earnedAch.name.Equals("2"))
                setAchievementUI(AchievementPanel_2, earnedAch);
            else if (earnedAch.name.Equals("3"))
                setAchievementUI(AchievementPanel_3, earnedAch);
            else if (earnedAch.name.Equals("4"))
                setAchievementUI(AchievementPanel_4, earnedAch);
        }
    }

    public void setAchievementUI(GameObject AchievementPanel, Achievement earnedAch)
    {
        desc = AchievementPanel.transform.GetChild(0).gameObject;
        desc.GetComponent<Text>().text = earnedAch.desc;
        locked = AchievementPanel.transform.GetChild(1).gameObject;
        locked.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
