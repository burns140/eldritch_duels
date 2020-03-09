using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditCalculation : MonoBehaviour
{
    public int CalculateCredits() //CHANGE TO ACCEPT GAME RESULTS AS AN ARGUMENT(S)
    {
        int basecredits = 40; //CHANGE BASE AMOUNT
        bool userWon = true; //CHANGE TO CHECK FOR WINNER IN ARGUMENTS
        bool AI = false;
        if (!userWon)
        {
            basecredits /= 2;
        }
        if (AI)
        {
            basecredits /= 2;
        }
        int turncount = 5; //CHANGE TO ARGUMENT, INITIALIZE TO 0 FOR TURN 1 AND INCREMENT PER TURN
        basecredits *= turncount;

        return basecredits;
    }
}
