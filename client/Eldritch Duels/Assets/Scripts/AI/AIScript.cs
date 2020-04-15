using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;

namespace eldritch{
    public enum AIDifficulty{
        EASY,
        NORMAL,
        HARD,
        EXTREME
    }
    
    public static class AIScript
    {
        public static AIDifficulty Difficulty = AIDifficulty.NORMAL;

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
            if(Difficulty == AIDifficulty.EASY){
                easyTurn(duelData);
            }else if(Difficulty == AIDifficulty.NORMAL){
                normalTurn(duelData);
            }else if(Difficulty == AIDifficulty.HARD){
                hardTurn(duelData);
            }else if(Difficulty == AIDifficulty.EXTREME){
                extremeTurn(duelData);
            }

        }
        public static void AIBlock(AIDuel duelData){

        }

        //easy AI make random choices 
        private static void easyTurn(AIDuel ad){
            //play cards
            //check hand size, mana, and random bool
            while(ad.oppState.inHand.Count>0 && ad.oppState.mana > 0 && randomBool()){

            }

            //attack
            //picks random cards on field to attack with

            

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

        private static bool randomBool(){
            return Random.Range(0,100) > 50;
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
