using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using eldritch.cards;

namespace eldritch{
    public class AIDuel : MonoBehaviour
    {
        public PlayerState myState;
        public PlayerState oppState;

        public Phase currentPhase = Phase.WAITING;

        private int currentTurn = 1;
        private int myTurnsNum = 0;

        public Text phaseText = null;
        public GameObject CardHolder;
        public GameObject MyHand;
        public GameObject MyField;
        public GameObject OppField;
        private bool myTurn = false;
        private bool hasRecalled = false;
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
                currentPhase = Phase.MAIN;
                phaseText.text = "ATTACK";
            }
        }
        void Update(){
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

        private IEnumerator startText(){
            if(Global.DuelMyTurn){
                gameText.text = "FIRST PLAYER";
            }else{
                gameText.text = "SECOND PLAYER";
            }

            yield return new WaitForSeconds(2);
            gameText.text = "";
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
                
            }
        }

        //management
        
        public bool CanCast(GameObject c){
            if(DuelFunctions.CanCast(Library.GetCard(c.name), myState)){
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

        public void CastCard(string cardName){

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

        }

        private void AITurn(){
            AIScript.AITurn(this);
            endOppTurn();
        }

        #region attack phase
        public List<AttackBlock> attackers = new List<AttackBlock>();
            //add attacker to attack with
            public void AddAttacker(GameObject attacker){
                if(attacker.GetComponent<Draggable>() == null){
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
                Debug.Log("# attacking: " + attackers.Count);
            }
            //remove creature to attack with
            public void RemoveAttacker(GameObject attacker){
                if(attacker.GetComponent<Draggable>() == null){
                    return;
                }
                for(int i = 0; i < attackers.Count;i++){
                    if(attackers[i].attacker.GetHashCode().Equals(attacker.GetHashCode())){
                        attackers.RemoveAt(i);
                        return;
                    }
                }
            }
            
            private GameObject atb; //placehoder for attacker
            private GameObject blockwith; //placeholder for blocker
            //set placeholder for attacker
            public void SetToBlock(GameObject attacker){
                if(atb != null){
                    atb.GetComponent<Draggable>().isBlocking = false;
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
                    blockwith.GetComponent<Draggable>().isBlocking = false;
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
                        attackers[i].blocker.GetComponent<Draggable>().isBlocking = false;
                        attackers[i].blocker.GetComponent<Image>().color = Color.white;
                        ab.blocker = null;
                        attackers[i] = ab;
                        attackers[i].attacker.GetComponent<Draggable>().isBlocking = false;
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
                        attackers[i].blocker.GetComponent<Draggable>().isBlocking = false;
                        attackers[i].blocker.GetComponent<Image>().color = Color.white;
                        ab.blocker = null;
                        attackers[i] = ab;
                        attackers[i].attacker.GetComponent<Draggable>().isBlocking = false;
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
                    blockwith.GetComponent<Draggable>().isBlocking = false;
                    blockwith = null;
                    return;
                }

                for(int i = 0; i < attackers.Count;i++){
                    if(attackers[i].attacker.GetHashCode().Equals(atb.GetHashCode())){
                        AttackBlock ab = attackers[i];
                        ab.blocker = blockwith;
                        attackers[i] = ab;
                        Debug.Log("Link made");
                        break;
                    }
                }
                atb = null;
                blockwith = null;
                Debug.Log("links: " + attackers.Count);
            }

            //format and send blocker string to opponent
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
                foreach(AttackBlock ab in attackers){
                    if(first){
                        data = data + ab.attacker.name;
                        first = false;
                    }else{
                        data = data + "," + ab.attacker.name;
                    }
                }
                currentPhase = Phase.WAITING;
                AIBlock();
            }

            private void AIBlock(){
                AIScript.AIBlock(this);
            }

            private void oppAttack(){
                foreach(AttackBlock ab in attackers){
                    
                    ab.attacker.GetComponent<Image>().color = Color.white;
                    if(ab.blocker != null)
                        ab.blocker.GetComponent<Image>().color = Color.white;
                    if(ab.blocker == null){
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
                        Destroy(card);
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
                        Destroy(card);
                        return;
                    }
                }
            }

            private void updateMyHealth(int hit){
                // @TODO get attack value from server (@KEVIN M)
                myState.hp -= hit; // Decrease attack from HP
                myHPImage.fillAmount = myState.hp/DuelFunctions.START_HEALTH; // Update my HP on UI
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
                    checkDeckCount();
                    //ai turn
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
                private void endOppTurn(){
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
                    OppField.transform.GetChild(i).GetComponent<Draggable>().isAttacking = false;
                    OppField.transform.GetChild(i).GetComponent<Draggable>().isBlocking = false;
                    OppField.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                }

                //reset me
                for(int i = 0; i< MyField.transform.childCount;i++){
                    MyField.transform.GetChild(i).GetComponent<Draggable>().isAttacking = false;
                    MyField.transform.GetChild(i).GetComponent<Draggable>().isBlocking = false;
                    MyField.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                }
            }

        #endregion

        #region endgame
            public void Surrender(){
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
    }
}
