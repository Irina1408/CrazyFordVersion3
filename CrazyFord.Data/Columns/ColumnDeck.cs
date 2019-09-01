using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrazyFord.Data.Columns
{
    using CrazyFord.Data.Columns.BaseClasses;
    using CrazyFord.Data.Cards;

    public class ColumnDeck: ColumnBase
    {
        #region Overrides

        public override void AddCard(ICard card)
        {
            if (!IsEmpty)
            {
                GetLastCard().AllowMove = false;
            }

            base.AddCard(card);

            card.Place = CardPlace.Deck;
            card.AllowMove = true;
            card.ShowFace();
        }

        public override void DeleteCard(ICard card)
        {
            base.DeleteCard(card);

            if (!IsEmpty)
            {
                GetLastCard().AllowMove = true;
            }
        }

        #endregion
    }
}
