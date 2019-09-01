using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrazyFord.Data.Columns;

namespace CrazyFord.Logic
{
    using CrazyFord.Data;
    using CrazyFord.Data.Cards;

    public class Game : INotifyPropertyChanged, IDisposable
    {
        #region Private fields

        private DateTime _gameTime;
        //private System.Windows.Threading.DispatcherTimer _timer;
        private Task _timer;
        private TimeSpan _timeout;
        private int _currentCardIndex;
        private int[] _cardSequence;

        #endregion

        #region Initialization

        public Game()
        {
            _currentCardIndex = 0;
            _cardSequence = new int[StaticGameData.CountCards];
            Columns = new Columns();

            InitGameTime();
            InitializeCards();
        }

        private void InitGameTime()
        {
            _gameTime = new DateTime();
            _timeout = new TimeSpan(0, 0, 1);
        }

        protected virtual void InitializeCards()
        {
            Cards = new Card[StaticGameData.CountCards];

            int index = 0;
            //loop for every deck
            for (int iDeck = 1; iDeck <= StaticGameData.CountDecks; iDeck++)
            {
                //loop for set lear, name and place in file settings of the cards
                for (int iLear = 0; iLear < StaticGameData.CountLears; iLear++)
                {
                    for (int iName = 0; iName < StaticGameData.CountCardNames; iName++)
                    {
                        Cards[index] = new Card(new DataCard((Lear)iLear, (Name)iName));
                        Cards[index].Hide();

                        index++;
                    }
                }

                //loop for set card settings if this card is joker
                for (int iJoker = index; iJoker < index + StaticGameData.CountJokers; iJoker++)
                {
                    Cards[iJoker] = new Card(new DataCard(Lear.None, CrazyFord.Data.Name.Joker));
                    Cards[iJoker].Hide();
                }

                index += StaticGameData.CountJokers;
            }
        }

        #endregion

        #region Public properties

        public String GameTime { get { return _gameTime.ToString("T"); } }

        public string DeckCardsNumber { get { return StaticGameData.CountCards - _currentCardIndex + " / " + Columns.ColDeck.Count; } }

        /// <summary>
        /// Gets all game cards
        /// </summary>
        public ICard[] Cards { get; protected set; }

        /// <summary>
        /// Gets game columns
        /// </summary>
        public Columns Columns { get; private set; }

        public bool IsGame { get; private set; }

        #endregion

        #region Public events

        public Action StartGameAction;
        public Action PauseGameAction;
        public Action ResumeGameAction;
        public Action WinGameAction;
        
        #endregion

        #region Public methods

        public virtual void Start()
        {
            // clear all previous data
            Columns.Clear();
            _currentCardIndex = 0;
            _cardSequence = CrazyFord.Logic.Utils.Shuffle(_cardSequence);
            
            DecomposeCardsIntoGameColumns();

            //revert game time
            _gameTime = new DateTime();
            OnPropertyChanged("DeckCardsNumber");

            if (StartGameAction != null)
                StartGameAction();

            // start game
            IsGame = true;
            RunTimer();

            // autocollect cards into result column
            AutocollectCards();
        }

        public virtual void Pause()
        {
            //pause timer
            IsGame = false;

            if (PauseGameAction != null)
                PauseGameAction();
        }

        public virtual void Resume()
        {
            if (ResumeGameAction != null)
                ResumeGameAction();

            RefreshGameSettings();
            //start game
            IsGame = true;
            RunTimer();
        }

        public virtual void Stop()
        {
            // hide all cards
            foreach (var card in Cards)
                card.Hide();

            IsGame = false;
        }

        public void CheckOnWin()
        {
            bool isWin = Columns.ColResult.All(columnResult => columnResult.IsFull);

            if (Columns.ColAdditional[Constants.iColJoker].Count != StaticGameData.CountJokers * StaticGameData.CountDecks)
            {
                isWin = false;
            }

            if (isWin)
            {
                // stop game
                IsGame = false;

                if (WinGameAction != null)
                    WinGameAction();

                if (StartNewGameAfterWin())
                {
                    Start();
                }
            }
        }

        /// <summary>
        /// Returns next card after current
        /// </summary>
        public ICard GetNextCard()
        {
            return Cards[_cardSequence[_currentCardIndex ++]];
        }

        public bool DeckHasUnvisibleCards()
        {
            return _currentCardIndex == _cardSequence.Length;
        }

        public void ReplaceCard(ICard card, IColumn columnTo)
        {
            DeleteFromLastPlace(card);
            columnTo.AddCard(card);
        }

