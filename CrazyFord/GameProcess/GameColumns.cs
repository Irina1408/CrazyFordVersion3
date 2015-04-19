using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CrazyFord.Data;
using CrazyFord.Data.Clumns;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord.GameProcess
{
    public class GameColumns
    {
        #region Private fields


        #endregion

        #region Init

        public GameColumns(ColumnBase.ChangeColumnDelegate alignDeckSequence)
        {
            ColResult = new ColumnResult[AdditionalData.CountResultColumns];
            ColGame = new ColumnGame[AdditionalData.CountGameColumns];
            ColAdditional = new AdditionalColumn[2];
            ColDeck = new ColumnDeck();

            InitializeColumns(alignDeckSequence);
        }

        private void InitializeColumns(ColumnBase.ChangeColumnDelegate alignDeckSequence)
        {
            for (int index = 0; index < ColResult.Length; index++)
            {
                ColResult[index] = new ColumnResult();
                ColResult[index].AfterAddCardEvent += AlignOnZindex;
            }

            for (int index = 0; index < ColGame.Length; index++)
            {
                ColGame[index] = new ColumnGame(index);
                ColGame[index].AfterAddCardEvent += AlignOnZindex;
            }

            ColAdditional[GameWindowConstants.iColKing] = new AdditionalColumn(CrazyFord.Data.Name.King);
            ColAdditional[GameWindowConstants.iColKing].AfterAddCardEvent += AlignOnZindex;
            ColAdditional[GameWindowConstants.iColJoker] = new AdditionalColumn(CrazyFord.Data.Name.Joker);
            ColAdditional[GameWindowConstants.iColJoker].AfterAddCardEvent += AlignOnZindex;

            ColDeck.AfterAddCardEvent += AlignOnZindex;
            ColDeck.AfterAddCardEvent += alignDeckSequence;
            ColDeck.AfterDeleteCardEvent += alignDeckSequence;
        }

        #endregion

        #region Public properties

        public ColumnResult[] ColResult { get; set; }
        public ColumnGame[] ColGame { get; set; }
        public AdditionalColumn[] ColAdditional { get; set; }
        public ColumnDeck ColDeck { get; set; }

        #endregion

        #region Private methods

        private void AlignOnZindex(ColumnBase column)
        {
            for (int index = 0; index < column.Count; index++)
            {
                Grid.SetZIndex(column[index], index);
            }
        }

        #endregion
    }
}
