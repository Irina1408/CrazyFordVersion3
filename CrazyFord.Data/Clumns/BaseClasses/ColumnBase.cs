using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data.Clumns.BaseClasses
{
    public class ColumnBase
    {
        #region Protected fields

        protected List<Card> Cards = new List<Card>();
        protected int MaxCount;

        #endregion

        #region Public fields

        public delegate void ChangeColumnDelegate(ColumnBase columnBase);

        public event ChangeColumnDelegate AfterAddCardEvent;

        public event ChangeColumnDelegate AfterDeleteCardEvent;

        #endregion

        #region Public properties

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

        public Card this [int index]
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

        #endregion

        #region Initialize

        public ColumnBase() 
        {

        }

        #endregion

        #region Public functions

        public virtual void AddCard(Card card)
        {
            Cards.Add(card);

            if(AfterAddCardEvent != null)
            {
                AfterAddCardEvent(this);
            }
        }
        
        public virtual void DeleteCard(Card card)
        {
            Cards.Remove(card);

            if (AfterDeleteCardEvent != null)
            {
                AfterDeleteCardEvent(this);
            }
        }

        public virtual void Clear()
        {
            Cards.Clear();
        }

        public Card GetLastCard()
        {
            if (Cards.Count > 0)
            {
                return Cards[Cards.Count - 1];
            }
            return null;
        }

        public Card GetFirstCard()
        {
            if (Cards.Count > 0)
            {
                return Cards[0];
            }
            return null;
        }

        public void SetAllowDrop(bool allowDrop = false)
        {
            foreach (Card card in Cards)
            {
                card.AllowDrop = allowDrop;
            }
        }

        public bool Contains(Card card)
        {
            return Cards.Contains(card);
        }

        public int GetCardIndex(Card card)
        {
            return Cards.IndexOf(card);
        }

        #endregion
    }
}
