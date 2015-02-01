using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord.Data.Clumns
{
    public class ColumnDeck: ColumnBase
    {
    #region Overrides

        public override void AddCard(Card card)
        {
            if (!IsEmpty)
            {
                GetLastCard().CanMove = false;
            }

            base.AddCard(card);

            card.Place = CardPlace.Deck;
            card.CanMove = true;
            card.Visibility = Visibility.Visible;
            card.ShowFace();
        }

        public override void DeleteCard(Card card)
        {
            base.DeleteCard(card);

            if (!IsEmpty)
            {
                GetLastCard().CanMove = true;
            }
        }

    #endregion
    }
}
