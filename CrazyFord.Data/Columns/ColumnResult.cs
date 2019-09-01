using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data.Columns
{
    using CrazyFord.Data.Columns.BaseClasses;
    using CrazyFord.Data.Cards;

    public class ColumnResult : ColumnBase
    {
        #region Public fields

        public Lear ColumnLear { get; private set; }

        #endregion

        #region Initialize

        public ColumnResult()
            : base()
        {
            this.MaxCount = StaticGameData.CountCardNames;
            this.ColumnLear = Lear.None;
        }

        #endregion
        
        #region Overrides

        public override void AddCard(ICard card)
        {
            if (base.Count == 0)
                ColumnLear = card.Data.CardLear;

            card.AllowMove = false;
            card.Place = CardPlace.ResultColumn;

            base.AddCard(card);
        }

        public override void DeleteCard(ICard card)
        {
            base.DeleteCard(card);

            if (base.Count == 0)
                ColumnLear = Lear.None;
        }

        public override void Clear()
        {
            base.Clear();

            this.ColumnLear = Lear.None;
        }

        #endregion
    }
}
