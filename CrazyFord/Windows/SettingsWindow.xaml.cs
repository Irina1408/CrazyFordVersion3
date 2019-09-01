using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CrazyFord.Logic;

namespace CrazyFord.Windows
{
    using AppStyle.Controls;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : AppWindow
    {
        #region Private fields

        private Dictionary<object,object> originalCurrentPairs;

        #endregion

        #region Initialization

        public SettingsWindow()
        {
            InitializeComponent();
            originalCurrentPairs = new Dictionary<object, object>();
        }

        #endregion

        #region Public methods

        public void LoadSettingsTab<T>(T settings, string header)
            where T : class, ICloneable<T>, new()
        {
            var clone = settings.Clone();
            originalCurrentPairs.Add(settings, clone);

            MainControl.Items.Add(new TabItem()
            {
                Header = header,
                Content = ViewHelper.GenerateGrid<T>(clone)
            });
        }

        #endregion

        #region Event handlers

        private void buttonOk_OnClick(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            Close();
        }

        private void buttonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Private methods

        private void SaveChanges()
        {
            foreach (var originalCurrentPair in originalCurrentPairs)
            {
                var toValue = originalCurrentPair.Key;
                CrazyFord.Utils.Utils.CopyProperties(originalCurrentPair.Value, ref toValue);
            }
        }

        #endregion
    }
}
