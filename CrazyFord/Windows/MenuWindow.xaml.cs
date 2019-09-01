using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Reflection;
using AppStyle.Controls;
using CrazyFord.Logic;

namespace CrazyFord.Windows
{
    public enum MenuResult
    {
        NewGame,
        ResumeGame,
        ExitGame
    }

    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : AppWindow
    {
        private bool _escPressed;

        public MenuWindow(bool isGame = false)
        {
            InitializeComponent();

            btnResumeGame.IsEnabled = isGame;
            Result = MenuResult.ResumeGame;
        }

        public MenuResult Result { get; private set; }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            Result = MenuResult.NewGame;
            this.Hide();
        }

        private void btnResumeGame_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.LoadSettingsTab(Settings.Instance, "Основные");
            settingsWindow.ShowDialog();
        }

        private void btnRules_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Data.StaticGameData.Rules);
        }
        
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("             Автор: Ирина                        \n             Версия: " + Assembly.GetEntryAssembly().GetName().Version);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Result = MenuResult.ExitGame;
            this.Hide();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _escPressed)
            {
                _escPressed = false;
                this.Hide();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _escPressed = true;
            }
        }

    }
}
