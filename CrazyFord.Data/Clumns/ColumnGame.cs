using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord.Data.Clumns
{
    public class ColumnGame : ColumnBase
    {
    #region Private fields

        private int _colIndex;

    #endregion

    #region Initialize

        public ColumnGame(int colNumber) 
            :base()
        {
            _colIndex = colNumber;
            this.MaxCount = AdditionalData.CountCardNames + colNumber;
        }

    #endregion

    #region Overrides

        public override void AddCard(Card card)
        {
            base.AddCard(card);

            card.Place = CardPlace.GameColumn;
        }

        public override void DeleteCard(Card card)
        {
            base.DeleteCard(card);

            Card tempCard = GetLastCard();
            if (tempCard != null)
            {
                if (!tempCard.IsShowedFace)
                {
                    tempCard.ShowFace();
                    tempCard.CanMove = true;
                }
            }
        }

    #endregion

    #region Public methods

        public Name GetCardMovingNameInColumn(Card card, int numJoker = 0)
        {
            //if is not joker
            if (card.Data.CardName != Name.Joker)
            {
                return card.Data.CardName + numJoker;
            }

            //if this joker is not last
            if (!Equals(GetLastCard(), card))
            {
                return GetCardMovingNameInColumn(Cards[Cards.IndexOf(card) + 1], numJoker + 1);
            }

            return Name.Joker;
        }

        public Name GetCardReceivingNameInColumn(Card card, int numJoker = 0)
        {
            //if is not joker
            if (card.Data.CardName != Name.Joker)
            {
                return card.Data.CardName - numJoker;
            }

            if (GetFirstCard().IsShowedFace && Equals(GetFirstCard(), card))
            {
                return Name.King - numJoker;
            }

            //if this joker is not first
            if (!Equals(GetFirstShowedCard(), card))
            {
                return GetCardReceivingNameInColumn(Cards[Cards.IndexOf(card) - 1], numJoker + 1);
            }

            return Name.Joker;
        }

        public Color GetCardColorInColumn(Card card)
        {
            if (card.Data.CardColor != Color.None)
            {
                return card.Data.CardColor;
            }

            Color cardColorDown = GetCardColorDown(card);

            return cardColorDown == Color.None ? GetCardColorUp(card) : cardColorDown;
        }

        public int GetCountBackCards()
        {
            int count = 0;

            foreach (Card card in Cards)
            {
                if (!card.IsShowedFace)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public int GetCardDistance(Card card, int cardDistanceFace, int cardDistanceBack)
        {
            return 0;
        }

    #endregion

    #region Private methods

        private Color GetCardColorDown(Card card, int numJoker = 0)
        {
            //if this card is not joker
            if (card.Data.CardName != Name.Joker)
            {
                return numJoker%2 != 0 ? card.Data.GetOppositeColor() : card.Data.CardColor;
            }

            //if this joker is not last
            if (!Equals(GetLastCard(), card))
            {
                return GetCardColorDown(Cards[Cards.IndexOf(card) + 1], numJoker + 1);
            }

            return Color.None;
        }

        private Color GetCardColorUp(Card card, int numJoker = 0)
        {
            //if this card is not joker
            if (card.Data.CardName != Name.Joker)
            {
                return numJoker % 2 != 0 ? card.Data.GetOppositeColor() : card.Data.CardColor;
            }

            //if this joker is not last
            if (!Equals(GetFirstShowedCard(), card))
            {
                return GetCardColorUp(Cards[Cards.IndexOf(card) - 1], numJoker + 1);
            }

            return Color.None;
        }

        private Card GetFirstShowedCard()
        {
            //get first showed card
            return this.Cards.FirstOrDefault(card => card.IsShowedFace);
        }

    #endregion
    }
}
