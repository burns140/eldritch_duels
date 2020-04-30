using eldritch;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementScene : MonoBehaviour
{

    public class Achievement
    {
        public string name;
        public string desc;

        public Achievement(string name, string desc)
        {
            this.name = name;
            this.desc = desc;
        }
    }

    public class AchievementRefer
    {
        public string name; 
        public Transform t;    // reference to a achievement
        public AchievementRefer(string name, Transform t)
        {
            this.name = name;
            this.t = t;
        }
    }

    List<Achievement> earned = new List<Achievement>();
    List<Achievement> all = new List<Achievement>();
    List<AchievementRefer> referList = new List<AchievementRefer>();

    string[] earnedNumArray;

    public GameObject AchievementList;  // Achievement panel list


    private GameObject desc;
    private GameObject locked;

    
    // Start is called before the first frame update
    void Start()
    {
        setAllAchievements();
        setAchievementsEarned();

        updateAllAchievement();
        updateEarnedAchievement();
    }

    void setAchievementsEarned()
    {
        AchievementRequest req = new AchievementRequest(Global.getID(), Global.getToken(), "getAchievements", "array");
        string res = Global.NetworkRequest(req);

        string[] vals = res.Split(','); // Array of numerical values that represent the index of that achievement in the total list of achievements
        earnedNumArray = vals;

        for (int i = 0; i < vals.Length; i++)
        {
            Request req2 = new Request(vals[i], Global.getToken(), "getOneAchievement");
            string res2 = Global.NetworkRequest(req2);
            string[] format = res2.Split(';');  // Array with format [name, description] of each achievement that matched a value queried above.
            Achievement temp = new Achievement(format[0], format[1]);
            earned.Add(temp);
            i++;
        }
    }

    void setAllAchievements()
    {
        string res = "";
        int i = 0;
        while (true)
        {
            Request req = new Request(i.ToString(), Global.getToken(), "getOneAchievement");
            res = Global.NetworkRequest(req);

            if (res.Contains("no achievement with that id")) {
                break;
            }

            string[] format = res.Split(';');
            Achievement temp = new Achievement(format[0], format[1]);
            all.Add(temp);
            i++;
        }
        i = 0;
    }



    public void updateAllAchievement() 
    {
        int i = 0;
        foreach (Achievement allAch in all)
        {
            Transform t = AchievementList.transform.GetChild(i++);
            referList.Add(new AchievementRefer(allAch.name, t));

            //desc
            desc = t.GetChild(0).gameObject;
            desc.GetComponent<Text>().text = allAch.desc.Split('-')[1];

            // locked
            locked = t.GetChild(1).gameObject;
            locked.SetActive(true); // assume every achievement is locked
        }
    }


    public void updateEarnedAchievement() 
    {
        foreach (Achievement earnAch in earned)
        {
            foreach (AchievementRefer refer in referList) 
            {
                if (refer.name.Equals(earnAch.name))
                {
                    Transform t = refer.t;
                    locked = t.GetChild(1).gameObject;
                    locked.SetActive(false); // unlock
                }
            }
        }
    }





    // Update is called once per frame
    void Update()
    {

    }

    
}

