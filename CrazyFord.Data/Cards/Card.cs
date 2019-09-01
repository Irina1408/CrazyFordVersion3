using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrazyFord.Data
{
    using CrazyFord.Data.Cards;

    public enum CardPlace
    {
        None = -1,
        ResultColumn,
        GameColumn,
        AdditionalColumn,
        Deck
    }

    public class Card : ICard
    {
        #region Initialization

        public Card(DataCard dataCard)
        {
            Data = dataCard;
            Place = CardPlace.None;
        }

        #endregion

        #region ICard implementation

        public DataCard Data { get; private set; }
        public CardPlace Place { get; set; }
        public bool IsShowedFace { get; private set; }
        public bool IsVisible { get; private set; }
        public bool AllowMove { get; set; }

        public virtual void ShowFace()
        {
            IsShowedFace = true;
            AllowMove = true;
        }
        public virtual void ShowCardBack()
        {
            IsShowedFace = false;
            AllowMove = false;
        }
        public virtual void Hide()
        {
            IsVisible = false;
        }

        #endregion
    }
}
