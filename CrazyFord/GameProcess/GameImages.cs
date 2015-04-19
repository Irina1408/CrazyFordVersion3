using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CrazyFord.Data;

namespace CrazyFord.GameProcess
{
    public class GameImages
    {
        #region Private fields

        private Grid _grid;
        private GameWindowData _gameWindowData;

        #endregion

        #region Init

        public GameImages()
        {
            Cards = new Card[AdditionalData.CountCards];
            ResultColImages = new Image[AdditionalData.CountResultColumns];
            GameColImages = new Image[AdditionalData.CountGameColumns];
            AdditionalColImages = new Image[GameWindowConstants.CountAdditionalColumns];
            DeckColImage = new Image();
        }

        public GameImages(Grid grid, GameWindowData gameWindowData)
            : this()
        {
            _grid = grid;
            _gameWindowData = gameWindowData;

            InitialzeResultColumnImages();
            InitializeGameColumnImages();
            InitializeAdditionalColumnImages();
            InitializeCards();
        }

        private void InitialzeResultColumnImages()
        {
            for (int index = 0; index < ResultColImages.Length; index++)
            {
                ResultColImages[index] = new Image();
                ResultColImages[index].Source = Helper.GetImageSourceFromResource("Resources/Ace.png");

                Helper.TuneCardImage(ResultColImages[index]);
                _grid.Children.Add(ResultColImages[index]);
                Grid.SetRow(ResultColImages[index], GameWindowConstants.iRowResColsGrid);
                Grid.SetColumn(ResultColImages[index], GetResGridColIndex(index));
            }
        }

        private void InitializeGameColumnImages()
        {
            for (int index = 0; index < GameColImages.Length; index++)
            {
                GameColImages[index] = new Image();
                GameColImages[index].Source = Helper.GetImageSourceFromResource("Resources/King.png");

                Helper.TuneCardImage(GameColImages[index]);
                _grid.Children.Add(GameColImages[index]);
                Grid.SetRow(GameColImages[index], GameWindowConstants.iRowGameColsGrid);
                Grid.SetColumn(GameColImages[index], GetGameGridColIndex(index));
            }
        }

        private void InitializeAdditionalColumnImages()
        {
            for (int index = 0; index < AdditionalColImages.Length; index++)
            {
                AdditionalColImages[index] = new Image();

                Helper.TuneCardImage(AdditionalColImages[index]);
                _grid.Children.Add(AdditionalColImages[index]);
                Grid.SetRow(AdditionalColImages[index], GameWindowConstants.iRowAdditionalColsGrid);
            }

            //deck
            Helper.TuneCardImage(DeckColImage);
            _grid.Children.Add(DeckColImage);
            Grid.SetRow(DeckColImage, GameWindowConstants.iRowAdditionalColsGrid);

            //static data
            DeckColImage.Source = Helper.GetImageSourceFromResource("Resources/CardBack/2.png");
            AdditionalColImages[GameWindowConstants.iImageKing].Source = Helper.GetImageSourceFromResource("Resources/King.png");
            AdditionalColImages[GameWindowConstants.iImageJoker].Source = Helper.GetImageSourceFromResource("Resources/Empty.png");

            Grid.SetColumn(DeckColImage, _gameWindowData.iGridColDeck);
            Grid.SetColumn(AdditionalColImages[GameWindowConstants.iImageKing], _gameWindowData.iGridColKing);
            Grid.SetColumn(AdditionalColImages[GameWindowConstants.iImageJoker], _gameWindowData.iGridColJoker);
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
                        Cards[index] = new Card(new DataCard((Lear)iLear, (Name)iName));
                        Cards[index].ImageSourceCardFace = Helper.GetImageSourceCardFaceByCardData(Cards[index].Data);

                        Helper.TuneCardImage(Cards[index]);
                        _grid.Children.Add(Cards[index]);
                        Cards[index].Visibility = Visibility.Hidden;

                        index++;
                    }
                }

                //loop for set card settings if this card is joker
                for (int iJoker = index; iJoker < index + AdditionalData.CountJokers; iJoker++)
                {
                    Cards[iJoker] = new Card(new DataCard(Lear.None, CrazyFord.Data.Name.Joker));
                    Cards[iJoker].ImageSourceCardFace = Helper.GetImageSourceFromResource("Resources/CardFace/Joker.png");

                    Helper.TuneCardImage(Cards[iJoker]);
                    _grid.Children.Add(Cards[iJoker]);
                    Cards[index].Visibility = Visibility.Hidden;
                }

                index += AdditionalData.CountJokers;
            }

            Card.ImageSourceCardBack = Helper.GetImageSourceFromResource("Resources/CardBack/2.png");
        }

        #endregion

        #region Public properties

        public Card[] Cards { get; set; }
        public Image[] ResultColImages { get; set; }
        public Image[] GameColImages { get; set; }
        public Image[] AdditionalColImages { get; set; }
        public Image DeckColImage { get; set; }

        #endregion

        #region Public methods

        public void SetImageSize(double width, double height)
        {
            for (int index = 0; index < GameColImages.Length; index++)
            {
                GameColImages[index].Width = width;
                GameColImages[index].Height = height;
            }

            for (int index = 0; index < ResultColImages.Length; index++)
            {
                ResultColImages[index].Width = width;
                ResultColImages[index].Height = height;
            }

            for (int index = 0; index < AdditionalColImages.Length; index++)
            {
                AdditionalColImages[index].Width = width;
                AdditionalColImages[index].Height = height;
            }

            DeckColImage.Width = width;
            DeckColImage.Height = height;

            for (int index = 0; index < Cards.Length; index++)
            {
                if (Cards[index] != null)
                {
                    Cards[index].Width = width;
                    Cards[index].Height = height;
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get result column index in grid
        /// </summary>
        private int GetResGridColIndex(int iCol)
        {
            return (iCol + _grid.ColumnDefinitions.Count / 2 - AdditionalData.CountResultColumns) * 2 + 1;
        }

        /// <summary>
        /// Get game column index in grid
        /// </summary>
        private int GetGameGridColIndex(int iCol)
        {
            return iCol * 2 + 1;
        }

        #endregion
    }
}
