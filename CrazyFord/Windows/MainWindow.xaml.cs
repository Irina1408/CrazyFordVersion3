using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using CrazyFord.Data;
using CrazyFord.Data.Clumns;
using CrazyFord.Data.Clumns.BaseClasses;
using CrazyFord.GameProcess;

namespace CrazyFord.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private fields

        private Button btnMenu;
        private Label lblCountCardDeck;
        private GameProcess.GameProcess _gameProcess;

        #endregion

        #region Initialize

        public MainWindow()
        {
            InitializeComponent();
            //test
            //grid.ShowGridLines = true;

            try
            {
                //set grid columns
                SetGridDefinitions();
                _gameProcess = new GameProcess.GameProcess(grid);

                //set binding
                lblTimer.DataContext = _gameProcess;
                MenuButtonInit();
                CountCardDeckLabelInit();

                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Loading error. Message: \n" + exception.Message);
            }
        }

        private void MenuButtonInit()
        {
            btnMenu = new Button();

            //set menu button settings
            grid.Children.Add(btnMenu);
            Grid.SetRow(btnMenu, 1);
            Grid.SetColumn(btnMenu, 1);
            //menuButton.Height = 50;
            //menuButton.Width = 150;
            Image tempImage = new Image();
            tempImage.Source = Helper.GetImageSourceFromResource("Resources/menu1.png");
            tempImage.Stretch = Stretch.Uniform;

            btnMenu.Content = tempImage;
            btnMenu.Margin = new Thickness(0, 0, -40, 40);
            btnMenu.Background = (Brush)this.FindResource("GenericButtonBrush");
            btnMenu.Click += (object sender, RoutedEventArgs e) =>
            {
                _gameProcess.Timer.Stop();

                MenuWindow menuWindow = new MenuWindow(_gameProcess.GameWindowData.IsGame);
                menuWindow.ShowDialog();

                _gameProcess.Timer.Start();

                switch (menuWindow.AfterMenuHideAction)
                {
                    case MenuAction.NewGame:
                        _gameProcess.NewGame();
                        break;

                    case MenuAction.Exit:
                        Close();
                        break;
                }

                menuWindow.Close();
                menuWindow = null;
            };

            btnMenu.IsCancel = true;
        }

        private void CountCardDeckLabelInit()
        {
            lblCountCardDeck = new Label
            {
                FontSize = 12,
                Visibility = Visibility.Visible,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            var bind = new Binding("CountCardDeck")
            {
                Mode = BindingMode.OneWay,
                Source = _gameProcess
            };

            lblCountCardDeck.SetBinding(ContentProperty, bind);

            //set label that shows number cards in deck
            grid.Children.Add(lblCountCardDeck);
            Grid.SetRow(lblCountCardDeck, 4);
            Grid.SetColumn(lblCountCardDeck, 1);
        }

        private void SetGridDefinitions()
        {
            for (int i = 0; i < AdditionalData.CountGameColumns; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(15, GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(cd);

                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ColumnDefinition cdef = new ColumnDefinition();
            cdef.Width = new GridLength(10, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(cdef);

            ////additional data
            //_gameProcess.GameWindowData.iGridColDeck = 1;
            //_gameProcess.GameWindowData.iGridColKing = grid.ColumnDefinitions.Count / 2 + 1;
            //_gameProcess.GameWindowData.iGridColJoker = grid.ColumnDefinitions.Count - 2;
        }

        #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                _gameProcess.GameWindowData.CardHeight = this.Height / 6.1;
                _gameProcess.GameWindowData.CardWidth = this.Width / 13;
                _gameProcess.WindowSizeChanged();

                lblCountCardDeck.FontSize = _gameProcess.GameWindowData.CardGameBackDistance * 1.5;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    _gameProcess.GameWindowData.CardHeight = MaxHeight / 6.1;
                    _gameProcess.GameWindowData.CardWidth = MaxWidth / 13;
                    break;

                case WindowState.Minimized:
                    _gameProcess.GameWindowData.CardHeight = this.MinHeight / 6.1;
                    _gameProcess.GameWindowData.CardWidth = this.MinWidth / 13;
                    break;

                case WindowState.Normal:
                    _gameProcess.GameWindowData.CardHeight = this.Height / 6.1;
                    _gameProcess.GameWindowData.CardWidth = this.Width / 13;
                    break;
            }

            _gameProcess.WindowSizeChanged();
        }
    }
}
