using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using eldritch.cards;
using System;

namespace eldritch{
    public class AIDuel : MonoBehaviour
    {
        public PlayerState myState;
        public PlayerState oppState;

        public Phase currentPhase = Phase.WAITING;

        public int currentTurn = 1;
        private int myTurnsNum = 0;

        public Text phaseText = null;
        public GameObject CardHolder;
        public GameObject MyHand;
        public GameObject MyField;
        public GameObject OppField;
        
        public bool myTurn = false;
        
        public bool hasRecalled = false;
        public Text gameText;
        public Text myManaText; // Text to show my current Mana
        public Text oppManaText; // Text to show opponent's current Mana
        public Text myHPText; // Text to show my current HP
        public Text oppHPText; // Text to show opponent's current HP
        public Image myHPImage; // Image to show my current HP

        public Image oppHPImage; // Image to show opponent's current HP
        private const string WON_PREF_KEY = "whowon"; // PREF KEY to store who won
        private const string CREDIT_PREF_KEY = "credits"; // PREF KEY to store credits
        public Button recallButton; // Recall button on the UI
        public GameObject cardBack1;
        public GameObject cardBack2;
        public GameObject cardBack3;
        public GameObject cardBack4;
        public bool haltAttack = false;
        private List<string> logWithoutDetail = new List<string>(); // Log to store moves without details
        private List<string> logWithDetail = new List<string>(); // Log to store moves with details
        public GameObject logPanel;
        public GameObject logPanelHolder;
        public GameObject buttonPrefab;
        public CardSounds cardsoundsScript;

        public GameObject soundCard; // object with audio source

        void Start(){
            //init myState
            myState.hp = DuelFunctions.START_HEALTH;
            myState.mana = 1;
            myState.library = DuelFunctions.ShuffleLibrary(DuelFunctions.GetLibrary());

            //init oppstate
            oppState.hp = DuelFunctions.START_HEALTH;
            oppState.mana = 1;
            oppState.library = DuelFunctions.ShuffleLibrary(AIScript.GetAILibrary());

            //init draws
            StartCoroutine(initalDraw());

            //chose first player
            if(AIScript.randomBool()){
                myTurn = true;
                gameText.text = "FIRST PLAYER";
                gameText.text = "";
                currentPhase = Phase.MAIN;
                phaseText.text = "ATTACK";
                
            }else{
                gameText.text = "SECOND PLAYER";
                gameText.text = "";
                AIScript.AITurn(this);
            }
            

            
            
        }

        private bool prevHalt;
        void Update(){
            loadLog();
            checkDeckCount(); // Check & update card back quantity on deck UI
            //update hp and mana
            setMyHealth(myState.hp);
            setOppHealth(oppState.hp);
            myHPText.text = myState.hp + " HP";
            myManaText.text = myState.mana + " MANA";
            oppHPText.text = oppState.hp + " HP";
            oppManaText.text = oppState.mana + " MANA";

            if(oppState.hp <= 0){
                endGame(true);
            }else if(myState.hp <= 0){
                endGame(false);
            }
            if(prevHalt && !haltAttack){
                myAttack();
            }
            prevHalt = haltAttack;
        }

        IEnumerator initalDraw(){
            //my draw
            int handCount=1;
            while(handCount<=6){ // To add 6 cards to hand
                Card b = DuelFunctions.DrawCard(ref myState);
                if(b == null)
                    break;
                GameObject c = (GameObject)Instantiate(CardHolder);
                c.GetComponent<Image>().sprite = null;
                c.GetComponent<Image>().material = b.CardImage;
                c.name = b.CardName;
                c.transform.SetParent(MyHand.transform, false); // Add card to hand
                myState.inHand.Add(b);
                handCount++; 
                yield return new WaitForSeconds(0.5f); 

                
            }

            handCount = 1;
            while(handCount<=6){ // To add 6 cards to hand
                Card b = DuelFunctions.DrawCard(ref oppState);
                if(b == null)
                    break;                
                oppState.inHand.Add(b);
                handCount++; 
                yield return new WaitForSeconds(0.5f); 
            }

            
        }

        

