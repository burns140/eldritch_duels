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
using SimpleFileBrowser;
using eldritch;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EditProfilePicScript : MonoBehaviour
{
    public Dropdown dropdown; // Picture Dropdown on the UI
    public Sprite[] pictures; // List of available pictures
    public Image errorimage; // Image for error message
    public InputField screenNameInput; // Screenname field on the UI
    public InputField bioInput; // Bio field on the UI
    public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" }; // for checking picture files
    public UnityEngine.UI.Button upload; // upload button
    private string bio; // save new bio to this string
    private string screenname; // save new screenname to this string
    private int picnum=0; // default profile pic is the first option
    private int temppicnum=0; // to change value on ui

    private const string EMAIL_PREF_KEY = "email"; // EMAIL PREF KEY to store user email

    // Start is called before the first frame update
    void Start()
    {
        upload.onClick.AddListener(openBrowser);
        dropdownSetup();
        displayPic();
        displayBio();
        displayScreenName();
    }

    public void onSave(){
        Debug.Log("Clicked on Save Profile");
        string testbio = bioInput.GetComponent<InputField>().text; // Get user input text
        string testusername = screenNameInput.GetComponent<InputField>().text; // Get user input text
        if(!String.IsNullOrWhiteSpace(testbio)){
            bio = testbio;
            Global.bio = bio; // update on global
        }
        if(!String.IsNullOrWhiteSpace(testusername)){
            screenname = testusername;
            Global.username = screenname; // update on global
        }
        Debug.Log("This is the new bio: "+bio);
        Debug.Log("This is the new screenname: "+screenname);  
        picnum = temppicnum;
        Global.avatar = picnum;
        Debug.Log("This is the new profilepicnum: "+picnum);  
        // Sending request to server to update bio, screen name, & profile pic
        EditProfileRequest req = new EditProfileRequest("editProfile", Global.getID(), Global.getToken(), bio, picnum, screenname);
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;

        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        PlayerPrefs.SetString(EMAIL_PREF_KEY,Global.email); // Get the user email from PLAYER PREFS;
        SceneManager.LoadScene("ProfileScene"); // Don't save profile changes and go back to Lobby
    }

    private void dropdownSetup(){
        dropdown.ClearOptions();
        List<Dropdown.OptionData> picItems = new List<Dropdown.OptionData>();

        foreach(var pic in pictures){
            var picOption = new Dropdown.OptionData(pic.name, pic);
            picItems.Add(picOption); 
        }

        if (Global.hasCustom)
        {
            Sprite uploaded = Global.getCustomAvatar();
            var customavatar = new Dropdown.OptionData("Uploaded", uploaded);
            picItems.Add(customavatar);
        }

        dropdown.AddOptions(picItems); // Adding all available picture to the Dropdown UI
    }

    public void handlePicName(int val){
        temppicnum = val; // Get selected picture index from the dropdown
        Debug.Log("Selected Profile Pic option: "+temppicnum.ToString());
    }

    private void displayPic(){
        int originalPic = Global.avatar; // Get original picnum from global variable
        picnum = Global.avatar; // in case it's cancelled
        Debug.Log("displayPic: Global.avatar value is "+Global.avatar);
        var dropdownInstance = dropdown.GetComponent<Dropdown>();
        if (Global.hasCustom)
        {
            dropdownInstance.value = 9;
        }
        else
        {
            dropdownInstance.value = originalPic; // Select original picture option on the UI
        }
    }

    private void displayBio(){

        string originalBio = Global.bio; // Get original bio from global variable
        bio = Global.bio; // in case it's cancelled
        Debug.Log("displayBio: Global.bio value is "+Global.bio);
        bioInput.GetComponent<InputField>().placeholder.GetComponent<Text>().text = originalBio; // Display original bio on the UI
    }

    private void displayScreenName(){

        string originalScreenname = Global.username; // Get original screenname from global variable
        screenname = Global.username; // in case it's cancelled
        Debug.Log("displaScreenName: Global.username value is "+Global.username);
        screenNameInput.GetComponent<InputField>().placeholder.GetComponent<Text>().text = originalScreenname; // Display original screenname on the UI
    }

    public void openBrowser()
    {
        FileBrowser.OnSuccess getpath = setpath;
        FileBrowser.OnCancel none = cancelled;
        Debug.Log("Opening browser");
        FileBrowser.ShowLoadDialog(getpath, none);

    }

    public void setpath(string path)
    {
        Debug.Log(path);
        Debug.Log("File select success");
        if (FileBrowser.Success)
        {
            if (ImageExtensions.Contains(Path.GetExtension(path).ToUpperInvariant()))
            {
                Debug.Log("Valid image!");

                byte[] imagebytes = File.ReadAllBytes(path);

                string bytetostring = Encoding.Default.GetString(imagebytes);

                profilepicture pfp = new profilepicture(bytetostring, Global.getToken(), Global.getID(), "setCustomAvatar");

                string json = JsonConvert.SerializeObject(pfp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                NetworkStream stream = Global.client.GetStream();

                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Global.hasCustom = true;
                Global.CustomAvatar = Global.getCustomAvatar();
                dropdownSetup();
                displayPic();
            }
            else
            {
                //MAKE ERROR MESSAGE
                errorimage.gameObject.SetActive(true);
                Debug.Log("Invalid file!");
            }
        }
        else
        {
            Debug.Log("Browser error");
        }
    }

    public void cancelled()
    {
        Debug.Log("File select cancelled");
    }

}
