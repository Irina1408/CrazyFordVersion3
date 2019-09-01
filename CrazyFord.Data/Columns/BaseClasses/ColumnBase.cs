using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data.Columns.BaseClasses
{
    using CrazyFord.Data.Cards;

    public class ColumnBase : IColumn
    {
        #region Protected fields

        protected List<ICard> Cards = new List<ICard>();
        protected int MaxCount;

        #endregion

        #region Initialization

        public ColumnBase()
        {

        }

        #endregion

        #region IColumn implementation

        public event ChangeCardColumnEventHandler AfterAddCard;
        public event ChangeCardColumnEventHandler AfterRemoveCard;

        public int Count
        {
            get { return Cards.Count; }
        }
        public bool IsFull
        {
            get
            {
                return Cards.Count >= MaxCount;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return Cards.Count == 0;
            }
        }
        public ICard this[int index]
        {
            get
            {
                if (index < Count)
                {
                    return Cards[index];
                }
                return null;
            }
            set
            {
                if (index < Count)
                {
                    Cards[index] = value;
                }
            }
        }

        public virtual void AddCard(ICard card)
        {
            Cards.Add(card);

            if (AfterAddCard != null)
                AfterAddCard(this, new ChangeCardColumnEventArgs(card, this));
        }
        public virtual void DeleteCard(ICard card)
        {
            card.Place = CardPlace.None;
            Cards.Remove(card);

            if (AfterRemoveCard != null)
                AfterRemoveCard(this, new ChangeCardColumnEventArgs(card, this));
        }
        public virtual void Clear()
        {
            // revert card placement
            foreach(ICard card in Cards)
            {
                card.Place = CardPlace.None;
            }

            Cards.Clear();
        }

        public bool Contains(ICard card)
        {
            return Cards.Contains(card);
        }
        public ICard GetLastCard()
        {
            if (Cards.Count > 0)
            {
                return Cards[Cards.Count - 1];
            }
            return null;
        }
        public ICard GetFirstCard()
        {
            if (Cards.Count > 0)
            {
                return Cards[0];
            }
            return null;
        }
        public int GetCardIndex(ICard card)
        {
            return Cards.IndexOf(card);
        }
        public ICard GetPreviewCard(ICard card)
        {
            int currentCardIndex = Cards.IndexOf(card);
            return currentCardIndex > 0 ? Cards[currentCardIndex - 1] : null;
        }

        #endregion

        #region Public functions
        
        public void SetAllowMove(bool allowMove = false)
        {
            foreach (ICard card in Cards)
            {
                card.AllowMove = allowMove;
            }
        }

        #endregion
    }
}
