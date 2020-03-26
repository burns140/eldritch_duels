using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;
using eldritch;

public static class DuelFunctions
{
    public static int MAX_FIELD = 7;
    public static bool CanCast(Card castingcard, PlayerState ps){
        if(castingcard.CardCost <= ps.mana && ps.onField.Count < MAX_FIELD){
            return true;
        }
        return false;
    }

    public static bool CanBlock(Card attacker, Card blocker){
        if(attacker.HasStealth){
            return false;
        }
        if(attacker.HasFly && !blocker.HasFly){
            return false;
        }

        return true;
    }

    public static List<Card> GetLibrary(){
        Deck d = Global.selectedDeck;
        if(d == null || d.DeckName.Equals("")){
            List<Card> clist = Global.StringToDeckByName("Test 0-32");
            Debug.Log(clist.Count);
            return clist;
        }
        
        List<Card> lib = new List<Card>();
        foreach(CardContainer cc in d.CardsInDeck){
            for(int i = 0; i < cc.count;i++){
                lib.Add(cc.c);
            }
        }
         Debug.Log(lib.Count);
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
        if(ps.library.Count == 0){
            return null;
        }
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

    public static void RecallCard(string cardName, ref PlayerState playerState, bool isMine){
        if(isMine){
            for(int i = 0; i < playerState.onField.Count;i++){
                if(playerState.onField[i].Equals(cardName)){
                    Card c = playerState.onField[i];
                    playerState.onField.RemoveAt(i);
                    playerState.inHand.Add(c);
                }
            }
        }else{
            for(int i = 0; i < playerState.oppField.Count;i++){
                if(playerState.oppField[i].Equals(cardName)){
                    playerState.oppField.RemoveAt(i);
                }
            }
        }
    }

    public static void destroyMinion(string cardName, ref PlayerState ps, bool iAttacked) {
        if (iAttacked) {
            for(int i = 0; i< ps.oppField.Count;i++){
                if(ps.oppField[i].CardName.Equals(cardName)){
                    ps.oppField.RemoveAt(i);
                }
            }
        } else {
            for(int i = 0; i< ps.onField.Count;i++){
                if(ps.oppField[i].CardName.Equals(cardName)){
                    ps.onField.RemoveAt(i);
                }
            }
        }
        return;
    }
}