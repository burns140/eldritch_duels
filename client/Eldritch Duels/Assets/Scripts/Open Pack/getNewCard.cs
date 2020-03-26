using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getNewCard : MonoBehaviour
{
    public Image frontImage;


    public void generateNewCard() 
    {
        // TODO: randomly generate a card

        // Load a sprite from "Assets/Resources/images/migo.png"
        // Do not add "Assets/Resources" and ".png"
        // make sure images folder is under Resources
        ChangeImage("images/migo");

    }

    public void ChangeImage(string newImageTitle)
    {
        frontImage.sprite = Resources.Load<Sprite>(newImageTitle);
    }
}
