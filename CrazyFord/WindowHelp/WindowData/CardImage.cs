using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CrazyFord.WindowHelp.WindowData
{
    using AppStyle.Controls;
    using CrazyFord.Data;
    using CrazyFord.Data.Cards;

    public class CardImage : AppImage, ICard
    {
        #region Initialize

        public CardImage()
            : base()
        {
            Place = CardPlace.None;
        }

        public CardImage(DataCard dataCard)
            : this()
        {
            Data = dataCard;

            var style = Application.Current.FindResource("CardImageGeneralStyle") as Style;
            if (style != null)
                Style = style;
        }

        #endregion

        #region Public properties

        public static ImageSource ImageSourceCardBack { private get; set; }
        public ImageSource ImageSourceCardFace { private get; set; }

        #endregion

        #region ICard implementation

        public DataCard Data { get; private set; }
        public CardPlace Place { get; set; }
        public bool IsShowedFace { get; private set; }
        public new bool IsVisible { get; private set; }
        public bool AllowMove { get; set; }

        public virtual void ShowFace()
        {
            ImageSource = ImageSourceCardFace;
            Visibility = System.Windows.Visibility.Visible;
            IsShowedFace = true;
            IsVisible = true;
            AllowMove = true;
        }

        public virtual void ShowCardBack()
        {
            ImageSource = ImageSourceCardBack;
            Visibility = System.Windows.Visibility.Visible;
            IsShowedFace = false;
            IsVisible = true;
            AllowMove = false;
        }

        public virtual void Hide()
        {
            Visibility = System.Windows.Visibility.Collapsed;
            IsVisible = false;
        }

        #endregion
    }
}
