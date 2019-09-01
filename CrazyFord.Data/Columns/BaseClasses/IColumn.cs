using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data
{
    using CrazyFord.Data.Cards;

    public interface IColumn
    {
        #region Event handlers

        event ChangeCardColumnEventHandler AfterAddCard;
        event ChangeCardColumnEventHandler AfterRemoveCard;

        #endregion

        #region Properties

        /// <summary>
        /// Gets count cards in the column
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Gets current column is full or not
        /// </summary>
        bool IsFull { get; }
        
        /// <summary>
        /// Gets current column is empty or not
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets or sets card into column by the index
        /// </summary>
        ICard this[int index] { get; set; }

        #endregion

        #region Methods

        void AddCard(ICard card);
        void DeleteCard(ICard card);
        void Clear();

        bool Contains(ICard card);
        ICard GetLastCard();
        ICard GetFirstCard();
        int GetCardIndex(ICard card);
        ICard GetPreviewCard(ICard card);

        #endregion
    }

    public delegate void ChangeCardColumnEventHandler(object sender, ChangeCardColumnEventArgs e);

    public class ChangeCardColumnEventArgs : EventArgs
    {
        public ChangeCardColumnEventArgs(ICard card, IColumn column)
        {
            Card = card;
            Column = column;
        }

        public ICard Card { get; private set; }
        public IColumn Column { get; private set; }
    }
}
