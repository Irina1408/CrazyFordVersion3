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

namespace CrazyFord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    #region Private fields

        #region Other fields

            private Button btnMenu = new Button();
            private Effect effect = new DropShadowEffect();
            private Label lblCountCardDeck = new Label();
            private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            private DateTime gameTime;
            private History history = new History();

        #endregion

        private GameImages _gameImages;
        private GameWindowData _gameWindowData;
        private GameColumns _gameColumns;

    #endregion

    #region Initialize

        public MainWindow()
        {
            InitializeComponent();
            //test
            //grid.ShowGridLines = true;

            try
            {
                _gameWindowData = new GameWindowData();
                //set grid columns
                SetGridDefinitions();
                _gameImages = new GameImages(grid, _gameWindowData);
                _gameColumns = new GameColumns(AlignDeckSequence);

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
                    timer.Stop();

                    MenuWindow menuWindow = new MenuWindow(_gameWindowData.IsGame);
                    menuWindow.ShowDialog();

                    timer.Start();

                    switch (menuWindow.AfterMenuHideAction)
                    {
                        case MenuAction.NewGame:
                            NewGame();
                            break;

                        case MenuAction.Exit:
                            Close();
                            break;
                    }

                    menuWindow.Close();
                    menuWindow = null;
                };

                btnMenu.IsCancel = true;

                //set label that shows number cards in deck
                grid.Children.Add(lblCountCardDeck);
                Grid.SetRow(lblCountCardDeck, 4);
                Grid.SetColumn(lblCountCardDeck, 1);
                lblCountCardDeck.FontSize = 12;
                lblCountCardDeck.Content = AdditionalData.CountCards + " / 0";
                lblCountCardDeck.Visibility = Visibility.Visible;
                lblCountCardDeck.HorizontalContentAlignment = HorizontalAlignment.Center;

                //events
                for (int index = 0; index < _gameImages.Cards.Length; index++)
                {
                    _gameImages.Cards[index].MouseDown += card_MouseDown;
                    _gameImages.Cards[index].Drop += card_Drop;
                }

                for (int index = 0; index < _gameImages.ResultColImages.Length; index++)
                {
                    _gameImages.ResultColImages[index].Drop += card_Drop;
                }

                for (int index = 0; index < _gameImages.GameColImages.Length; index++)
                {
                    _gameImages.GameColImages[index].Drop += card_Drop;
                }

                for (int index = 0; index < _gameImages.AdditionalColImages.Length; index++)
                {
                    _gameImages.AdditionalColImages[index].Drop += card_Drop;
                }

                _gameImages.DeckColImage.MouseDown += Deck_MouseDown;
                _gameImages.DeckColImage.MouseUp += Deck_MouseUp;

                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

                timer.Tick += timerTick;
                timer.Interval = new TimeSpan(0, 0, 1);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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

            //additional data
            _gameWindowData.iGridColDeck = 1;
            _gameWindowData.iGridColKing = grid.ColumnDefinitions.Count / 2 + 1;
            _gameWindowData.iGridColJoker = grid.ColumnDefinitions.Count - 2;
        }

    #endregion

    #region Private methods

        private void NewGame()
        {
            //reset game data
            _gameWindowData.CurCardIndex = 0;

            //surffe card sequence
            _gameWindowData.CardSequence = Helper.Shuffle(_gameWindowData.CardSequence);

            for (int index = 0; index < AdditionalData.CountCards; index++)
            {
                _gameImages.Cards[index].Visibility = Visibility.Hidden;
                _gameImages.Cards[index].Place = CardPlace.None;
            }

            //card deck visible
            _gameImages.DeckColImage.Visibility = Visibility.Visible;

            //clear column data
            for (int index = 0; index < _gameColumns.ColResult.Length; index++)
            {
                _gameColumns.ColResult[index].Clear();
            }

            for (int index = 0; index < _gameColumns.ColGame.Length; index++)
            {
                _gameColumns.ColGame[index].Clear();
            }

            for (int index = 0; index < _gameColumns.ColAdditional.Length; index++)
            {
                _gameColumns.ColAdditional[index].Clear();
            }

            _gameColumns.ColDeck.Clear();

            //add cards in game columns
            for (int iCol = 0; iCol < AdditionalData.CountGameColumns; iCol++)
            {
                for (int index = 0; index < iCol + 1; index++)
                {
                    int iCard = _gameWindowData.CardSequence[_gameWindowData.CurCardIndex];
                    _gameColumns.ColGame[iCol].AddCard(_gameImages.Cards[iCard]);

                    _gameImages.Cards[iCard].Visibility = Visibility.Visible;

                    Grid.SetRow(_gameImages.Cards[iCard], GameWindowConstants.iRowGameColsGrid);
                    Grid.SetColumn(_gameImages.Cards[iCard], GetGameGridColIndex(iCol));

                    _gameImages.Cards[iCard].Margin = new Thickness(0, _gameColumns.ColGame[iCol].GetCardIndex(_gameImages.Cards[iCard]) * _gameWindowData.CardGameBackDistance, 0, 0);

                    //replace card
                    if (index == iCol)
                    {
                        _gameImages.Cards[iCard].ShowFace();
                    }
                    else
                    {
                        _gameImages.Cards[iCard].ShowCardBack();
                    }

                    _gameWindowData.CurCardIndex++;
                }
            }

            lblCountCardDeck.Content = AdditionalData.CountCards - _gameWindowData.CurCardIndex + " / 0";
            _gameWindowData.IsGame = true;
            //clear actions history
            history.Clear();

            gameTime = new DateTime();
            timer.Start();
        }

        private void AlignDeckSequence(ColumnBase column)
        {
            int difference = column.Count - AdditionalData.CountVisibleCardsInDeck;
            difference = difference > 0 ? difference : 0;

            for (int index = 0; index < column.Count; index++)
            {
                if (index < difference)
                {
                    column[index].Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    column[index].Margin = new Thickness((index - difference) * _gameWindowData.CardDeckDistance, 
                        0, -(index - difference) * _gameWindowData.CardDeckDistance, 0);
                }
            }
            lblCountCardDeck.Content = AdditionalData.CountCards - _gameWindowData.CurCardIndex + " / " + _gameColumns.ColDeck.Count;
        }

        private void CheckOnWin()
        {
            bool isWin = true;

            foreach (ColumnResult columnResult in _gameColumns.ColResult)
            {
                if (!columnResult.IsFull)
                {
                    isWin = false;
                    break;
                }
            }

            if (_gameColumns.ColAdditional[GameWindowConstants.iColJoker].Count != AdditionalData.CountJokers)
            {
                isWin = false;
            }

            if (isWin)
            {
                timer.Stop();

                if (MessageBox.Show("Вы выиграли! \n Хотите начать новую игру?", "Поздравляем!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.OK)
                {
                    NewGame();
                }
                else
                {
                    foreach (Card card in _gameImages.Cards)
                    {
                        card.Visibility = Visibility.Hidden;
                    }

                    _gameWindowData.IsGame = false;
                }
            }
        }

        private void WindowSizeChanged()
        {
            if (_gameWindowData.CardHeight / _gameWindowData.CardWidth > 1.4)
            {
                _gameWindowData.CardHeight = _gameWindowData.CardWidth * 1.4;
            }
            else
            {
                _gameWindowData.CardWidth = _gameWindowData.CardHeight / 1.4;
            }

            _gameImages.SetImageSize(_gameWindowData.CardWidth, _gameWindowData.CardHeight);

            //change distance
            _gameWindowData.CardGameFaceDistance = _gameWindowData.CardHeight / 5;
            _gameWindowData.CardDeckDistance = _gameWindowData.CardWidth / 4;
            _gameWindowData.CardGameBackDistance = _gameWindowData.CardGameFaceDistance / 2;

            bool lastShowedFace = false;
            double cardDistance = 0;

            for (int iCol = 0; iCol < _gameColumns.ColGame.Length; iCol++)
            {
                for (int iCard = 0; iCard < _gameColumns.ColGame[iCol].Count; iCard++)
                {
                    //if the previos card was not visible
                    if (iCard != 0)
                    {
                        if (!lastShowedFace)
                        {
                            cardDistance += _gameWindowData.CardGameBackDistance;
                        }
                        else
                        {
                            cardDistance += _gameWindowData.CardGameFaceDistance;
                        }
                    }

                    lastShowedFace = _gameColumns.ColGame[iCol][iCard].IsShowedFace;

                    _gameColumns.ColGame[iCol][iCard].Margin = new Thickness(0, cardDistance, 0, 0);
                }

                cardDistance = 0;
                lastShowedFace = false;
            }

            AlignDeckSequence(_gameColumns.ColDeck);
            lblCountCardDeck.FontSize = _gameWindowData.CardGameBackDistance * 1.5;
        }

        private void timerTick(object sender, EventArgs e)
        {
            gameTime = gameTime.AddSeconds(1);
            lblTimer.Content = gameTime.ToString("T");
        }

        #region "Get index" methods

            /// <summary>
            /// Get game column index in grid
            /// </summary>
            private int GetGameGridColIndex(int iCol)
            {
                return iCol * 2 + 1;
            }

            /// <summary>
            /// Get result column index in grid
            /// </summary>
            private int GetResGridColIndex(int iCol)
            {
                return (iCol + grid.ColumnDefinitions.Count / 2 - AdditionalData.CountResultColumns) * 2 + 1;
            }

            /// <summary>
            /// Get game column index
            /// </summary>
            private int GetGameColIndex(int iColGrid)
            {
                return (iColGrid - 1) / 2;
            }

            /// <summary>
            /// Get result column index
            /// </summary>
            private int GetResColIndex(int iColGrid)
            {
                return (iColGrid - 1) / 2 - grid.ColumnDefinitions.Count / 2 + AdditionalData.CountResultColumns;
            }

            /// <summary>
            /// Returns game column index where card is plased
            /// </summary>
            private int? GetGameColIndex(Card card)
            {
                for (int index = 0; index < _gameColumns.ColGame.Length; index++)
                {
                    if (_gameColumns.ColGame[index].Contains(card))
                    {
                        return index;
                    }
                }
                return null;
            } 

        #endregion

        #region Deck click

            private bool _isDeckMouseDown = false;
            private void Deck_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (_gameWindowData.IsGame)
                _isDeckMouseDown = true;
            }

            private void Deck_MouseUp(object sender, MouseButtonEventArgs e)
            {
                if (_isDeckMouseDown)
                {
                    _isDeckMouseDown = false;

                    int iCard = _gameWindowData.CardSequence[_gameWindowData.CurCardIndex];

                    //add in deck
                    _gameColumns.ColDeck.AddCard(_gameImages.Cards[iCard]);
                    _gameWindowData.CurCardIndex++;

                    //set card is visible
                    //_cards[iCard].Margin = new Thickness(_colDeck.GetCardIndex(_cards[iCard]) * _cardDeckDistance, 0, -_colDeck.GetCardIndex(_cards[iCard]) * _cardDeckDistance, 0);
                    //set card row and column
                    Grid.SetColumn(_gameImages.Cards[iCard], _gameWindowData.iGridColDeck + 2);
                    Grid.SetRow(_gameImages.Cards[iCard], GameWindowConstants.iRowAdditionalColsGrid);
                    lblCountCardDeck.Content = AdditionalData.CountCards - _gameWindowData.CurCardIndex + " / " + _gameColumns.ColDeck.Count;

                    if (_gameWindowData.CurCardIndex >= _gameWindowData.CardSequence.Length)
                    {
                        _gameImages.DeckColImage.Visibility = Visibility.Hidden;
                    }
                }
            }

        #endregion

        #region Card moving

            //private Card _cardMove;
            private List <Card> _cardsMove = new List<Card>();

            /// <summary>
            /// Check all last cards in every game column and in additional columns.
            /// If card suitables set property AllowDrop = true
            /// </summary>
            private void CheckOnAllowDrop(Card card)
            {
                Card lastCard;
                Data.Name cardName;
                Data.Color cardColor;

                if (card.Place == CardPlace.GameColumn)
                {
                    cardName = _gameColumns.ColGame[(int)GetGameColIndex(card)].GetCardMovingNameInColumn(card);
                    cardColor = _gameColumns.ColGame[(int)GetGameColIndex(card)].GetCardColorInColumn(card);
                }
                else
                {
                    cardName = card.Data.CardName;
                    cardColor = card.Data.CardColor;
                }

                //game columns
                for (int index = 0; index < _gameColumns.ColGame.Length; index++)
                {
                    //set allowDrop = false for every card in column
                    _gameColumns.ColGame[index].SetAllowDrop();
                    //set allowDrop = false for image in this column
                    _gameImages.GameColImages[index].AllowDrop = false;
                    //get last card
                    lastCard = _gameColumns.ColGame[index].GetLastCard();

                    if (lastCard != null)
                    {
                        Data.Name lastCardName = _gameColumns.ColGame[index].GetCardReceivingNameInColumn(lastCard);
                        Data.Color lastCardColor = _gameColumns.ColGame[index].GetCardColorInColumn(lastCard);

                        if (lastCardName != Data.Name.Joker)
                        {
                            if (cardName == Data.Name.Joker ||
                                ( cardName == lastCardName - 1 && cardColor != lastCardColor))
                            {
                                lastCard.AllowDrop = true;
                            }
                        }
                    }
                    else
                    {
                        if (cardName == Data.Name.Joker || cardName == Data.Name.King)
                        {
                            _gameImages.GameColImages[index].AllowDrop = true;
                        }
                    }
                }

                lastCard = null;

                //if move 1 card
                if (card.Place != CardPlace.GameColumn ||
                    _gameColumns.ColGame[(int)GetGameColIndex(card)].GetLastCard().Equals(card))
                {

                    //result columns
                    for (int index = 0; index < _gameColumns.ColResult.Length; index++)
                    {
                        //set allowDrop = false for every card in column
                        _gameColumns.ColResult[index].SetAllowDrop();
                        //set allowDrop = false for image in this column
                        _gameImages.ResultColImages[index].AllowDrop = false;
                        //get last card
                        lastCard = _gameColumns.ColResult[index].GetLastCard();

                        if (lastCard != null)
                        {
                            if (card.Data.CardLear == lastCard.Data.CardLear &&
                                card.Data.CardName == lastCard.Data.CardName + 1)
                            {
                                lastCard.AllowDrop = true;
                            }
                        }
                        else
                        {
                            if (card.Data.CardName == Data.Name.Ace)
                            {
                                _gameImages.ResultColImages[index].AllowDrop = true;
                            }
                        }
                    }

                    lastCard = null;

                    //additional columns
                    for (int index = 0; index < _gameColumns.ColAdditional.Length; index++)
                    {
                        //set allowDrop = false for every card in column
                        _gameColumns.ColAdditional[index].SetAllowDrop();
                        //set allowDrop = false for image in this column
                        _gameImages.AdditionalColImages[index].AllowDrop = false;
                        //get last card
                        lastCard = _gameColumns.ColAdditional[index].GetLastCard();

                        if (lastCard != null)
                        {
                            if (card.Data.CardName == _gameColumns.ColAdditional[index].CardName)
                            {
                                lastCard.AllowDrop = true;
                            }
                        }
                        else
                        {
                            if (card.Data.CardName == _gameColumns.ColAdditional[index].CardName)
                            {
                                _gameImages.AdditionalColImages[index].AllowDrop = true;
                            }
                        }
                    }
                }
            }

            private void card_MouseDown(object sender, MouseButtonEventArgs e)
            {
                Card card = (Card) sender;

                if (card.CanMove)
                {
                    CheckOnAllowDrop(card);
                    SetMovingCards(card);
                    DragDrop.DoDragDrop(card, card, DragDropEffects.Move);

                    foreach (Card movingCard in _cardsMove)
                    {
                        movingCard.Effect = null;
                    }
                }
            }

            private void card_Drop(object sender, DragEventArgs e)
            {
                Image tempImage = sender as Image;
                int iCol = Grid.GetColumn(tempImage);
                int iRow = Grid.GetRow(tempImage);

                //set card row and column
                Grid.SetColumn(_cardsMove[0], iCol);
                Grid.SetRow(_cardsMove[0], iRow);

                AddIntoNewPlace(iCol, iRow);
            }

            private void DeleteCardsPromLastPlace()
            {
                Card card = _cardsMove[0];

                switch (card.Place)
                {
                    case CardPlace.None:
                        //throw new Exception("Card isn`t placed in any column.");
                        break;

                    case CardPlace.ResultColumn:
                        //can`t move this card
                        throw new Exception("Can`t move from result column.");
                        break;

                    case CardPlace.GameColumn:
                        int? iColGame = GetGameColIndex(card);
                        if (iColGame != null)
                        {
                            for (int index = _cardsMove.Count - 1; index >= 0; index--)
                            {
                                _gameColumns.ColGame[(int)iColGame].DeleteCard(_cardsMove[index]);
                            }
                        }
                        else
                        {
                            throw new Exception("Index game column in null.");
                        }
                        break;

                    case CardPlace.AdditionalColumn:
                        if (_gameColumns.ColAdditional[GameWindowConstants.iColKing].Contains(card))
                        {
                            _gameColumns.ColAdditional[GameWindowConstants.iColKing].DeleteCard(card);
                        }
                        else
                        {
                            _gameColumns.ColAdditional[GameWindowConstants.iColJoker].DeleteCard(card);
                        }
                        break;

                    case CardPlace.Deck:
                        if (_gameColumns.ColDeck.Contains(card))
                        {
                            _gameColumns.ColDeck.DeleteCard(card);
                        }
                        else
                        {
                            throw new Exception("Unknown card in deck");
                        }
                        break;
                }
            }
            
            private void AddIntoNewPlace(int iCol, int iRow)
            {
                int iCurrCol;
                Card card = _cardsMove[0];
                
                switch (iRow)
                {
                    //if there is result columns
                    case GameWindowConstants.iRowResColsGrid:
                        iCurrCol = GetResColIndex(iCol);
                        //delete card from last place
                        DeleteCardsPromLastPlace();
                        _gameColumns.ColResult[iCurrCol].AddCard(card);
                        card.Margin = new Thickness(0, 0, 0, 0);
                        CheckOnWin();
                        break;

                    case GameWindowConstants.iRowGameColsGrid:
                        iCurrCol = GetGameColIndex(iCol);
                        //delete card from last place
                        DeleteCardsPromLastPlace();
                        for (int index = _cardsMove.Count - 1; index >= 0; index--)
                        {
                            _gameColumns.ColGame[iCurrCol].AddCard(_cardsMove[index]);

                            int countBackCards = _gameColumns.ColGame[iCurrCol].GetCountBackCards();

                            Double cardDistance = countBackCards * _gameWindowData.CardGameBackDistance +
                                                  (_gameColumns.ColGame[iCurrCol].GetCardIndex(_cardsMove[index]) -
                                                   countBackCards) * _gameWindowData.CardGameFaceDistance;

                            _cardsMove[index].Margin = new Thickness(0, cardDistance, 0, 0);
                            //set card row and column
                            Grid.SetColumn(_cardsMove[index], iCol);
                            Grid.SetRow(_cardsMove[index], iRow);
                        }
                        break;

                    case GameWindowConstants.iRowAdditionalColsGrid:
                        //delete card from last place
                        DeleteCardsPromLastPlace();

                        if (iCol == _gameWindowData.iGridColKing)
                        {
                            _gameColumns.ColAdditional[GameWindowConstants.iColKing].AddCard(card);
                        }
                        else
                        {
                            _gameColumns.ColAdditional[GameWindowConstants.iColJoker].AddCard(card);
                        }
                        card.Margin = new Thickness(0, 0, 0, 0);
                        CheckOnWin();
                        break;

                    default:
                        throw new Exception("Invalid column index.");
                }
            }

            private void SetMovingCards(Card card)
            {
                _cardsMove.Clear();

                switch (card.Place)
                {
                    case CardPlace.GameColumn:
                        int? iColGame = GetGameColIndex(card);
                        if (iColGame != null)
                        {
                            for (int index = _gameColumns.ColGame[(int)iColGame].Count - 1; index >= 0; index--)
                            {
                                Card lastCard = _gameColumns.ColGame[(int)iColGame][index];
                                _cardsMove.Add(lastCard);

                                if (lastCard.Equals(card))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Index game column in null.");
                        }
                        break;

                    default:
                        _cardsMove.Add(card);
                        break;
                }

                System.Windows.Controls.Border border = new Border();
                border.Background = Brushes.AntiqueWhite;
                border.BorderThickness = new Thickness(2,2,2,2);             

                foreach (Card movingCard in _cardsMove)
                {
                    movingCard.Effect = effect;
                }
            }

        #endregion

    #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                _gameWindowData.CardHeight = this.Height / 6.1;
                _gameWindowData.CardWidth = this.Width / 13;
                WindowSizeChanged();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    _gameWindowData.CardHeight = MaxHeight / 6.1;
                    _gameWindowData.CardWidth = MaxWidth / 13;
                    break;

                case WindowState.Minimized:
                    _gameWindowData.CardHeight = this.MinHeight / 6.1;
                    _gameWindowData.CardWidth = this.MinWidth / 13;
                    break;

                case WindowState.Normal:
                    _gameWindowData.CardHeight = this.Height / 6.1;
                    _gameWindowData.CardWidth = this.Width / 13;
                    break;
            }

            WindowSizeChanged();
        }

    }
}
