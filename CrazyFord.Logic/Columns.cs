using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Logic
{
    using CrazyFord.Data;
    using CrazyFord.Data.Cards;
    using CrazyFord.Data.Columns;
    using CrazyFord.Data.Columns.BaseClasses;

    public class Columns
    {
        #region Initialization

        public Columns()
        {
            ColResult = new ColumnResult[StaticGameData.CountResultColumns];
            ColGame = new ColumnGame[StaticGameData.CountGameColumns];
            ColAdditional = new AdditionalColumn[2];
            ColDeck = new ColumnDeck();

            // initialize every column
            for (int index = 0; index < ColResult.Length; index++)
            {
                ColResult[index] = new ColumnResult();
            }

            for (int index = 0; index < ColGame.Length; index++)
            {
                ColGame[index] = new ColumnGame(index);
            }

            ColAdditional[Constants.iColKing] = new AdditionalColumn(CrazyFord.Data.Name.King);
            ColAdditional[Constants.iColJoker] = new AdditionalColumn(CrazyFord.Data.Name.Joker);
        }

        #endregion

        #region Public properties

        public ColumnResult[] ColResult { get; set; }
        public ColumnGame[] ColGame { get; set; }
        public AdditionalColumn[] ColAdditional { get; set; }
        public ColumnDeck ColDeck { get; set; }

        #endregion

        #region Public methods

        #region Index methods

        /// <summary>
        /// Returns game column index where card is placed
        /// </summary>
        public int? GetGameColIndex(ICard card)
        {
            for (int index = 0; index < ColGame.Length; index++)
            {
                if (ColGame[index].Contains(card))
                    return index;
            }
            return null;
        }

        /// <summary>
        /// Returns result column index where card is placed
        /// </summary>
        public int? GetResultColIndex(ICard card)
        {
            for (int index = 0; index < ColResult.Length; index++)
            {
                if (ColResult[index].Contains(card))
                    return index;
            }
            return null;
        }

        /// <summary>
        /// Returns game column index throw all game columns
        /// </summary>
        public int GetGameColIndex(IColumn column)
        {
            for (int i = 0; i < ColGame.Length; i++ )
            {
                if (ColGame[i] == column)
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Returns result column index throw all game columns
        /// </summary>
        public int GetResultColIndex(IColumn column)
        {
            for (int i = 0; i < ColResult.Length; i++)
            {
                if (ColResult[i] == column)
                    return i;
            }

            return 0;
        }

        #endregion

        #region Event (seting) methods ???

        /// <summary>
        /// Sets event handler after add card in every column
        /// </summary>
        public void SetAfterAddCardEvent(ChangeCardColumnEventHandler afterAddCardEvent)
        {
            if (afterAddCardEvent == null) return;

            foreach (var col in ColResult)
                col.AfterAddCard += afterAddCardEvent;

            foreach (var col in ColGame)
                col.AfterAddCard += afterAddCardEvent;

            ColAdditional[Constants.iColKing].AfterAddCard += afterAddCardEvent;
            ColAdditional[Constants.iColJoker].AfterAddCard += afterAddCardEvent;
            ColDeck.AfterAddCard += afterAddCardEvent;
        }

        /// <summary>
        /// Sets event handler after remove card in every column
        /// </summary>
        public void SetAfterRemoveCardEvent(ChangeCardColumnEventHandler afterRemoveCardEvent)
        {
            if (afterRemoveCardEvent == null) return;

            foreach (var col in ColResult)
                col.AfterRemoveCard += afterRemoveCardEvent;

            foreach (var col in ColGame)
                col.AfterRemoveCard += afterRemoveCardEvent;

            ColAdditional[Constants.iColKing].AfterRemoveCard += afterRemoveCardEvent;
            ColAdditional[Constants.iColJoker].AfterRemoveCard += afterRemoveCardEvent;
            ColDeck.AfterRemoveCard += afterRemoveCardEvent;
        }

        /// <summary>
        /// Sets event handler on deck column changed (adding card and removing card)
        /// </summary>
        public void SetDeckColumnChangedEvent(ChangeCardColumnEventHandler deckColumnChanged)
        {
            if (deckColumnChanged == null) return;

            ColDeck.AfterAddCard += deckColumnChanged;
            ColDeck.AfterRemoveCard += deckColumnChanged;
        }

        #endregion

        /// <summary>
        /// Clear all columns
        /// </summary>
        public void Clear()
        {
            // clear all result columns
            foreach (var col in ColResult)
                col.Clear();

            // clear all game columns
            foreach (var col in ColGame)
                col.Clear();

            // clear all additional columns
            foreach (var col in ColAdditional)
                col.Clear();
            
            // clear deck
            ColDeck.Clear();
        }

        #endregion
    }
}