        #region game flow
        public void NextPhase(){
            if(phaseText == null){
                return;
            }
            if(currentPhase == Phase.MAIN){
                currentPhase = Phase.ATTACK;
                phaseText.text = "CONFIRM";
            }else if(currentPhase == Phase.ATTACK && myTurn){
                currentPhase = Phase.WAITING;
                phaseText.text = "WAITING";
                confirmAttackers();
            }else if(!myTurn && currentPhase == Phase.PRE_BLOCK){
                phaseText.text = "CONFIRM";
                currentPhase = Phase.BLOCK;
            }else if(currentPhase == Phase.BLOCK && !myTurn){
                currentPhase = Phase.END;
                confirmBlockers();
            }
        }


        //begin turn
        private void drawCard(){
            if(myState.library.Count>0){
                Card b = DuelFunctions.DrawCard(ref myState);
                myState.inHand.Add(b);
                GameObject c = (GameObject)Instantiate(CardHolder);
                c.GetComponent<Image>().sprite = null;
                c.GetComponent<Image>().material = b.CardImage;
                c.name = b.CardName;
                c.transform.SetParent(MyHand.transform, false); // Add card to my play area
                logWithoutDetail.Add("YOU drew "+c.name);
                logWithDetail.Add("YOU drew "+c.name);
                loadLog();
            }
        }

        //management
        
        public bool CanCast(GameObject c){
            if(DuelFunctions.CanCast(Library.GetCard(c.name), myTurn? myState : oppState)){
                return true;
            }
            return false;
        }
        public bool CanRecall(){
            if(myState.onField.Count < (DuelFunctions.MAX_FIELD - 1) || hasRecalled){
                return false;
            }
            return true;
        }
        //actions

        public void CastCard(string cardName, GameObject c){
            if(myTurn){
                if(myState.onField.Count >= DuelFunctions.MAX_FIELD){
                    return;
                }
                cardsoundsScript.cardPlayedSound(c);
                Global.cardsPlayedThisGame++;
                for(int i = 0; i < myState.inHand.Count;i++){
                    if(myState.inHand[i].CardName.Equals(cardName) && DuelFunctions.CanCast(myState.inHand[i], myState)){
                        Card played = myState.inHand[i];
                        //edit state
                        myState.mana -= played.CardCost;
                        myState.onField.Add(played);
                        myState.inHand.RemoveAt(i);
                        logWithoutDetail.Add("YOU played "+cardName);
                        logWithDetail.Add("YOU played "+cardName+" by using "+played.CardCost+" mana");
                        loadLog();
                        StartCoroutine(resolveAbilities(played, c));                        
                        return;
                        
                    }
                }
            }else{
                Card played = null;
                //update manager
                for(int i = 0; i < oppState.inHand.Count;i++){
                    if(oppState.inHand[i].CardName.Equals(cardName) && DuelFunctions.CanCast(oppState.inHand[i], oppState)){
                        played = oppState.inHand[i];
                        //edit state
                        oppState.mana -= played.CardCost;
                        oppState.onField.Add(played);
                        oppState.inHand.RemoveAt(i);
                        logWithoutDetail.Add("AI played "+cardName);
                        logWithDetail.Add("AI played "+cardName+ " by using "+played.CardCost+" mana");
                        loadLog();
                        StartCoroutine(resolveAbilities(played, c));                        
                        break;
                        
                    }
                }

                //update ui
                GameObject c2 = (GameObject)Instantiate(CardHolder);
                c2.GetComponent<Image>().sprite = null;
                c2.GetComponent<Image>().material = played.CardImage;
                c2.name = played.CardName;
                c2.transform.SetParent(OppField.transform, false); // Add card to opp play area

                
                
                StartCoroutine(resolveAbilities(played, c2));
            }
        }