        public void AutocollectCards()
        {
            if (!Settings.Instance.AutoCollectionCards) return;

            // indicate any card autocollected in this circle or not
            bool collect = false;

            // loop on every game columns
            foreach (var columnGame in Columns.ColGame)
            {
                var lastGameCard = columnGame.GetLastCard();

                // we should not replace jokers
                if (lastGameCard == null || lastGameCard.Data.CardName == Name.Joker) continue;

                collect = FindNewPlace(lastGameCard) || collect;
            }

            // check deck
            var lastDeckCard = Columns.ColDeck.GetLastCard();
            if (lastDeckCard != null) collect = FindNewPlace(lastDeckCard) || collect;

            // if any card was replaced on this iteration repeat iteration
            if(collect) AutocollectCards();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Find new place throw result columns and when find corresponding column replace card
        /// </summary>
        private bool FindNewPlace(ICard card)
        {
            // loop on every result column
            foreach (var columnResult in Columns.ColResult)
            {
                bool replace = false;

                // if result column is empty
                if (columnResult.ColumnLear == Lear.None)
                {
                    if (card.Data.CardName == Name.Ace)
                        replace = true;
                }
                else if (card.Data.CardLear == columnResult.ColumnLear)
                {
                    var lastResultCard = columnResult.GetLastCard();

                    // if last card in game column is appropriate replace it
                    if (lastResultCard != null && (int)card.Data.CardName == ((int)lastResultCard.Data.CardName) + 1)
                        replace = true;
                }

                if (replace)
                {
                    ReplaceCard(card, columnResult);
                    return true;
                }
            }

            return false;
        }

        private void DecomposeCardsIntoGameColumns()
        {
            // hide all cards
            foreach (var card in Cards)
                card.Hide();

            //add cards in game columns
            for (int iCol = 0; iCol < Columns.ColGame.Length; iCol++)
            {
                for (int index = 0; index < iCol + 1; index++)
                {
                    int iCard = _cardSequence[_currentCardIndex];
                    
                    //show card
                    if (index == iCol)
                        Cards[iCard].ShowFace();
                    else
                        Cards[iCard].ShowCardBack();

                    Columns.ColGame[iCol].AddCard(Cards[iCard]);

                    _currentCardIndex++;
                }
            }
        }

        private void RunTimer()
        {
            _timer = Task.Run(() =>
            {
                while (IsGame)
                {
                    // wait 1 second
                    _timer.Wait(_timeout);

                    // write new datetime
                    _gameTime = _gameTime.AddSeconds(1);
                    OnPropertyChanged("GameTime");
                }
            });
        }

        /// <summary>
        /// Delete card from last place (current column)
        /// </summary>
        private void DeleteFromLastPlace(ICard card)
        {
            switch (card.Place)
            {
                case CardPlace.None:
                    //throw new Exception("Card isn`t placed in any column.");
                    break;

                case CardPlace.ResultColumn:
                    int? iColResult = Columns.GetResultColIndex(card);

                    if (iColResult != null)
                        Columns.ColResult[(int)iColResult].DeleteCard(card);
                    else
                        throw new Exception("Index result column in null.");
                    break;

                    break;

                case CardPlace.GameColumn:
                    int? iColGame = Columns.GetGameColIndex(card);

                    if (iColGame != null)
                        Columns.ColGame[(int)iColGame].DeleteCard(card);
                    else
                        throw new Exception("Index game column in null.");
                    break;

                case CardPlace.AdditionalColumn:

                    if (Columns.ColAdditional[Constants.iColKing].Contains(card))
                        Columns.ColAdditional[Constants.iColKing].DeleteCard(card);
                    else
                        Columns.ColAdditional[Constants.iColJoker].DeleteCard(card);

                    break;

                case CardPlace.Deck:

                    if (Columns.ColDeck.Contains(card))
                        Columns.ColDeck.DeleteCard(card);
                    else
                        throw new Exception("Unknown card in deck");

                    break;
            }
        }

        private void RefreshGameSettings()
        {
            // refresh every no empty column on last card moving
            foreach (var lastCard in Columns.ColResult.Where(col => col.Count > 0).Select(column => column.GetLastCard()))
            {
                lastCard.AllowMove = Settings.Instance.CardGetFromResultColumn;
            }
        }

        protected virtual bool StartNewGameAfterWin()
        {
            return true;
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

        #region IDisposable implementation

        public void Dispose()
        {
            if (_timer != null &&
                _timer.Status == TaskStatus.Running)
            {
                IsGame = false;
                Task.WaitAll(new[] { _timer });
                _timer.Dispose();
            }
        }

        #endregion
    }
}
