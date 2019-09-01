using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
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

namespace CrazyFord.Windows
{
    using AppStyle.Controls;
    using CrazyFord.Data;
    using CrazyFord.Data.Cards;
    using CrazyFord.Data.Columns;
    using CrazyFord.Logic;
    using CrazyFord.WindowHelp;
    using CrazyFord.WindowHelp.WindowData;
    using CrazyFord.WindowHelp.WindowLogic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Private fields

        private CustomGame _game;
        private GameWindowData _gameWindowData;
        private ImageViewer _imageViewer;

        #endregion

        #region Initialize

        public MainWindow()
        {
            InitializeComponent();
            //test
            //grid.ShowGridLines = true;

            try
            {
                // init game logic
                _game = new CustomGame();
                _gameWindowData = new GameWindowData();

                // configure window
                ConfigureGridDefinitions();

                // init viewer
                _imageViewer = new ImageViewer(grid, _gameWindowData, _game.Cards as AppImage[]);

                // set datacontext
                this.DataContext = _game;

                // set max screen size
                //this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                //this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

                ConfigureEvents();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Loading error. Message: \n" + exception.Message);
            }
        }

        private void ConfigureGridDefinitions()
        {
            for (int i = 0; i < StaticGameData.CountGameColumns; i++)
            {
                var cd = new ColumnDefinition {Width = new GridLength(15, GridUnitType.Pixel)};
                grid.ColumnDefinitions.Add(cd);

                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            var cdef = new ColumnDefinition {Width = new GridLength(10, GridUnitType.Pixel)};
            grid.ColumnDefinitions.Add(cdef);

            // game window data
            _gameWindowData.iGridColDeck = 1;
            _gameWindowData.iGridColKing = grid.ColumnDefinitions.Count / 2 + 1;
            _gameWindowData.iGridColJoker = grid.ColumnDefinitions.Count - 2;
        }

        private void ConfigureEvents()
        {
            // main game actions
            _game.StartGameAction += NewGame;

            // add configuring every card adding to the game column
            foreach(var column in _game.Columns.ColGame)
            {
                column.AfterAddCard += (sender, e) =>
                    {
                        var cardImage = e.Card as CardImage;
                        Grid.SetRow(cardImage, GameWindowConstants.iRowGameColsGrid);
                        Grid.SetColumn(cardImage, grid.GetGameGridColIndex(_game.Columns.GetGameColIndex(e.Column)));

                        //set top thickness corresponding to the previous showed card
                        ICard previewCard = e.Column.GetPreviewCard(e.Card);
                        double cardDistance;

                        // if previous card in the column exists 
                        if (previewCard == null)
                        {
                            cardDistance = 0;
                        }
                        else
                        {
                            // if previous card with hidden face
                            if (previewCard.IsShowedFace)
                            {
                                // find count back cards in column
                                int countBackCards = (e.Column as ColumnGame).GetCountBackCards();

                                // set card distance according to the back cards count 
                                //and cards with visible face
                                cardDistance = countBackCards * _gameWindowData.CardGameBackDistance
                                                        + (e.Column.GetCardIndex(e.Card) - countBackCards)
                                                        * _gameWindowData.CardGameFaceDistance;
                            }
                            else
                            {
                                cardDistance = e.Column.GetCardIndex(e.Card) * _gameWindowData.CardGameBackDistance;
                            }
                        }

                        cardImage.Margin = new Thickness(0, cardDistance, 0, 0);

                        if (cardImage.Height + cardDistance >
                            grid.RowDefinitions[GameWindowConstants.iRowGameColsGrid].ActualHeight)
                        {
                            _imageViewer.RefreshCardDistanceInColumns(new []{e.Column}, 
                                grid.RowDefinitions[GameWindowConstants.iRowGameColsGrid].ActualHeight);
                        }
                    };

                column.AfterRemoveCard += (sender, e) =>
                {
                    double cardDistance = (e.Card as CardImage).Margin.Top;

                    // if in this column card distance was minimized
                    if (_gameWindowData.CardHeight + cardDistance >
                        grid.RowDefinitions[GameWindowConstants.iRowGameColsGrid].ActualHeight)
                    {
                        // refresh card distance
                        _imageViewer.RefreshCardDistanceInColumns(new[] {e.Column},
                            grid.RowDefinitions[GameWindowConstants.iRowGameColsGrid].ActualHeight);
                    }
                };
            }

            // add configuring every card adding to the deck
            _game.Columns.ColDeck.AfterAddCard += (sender, e) =>
            {
                var cardImage = e.Card as CardImage;

                //set card margin
                cardImage.Margin = new Thickness(
                    e.Column.GetCardIndex(e.Card) * _gameWindowData.CardDeckDistance, 
                    0,
                    -e.Column.GetCardIndex(e.Card) * _gameWindowData.CardDeckDistance, 
                    0);

                //set card row and column
                Grid.SetColumn(cardImage, _gameWindowData.iGridColDeck + 2);
                Grid.SetRow(cardImage, GameWindowConstants.iRowAdditionalColsGrid);

                _game.OnPropertyChanged("DeckCardsNumber");

                if (_game.DeckHasUnvisibleCards())
                {
                    _imageViewer.DeckColImage.Visibility = Visibility.Collapsed;
                }

                _imageViewer.AlignDeckSequence(_game.Columns.ColDeck);
            };

            _game.Columns.ColDeck.AfterRemoveCard += (sender, e) =>
            {
                _imageViewer.AlignDeckSequence(_game.Columns.ColDeck);
                _game.OnPropertyChanged("DeckCardsNumber");
            };

            // add configuring every card adding to the result column
            foreach (var column in _game.Columns.ColResult)
            {
                column.AfterAddCard += (sender, e) =>
                {
                    var cardImage = e.Card as CardImage;

                    cardImage.Margin = new Thickness(0, 0, 0, 0);

                    //set card row and column
                    Grid.SetColumn(cardImage, grid.GetResGridColIndex(_game.Columns.GetResultColIndex(e.Column)));
                    Grid.SetRow(cardImage, GameWindowConstants.iRowResColsGrid);

                    if (Settings.Instance.CardGetFromResultColumn)
                    {
                        ICard prevCard = e.Column.Count > 1 ? e.Column[e.Column.Count - 2] : null;

                        if (prevCard != null)
                            prevCard.AllowMove = false;

                        e.Card.AllowMove = true;
                    }
                };

                column.AfterRemoveCard += (sender, e) =>
                {
                    if (Settings.Instance.CardGetFromResultColumn)
                    {
                        ICard lastCard = e.Column.GetLastCard();

                        if (lastCard != null)
                            lastCard.AllowMove = true;
                    }
                };
            }

            // add configuring every card adding to the additional column
            foreach (var column in _game.Columns.ColAdditional)
            {
                column.AfterAddCard += (sender, e) =>
                {
                    var cardImage = e.Card as CardImage;

                    cardImage.Margin = new Thickness(0, 0, 0, 0);

                    //set card row and column
                    Grid.SetColumn(cardImage, (e.Column as AdditionalColumn).CardName == Data.Name.King 
                        ? _gameWindowData.iGridColKing
                        : _gameWindowData.iGridColJoker);
                    Grid.SetRow(cardImage, GameWindowConstants.iRowAdditionalColsGrid);
                };
            }

            // configure all column events
            _game.Columns.SetAfterAddCardEvent((sender, e) =>
            {
                for (int index = 0; index < e.Column.Count; index++)
                {
                    Grid.SetZIndex(e.Column[index] as CardImage, index + 1);
                }
            });

            // image handlers
            _imageViewer.DeckColImage.MouseDown += (sender, e) =>
            {
                if (_game.IsGame)
                    _gameWindowData.IsDeckMouseDown = true;
            };

            _imageViewer.DeckColImage.MouseUp += Deck_MouseUp;
            _imageViewer.SetCardEvents(card_MouseDown, card_Drop);
        }

        #endregion

        #region Control handlers

        private void btnMenu_OnClick(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow(_game.IsGame);

            // pause game
            _game.Pause();

            // show game menu
            menu.ShowDialog();

            switch(menu.Result)
            {
                case MenuResult.NewGame:
                    _game.Start();
                    break;
                    
                case MenuResult.ResumeGame:
                    _game.Resume();
                    break;

                case MenuResult.ExitGame:
                    this.Close();
                    break;
            }

            menu.Close();
            menu = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                _gameWindowData.CardHeight = this.Height / GameWindowConstants.CardHeightScale;
                _gameWindowData.CardWidth = this.Width / GameWindowConstants.CardWidthScale;
                //_gameWindowData.WindowSizeChanged();
                WindowSizeChanged();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            double height = 0;
            double width = 0;

            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    height = MaxHeight;
                    width = MaxWidth;
                    break;

                case WindowState.Minimized:
                    height = this.MinHeight;
                    width = this.MinWidth;
                    break;

                case WindowState.Normal:
                    height = this.Height;
                    width = this.Width;
                    break;
            }

            _gameWindowData.CardHeight = height / GameWindowConstants.CardHeightScale;
            _gameWindowData.CardWidth = width / GameWindowConstants.CardWidthScale;

            //_gameProcess.WindowSizeChanged();
            WindowSizeChanged();
        }

        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            _game.Dispose(); 
            Application.Current.Shutdown();
        }

        #region Image handlers

        private void Deck_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_gameWindowData.IsDeckMouseDown)
            {
                _gameWindowData.IsDeckMouseDown = false;
                
                //add in deck
                _game.Columns.ColDeck.AddCard(_game.GetNextCard());
                _game.AutocollectCards();
            }
        }

        #region Card moving

        private List<CardImage> _cardsMove = new List<CardImage>();

        /// <summary>
        /// Check all last cards in every game column and in additional columns.
        /// If card suitables set property AllowDrop = true
        /// </summary>
        private void CheckOnAllowDrop(ICard card)
        {
            ICard lastCard;
            Data.Name cardName;
            Data.Color cardColor;

            cardName = card.Place == CardPlace.GameColumn
                ? _game.Columns.ColGame[(int)_game.Columns.GetGameColIndex(card)].GetCardMovingNameInColumn(card)
                : card.Data.CardName;

            cardColor = card.Place == CardPlace.GameColumn
                ? _game.Columns.ColGame[(int)_game.Columns.GetGameColIndex(card)].GetCardColorInColumn(card)
                : card.Data.CardColor;

            //set allowDrop = false for every card in game
            _imageViewer.SetAllowDrop();

            //game columns
            for (int index = 0; index < _game.Columns.ColGame.Length; index++)
            {
                //get last card in the column
                lastCard = _game.Columns.ColGame[index].GetLastCard();

                // if last card exists
                if (lastCard != null)
                {
                    Data.Name lastCardName = _game.Columns.ColGame[index].GetCardReceivingNameInColumn(lastCard);
                    Data.Color lastCardColor = _game.Columns.ColGame[index].GetCardColorInColumn(lastCard);

                    if (lastCardName != Data.Name.Joker)
                    {
                        (lastCard as CardImage).AllowDrop = cardName == Data.Name.Joker ||
                                                            (cardName == lastCardName - 1 && cardColor != lastCardColor);
                    }
                }
                else
                {
                    _imageViewer.GameColImages[index].AllowDrop = cardName == Data.Name.Joker ||
                                                                  cardName == Data.Name.King;
                }
            }

            lastCard = null;

            //if move 1 card
            if (card.Place != CardPlace.GameColumn ||
                _game.Columns.ColGame[(int)_game.Columns.GetGameColIndex(card)].GetLastCard().Equals(card))
            {

                //result columns
                for (int index = 0; index < _game.Columns.ColResult.Length; index++)
                {
                    //get last card
                    lastCard = _game.Columns.ColResult[index].GetLastCard();

                    if (lastCard != null)
                    {
                        if (card.Data.CardLear == lastCard.Data.CardLear &&
                            card.Data.CardName == lastCard.Data.CardName + 1)
                        {
                            (lastCard as CardImage).AllowDrop = true;
                        }
                    }
                    else
                    {
                        _imageViewer.ResultColImages[index].AllowDrop = card.Data.CardName == Data.Name.Ace;
                    }
                }

                lastCard = null;

                //additional columns
                for (int index = 0; index < _game.Columns.ColAdditional.Length; index++)
                {
                    //get last card
                    lastCard = _game.Columns.ColAdditional[index].GetLastCard();

                    if (lastCard != null)
                    {
                        if (card.Data.CardName == _game.Columns.ColAdditional[index].CardName)
                        {
                            (lastCard as CardImage).AllowDrop = true;
                        }
                    }
                    else
                    {
                        if (card.Data.CardName == _game.Columns.ColAdditional[index].CardName)
                        {
                            _imageViewer.AdditionalColImages[index].AllowDrop = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set cards to move
        /// </summary>
        private void SetMovingCards(ICard card)
        {
            _cardsMove.Clear();

            switch (card.Place)
            {
                case CardPlace.GameColumn:
                    int? iColGame = _game.Columns.GetGameColIndex(card);
                    if (iColGame != null)
                    {
                        for (int index = _game.Columns.ColGame[(int)iColGame].Count - 1; index >= 0; index--)
                        {
                            var lastCard = _game.Columns.ColGame[(int)iColGame][index];
                            _cardsMove.Add(lastCard as CardImage);

                            if (lastCard.Equals(card))
                                break;
                        }
                    }
                    else
                    {
                        throw new Exception("Index game column in null.");
                    }
                    break;

                default:
                    _cardsMove.Add(card as CardImage);
                    break;
            }
            
            foreach (var movingCard in _cardsMove)
                movingCard.BorderThickness = new Thickness(2);
        }

        private void card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var card = (CardImage)sender;

            if (card.AllowMove)
            {
                CheckOnAllowDrop(card);
                SetMovingCards(card);
                DragDrop.DoDragDrop(card, card, DragDropEffects.Move);

                foreach (var movingCard in _cardsMove)
                    movingCard.BorderThickness = new Thickness(0);
            }
        }

        private void card_Drop(object sender, DragEventArgs e)
        {
            AppImage tempImage = sender as AppImage;
            int iCol = Grid.GetColumn(tempImage);
            int iRow = Grid.GetRow(tempImage);

            AddIntoNewPlace(iCol, iRow);
        }

        private void AddIntoNewPlace(int iGridCol, int iGridRow)
        {
            int iCurrCol;
            CardImage card = _cardsMove[0];
            CardPlace prevCardPlace = card.Place;

            switch (iGridRow)
            {
                //if there is result columns
                case GameWindowConstants.iRowResColsGrid:
                    iCurrCol = grid.GetResColIndex(iGridCol);
                    // replace moved card
                    _game.ReplaceCard(card, _game.Columns.ColResult[iCurrCol]);
                    break;

                case GameWindowConstants.iRowGameColsGrid:
                    iCurrCol = grid.GetGameColIndex(iGridCol);
                    // replace cards
                     for (int index = _cardsMove.Count - 1; index >= 0; index--)
                         _game.ReplaceCard(_cardsMove[index], _game.Columns.ColGame[iCurrCol]);
                    if(prevCardPlace != CardPlace.ResultColumn) _game.AutocollectCards();
                    break;

                case GameWindowConstants.iRowAdditionalColsGrid:
                    // replace card
                    _game.ReplaceCard(card,
                        iGridCol == _gameWindowData.iGridColKing
                            ? _game.Columns.ColAdditional[Constants.iColKing]
                            : _game.Columns.ColAdditional[Constants.iColJoker]);

                    _game.AutocollectCards();
                    break;

                default:
                    throw new Exception("Invalid column index.");
            }

            _game.CheckOnWin();
        }

        #endregion

        #endregion

        #endregion

        #region Private methods

        private void WindowSizeChanged()
        {
            // align card height and card width
            if (_gameWindowData.CardHeight / _gameWindowData.CardWidth > 1.4)
                _gameWindowData.CardHeight = _gameWindowData.CardWidth * 1.4;
            else
                _gameWindowData.CardWidth = _gameWindowData.CardHeight / 1.4;
            
            // change distance
            _gameWindowData.CardGameFaceDistance = _gameWindowData.CardHeight / 5;
            _gameWindowData.CardDeckDistance = _gameWindowData.CardWidth / 4;
            _gameWindowData.CardGameBackDistance = _gameWindowData.CardGameFaceDistance / 2;

            // refresh card view
            _imageViewer.RefreshImageSize();
            _imageViewer.RefreshCardDistanceInColumns(_game.Columns.ColGame, 
                grid.RowDefinitions[GameWindowConstants.iRowGameColsGrid].ActualHeight);
            _imageViewer.AlignDeckSequence(_game.Columns.ColDeck);

            // refresh DeckCardsNumber
            _game.OnPropertyChanged("DeckCardsNumber");

            // set new corresponding font size
            lblDeckCardsNumber.FontSize = _gameWindowData.CardGameBackDistance * 1.5;
            BottomBarFontSize = _gameWindowData.CardGameBackDistance * 1.5;
        }

        private void NewGame()
        {
            _imageViewer.ConfigureNewGameImages();
        }

        #endregion
    }
}