        private IEnumerator resolveAbilities(Card card, GameObject c){
            if(card.Abilities != null){
                if(myTurn){ //resolve for me
                    foreach(Effect e in card.Abilities){
                        if(e.GetTargetType() == EffectTarget.SELF){
                            e.execute(ref myState);
                        }else if(e.GetTargetType() == EffectTarget.OPPONENT){
                            e.execute(ref oppState);
                        }else if(e.GetTargetType() == EffectTarget.DRAW){
                            drawCard();
                        }
                    }
                }else{ //resolve for opponent
                    foreach(Effect e in card.Abilities){
                        if(e.GetTargetType() == EffectTarget.OPPONENT){
                            e.execute(ref myState);
                        }else if(e.GetTargetType() == EffectTarget.SELF){
                            e.execute(ref oppState);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(.5f);
            if(card.SpellType == CardType.SPELL){
                
                //destroy card
                if(myTurn)
                    destroyMyCard(c);
                else
                    destroyOppCard(c);
            }
        }
        public void RecallCard(string cardName){
            if(myTurn){
                if(myState.onField.Count >= DuelFunctions.MAX_FIELD || true){ //remove true for demo
                    Card recalled = null;
                    //update manager
                    for(int i = 0; i< myState.onField.Count;i++){
                        if(myState.onField[i].CardName.Equals(cardName)){
                            recalled = myState.onField[i];
                            myState.onField.RemoveAt(i);
                            myState.inHand.Add(recalled);
                            break;
                        }
                    }
                    if(recalled == null){
                        return;
                    }
                    hasRecalled = true;
                    recallButton.gameObject.GetComponent<Image>().color = Color.red;                    
                    logWithoutDetail.Add("YOU recalled "+cardName);
                    logWithDetail.Add("YOU recalled "+cardName);
                    loadLog();
                }
                   
            }else{
                //update manager
                for(int i = 0; i < oppState.onField.Count;i++){
                    if(oppState.onField[i].CardName.Equals(cardName)){
                        oppState.onField.RemoveAt(i);
                        logWithoutDetail.Add("AI recalled "+cardName);
                        logWithDetail.Add("AI recalled "+cardName);
                        loadLog();
                        break;
                    }
                }
                //update ui
                foreach(Transform c in OppField.transform){
                    if(c.gameObject.name.Equals(cardName)){
                        Destroy(c.gameObject);
                        break;
                    }
                }
            }
        }

        public void RecallClick(){
            if(myTurn && currentPhase == Phase.MAIN)
            this.currentPhase = Phase.RECALL;
        }

        private void AITurn(){
            
            AIScript.AITurn(this);
            //endOppTurn();
        }

        #region attack phase
        public List<AttackBlock> attackers = new List<AttackBlock>();
            //add attacker to attack with
            public void AddAttacker(GameObject attacker){
                if(attacker.GetComponent<DraggableAI>() == null){
                    return;
                }
                //check if already in attack list
                foreach(AttackBlock g in attackers){
                    if(g.attacker.GetHashCode().Equals(attacker.GetHashCode())){
                        return;
                    }
                }
                AttackBlock at;
                at.attacker = attacker;
                at.attackCard = null;
                foreach(Card c in myState.onField){
                    if(c.CardName.Equals(attacker.name)){
                        at.attackCard = c;
                        break;
                    }
                }
                at.blocker = null;
                attackers.Add(at);
            }
            //remove creature to attack with
            public void RemoveAttacker(GameObject attacker){
                if(attacker.GetComponent<DraggableAI>() == null){
                    return;
                }
                for(int i = 0; i < attackers.Count;i++){
                    if(attackers[i].attacker.GetHashCode().Equals(attacker.GetHashCode())){
                        attackers.RemoveAt(i);
                        return;
                    }
                }
            }

            public void ToBlockPhase(){
                if(myState.onField.Count == 0){
                    confirmBlockers();
                    return;
                }else if(attackers.Count == 0){
                    confirmBlockers();
                    return;
                }
                phaseText.text = "BLOCK";
                currentPhase = Phase.PRE_BLOCK;

            }
            
            private GameObject atb; //placehoder for attacker
            private GameObject blockwith; //placeholder for blocker
            //set placeholder for attacker
            public void SetToBlock(GameObject attacker){
                if(atb != null){
                    atb.GetComponent<DraggableAI>().isBlocking = false;
                    atb.GetComponent<Image>().color = Color.red;
                }
                this.atb = attacker;
                if(atb != null && blockwith != null){
                    makeAttackBlockLink();
                }
            }
            //set placeholder for blocker
            public void SetBlocker(GameObject blocker){
                if(blockwith != null){
                    blockwith.GetComponent<DraggableAI>().isBlocking = false;
                    blockwith.GetComponent<Image>().color = Color.white;
                }
                this.blockwith = blocker;
                if(atb != null && blockwith != null){
                    makeAttackBlockLink();
                }
            }
            //remove place holder for blocker
            public void RemoveBlocker(GameObject blocker){
                for(int i = 0; i < attackers.Count;i++){
                    if(attackers[i].blocker != null && attackers[i].blocker.GetHashCode().Equals(blocker.GetHashCode())){
                        AttackBlock ab = attackers[i];
                        attackers[i].blocker.GetComponent<DraggableAI>().isBlocking = false;
                        attackers[i].blocker.GetComponent<Image>().color = Color.white;
                        ab.blocker = null;
                        attackers[i] = ab;
                        attackers[i].attacker.GetComponent<DraggableAI>().isBlocking = false;
                        attackers[i].attacker.GetComponent<Image>().color = Color.red;
                        break;
                    }
                }
            }
            //remove placeholder for attacker
            public void RemoveToBlock(GameObject attacker){
                for(int i = 0; i < attackers.Count;i++){
                    if(attackers[i].attacker.GetHashCode().Equals(attacker.GetHashCode())){
                        AttackBlock ab = attackers[i];
                        attackers[i].blocker.GetComponent<DraggableAI>().isBlocking = false;
                        attackers[i].blocker.GetComponent<Image>().color = Color.white;
                        ab.blocker = null;
                        attackers[i] = ab;
                        attackers[i].attacker.GetComponent<DraggableAI>().isBlocking = false;
                        attackers[i].attacker.GetComponent<Image>().color = Color.red;
                        break;
                    }
                }
            }

            //sets up link between attacker and blocker
            private void makeAttackBlockLink(){
                //check if can block
                if(!DuelFunctions.CanBlock(Library.GetCard(atb.name), Library.GetCard(blockwith.name))){
                    blockwith.GetComponent<Image>().color = Color.white;
                    blockwith.GetComponent<DraggableAI>().isBlocking = false;
                    blockwith = null;
                    return;
                }
                string saveBlockedLog="";
                bool blocked = false;
                for(int i = 0; i < attackers.Count;i++){
                    if(attackers[i].attacker.GetHashCode().Equals(atb.GetHashCode())){
                        AttackBlock ab = attackers[i];
                        ab.blocker = blockwith;
                        attackers[i] = ab;
                        saveBlockedLog = saveBlockedLog + "," + ab.attacker.name + "with" + ab.blocker.name;
                        blocked = true;
                        break;
                    }
                }
                if(blocked){
                    logWithoutDetail.Add("YOU blocked");
                    logWithDetail.Add("YOU blocked attacks"+saveBlockedLog);
                    loadLog();
                }
                atb = null;
                blockwith = null;
            }

            
            private void confirmBlockers(){
                
                currentPhase = Phase.WAITING;
                oppAttack();
            }

            //format and sent attacker to opponent
            private void confirmAttackers(){
                if(attackers.Count == 0){
                    endMyTurn();
                    return;
                }
                string data = "attack:";
                bool first = true;
                string saveAttackLog = "";
                logWithoutDetail.Add("YOU attacked");
                foreach(AttackBlock ab in attackers){
                    if(first){
                        data = data + ab.attacker.name;
                        first = false;
                    }else{
                        data = data + "," + ab.attacker.name;
                    }
                    saveAttackLog = saveAttackLog + "," + ab.attacker.name;
                }
                logWithDetail.Add("YOU attacked with cards"+saveAttackLog);
                loadLog();
                currentPhase = Phase.WAITING;
                AIBlock();
                if(!haltAttack)
                    myAttack();
            }

            private void myAttack(){
        
                foreach(AttackBlock ab in attackers){
                    if(ab.blocker != null)
                        ab.blocker.GetComponent<Image>().color = Color.white;
                    ab.attacker.GetComponent<Image>().color = Color.white;
                    if(ab.blocker == null && ab.attackCard != null){
                        cardsoundsScript.cardAttackSound(soundCard);
                        updateOppHealth(ab.attackCard.AttackPower);
                    }else if(ab.attackCard != null){
                        Card blocker = Library.GetCard(ab.blocker.name);
                        if(blocker.DefencePower > ab.attackCard.AttackPower){
                            destroyMyCard(ab.attacker);
                        }else if(blocker.DefencePower < ab.attackCard.AttackPower){
                            destroyOppCard(ab.blocker);
                        }else if(ab.attackCard.AttackPower != 0){
                            destroyMyCard(ab.attacker);
                            destroyOppCard(ab.blocker);
                        }
                    }
                }

                attackers.Clear();
                endMyTurn();
            }

            private void AIBlock(){
                Debug.Log("To AI...");
                AIScript.AIBlock(this);
            }

            public void addOppAttacker(string attacker){
                for(int i = 0; i < OppField.transform.childCount;i++){
                    if(OppField.transform.GetChild(i).gameObject.GetComponent<DraggableAI>() != null && //check child is a card placeholder
                    !OppField.transform.GetChild(i).gameObject.GetComponent<DraggableAI>().isAttacking &&
                    OppField.transform.GetChild(i).gameObject.name.Equals(attacker)){ //check card is not already set to attacker
                        AttackBlock attackBlock; //create blank attacker blocker struct
                        OppField.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.red;
                        attackBlock.attacker = OppField.transform.GetChild(i).gameObject; //add teh attacker ui object
                        attackBlock.attackCard = Library.GetCard(attacker); //get the attacker card
                        attackBlock.blocker = null;
                        OppField.transform.GetChild(i).gameObject.GetComponent<DraggableAI>().isAttacking = true; //set card to is attacking
                        attackers.Add(attackBlock); //add attacker to list
                        break;
                    }
                }
                
            }
            public void addOppBlocker(string blocker){
                Debug.Log("AI blocking: " + blocker);
                string[] abpair = blocker.Split('-');
                    for(int a = 0; a < attackers.Count;a++){
                        AttackBlock ab = attackers[a];
                        if(ab.attacker.name.Equals(abpair[0]) && ab.blocker == null){
                            //find ui card
                            for(int i = 0; i < OppField.transform.childCount;i++){
                                if(OppField.transform.GetChild(i).gameObject.name.Equals(abpair[1]) &&
                                !OppField.transform.GetChild(i).gameObject.GetComponent<DraggableAI>().isBlocking){
                                    OppField.transform.GetChild(i).gameObject.GetComponent<DraggableAI>().isBlocking = true;
                                    OppField.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.gray;
                                    ab.blocker = OppField.transform.GetChild(i).gameObject;
                                    attackers[a] = ab;
                                    return;
                                }
                            }
                        }
                    }
            }

            private void oppAttack(){
                foreach(AttackBlock ab in attackers){
                    
                    ab.attacker.GetComponent<Image>().color = Color.white;
                    if(ab.blocker != null)
                        ab.blocker.GetComponent<Image>().color = Color.white;
                    if(ab.blocker == null){
                        cardsoundsScript.cardAttackSound(soundCard);
                        updateMyHealth(ab.attackCard.AttackPower);
                    }else{
                        Card blocker = Library.GetCard(ab.blocker.name);
                        if(blocker.DefencePower < ab.attackCard.AttackPower){
                            destroyMyCard(ab.blocker);
                        }else if (blocker.DefencePower > ab.attackCard.AttackPower){
                            destroyOppCard(ab.attacker);
                        }else if(ab.attackCard.AttackPower != 0){
                            destroyMyCard(ab.blocker);
                            destroyOppCard(ab.attacker);
                        }
                    }
                }

                attackers.Clear();
                endOppTurn();
            }
            private void destroyMyCard(GameObject card){
                for(int i = 0; i < myState.onField.Count;i++){
                    if(myState.onField[i].CardName.Equals(card.name)){
                        myState.onField.RemoveAt(i);
                        //TODO death animation
                        cardsoundsScript.cardDestroySound(soundCard);
                        Destroy(card);
                        logWithoutDetail.Add("YOUR card "+card.name+" is destroyed");
                        logWithDetail.Add("YOUR card "+card.name+" is destroyed");
                        loadLog();
                        return;
                    }
                }
            }
            private void destroyOppCard(GameObject card){
                //find in oppState.onField
                for(int i = 0; i < oppState.onField.Count;i++){
                    if(oppState.onField[i].CardName.Equals(card.name)){
                        oppState.onField.RemoveAt(i);
                        //TODO death animation
                        cardsoundsScript.cardDestroySound(soundCard);
                        Destroy(card);
                        logWithoutDetail.Add("AI card "+card.name+" is destroyed");
                        logWithDetail.Add("AI card "+card.name+" is destroyed");
                        loadLog();
                        return;
                    }
                }
            }

            private void updateOppHealth(int hit){
                oppState.hp -= hit; // Decrease attack from HP
                oppHPImage.fillAmount = (float)oppState.hp/DuelFunctions.START_HEALTH; // Update opponent's HP on UI
                Debug.Log("fillamount: "+oppHPImage.fillAmount);
                logWithoutDetail.Add("YOU successfully hit AI");
                logWithDetail.Add("AI's health decreased by "+hit+" AI's health is now "+oppState.hp);
                loadLog();
            }

            private void updateMyHealth(int hit){
                // @TODO get attack value from server (@KEVIN M)
                myState.hp -= hit; // Decrease attack from HP
                myHPImage.fillAmount = (float)myState.hp/DuelFunctions.START_HEALTH; // Update my HP on UI
                logWithoutDetail.Add("YOU were hit");
                logWithDetail.Add("YOUR health decreased by "+hit+" YOUR health is now "+myState.hp);
                loadLog();
            }

            private void setOppHealth(int health){
                oppState.hp = health;
                oppHPImage.fillAmount = oppState.hp/DuelFunctions.START_HEALTH;
            }

            private void setMyHealth(int health){
                myState.hp = health;
                myHPImage.fillAmount = myState.hp/DuelFunctions.START_HEALTH;
            }
            #endregion



        //end turn
        public void DiscardCard(GameObject card){

        }
        private void endMyTurn(){
            resetStates();
            myTurn = false; // No longer my turn
            currentPhase = Phase.WAITING; //wait for opp move
            oppState.mana = oppState.mana + currentTurn /8 + 1; //increase mana
            phaseText.text = "WAITING";
            hasRecalled = false;
            if(CanRecall()){
                recallButton.gameObject.GetComponent<Image>().color = Color.green;
                ColorBlock cb = recallButton.gameObject.GetComponent<Button>().colors;
                cb.normalColor = Color.green;
                recallButton.gameObject.GetComponent<Button>().colors = cb;
            }
            else{
                recallButton.gameObject.GetComponent<Image>().color = Color.red;
                ColorBlock cb = recallButton.gameObject.GetComponent<Button>().colors;
                cb.normalColor = Color.red;
                recallButton.gameObject.GetComponent<Button>().colors = cb;
            }

            currentTurn++;
            myTurnsNum++;
            //checkDeckCount();
            //ai turn
            Card b = DuelFunctions.DrawCard(ref oppState);
            if(b!= null)              
                oppState.inHand.Add(b);
            AITurn();
        }

        private void checkDeckCount(){
            if(myState.library.Count<30){
                cardBack1.SetActive(false); // Hide first card in deck
            }
            if(myState.library.Count<20){
                cardBack2.SetActive(false); // Hide second card in deck
            }
            if(myState.library.Count<10){
                cardBack3.SetActive(false); // Hide third card in deck
            }
            if(myState.library.Count<1){
                cardBack4.SetActive(false); // Hide fourth/last card in deck
            }
        }


        // End opponent's play
        public void endOppTurn(){
            Debug.Log("AI finished");
            resetStates();
            myTurn = true; // Now it's my turn
            currentPhase = Phase.MAIN; //start my turn
            myState.mana = myState.mana + currentTurn /8 + 1; //increase mana
            phaseText.text = "ATTACK";
            currentTurn++;
            drawCard();
        }

        private void resetStates(){
            //reset opp
            for(int i = 0; i< OppField.transform.childCount;i++){
                OppField.transform.GetChild(i).GetComponent<DraggableAI>().isAttacking = false;
                OppField.transform.GetChild(i).GetComponent<DraggableAI>().isBlocking = false;
                OppField.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }

            //reset me
            for(int i = 0; i< MyField.transform.childCount;i++){
                MyField.transform.GetChild(i).GetComponent<DraggableAI>().isAttacking = false;
                MyField.transform.GetChild(i).GetComponent<DraggableAI>().isBlocking = false;
                MyField.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }

        public void EndTurnNow(){
        if(myTurn && currentPhase != Phase.WAITING){
            endMyTurn();
        }
    }

        #endregion

        #region endgame
            public void Surrender(){
                Global.surrender = 1;
                endGame(false);
            }

            public void AISurrender(){
                endGame(true);
            }
            // End game
            private void endGame(bool iWin){
                // Calculate credits
                // Change scene
                Global.DuelMyTurn = false;
                Global.inQueue = false;
                Global.numTurns = currentTurn;
                if(iWin){
                    PlayerPrefs.SetString(WON_PREF_KEY, "you");
                }
                else{
                    PlayerPrefs.SetString(WON_PREF_KEY, "opp");
                }
                int cred = Global.addDuelCredits(iWin, myTurnsNum, true);
                PlayerPrefs.SetInt(CREDIT_PREF_KEY, cred);
                //PlayerPrefs.SetString(OPP_PROFILE_PREF_KEY, );
                SceneManager.LoadScene("EndDuel");
            }
        #endregion

        public void showLogPanel(){
            if(logPanelHolder.activeSelf){
                logPanelHolder.SetActive(false);
                gameText.text="";
            }
            else{
                loadLog();
                logPanelHolder.SetActive(true); 
            }
        }
        public void loadLog(){
            Button[] gameObjects = logPanel.GetComponentsInChildren<Button>(); // Get previous search results
            foreach(Button o in gameObjects){ 
                Destroy(o.gameObject); // Destroy all previous search results
            }
            for(int i=0; i<logWithoutDetail.Count; i++){
                GameObject logTextObject = (GameObject)Instantiate(buttonPrefab);
                logTextObject.GetComponentInChildren<Text>().text = logWithoutDetail[i];
                logTextObject.SetActive(true);
                logTextObject.transform.SetParent(logPanel.transform, false); // Add logs to log panel
                logTextObject.name = ""+i;
            }
            /*for(int j=0; j<10; j++){
                GameObject logTextObject = (GameObject)Instantiate(buttonPrefab);
                logTextObject.GetComponentInChildren<Text>().text = "test value";
                logTextObject.SetActive(true);
                logTextObject.transform.SetParent(logPanel.transform, false); // Add logs to log panel
                logTextObject.name = ""+j;
                logWithDetail.Add("test value with details");
            }*/
        }

        public void logClicked(Button btn){
            Debug.Log("BUTTON NAME: "+btn.name);
            Debug.Log("BUTTON TEXT: "+btn.GetComponentInChildren<Text>().text);
            int detailPos = Int32.Parse(btn.name);
            Debug.Log("DETAIL LOG: "+logWithDetail[detailPos]);
            gameText.text = logWithDetail[detailPos];
        }
    }
}
