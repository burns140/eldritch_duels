using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProfilePicScript : MonoBehaviour
{

    public Dropdown dropdown;
    public Sprite[] pictures;

    private string bio;
    private string screenName;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.ClearOptions();
        List<Dropdown.OptionData> picItems = new List<Dropdown.OptionData>();

        foreach(var pic in pictures){
            var picOption = new Dropdown.OptionData(pic.name, pic);
            picItems.Add(picOption);
        }

        dropdown.AddOptions(picItems);

    }

    public void onSave(){
        Debug.Log("Clicked on Save Profile");
    }

    public void getBio(){
        Debug.Log("Ended Editing Bio on Frontend");
    }

    public void getScreenName(){
        Debug.Log("Ended Editing ScreenName on Frontend");
    }

}
