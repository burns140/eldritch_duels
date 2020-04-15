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


            //chose first player
        }

        #region game flow
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
        public void NextPhase(){

        }
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
        public void RecallCard(string cardName){

        }

        private void AITurn(){
            AIScript.AITurn(this);
            endOppTurn();
        }



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

            //ai turn
            AITurn();
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
