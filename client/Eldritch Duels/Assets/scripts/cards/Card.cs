using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards
{

    public enum CardType
    {
        CREATURE,
        SPELL,
        ARTIFACT,
        ARTIFACT_CREATURE,
        PLAYER,
        NULL
    }
    public enum CardRarity
    {
        COMMON,
        RARE,
        LEGENDARY,
        NULL
    }

    public class Card : MonoBehaviour
    {
        #region card_params
        private string cardName = "";
        public string CardName
        {
            get { return this.cardName; }
        }

        private int id = -1;
        public int CardID
        {
            get { return this.id; }
        }

        private int owned = 0;
        public int CopiesOwned
        {
            get { return this.owned; }
            set { if (value >= 0) { this.owned = value; } }
        }

        private int cost = 0;
        public int CardCost
        {
            get { return this.cost; }
            set { if (value >= 0) { this.cost = value; } }
        }

        private int power = 0;
        public int AttackPower
        {
            get { return this.power; }
            set { this.power = value; }
        }

        private int defence = 0;
        private int maxDef = -1;
        public int DefencePower
        {
            get { return this.defence; }
            set { this.defence = value; if (this.maxDef == -1) { maxDef = value; } }
        }

        private CardType type = CardType.NULL;
        public CardType SpellType
        {
            get { return this.type; }
            set { this.type = value; }
        }

        private CardRarity rartiy = CardRarity.NULL;
        public CardRarity SpellRarity
        {
            get { return this.rartiy; }
            set { this.rartiy = value; }
        }

        private Material cardImage = null;
        public Material CardImage
        {
            get { return this.cardImage; }
            set { if (value != null) { this.cardImage = value; } }
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
            if(this.type != CardType.NULL && this.type != CardType.ARTIFACT)
            {
                this.defence -= amount;
            }

            if (this.defence <= 0 && this.type == CardType.PLAYER)
            {
                //TODO player loss
            }
        }


        //reset values and check if card is alive
        public void EndOfTurn()
        {
            this.defence = this.maxDef;

        }
    }
}
