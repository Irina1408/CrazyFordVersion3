using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data.Columns
{
    using CrazyFord.Data.Columns.BaseClasses;
    using CrazyFord.Data.Cards;

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
            this.MaxCount = StaticGameData.CountCardNames + colNumber;
        }

        #endregion

        #region Overrides

        public override void AddCard(ICard card)
        {
            base.AddCard(card);

            card.Place = CardPlace.GameColumn;
        }

        public override void DeleteCard(ICard card)
        {
            base.DeleteCard(card);

            ICard tempCard = GetLastCard();
            if (tempCard != null)
            {
                if (!tempCard.IsShowedFace)
                {
                    tempCard.ShowFace();
                    tempCard.AllowMove = true;
                }
            }
        }

        #endregion

        #region Public methods

        public Name GetCardMovingNameInColumn(ICard card, int numJoker = 0)
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

        public Name GetCardReceivingNameInColumn(ICard card, int numJoker = 0)
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

        public Color GetCardColorInColumn(ICard card)
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

            foreach (ICard card in Cards)
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

        public int GetCardDistance(ICard card, int cardDistanceFace, int cardDistanceBack)
        {
            return 0;
        }

        #endregion

        #region Private methods

        private Color GetCardColorDown(ICard card, int numJoker = 0)
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

        private Color GetCardColorUp(ICard card, int numJoker = 0)
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

        private ICard GetFirstShowedCard()
        {
            //get first showed card
            return this.Cards.FirstOrDefault(card => card.IsShowedFace);
        }

        #endregion
    }
}
