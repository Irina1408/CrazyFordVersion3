using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CrazyFord.Data;
using CrazyFord.Data.Clumns;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord.GameProcess
{
    public class GameProcess: INotifyPropertyChanged
    {
        #region Private fields

        private DateTime _gameTime;
        private History _history;
        private Grid _grid;

        #endregion

        #region Init

        public GameProcess(Grid grid)
        {
            GameWindowData = new GameWindowData();
            GameData = new GameData();
            _history = new History();

            _grid = grid;

            //additional data
            GameWindowData.iGridColDeck = 1;
            GameWindowData.iGridColKing = grid.ColumnDefinitions.Count / 2 + 1;
            GameWindowData.iGridColJoker = grid.ColumnDefinitions.Count - 2;

            this.GameImages = new GameImages(_grid, GameWindowData);
            this.GameColumns = new GameColumns(AlignDeckSequence);

            Timer = new System.Windows.Threading.DispatcherTimer();

            Timer.Tick += timerTick;
            Timer.Interval = new TimeSpan(0, 0, 1);

            GameImages.SetEvents(card_MouseDown, card_Drop, Deck_MouseUp);
        }

        #endregion

        #region Public properties

        public GameImages GameImages { get; private set; }
        public GameWindowData GameWindowData { get; private set; }
        public GameColumns GameColumns { get; private set; }
        public GameData GameData { get; private set; }
        public System.Windows.Threading.DispatcherTimer Timer { get; private set; }

        public String GameTime
        {
            get { return _gameTime.ToString("T"); }
        }

        public String CountCardDeck
        {
            get
            {
                return AdditionalData.CountCards - GameWindowData.CurCardIndex + " / " + GameColumns.ColDeck.Count;
            }
        }

        #endregion

        #region Public methods

        public void WindowSizeChanged()
        {
            if (GameWindowData.CardHeight / GameWindowData.CardWidth > 1.4)
            {
                GameWindowData.CardHeight = GameWindowData.CardWidth * 1.4;
            }
            else
            {
                GameWindowData.CardWidth = GameWindowData.CardHeight / 1.4;
            }

            GameImages.SetImageSize(GameWindowData.CardWidth, GameWindowData.CardHeight);

            //change distance
            GameWindowData.CardGameFaceDistance = GameWindowData.CardHeight / 5;
            GameWindowData.CardDeckDistance = GameWindowData.CardWidth / 4;
            GameWindowData.CardGameBackDistance = GameWindowData.CardGameFaceDistance / 2;

            bool lastShowedFace = false;
            double cardDistance = 0;

            for (int iCol = 0; iCol < GameColumns.ColGame.Length; iCol++)
            {
                for (int iCard = 0; iCard < GameColumns.ColGame[iCol].Count; iCard++)
                {
                    //if the previos card was not visible
                    if (iCard != 0)
                    {
                        if (!lastShowedFace)
                        {
                            cardDistance += GameWindowData.CardGameBackDistance;
                        }
                        else
                        {
                            cardDistance += GameWindowData.CardGameFaceDistance;
                        }
                    }

                    lastShowedFace = GameColumns.ColGame[iCol][iCard].IsShowedFace;

                    GameColumns.ColGame[iCol][iCard].Margin = new Thickness(0, cardDistance, 0, 0);
                }

                cardDistance = 0;
                lastShowedFace = false;
            }

            AlignDeckSequence(GameColumns.ColDeck);
        }

        public void NewGame()
        {
            //reset game data
            GameWindowData.CurCardIndex = 0;

            //surffe card sequence
            GameWindowData.CardSequence = Helper.Shuffle(GameWindowData.CardSequence);

            for (int index = 0; index < AdditionalData.CountCards; index++)
            {
                GameImages.Cards[index].Visibility = Visibility.Hidden;
                GameImages.Cards[index].Place = CardPlace.None;
            }

            //card deck visible
            GameImages.DeckColImage.Visibility = Visibility.Visible;

            //clear column data
            for (int index = 0; index < GameColumns.ColResult.Length; index++)
            {
                GameColumns.ColResult[index].Clear();
            }

            for (int index = 0; index < GameColumns.ColGame.Length; index++)
            {
                GameColumns.ColGame[index].Clear();
            }

            for (int index = 0; index < GameColumns.ColAdditional.Length; index++)
            {
                GameColumns.ColAdditional[index].Clear();
            }

            GameColumns.ColDeck.Clear();

            //add cards in game columns
            for (int iCol = 0; iCol < AdditionalData.CountGameColumns; iCol++)
            {
                for (int index = 0; index < iCol + 1; index++)
                {
                    int iCard = GameWindowData.CardSequence[GameWindowData.CurCardIndex];
                    GameColumns.ColGame[iCol].AddCard(GameImages.Cards[iCard]);

                    GameImages.Cards[iCard].Visibility = Visibility.Visible;

                    Grid.SetRow(GameImages.Cards[iCard], GameWindowConstants.iRowGameColsGrid);
                    Grid.SetColumn(GameImages.Cards[iCard], GetGameGridColIndex(iCol));

                    GameImages.Cards[iCard].Margin = new Thickness(0, GameColumns.ColGame[iCol].GetCardIndex(GameImages.Cards[iCard]) * GameWindowData.CardGameBackDistance, 0, 0);

                    //replace card
                    if (index == iCol)
                    {
                        GameImages.Cards[iCard].ShowFace();
                    }
                    else
                    {
                        GameImages.Cards[iCard].ShowCardBack();
                    }

                    GameWindowData.CurCardIndex++;
                }
            }

            //lblCountCardDeck.Content = AdditionalData.CountCards - GameData.CurCardIndex + " / 0";
            OnPropertyChanged("CountCardDeck");
            GameWindowData.IsGame = true;
            //clear actions history
            _history.Clear();

            _gameTime = new DateTime();
            Timer.Start();
        }

        #endregion

        #region Private methods

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
                    column[index].Margin = new Thickness((index - difference) * GameWindowData.CardDeckDistance,
                        0, -(index - difference) * GameWindowData.CardDeckDistance, 0);
                }
            }

            OnPropertyChanged("CountCardDeck");
        }

        private void CheckOnWin()
        {
            bool isWin = true;

            foreach (ColumnResult columnResult in GameColumns.ColResult)
            {
                if (!columnResult.IsFull)
                {
                    isWin = false;
                    break;
                }
            }

            if (GameColumns.ColAdditional[GameWindowConstants.iColJoker].Count != AdditionalData.CountJokers)
            {
                isWin = false;
            }

            if (isWin)
            {
                Timer.Stop();

                if (MessageBox.Show("Вы выиграли! \n Хотите начать новую игру?", "Поздравляем!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.OK)
                {
                    NewGame();
                }
                else
                {
                    foreach (Card card in GameImages.Cards)
                    {
                        card.Visibility = Visibility.Hidden;
                    }

                    GameWindowData.IsGame = false;
                }
            }
        }

        #region Card moving

        private List<Card> _cardsMove = new List<Card>();

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
                cardName = GameColumns.ColGame[(int)GameColumns.GetGameColIndex(card)].GetCardMovingNameInColumn(card);
                cardColor = GameColumns.ColGame[(int)GameColumns.GetGameColIndex(card)].GetCardColorInColumn(card);
            }
            else
            {
                cardName = card.Data.CardName;
                cardColor = card.Data.CardColor;
            }

            //game columns
            for (int index = 0; index < GameColumns.ColGame.Length; index++)
            {
                //set allowDrop = false for every card in column
                GameColumns.ColGame[index].SetAllowDrop();
                //set allowDrop = false for image in this column
                GameImages.GameColImages[index].AllowDrop = false;
                //get last card
                lastCard = GameColumns.ColGame[index].GetLastCard();

                if (lastCard != null)
                {
                    Data.Name lastCardName = GameColumns.ColGame[index].GetCardReceivingNameInColumn(lastCard);
                    Data.Color lastCardColor = GameColumns.ColGame[index].GetCardColorInColumn(lastCard);

                    if (lastCardName != Data.Name.Joker)
                    {
                        if (cardName == Data.Name.Joker ||
                            (cardName == lastCardName - 1 && cardColor != lastCardColor))
                        {
                            lastCard.AllowDrop = true;
                        }
                    }
                }
                else
                {
                    if (cardName == Data.Name.Joker || cardName == Data.Name.King)
                    {
                        GameImages.GameColImages[index].AllowDrop = true;
                    }
                }
            }

            lastCard = null;

            //if move 1 card
            if (card.Place != CardPlace.GameColumn ||
                GameColumns.ColGame[(int)GameColumns.GetGameColIndex(card)].GetLastCard().Equals(card))
            {

                //result columns
                for (int index = 0; index < GameColumns.ColResult.Length; index++)
                {
                    //set allowDrop = false for every card in column
                    GameColumns.ColResult[index].SetAllowDrop();
                    //set allowDrop = false for image in this column
                    GameImages.ResultColImages[index].AllowDrop = false;
                    //get last card
                    lastCard = GameColumns.ColResult[index].GetLastCard();

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
                            GameImages.ResultColImages[index].AllowDrop = true;
                        }
                    }
                }

                lastCard = null;

                //additional columns
                for (int index = 0; index < GameColumns.ColAdditional.Length; index++)
                {
                    //set allowDrop = false for every card in column
                    GameColumns.ColAdditional[index].SetAllowDrop();
                    //set allowDrop = false for image in this column
                    GameImages.AdditionalColImages[index].AllowDrop = false;
                    //get last card
                    lastCard = GameColumns.ColAdditional[index].GetLastCard();

                    if (lastCard != null)
                    {
                        if (card.Data.CardName == GameColumns.ColAdditional[index].CardName)
                        {
                            lastCard.AllowDrop = true;
                        }
                    }
                    else
                    {
                        if (card.Data.CardName == GameColumns.ColAdditional[index].CardName)
                        {
                            GameImages.AdditionalColImages[index].AllowDrop = true;
                        }
                    }
                }
            }
        }

        private void card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Card card = (Card)sender;

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
                //break;

                case CardPlace.GameColumn:
                    int? iColGame = GameColumns.GetGameColIndex(card);
                    if (iColGame != null)
                    {
                        for (int index = _cardsMove.Count - 1; index >= 0; index--)
                        {
                            GameColumns.ColGame[(int)iColGame].DeleteCard(_cardsMove[index]);
                        }
                    }
                    else
                    {
                        throw new Exception("Index game column in null.");
                    }
                    break;

                case CardPlace.AdditionalColumn:
                    if (GameColumns.ColAdditional[GameWindowConstants.iColKing].Contains(card))
                    {
                        GameColumns.ColAdditional[GameWindowConstants.iColKing].DeleteCard(card);
                    }
                    else
                    {
                        GameColumns.ColAdditional[GameWindowConstants.iColJoker].DeleteCard(card);
                    }
                    break;

                case CardPlace.Deck:
                    if (GameColumns.ColDeck.Contains(card))
                    {
                        GameColumns.ColDeck.DeleteCard(card);
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
                    GameColumns.ColResult[iCurrCol].AddCard(card);
                    card.Margin = new Thickness(0, 0, 0, 0);
                    CheckOnWin();
                    break;

                case GameWindowConstants.iRowGameColsGrid:
                    iCurrCol = GetGameColIndex(iCol);
                    //delete card from last place
                    DeleteCardsPromLastPlace();
                    for (int index = _cardsMove.Count - 1; index >= 0; index--)
                    {
                        GameColumns.ColGame[iCurrCol].AddCard(_cardsMove[index]);

                        int countBackCards = GameColumns.ColGame[iCurrCol].GetCountBackCards();

                        Double cardDistance = countBackCards * GameWindowData.CardGameBackDistance +
                                                (GameColumns.ColGame[iCurrCol].GetCardIndex(_cardsMove[index]) -
                                                countBackCards) * GameWindowData.CardGameFaceDistance;

                        _cardsMove[index].Margin = new Thickness(0, cardDistance, 0, 0);
                        //set card row and column
                        Grid.SetColumn(_cardsMove[index], iCol);
                        Grid.SetRow(_cardsMove[index], iRow);
                    }
                    break;

                case GameWindowConstants.iRowAdditionalColsGrid:
                    //delete card from last place
                    DeleteCardsPromLastPlace();

                    if (iCol == GameWindowData.iGridColKing)
                    {
                        GameColumns.ColAdditional[GameWindowConstants.iColKing].AddCard(card);
                    }
                    else
                    {
                        GameColumns.ColAdditional[GameWindowConstants.iColJoker].AddCard(card);
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
                    int? iColGame = GameColumns.GetGameColIndex(card);
                    if (iColGame != null)
                    {
                        for (int index = GameColumns.ColGame[(int)iColGame].Count - 1; index >= 0; index--)
                        {
                            Card lastCard = GameColumns.ColGame[(int)iColGame][index];
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

            Effect effect = new DropShadowEffect();

            foreach (Card movingCard in _cardsMove)
            {
                movingCard.Effect = effect;
            }
        }

        #endregion

        #region Timer

        private void timerTick(object sender, EventArgs e)
        {
            _gameTime = _gameTime.AddSeconds(1);
            OnPropertyChanged("GameTime");
        }

        #endregion

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
            return (iCol + _grid.ColumnDefinitions.Count / 2 - AdditionalData.CountResultColumns) * 2 + 1;
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
            return (iColGrid - 1) / 2 - _grid.ColumnDefinitions.Count / 2 + AdditionalData.CountResultColumns;
        }

        #endregion

        #region Deck click

        private void Deck_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (GameWindowData.IsDeckMouseDown)
            {
                GameWindowData.IsDeckMouseDown = false;

                int iCard = GameWindowData.CardSequence[GameWindowData.CurCardIndex];

                //add in deck
                GameColumns.ColDeck.AddCard(GameImages.Cards[iCard]);
                GameWindowData.CurCardIndex++;

                //set card is visible
                //_cards[iCard].Margin = new Thickness(_colDeck.GetCardIndex(_cards[iCard]) * _cardDeckDistance, 0, -_colDeck.GetCardIndex(_cards[iCard]) * _cardDeckDistance, 0);
                //set card row and column
                Grid.SetColumn(GameImages.Cards[iCard], GameWindowData.iGridColDeck + 2);
                Grid.SetRow(GameImages.Cards[iCard], GameWindowConstants.iRowAdditionalColsGrid);
                OnPropertyChanged("CountCardDeck");

                if (GameWindowData.CurCardIndex >= GameWindowData.CardSequence.Length)
                {
                    GameImages.DeckColImage.Visibility = Visibility.Hidden;
                }
            }
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
