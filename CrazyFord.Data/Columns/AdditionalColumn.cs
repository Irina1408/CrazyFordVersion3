using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data.Columns
{
    using CrazyFord.Data.Columns.BaseClasses;
    using CrazyFord.Data.Cards;

    public class AdditionalColumn : ColumnBase
    {
        #region Public properties

        /// <summary>
        /// Additional column has cards only with certain card name
        /// </summary>
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

        public override void AddCard(ICard card)
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
