using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord.Data.Clumns
{
    public class AdditionalColumn : ColumnBase
    {
    #region Public properties

        public Name CardName { get; private set; }

    #endregion

    #region Initialize

        public AdditionalColumn(Name name)
            :base()
        {
            CardName = name;
        }

    #endregion

    #region Overrides

        public override void AddCard(Card card)
        {
            if (card.Data.CardName == CardName)
            {
                base.AddCard(card);

                card.Place = CardPlace.AdditionalColumn;
            }
        }

    #endregion
    }
}
