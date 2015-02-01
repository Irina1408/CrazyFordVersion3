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
using CrazyFord.Data;

namespace CrazyFord
{
    public enum MenuAction
    {
        None,
        NewGame,
        Exit
    }
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private bool escPressed = false;
        public MenuAction AfterMenuHideAction { get; private set; }

        public MenuWindow()
        {
            InitializeComponent();

            //set colors
            btnNewGame.Background = Helper.GetButtonBackBrush();
            btnResumeGame.Background = Helper.GetButtonBackBrush();
            btnRules.Background = Helper.GetButtonBackBrush();
            btnExit.Background = Helper.GetButtonBackBrush();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            AfterMenuHideAction = MenuAction.NewGame;
            btnResumeGame.IsEnabled = true;
            this.Hide();
        }

        private void btnResumeGame_Click(object sender, RoutedEventArgs e)
        {
            //AfterMenuHideAction = MenuAction.None;
            this.Hide();
        }

        private void btnRules_Click(object sender, RoutedEventArgs e)
        {
            //AfterMenuHideAction = MenuAction.None;
            MessageBox.Show(AdditionalData.Rules);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            AfterMenuHideAction = MenuAction.Exit;
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            AfterMenuHideAction = MenuAction.None;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && escPressed)
            {
                escPressed = false;
                this.Hide();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                escPressed = true;
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

    }
}
