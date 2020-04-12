using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSounds : MonoBehaviour
{
    public AudioClip audioClipPlayedCard; // The audio clip when card is played;
    public AudioClip audioClipAttackCard; // The audio clip when card is attacking;
    public AudioClip audioClipDestroyCard; // The audio clip when card is destroyed;
    
    // Sound when card is played
    public void cardPlayedSound(GameObject cardObject)
    {
        cardObject.GetComponent<AudioSource>().clip = audioClipPlayedCard; // Set audio clip
        cardObject.GetComponent<AudioSource>().Play(); // Play the new clip
        Debug.Log("Played the PlayedCard sound");
    }

    // Sound when card is attacking
    public void cardAttackSound(GameObject cardObject){
        cardObject.GetComponent<AudioSource>().clip = audioClipAttackCard; // Set audio clip
        cardObject.GetComponent<AudioSource>().Play(); // Play the new clip
        Debug.Log("Played the AttackCard sound");
    }

    // Sound when card is destroyed
    public void cardDestroySound(GameObject cardObject){
        cardObject.GetComponent<AudioSource>().clip = audioClipDestroyCard; // Set audio clip
        cardObject.GetComponent<AudioSource>().Play(); // Play the new clip
        Debug.Log("Played the DestroyCard sound");
    }

}
