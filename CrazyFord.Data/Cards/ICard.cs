using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data.Cards
{
    public interface ICard
    {
        #region Properties

        DataCard Data { get; }
        CardPlace Place { get; set; }
        bool IsShowedFace { get; }
        bool IsVisible { get; }
        bool AllowMove { get; set; }

        #endregion

        #region Methods

        void ShowFace();
        void ShowCardBack();
        void Hide();

        #endregion
    }
}
