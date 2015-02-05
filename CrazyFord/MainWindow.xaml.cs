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

namespace CrazyFord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    #region Private fields

        #region Constants

            //rows for every game parts
            private const int _iRowResColsGrid = 1;
            private const int _iRowGameColsGrid = 3;
            private const int _iRowAdditionalColsGrid = 5;

            //Image[] _additionalColImage
            private const int _iImageKing = 0;
            private const int _iImageJoker = 1;

            //AdditionalColumn[] _colAdditional
            private const int _iColKing = 0;
            private const int _iColJoker = 1;

            private const int _sizeIncreasing = 50;

        #endregion

        #region Images

            private Card[] _cards = new Card[AdditionalData.CountCards];
            private Image[] _resultColImages = new Image[AdditionalData.CountResultColumns];
            private Image[] _gameColImages = new Image[AdditionalData.CountGameColumns];
            private Image[] _additionalColImages = new Image[2];
            private Image _deckColImage = new Image();

        #endregion

        #region Columns

            private ColumnResult[] _colResult = new ColumnResult[AdditionalData.CountResultColumns];
            private ColumnGame[] _colGame = new ColumnGame[AdditionalData.CountGameColumns];
            private AdditionalColumn[] _colAdditional = new AdditionalColumn[2];
            private ColumnDeck _colDeck = new ColumnDeck();

        #endregion

        #region Other fields

            private double _cardWidth;
            private double _cardHeight;

            private double _cardGameFaceDistance;
            private double _cardGameBackDistance;
            private double _cardDeckDistance;

            private int[] _cardSequence = new int[AdditionalData.CountCards];
            private int _curCardIndex;

            private int _iGridColDeck;
            private int _iGridColKing;
            private int _iGridColJoker;

            private bool _isGame = false;

            private MenuWindow menuWindow = new MenuWindow();
            private Button btnMenu = new Button();
            System.Windows.Media.Effects.Effect effect = new DropShadowEffect();

        #endregion

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
                //initialize images
                InitialzeResultColumnImages();
                InitializeGameColumnImages();
                InitializeAdditionalColumnImages();
                InitializeCards();
                //initialize columns
                InitializeColumns();

                //set menu button settings
                grid.Children.Add(btnMenu);
                Grid.SetRow(btnMenu, 1);
                Grid.SetColumn(btnMenu, 1);
                //menuButton.Height = 50;
                //menuButton.Width = 150;
                btnMenu.Content = "Меню";
                btnMenu.Margin = new Thickness(0, 0, -50, 50);
                btnMenu.Background = Helper.GetButtonBackBrush();
                btnMenu.Click += (object sender, RoutedEventArgs e) =>
                {
                    menuWindow.ShowDialog();

                    switch (menuWindow.AfterMenuHideAction)
                    {
                        case MenuAction.NewGame:
                            NewGame();
                            break;

                        case MenuAction.Exit:
                            this.Close();
                            break;
                    }
                };

                btnMenu.IsCancel = true;

                //events
                for (int index = 0; index < _cards.Length; index++)
                {
                    _cards[index].MouseDown += card_MouseDown;
                    _cards[index].Drop += card_Drop;
                }

                for (int index = 0; index < _resultColImages.Length; index++)
                {
                    _resultColImages[index].Drop += card_Drop;
                }

                for (int index = 0; index < _gameColImages.Length; index++)
                {
                    _gameColImages[index].Drop += card_Drop;
                }

                for (int index = 0; index < _additionalColImages.Length; index++)
                {
                    _additionalColImages[index].Drop += card_Drop;
                }

                _deckColImage.MouseDown += Deck_MouseDown;
                _deckColImage.MouseUp += Deck_MouseUp;

                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
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
            _iGridColDeck = 1;
            _iGridColKing = grid.ColumnDefinitions.Count / 2 + 1;
            _iGridColJoker = grid.ColumnDefinitions.Count - 2;
        }

        private void InitialzeResultColumnImages()
        {
            for (int index = 0; index < _resultColImages.Length; index++)
            {
                _resultColImages[index] = new Image();
                _resultColImages[index].Source = Helper.GetImageSourceFromResource("Resources/Ace.png");

                Helper.TuneCardImage(_resultColImages[index]);
                grid.Children.Add(_resultColImages[index]);
                Grid.SetRow(_resultColImages[index], _iRowResColsGrid);
                Grid.SetColumn(_resultColImages[index], GetResGridColIndex(index));
            }
        }

        private void InitializeGameColumnImages()
        {
            for (int index = 0; index < _gameColImages.Length; index++)
            {
                _gameColImages[index] = new Image();
                _gameColImages[index].Source = Helper.GetImageSourceFromResource("Resources/King.png");

                Helper.TuneCardImage(_gameColImages[index]);
                grid.Children.Add(_gameColImages[index]);
                Grid.SetRow(_gameColImages[index], _iRowGameColsGrid);
                Grid.SetColumn(_gameColImages[index], GetGameGridColIndex(index));
            }
        }

        private void InitializeAdditionalColumnImages()
        {
            for (int index = 0; index < _additionalColImages.Length; index++)
            {
                _additionalColImages[index] = new Image();

                Helper.TuneCardImage(_additionalColImages[index]);
                grid.Children.Add(_additionalColImages[index]);
                Grid.SetRow(_additionalColImages[index], _iRowAdditionalColsGrid);
            }

            //deck
            Helper.TuneCardImage(_deckColImage);
            grid.Children.Add(_deckColImage);
            Grid.SetRow(_deckColImage, _iRowAdditionalColsGrid);

            //static data
            _deckColImage.Source = Helper.GetImageSourceFromResource("Resources/CardBack/2.png");
            _additionalColImages[_iImageKing].Source = Helper.GetImageSourceFromResource("Resources/King.png");
            _additionalColImages[_iImageJoker].Source = Helper.GetImageSourceFromResource("Resources/Empty.png");

            Grid.SetColumn(_deckColImage, _iGridColDeck);
            Grid.SetColumn(_additionalColImages[_iImageKing], _iGridColKing);
            Grid.SetColumn(_additionalColImages[_iImageJoker], _iGridColJoker);
        }

        private void InitializeCards()
        {
            int index = 0;
            //loop for every deck
            for (int iDeck = 1; iDeck <= AdditionalData.CountDecks; iDeck++)
            {
                //loop for set lear, name and place in file settings of the cards
                for (int iLear = 0; iLear < AdditionalData.CountLears; iLear++)
                {
                    for (int iName = 0; iName < AdditionalData.CountCardNames; iName++)
                    {
                        _cards[index] = new Card(new DataCard((Lear)iLear, (Name)iName));
                        _cards[index].ImageSourceCardFace = Helper.GetImageSourceCardFaceByCardData(_cards[index].Data);

                        Helper.TuneCardImage(_cards[index]);
                        grid.Children.Add(_cards[index]);
                        _cards[index].Visibility = Visibility.Hidden;

                        index++;
                    }
                }

                //loop for set card settings if this card is joker
                for (int iJoker = index; iJoker < index + AdditionalData.CountJokers; iJoker++)
                {
                    _cards[iJoker] = new Card(new DataCard(Lear.None, CrazyFord.Data.Name.Joker));
                    _cards[iJoker].ImageSourceCardFace = Helper.GetImageSourceFromResource("Resources/CardFace/Joker.png");

                    Helper.TuneCardImage(_cards[iJoker]);
                    grid.Children.Add(_cards[iJoker]);
                    _cards[index].Visibility = Visibility.Hidden;
                }

                index += AdditionalData.CountJokers;
            }

            Card.ImageSourceCardBack = Helper.GetImageSourceFromResource("Resources/CardBack/2.png");
        }

        private void InitializeColumns()
        {
            for (int index = 0; index < _colResult.Length; index++)
            {
                _colResult[index] = new ColumnResult();
                _colResult[index].AfterAddCardEvent += AlignOnZindex;
            }

            for (int index = 0; index < _colGame.Length; index++)
            {
                _colGame[index] = new ColumnGame(index);
                _colGame[index].AfterAddCardEvent += AlignOnZindex;
            }

            _colAdditional[_iColKing] = new AdditionalColumn(CrazyFord.Data.Name.King);
            _colAdditional[_iColKing].AfterAddCardEvent += AlignOnZindex;
            _colAdditional[_iColJoker] = new AdditionalColumn(CrazyFord.Data.Name.Joker);
            _colAdditional[_iColJoker].AfterAddCardEvent += AlignOnZindex;

            _colDeck.AfterAddCardEvent += AlignOnZindex;
            _colDeck.AfterAddCardEvent += AlignDeckSequence;
            _colDeck.AfterDeleteCardEvent += AlignDeckSequence;
        }

    #endregion

    #region Private methods

        private void NewGame()
        {
            //reset game data
            _curCardIndex = 0;

            //surffe card sequence
            Helper.Shuffle(ref _cardSequence);
            for (int index = 0; index < AdditionalData.CountCards; index++)
            {
                _cards[index].Visibility = Visibility.Hidden;
                _cards[index].Place = CardPlace.None;
            }

            //card deck visible
            _deckColImage.Visibility = Visibility.Visible;

            //clear column data
            for (int index = 0; index < _colResult.Length; index++)
            {
                _colResult[index].Clear();
            }

            for (int index = 0; index < _colGame.Length; index++)
            {
                _colGame[index].Clear();
            }

            for (int index = 0; index < _colAdditional.Length; index++)
            {
                _colAdditional[index].Clear();
            }

            _colDeck.Clear();

            //add cards in game columns
            for (int iCol = 0; iCol < AdditionalData.CountGameColumns; iCol++)
            {
                for (int index = 0; index < iCol + 1; index++)
                {
                    int iCard = _cardSequence[_curCardIndex];
                    _colGame[iCol].AddCard(_cards[iCard]);

                    _cards[iCard].Visibility = Visibility.Visible;

                    Grid.SetRow(_cards[iCard], _iRowGameColsGrid);
                    Grid.SetColumn(_cards[iCard], GetGameGridColIndex(iCol));

                    _cards[iCard].Margin = new Thickness(0, _colGame[iCol].GetCardIndex(_cards[iCard]) * _cardGameBackDistance, 0, 0);

                    //replace card
                    if (index == iCol)
                    {
                        _cards[iCard].ShowFace();
                    }
                    else
                    {
                        _cards[iCard].ShowCardBack();
                    }

                    _curCardIndex++;
                }
            }

            _isGame = true;
        }

        private void AlignOnZindex(ColumnBase column)
        {
            for (int index = 0; index < column.Count; index++)
            {
                Grid.SetZIndex(column[index], index);
            }
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
                    column[index].Margin = new Thickness((index - difference) * _cardDeckDistance, 0, -(index - difference) * _cardDeckDistance, 0);
                }
            }
        }

        private void CheckOnWin()
        {
            bool isWin = true;

            foreach (ColumnResult columnResult in _colResult)
            {
                if (!columnResult.IsFull)
                {
                    isWin = false;
                    break;
                }
            }

            if (_colAdditional[_iColJoker].Count != AdditionalData.CountJokers)
            {
                isWin = false;
            }

            if (isWin)
            {
                if (MessageBox.Show("Вы выиграли! \n Хотите начать новую игру?", "Поздравляем!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.OK)
                {
                    NewGame();
                }
                else
                {
                    foreach (Card card in _cards)
                    {
                        card.Visibility = Visibility.Hidden;
                    }

                    _isGame = false;
                }
            }
        }

        private void WindowSizeChanged()
        {
            if (_cardHeight / _cardWidth > 1.4)
            {
                _cardHeight = _cardWidth * 1.4;
            }
            else
            {
                _cardWidth = _cardHeight / 1.4;
            }

            for (int index = 0; index < _gameColImages.Length; index++)
            {
                _gameColImages[index].Width = _cardWidth;
                _gameColImages[index].Height = _cardHeight;
            }

            for (int index = 0; index < _resultColImages.Length; index++)
            {
                _resultColImages[index].Width = _cardWidth;
                _resultColImages[index].Height = _cardHeight;
            }

            for (int index = 0; index < _additionalColImages.Length; index++)
            {
                _additionalColImages[index].Width = _cardWidth;
                _additionalColImages[index].Height = _cardHeight;
            }

            _deckColImage.Width = _cardWidth;
            _deckColImage.Height = _cardHeight;

            for (int index = 0; index < _cards.Length; index++)
            {
                if (_cards[index] != null)
                {
                    _cards[index].Width = _cardWidth;
                    _cards[index].Height = _cardHeight;
                }
            }

            //change distance
            _cardGameFaceDistance = _cardHeight / 5;
            _cardDeckDistance = _cardWidth / 4;
            _cardGameBackDistance = _cardGameFaceDistance / 2;

            bool lastShowedFace = false;
            double cardDistance = 0;

            for (int iCol = 0; iCol < _colGame.Length; iCol++)
            {
                for (int iCard = 0; iCard < _colGame[iCol].Count; iCard++)
                {
                    //if the previos card was not visible
                    if (iCard != 0)
                    {
                        if (!lastShowedFace)
                        {
                            cardDistance += _cardGameBackDistance;
                        }
                        else
                        {
                            cardDistance += _cardGameFaceDistance;
                        }
                    }

                    lastShowedFace = _colGame[iCol][iCard].IsShowedFace;

                    _colGame[iCol][iCard].Margin = new Thickness(0, cardDistance, 0, 0);
                }

                cardDistance = 0;
                lastShowedFace = false;
            }

            AlignDeckSequence(_colDeck);
        }

        #region "Get index" methods

            /// <summary>
            /// Get game column index in grid
            /// </summary>
            /// <param name="iCol">Column index</param>
            /// <returns>Index</returns>
            private int GetGameGridColIndex(int iCol)
            {
                return iCol * 2 + 1;
            }

            /// <summary>
            /// Get result column index in grid
            /// </summary>
            /// <param name="iCol">Column index</param>
            /// <returns>Index</returns>
            private int GetResGridColIndex(int iCol)
            {
                return (iCol + grid.ColumnDefinitions.Count / 2 - AdditionalData.CountResultColumns) * 2 + 1;
            }

            /// <summary>
            /// Get game column index
            /// </summary>
            /// <param name="iColGrid">Column index in grid</param>
            /// <returns>Index</returns>
            private int GetGameColIndex(int iColGrid)
            {
                return (iColGrid - 1) / 2;
            }

            /// <summary>
            /// Get result column index
            /// </summary>
            /// <param name="iColGrid">Column index in grid</param>
            /// <returns>Index</returns>
            private int GetResColIndex(int iColGrid)
            {
                return (iColGrid - 1) / 2 - grid.ColumnDefinitions.Count / 2 + AdditionalData.CountResultColumns;
            }

            /// <summary>
            /// Returns game column index where card is plased
            /// </summary>
            /// <param name="card">Card</param>
            /// <returns>Column Index</returns>
            private int? GetGameColIndex(Card card)
            {
                for (int index = 0; index < _colGame.Length; index++)
                {
                    if (_colGame[index].Contains(card))
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
                if (_isGame)
                _isDeckMouseDown = true;
            }

            private void Deck_MouseUp(object sender, MouseButtonEventArgs e)
            {
                if (_isDeckMouseDown)
                {
                    _isDeckMouseDown = false;

                    int iCard = _cardSequence[_curCardIndex];

                    //add in deck
                    _colDeck.AddCard(_cards[iCard]);
                    _curCardIndex++;

                    //set card is visible
                    //_cards[iCard].Margin = new Thickness(_colDeck.GetCardIndex(_cards[iCard]) * _cardDeckDistance, 0, -_colDeck.GetCardIndex(_cards[iCard]) * _cardDeckDistance, 0);
                    //set card row and column
                    Grid.SetColumn(_cards[iCard], _iGridColDeck + 2);
                    Grid.SetRow(_cards[iCard], _iRowAdditionalColsGrid);

                    if (_curCardIndex >= _cardSequence.Length)
                    {
                        _deckColImage.Visibility = Visibility.Hidden;
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
                    cardName = _colGame[(int)GetGameColIndex(card)].GetCardMovingNameInColumn(card);
                    cardColor = _colGame[(int) GetGameColIndex(card)].GetCardColorInColumn(card);
                }
                else
                {
                    cardName = card.Data.CardName;
                    cardColor = card.Data.CardColor;
                }

                //game columns
                for (int index = 0; index < _colGame.Length; index++)
                {
                    //set allowDrop = false for every card in column
                    _colGame[index].SetAllowDrop();
                    //set allowDrop = false for image in this column
                    _gameColImages[index].AllowDrop = false;
                    //get last card
                    lastCard = _colGame[index].GetLastCard();

                    if (lastCard != null)
                    {
                        Data.Name lastCardName = _colGame[index].GetCardReceivingNameInColumn(lastCard);
                        Data.Color lastCardColor = _colGame[index].GetCardColorInColumn(lastCard);

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
                            _gameColImages[index].AllowDrop = true;
                        }
                    }
                }

                lastCard = null;

                //if move 1 card
                if (card.Place != CardPlace.GameColumn ||
                    _colGame[(int)GetGameColIndex(card)].GetLastCard().Equals(card) )
                {

                    //result columns
                    for (int index = 0; index < _colResult.Length; index++)
                    {
                        //set allowDrop = false for every card in column
                        _colResult[index].SetAllowDrop();
                        //set allowDrop = false for image in this column
                        _resultColImages[index].AllowDrop = false;
                        //get last card
                        lastCard = _colResult[index].GetLastCard();

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
                                _resultColImages[index].AllowDrop = true;
                            }
                        }
                    }

                    lastCard = null;

                    //additional columns
                    for (int index = 0; index < _colAdditional.Length; index++)
                    {
                        //set allowDrop = false for every card in column
                        _colAdditional[index].SetAllowDrop();
                        //set allowDrop = false for image in this column
                        _additionalColImages[index].AllowDrop = false;
                        //get last card
                        lastCard = _colAdditional[index].GetLastCard();

                        if (lastCard != null)
                        {
                            if (card.Data.CardName == _colAdditional[index].CardName)
                            {
                                lastCard.AllowDrop = true;
                            }
                        }
                        else
                        {
                            if (card.Data.CardName == _colAdditional[index].CardName)
                            {
                                _additionalColImages[index].AllowDrop = true;
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
                                _colGame[(int)iColGame].DeleteCard(_cardsMove[index]);
                            }
                        }
                        else
                        {
                            throw new Exception("Index game column in null.");
                        }
                        break;

                    case CardPlace.AdditionalColumn:
                        if (_colAdditional[_iColKing].Contains(card))
                        {
                            _colAdditional[_iColKing].DeleteCard(card);
                        }
                        else
                        {
                            _colAdditional[_iColJoker].DeleteCard(card);
                        }
                        break;

                    case CardPlace.Deck:
                        if (_colDeck.Contains(card))
                        {
                            _colDeck.DeleteCard(card);
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
                    case _iRowResColsGrid:
                        iCurrCol = GetResColIndex(iCol);
                        //delete card from last place
                        DeleteCardsPromLastPlace();
                        _colResult[iCurrCol].AddCard(card);
                        card.Margin = new Thickness(0, 0, 0, 0);
                        CheckOnWin();
                        break;

                    case _iRowGameColsGrid:
                        iCurrCol = GetGameColIndex(iCol);
                        //delete card from last place
                        DeleteCardsPromLastPlace();
                        for (int index = _cardsMove.Count - 1; index >= 0; index--)
                        {
                            _colGame[iCurrCol].AddCard(_cardsMove[index]);

                            int countBackCards = _colGame[iCurrCol].GetCountBackCards();

                            Double cardDistance = countBackCards * _cardGameBackDistance +
                                                  (_colGame[iCurrCol].GetCardIndex(_cardsMove[index]) -
                                                   countBackCards) * _cardGameFaceDistance;

                            _cardsMove[index].Margin = new Thickness(0, cardDistance, 0, 0);
                            //set card row and column
                            Grid.SetColumn(_cardsMove[index], iCol);
                            Grid.SetRow(_cardsMove[index], iRow);
                        }
                        break;

                    case _iRowAdditionalColsGrid:
                        //delete card from last place
                        DeleteCardsPromLastPlace();

                        if (iCol == _iGridColKing)
                        {
                            _colAdditional[_iColKing].AddCard(card);
                        }
                        else
                        {
                            _colAdditional[_iColJoker].AddCard(card);
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
                            for (int index = _colGame[(int) iColGame].Count - 1; index >= 0; index --)
                            {
                                Card lastCard = _colGame[(int)iColGame][index];
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
                _cardHeight = this.Height / 6;
                _cardWidth = this.Width / 13;
                WindowSizeChanged();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            menuWindow.Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    _cardHeight = MaxHeight / 6;
                    _cardWidth = MaxWidth / 13;
                    break;

                case WindowState.Minimized:
                    _cardHeight = this.MinHeight / 6;
                    _cardWidth = this.MinWidth / 13;
                    break;

                case WindowState.Normal:
                    _cardHeight = this.Height / 6;
                    _cardWidth = this.Width / 13;
                    break;
            }

            WindowSizeChanged();
        }

    }
}
