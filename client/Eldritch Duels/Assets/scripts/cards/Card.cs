using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards
{
    [System.Serializable]
    public enum CardType
    {
        CREATURE,
        SPELL,
        PLAYER,
        NULL
    }
    [System.Serializable]
    public enum CardRarity
    {
        COMMON,
        RARE,
        LEGENDARY,
        NULL
    }

    [System.Serializable]
    public class Card 
    {
        #region card_params
        [SerializeField]
        private string cardName = "";
        public string CardName
        {
            get { return this.cardName; }
        }
        [SerializeField]
        private int id = -1;
        public int CardID
        {
            get { return this.id; }
        }
        [SerializeField]
        private int owned = 0;
        public int CopiesOwned
        {
            get { return this.owned; }
            set { if (value >= 0) { this.owned = value; } }
        }
        [SerializeField]
        private int cost = 0;
        public int CardCost
        {
            get { return this.cost; }
            set { if (value >= 0) { this.cost = value; } }
        }
        [SerializeField]
        private int power = 0;
        public int AttackPower
        {
            get { return this.power; }
            set { this.power = value; }
        }
        [SerializeField]
        private int defence = 0;
        private int maxDef = -1;
        public int DefencePower
        {
            get { return this.defence; }
            set { this.defence = value; if (this.maxDef == -1) { maxDef = value; } }
        }
        [SerializeField]
        private CardType type = CardType.NULL;
        public CardType SpellType
        {
            get { return this.type; }
            set { this.type = value; }
        }
        [SerializeField]
        private CardRarity rartiy = CardRarity.NULL;
        public CardRarity SpellRarity
        {
            get { return this.rartiy; }
            set { this.rartiy = value; }
        }
        [SerializeField]
        private Material cardImage = null;
        public Material CardImage
        {
            get { return this.cardImage; }
            set { if (value != null) { this.cardImage = value; } }
        }

        //get the number of cards not in a deck
        //returns the number of cards that can be used in card crafting
        public int SurplusCopies
        {
            get
            {
                int count = 0;
                foreach (Deck d in Global.userDecks)
                {
                    int tmp = d.AmountInDeck(this.cardName);
                    if (tmp > count)
                        count = tmp;
                }
                if (count > CopiesOwned)
                    count = CopiesOwned;
                return CopiesOwned - count;
            }
        }


        #endregion

        public Card(int id, string name)
        {
            this.id = id;
            this.cardName = name;
        }

        public void CardDestroyed()
        {
            if(this.type == CardType.PLAYER)
            {
                //TODO add player loss
            }else
            {
                //TODO move card to graveyard
            }
        }

        public void DealDamage(int amount)
        {
            if(this.type != CardType.NULL)
            {
                this.defence -= amount;
            }

            if (this.defence <= 0 && this.type == CardType.PLAYER)
            {
                //TODO player loss
            }
        }


        public void EndTurn()
        {
            //restor values

        }
    }
}
