using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;
using eldritch;

public static class DuelFunctions
{
    public static bool CanCast(Card castingcard, PlayerState ps){
        if(castingcard.CardCost <= ps.mana){
            return true;
        }
        return false;
    }

    public static bool CanBlock(Card attacker, Card blocker){
        if(attacker.HasFly && !blocker.HasFly){
            return false;
        }

        return true;
    }

    public static List<Card> GetLibrary(){
        Deck d = Global.selectedDeck;
        if(d == null)
            return null;
        
        List<Card> lib = new List<Card>();
        foreach(CardContainer cc in d.CardsInDeck){
            for(int i = 0; i < cc.count;i++){
                lib.Add(cc.c);
            }
        }
        return lib;
    }

    public static List<Card> ShuffleLibrary(List<Card> lib){
        List<Card> newLib = new List<Card>();
        while(lib.Count > 0){
            int rn = Random.Range(0,lib.Count);
            newLib.Add(lib[rn]);
            lib.RemoveAt(rn);
        }

        return newLib;
    }

    public static Card DrawCard(ref PlayerState ps){
        Card c = ps.library[0];
        ps.library.RemoveAt(0);
        return c;
    }

    public static Card RemoveFromHand(Card card, ref PlayerState ps){
        Card c;
        for(int i = 0; i < ps.inHand.Count;i++){
            if(ps.inHand[i].CardName.Equals(card.CardName)){
                c = ps.inHand[i];
                ps.inHand.RemoveAt(i);
                return c;
            }
            
        }
        return null;
    }
}
