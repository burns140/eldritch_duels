using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;
using System.Threading;

namespace eldritch{
    public enum AIDifficulty{
        EASY,
        NORMAL,
        HARD,
        EXTREME
    }
    
    public static class AIScript
    {
        public static AIDifficulty Difficulty = AIDifficulty.EASY;
        private static int AIRandomAdjust = 10;

        #region script stuff
        public static List<Card> GetAILibrary(){
            List<Card> res = new List<Card>();
            string s = easyDecks[0];
            if(Difficulty == AIDifficulty.EASY && easyDecks.Length >0){
                int rn = Random.Range(0,easyDecks.Length);
                s = easyDecks[rn];                
            }else if(Difficulty == AIDifficulty.NORMAL && normalDecks.Length >0){
                int rn = Random.Range(0,normalDecks.Length);
                s = normalDecks[rn]; 
            }else if(Difficulty == AIDifficulty.HARD && hardDecks.Length >0){
                int rn = Random.Range(0,hardDecks.Length);
                s = hardDecks[rn]; 
            }else if(Difficulty == AIDifficulty.EXTREME && extremeDecks.Length >0){
                int rn = Random.Range(0,extremeDecks.Length);
                s = extremeDecks[rn]; 
            }
            string[] pass = s.Split(',');
            foreach(string si in pass){
                string cname = si.Split('-')[0];
                int count = int.Parse(si.Split('-')[1]);
                Card c = Library.GetCard(cname);
                for(int i = 0;i<count;i++){
                    res.Add(c);
                }
            }

            return res;
        }

        public static void AITurn(AIDuel duelData){
            //AI draw card
            DuelFunctions.DrawCard(ref duelData.oppState);

            if(Difficulty == AIDifficulty.EASY){
                duelData.StartCoroutine(easyTurn(duelData));
            }else if(Difficulty == AIDifficulty.NORMAL){
                normalTurn(duelData);
            }else if(Difficulty == AIDifficulty.HARD){
                hardTurn(duelData);
            }else if(Difficulty == AIDifficulty.EXTREME){
                extremeTurn(duelData);
            }

        }
        public static void AIBlock(AIDuel duelData){
            if(duelData.oppState.onField.Count ==0){
                return;
            }
            if(Difficulty == AIDifficulty.EASY){
                easyBlock(duelData);
            }else if(Difficulty == AIDifficulty.NORMAL){
                normalBlock(duelData);
            }else if(Difficulty == AIDifficulty.HARD){
                hardBlock(duelData);
            }else if(Difficulty == AIDifficulty.EXTREME){
                extremeBlock(duelData);
            }
        }
        private static void easyBlock(AIDuel ad){
            
            int blockCount = ad.OppField.transform.childCount;
            List<GameObject> attackers = new List<GameObject>();
            foreach(Transform t in ad.MyField.transform){
                if(t.gameObject.GetComponent<DraggableAI>().isAttacking){
                    attackers.Add(t.gameObject);
                }
            }
            int attackCount = attackers.Count;
            int mod = Random.Range(0,attackCount);
            int end = attackCount > blockCount? blockCount : attackCount;
            for(int i = 0; i < end; i++){
                string attacker = attackers[(i+mod)%attackCount].name;
                string blocker = ad.OppField.transform.GetChild(i).gameObject.name;
                if(randomBool()){
                    ad.addOppBlocker(attacker+"-"+blocker);
                }
            }
        }
        private static void normalBlock(AIDuel ad){
            
        }
        private static void hardBlock(AIDuel ad){
            
        }
        private static void extremeBlock(AIDuel ad){
            
        }

        //easy AI make random choices 
        private static IEnumerator easyTurn(AIDuel ad){
            //wait
            yield return new WaitForSeconds(.5f);
            //play cards
            //check hand size, mana, and random bool
            while(ad.oppState.inHand.Count>0 && ad.oppState.mana > 0 && randomBool()){
                if(!PlayRandomCard(ad)){
                    break;
                }
                yield return new WaitForSeconds(.5f);
            }

            //attack
            //picks random cards on field to attack with
            foreach(Transform t in ad.OppField.transform){
                if(randomBool()){
                    ad.addOppAttacker(t.gameObject.name);
                }
            }
            //player blocks
            ad.ToBlockPhase();
            

        }
        //normal AI only looks at its state
        private static void normalTurn(AIDuel ad){
            
        }
        //hard ai looks at both states
        private static void hardTurn(AIDuel ad){
            
        }
        //extreme ai is harder version of hard ai
        private static void extremeTurn(AIDuel ad){
            
        }

        private static bool PlayRandomCard(AIDuel ad){
            
            int handSize = ad.oppState.inHand.Count;
            int mod = Random.Range(0,handSize);
            for(int i = 0; i<handSize;i++){
                if(ad.oppState.inHand[(i+mod)%handSize].CardCost <= ad.oppState.mana){
                    ad.CastCard(ad.oppState.inHand[(i+mod)%handSize].CardName, null);
                    return true;
                }
            }

            return false;
        }
        private static void PlayCard(AIDuel ad, string cardName){
            
        }

        private static void RandomAttacks(AIDuel ad){

        }

        private static void AttackWithCard(AIDuel ad){

        }

        public static bool randomBool(){
            return (Random.Range(0,100) + AIRandomAdjust) > 50;
        }

        #endregion

        #region AI voodoo

        #endregion


        #region library lists
            private static string[] easyDecks = new string[] {"Mi_Go-20,Blood Vial-12"};
            private static string[] normalDecks = new string[] {"Mi_Go-20,Blood Vial-12"};
            private static string[] hardDecks = new string[] {"Mi_Go-20,Blood Vial-12"};
            private static string[] extremeDecks = new string[] {"Mi_Go-20,Blood Vial-12"};
        #endregion
        
    }
}
