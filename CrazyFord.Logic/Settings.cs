using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Logic
{
    public class Settings : INotifyPropertyChanged, ICloneable<Settings>
    {
        #region Private fields

        private static Settings settings;
        private bool cardGetFromResultColumn;
        private bool autoCollectionCards;

        #endregion

        #region Initialization

        public Settings()
        {
            cardGetFromResultColumn = true;
            autoCollectionCards = true;
        }

        #endregion

        #region Public properties

        [Bindable(false)]
        public static Settings Instance
        {
            get { return settings ?? (settings = new Settings()); }
        }

        //[Description("Possibility to take cards from the result columns")]
        [Description("Возможность брать карты из результатирующих колонок")]
        [DefaultValue(true)]
        public bool CardGetFromResultColumn
        {
            get { return cardGetFromResultColumn; }
            set
            {
                cardGetFromResultColumn = value;
                OnPropertyChanged("CardGetFromResultColumn");
            }
        }

        [Description("Автоматический сбор карт")]
        [DefaultValue(true)]
        public bool AutoCollectionCards
        {
            get { return autoCollectionCards; }
            set
            {
                autoCollectionCards = value;
                OnPropertyChanged("AutoCollectionCards");
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ICloneable implementation

        public Settings Clone()
        {
            return (Settings)this.MemberwiseClone();
        }

        #endregion
    }

    public interface ICloneable<out T>
        where T : class
    {
        T Clone();
    }
}
