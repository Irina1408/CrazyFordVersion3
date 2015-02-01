using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord.Data.Clumns
{
    public class ColumnResult : ColumnBase
    {
    #region Public fields

        public Lear ColumnLear { get; private set; }

    #endregion

    #region Initialize

        public ColumnResult()
            : base()
        {
            this.MaxCount = AdditionalData.CountCardNames;
        }

    #endregion
        
    #region Overrides

        public override void AddCard(Card card)
        {
            if (base.Count == 0)
            {
                ColumnLear = card.Data.CardLear;
            }

            base.AddCard(card);
            card.CanMove = false;
            card.Place = CardPlace.ResultColumn;
        }

    #endregion
    }
}
