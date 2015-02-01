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
    public enum CardPlace
    {
        None = -1,
        ResultColumn,
        GameColumn,
        AdditionalColumn,
        Deck
    }

    public class Card: Image
    {
    #region Private fields

        //private bool _showFace;

    #endregion

    #region Public properties

        public static ImageSource ImageSourceCardBack { private get; set; }
        public ImageSource ImageSourceCardFace { private get; set; }
        public DataCard Data { get; private set; }
        public CardPlace Place { get; set; }
        public bool IsShowedFace { get; private set; }
        public bool CanMove { get; set; }

    #endregion

    #region Initialize

        public Card(DataCard dataCard)
        {
            Data = dataCard;
            Place = CardPlace.None;
        }

    #endregion

    #region Public methods

        public void ShowFace()
        {
            this.Source = ImageSourceCardFace;
            IsShowedFace = true;
            CanMove = true;
        }

        public void ShowCardBack()
        {
            this.Source = ImageSourceCardBack;
            IsShowedFace = false;
            CanMove = false;
        }

    #endregion
    }
}
