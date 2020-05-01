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

    public enum SortType{
        WEIGHT,
        COST,
        POWER,
        DEFENCE
    }

    public static class Weights{
        public static float MANA = .25f;
        public static float TURN_DIFFERENCE = .5f;
        public static float POWER = 1;
        
    }

    
    
    public static class AIScript
    {
        private struct WeightContainer{
            public int weight;
            public Card card;
        }

        public static AIDifficulty Difficulty = AIDifficulty.EXTREME;
        private static int AIRandomAdjust = 10; //higher the value, the less random; recommended range (-50, 50)

        #region general AI 
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
                duelData.StartCoroutine(normalTurn(duelData));
            }else if(Difficulty == AIDifficulty.HARD){
                duelData.StartCoroutine(hardTurn(duelData));
            }else if(Difficulty == AIDifficulty.EXTREME){
                duelData.StartCoroutine(hardTurn(duelData));
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
                blockHard(duelData);
            }else if(Difficulty == AIDifficulty.EXTREME){
                blockHard(duelData);
            }
        }

        private static void insertAdd(WeightContainer item, SortType type, ref List<WeightContainer> wts){
                if(wts.Count == 0){
                    wts.Add(item);
                    return;
                }
                for(int i = 0; i < wts.Count;i++){
                    if(type == SortType.WEIGHT && wts[i].weight > item.weight){
                        wts.Insert(i,item);
                        return;
                    }else if(type == SortType.POWER && wts[i].card.AttackPower > item.card.AttackPower){
                        wts.Insert(i,item);
                        return;
                    }else if(type == SortType.COST && wts[i].card.CardCost > item.card.CardCost){
                        wts.Insert(i,item);
                        return;
                    }else if(type == SortType.DEFENCE && wts[i].card.DefencePower > item.card.DefencePower){
                        wts.Insert(i,item);
                        return;
                    }
                }
                wts.Add(item);
            }

            private static int turnsToCast(int turn, int cost, int mana){
                int turns = 0;
                for(int i = mana; i < cost;){
                    mana = mana + turn/8 + 1;
                    i = mana;
                    turn++;
                    turns++;
                }
                return turns;
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
            private static bool PlayCard(AIDuel ad, Card c){
                if(DuelFunctions.CanCast(c, ad.oppState)){
                    ad.CastCard(c.CardName, null);
                    return true;
                }
                return false;
            }
        

        public static bool randomBool(){
            return (Random.Range(0,100) + AIRandomAdjust) > 50;
        }


        #endregion

        #region AI voodoo

            //easy AI make random choices
            #region easy AI
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

            #endregion
            
            private static void hardBlock(AIDuel ad){
                
            }
            private static void extremeBlock(AIDuel ad){
                
            }

             
            
            //normal AI only looks at its state
            #region normal AI
                private static IEnumerator normalTurn(AIDuel ad){
                    Debug.Log("AI starting...");
                    //wait
                    yield return new WaitForSeconds(.5f);

                    //play cards
                    Debug.Log("AI Casting...");
                    int count = ad.oppState.inHand.Count;
                    while(count > 0 && castNormal(ref ad)){
                        Debug.Log(count);
                        count--;
                        yield return new WaitForSeconds(.5f);
                        
                    }
                    

                    //attack
                    Debug.Log("AI attacking...");
                    attackNormal(ad);

                    //to block
                    ad.ToBlockPhase(); 

                }

                private static bool castNormal(ref AIDuel ad){
                    //create weight list 
                    List<WeightContainer> wts = new List<WeightContainer>();
                    foreach(Card c in ad.oppState.inHand){
                        WeightContainer wc;
                        wc.card = c;
                        wc.weight = normalWeight(ad, c);
                        insertAdd(wc, SortType.WEIGHT, ref wts);
                    }
                    Debug.Log("AI First Choice: " + wts[0].card.CardName);
                    if(!PlayCard(ad, wts[0].card)){
                        return false;
                    }
                    return true;
                }
                private static void attackNormal(AIDuel aIDuel){
                    //create list
                    List<WeightContainer> wts = new List<WeightContainer>();
                    foreach(Card c in aIDuel.oppState.onField){
                        WeightContainer wc;
                        wc.card = c;
                        wc.weight = c.AttackPower;
                        insertAdd(wc, SortType.WEIGHT, ref wts);
                    }
                    int maxAttack = Random.Range(wts.Count/2, wts.Count);
                    for(int i = 0; i < maxAttack; i++){
                        aIDuel.addOppAttacker(wts[wts.Count-i-1].card.CardName);
                    }

                }
                private static int normalWeight(AIDuel data, Card c){
                    float res = 0;
                    res = (c.CardCost * Weights.MANA) + (turnsToCast(data.currentTurn, c.CardCost, data.oppState.mana) * Weights.TURN_DIFFERENCE) + (c.AttackPower * Weights.POWER) + Random.Range(0,3);
                    if(data.oppState.hp < 20 && c.SpellType == CardType.SPELL){
                        res += 1;
                    }
                    if(data.oppState.hp < 10 && c.SpellType == CardType.SPELL){
                        res += 2;
                    }
                    if(c.CardCost == 0){
                        res = -5; //AI will play 0 cost cards
                    }
                    return (int)res;
                }

                private static void normalBlock(AIDuel ad){
                    Debug.Log("AI Blocking...");
                    //make lists
                    List<WeightContainer> attackers = new List<WeightContainer>();
                    List<WeightContainer> flyAttackers = new List<WeightContainer>();
                    List<WeightContainer> blockers = new List<WeightContainer>();
                    
                    //get attackers
                    foreach(AttackBlock ab in ad.attackers){
                        WeightContainer wc;
                        if(ab.attackCard.HasFly){
                            wc.card = ab.attackCard;
                            wc.weight = 0;
                            insertAdd(wc, SortType.POWER, ref flyAttackers);
                        }else{
                            wc.card = ab.attackCard;
                            wc.weight = 0;
                            insertAdd(wc, SortType.POWER, ref attackers);
                        }
                    }
                    //get blockers
                    foreach(Card c in ad.oppState.onField){
                        WeightContainer wc;
                        wc.card = c;
                        wc.weight = 0;
                        insertAdd(wc, SortType.DEFENCE, ref blockers);

                    }
                    //block strongest fliers with weakest
                    int count = flyAttackers.Count-1;
                    for(int j = count; j >= 0;j--){
                        for(int i = 0; i < blockers.Count; i++){
                            if(blockers[i].card.HasFly){
                                ad.addOppBlocker(flyAttackers[count].card.CardName + "-" + blockers[i].card.CardName);
                                blockers.RemoveAt(i);
                                break;
                            }
                        }

                        count--;
                    }

                    //block strongest attackers
                    count = attackers.Count-1;
                    for(int j = count; j >= 0;j--){
                        if(blockers.Count==0){
                            break;
                        }
                        ad.addOppBlocker(attackers[count].card.CardName + "-" + blockers[0].card.CardName);
                        blockers.RemoveAt(0);
                        count--;
                    }


                }

            #endregion
            //hard ai looks at both states
           

            #region hard AI

            public static IEnumerator hardTurn(AIDuel ad){
                Debug.Log("AI starting...");
                    //wait
                    yield return new WaitForSeconds(.5f);

                    //play cards
                    Debug.Log("AI Casting...");
                    int count = ad.oppState.inHand.Count;
                    while(count > 0 && hardCast(ref ad)){
                        Debug.Log(count);
                        count--;
                        yield return new WaitForSeconds(.5f);
                        
                    }
                    

                    //attack
                    Debug.Log("AI attacking...");
                    attackHard(ad);

                    //to block
                    ad.ToBlockPhase(); 
            }
            private static bool hardCast(ref AIDuel ad){
                //create weight list 
                List<WeightContainer> wts = new List<WeightContainer>();
                foreach(Card c in ad.oppState.inHand){
                    WeightContainer wc;
                    wc.card = c;
                    wc.weight = normalWeight(ad, c);
                    insertAdd(wc, SortType.WEIGHT, ref wts);
                }
                Debug.Log("AI First Choice: " + wts[0].card.CardName);
                if(!PlayCard(ad, wts[0].card)){
                    return false;
                }
                return true;
            }

            private static int hardWeight(AIDuel data, Card c){
                float res = 0;
                    res = (c.CardCost * Weights.MANA) + (turnsToCast(data.currentTurn, c.CardCost, data.oppState.mana) * Weights.TURN_DIFFERENCE) + (c.AttackPower * Weights.POWER) + (c.DefencePower * Weights.POWER) + Random.Range(0,3);
                    if(data.oppState.hp < 20 && c.SpellType == CardType.SPELL){
                        res += 1;
                    }
                    if(data.oppState.hp < 10 && c.SpellType == CardType.SPELL){
                        res += 2;
                    }
                    if(c.CardCost == 0){
                        res = -5; //AI will play 0 cost cards
                    }
                    if(c.HasFly){
                        res -= 1;
                    }
                    return (int)res;
            }

            private static void attackHard(AIDuel aIDuel){
                    //create list
                    List<WeightContainer> wts = new List<WeightContainer>();

                    if(aIDuel.myState.onField.Count == 0){
                        //attack with all
                        foreach(Card c in aIDuel.oppState.onField){                        
                            aIDuel.addOppAttacker(c.CardName);                        
                        }
                        return;
                    }
                    //attack all fliers
                    foreach(Card c in aIDuel.oppState.onField){
                        if(c.HasFly){
                            aIDuel.addOppAttacker(c.CardName);
                        }
                    }
                    foreach(Card c in aIDuel.oppState.onField){
                        WeightContainer wc;
                        wc.card = c;
                        wc.weight = c.AttackPower;
                        if(!c.HasFly)
                            insertAdd(wc, SortType.WEIGHT, ref wts);
                    }
                    int maxAttack = Random.Range(wts.Count/2, wts.Count);
                    for(int i = 0; i < maxAttack; i++){
                        aIDuel.addOppAttacker(wts[wts.Count-i-1].card.CardName);
                    }

                }

                private static void blockHard(AIDuel ad){
                    Debug.Log("AI Blocking...");
                    //make lists
                    List<WeightContainer> attackers = new List<WeightContainer>();
                    List<WeightContainer> flyAttackers = new List<WeightContainer>();
                    List<WeightContainer> blockers = new List<WeightContainer>();

                    
                    
                    //get attackers
                    foreach(AttackBlock ab in ad.attackers){
                        WeightContainer wc;
                        if(ab.attackCard.HasFly){
                            wc.card = ab.attackCard;
                            wc.weight = 0;
                            insertAdd(wc, SortType.POWER, ref flyAttackers);
                        }else{
                            wc.card = ab.attackCard;
                            wc.weight = 0;
                            insertAdd(wc, SortType.POWER, ref attackers);
                        }
                    }
                    //get blockers
                    foreach(Card c in ad.oppState.onField){
                        WeightContainer wc;
                        wc.card = c;
                        wc.weight = 0;
                        insertAdd(wc, SortType.DEFENCE, ref blockers);

                    }
                    //block strongest fliers with weakest
                    int blocked = 0;
                    int count = flyAttackers.Count-1;
                    for(int j = count; j >= 0;j--){
                        for(int i = 0; i < blockers.Count; i++){
                            if(blockers[i].card.HasFly && stratBlock(flyAttackers[count].card, blockers[i].card)){
                                ad.addOppBlocker(flyAttackers[count].card.CardName + "-" + blockers[i].card.CardName);
                                blockers.RemoveAt(i);
                                blocked++;
                                break;
                            }
                        }

                        count--;
                    }
                    //block the srongest flying creature with weakest
                    if(flyAttackers.Count > 0){
                        for(int i = 0; i < blockers.Count; i++){
                            if(blockers[i].card.HasFly && flyAttackers[flyAttackers.Count-1].card.AttackPower > 3){
                                ad.addOppBlocker(flyAttackers[flyAttackers.Count-1].card.CardName + "-" + blockers[i].card.CardName);
                                blockers.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    //block strongest attackers
                    blocked = 0;
                    for(int j = 0; j < attackers.Count;j++){
                        if(blockers.Count == 0){
                            break;
                        }
                        for(int i = 0; i < blockers.Count; i++){
                            if(stratBlock(attackers[j].card, blockers[i].card)){
                                ad.addOppBlocker(attackers[j].card.CardName + "-" + blockers[i].card.CardName);
                                blockers.RemoveAt(i);
                                blocked++;
                                break;
                            }
                        }                        
                    }
                    if(blocked == 0 && blockers.Count > 0 && attackers[attackers.Count-1].card.AttackPower > 3){
                        ad.addOppBlocker(attackers[attackers.Count-1].card.CardName + "-" + blockers[0].card.CardName);
                    }
                }

                private static int attackPotential(PlayerState state){
                    int res = 0;
                    foreach(Card c in state.onField){
                        res += c.AttackPower;
                    }
                    return res;
                }

                private static bool stratBlock(Card a, Card b){
                    return a.AttackPower < b.DefencePower || a.DefencePower == 1 || a.DefencePower == b.AttackPower;
                }

            #endregion
            

        #endregion


        #region library lists
            private static string[] easyDecks = new string[] {"Mi_Go-20,Blood Vial-12",
                                                                "Mi_Go-20,Mi_Go Worker-12"};
            private static string[] normalDecks = new string[] {"Mi_Go-10,Mi_Go Worker-10,Blood Vial-7,Madman's Knowledge-5"};
            private static string[] hardDecks = new string[] {"Mi_Go Worker-10,Blood Vial-8,Madman's Knowledge-2,Great One's Wisdom-1",
                                                                    "Mi_Go Worker-10,Mi_Go Queen-4,Mi_Go Zombie-8,Mi_Go-10"};
            private static string[] extremeDecks = new string[] {"Great One's Wisdom-4,Madman's Knowledge-10,Mi_Go Queen-4,Mi_Go Zombie-6,Moon Presence-2,Lady Maria-4,Pungent Blood Cocktail-2",
                                                                    "Great One's Wisdom-4,Madman's Knowledge-10,Mi_Go Queen-4,Mi_Go Zombie-6,Chime Maiden-5,Mi_Go Worker-3"};
        #endregion
        
    }
}
