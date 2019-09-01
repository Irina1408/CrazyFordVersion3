using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CrazyFord.Windows
{
    using AppStyle.Controls;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : AppWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        //public void LoadSettingsGrid<T>(T settings)
        //{
        //    MainGrid.Children.Add(ViewHelper.GenerateGrid<T>(settings));
        //}
    }
}
