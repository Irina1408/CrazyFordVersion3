using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AppStyle.Controls;
using CrazyFord.Data.Cards;
using CrazyFord.Data.Columns;

namespace CrazyFord.WindowHelp
{
    using CrazyFord.Data;
    using CrazyFord.Logic;
    using CrazyFord.WindowHelp.WindowData;

    public class ImageViewer
    {
        #region Private fields

        private GameWindowData _gameWindowData;

        #endregion

        #region Init

        public ImageViewer()
        {
            ResultColImages = new AppImage[StaticGameData.CountResultColumns];
            GameColImages = new AppImage[StaticGameData.CountGameColumns];
            AdditionalColImages = new AppImage[GameWindowConstants.CountAdditionalColumns];
            DeckColImage = new AppImage();
        }

        public ImageViewer(Grid grid, GameWindowData gameWindowData, AppImage[] cards)
            : this()
        {
            _gameWindowData = gameWindowData;
            Cards = cards;

            ConfigureCards(grid);
            InitialzeResultColumnImages(grid);
            InitializeGameColumnImages(grid);
            InitializeAdditionalColumnImages(grid);
        }

        private void ConfigureCards(Grid grid)
        {
            foreach (var card in Cards)
            {
                grid.Children.Add(card);
                card.VerticalAlignment = VerticalAlignment.Top;
                card.Stretch = Stretch.Fill;
            }
        }

        private void InitialzeResultColumnImages(Grid grid)
        {
            for (int index = 0; index < ResultColImages.Length; index++)
            {
                ResultColImages[index] = new AppImage
                {
                    ImageSource = Helper.GetImageSourceFromResource("Resources/Ace.png")
                };

                grid.Children.Add(ResultColImages[index]);
                Grid.SetRow(ResultColImages[index], GameWindowConstants.iRowResColsGrid);
                Grid.SetColumn(ResultColImages[index], grid.GetResGridColIndex(index));
            }
        }

        private void InitializeGameColumnImages(Grid grid)
        {
            for (int index = 0; index < GameColImages.Length; index++)
            {
                GameColImages[index] = new AppImage
                {
                    ImageSource = Helper.GetImageSourceFromResource("Resources/King.png")
                };

                grid.Children.Add(GameColImages[index]);
                Grid.SetRow(GameColImages[index], GameWindowConstants.iRowGameColsGrid);
                Grid.SetColumn(GameColImages[index], grid.GetGameGridColIndex(index));
            }
        }

        private void InitializeAdditionalColumnImages(Grid grid)
        {
            for (int index = 0; index < AdditionalColImages.Length; index++)
            {
                AdditionalColImages[index] = new AppImage();

                grid.Children.Add(AdditionalColImages[index]);
                Grid.SetRow(AdditionalColImages[index], GameWindowConstants.iRowAdditionalColsGrid);
            }

            //deck
            grid.Children.Add(DeckColImage);
            Grid.SetRow(DeckColImage, GameWindowConstants.iRowAdditionalColsGrid);

            //static data
            DeckColImage.ImageSource = Helper.GetImageSourceFromResource("Resources/CardBack/2.png");
            AdditionalColImages[GameWindowConstants.iImageKing].ImageSource = Helper.GetImageSourceFromResource("Resources/King.png");
            AdditionalColImages[GameWindowConstants.iImageJoker].ImageSource = Helper.GetImageSourceFromResource("Resources/Empty.png");

            Grid.SetColumn(DeckColImage, _gameWindowData.iGridColDeck);
            Grid.SetColumn(AdditionalColImages[GameWindowConstants.iImageKing], _gameWindowData.iGridColKing);
            Grid.SetColumn(AdditionalColImages[GameWindowConstants.iImageJoker], _gameWindowData.iGridColJoker);
        }

        #endregion

        #region Public properties

        public AppImage[] Cards { get; private set; }
        public AppImage[] ResultColImages { get; private set; }
        public AppImage[] GameColImages { get; private set; }
        public AppImage[] AdditionalColImages { get; private set; }
        public AppImage DeckColImage { get; private set; }

        #endregion

        #region Public methods

        public void RefreshImageSize()
        {
            double width = _gameWindowData.CardWidth;
            double height = _gameWindowData.CardHeight;

            foreach (var image in GameColImages)
            {
                image.Width = width;
                image.Height = height;
            }

            foreach (var image in ResultColImages)
            {
                image.Width = width;
                image.Height = height;
            }

            foreach (var image in AdditionalColImages)
            {
                image.Width = width;
                image.Height = height;
            }

            DeckColImage.Width = width;
            DeckColImage.Height = height;

            foreach (var image in Cards)
            {
                image.Width = width;
                image.Height = height;
            }
        }
        
        public void RefreshCardDistanceInColumns(IColumn[] columns, double maxDistance)
        {
            bool lastShowedFace = false;
            double cardDistance = 0;

            foreach (var col in columns)
            {
                double lastCardDistance = (col.GetLastCard() as CardImage) != null 
                    ? (col.GetLastCard() as CardImage).Margin.Top
                    : 0.0;
                double scale = _gameWindowData.CardHeight + lastCardDistance > maxDistance
                    ? (_gameWindowData.CardHeight + lastCardDistance - maxDistance) / col.Count
                    : 0.0;
                    
                for (int iCard = 0; iCard < col.Count; iCard++)
                {
                    //if the previos card was not visible
                    if (iCard != 0)
                        cardDistance += !lastShowedFace 
                            ? (_gameWindowData.CardGameBackDistance - scale) 
                            : (_gameWindowData.CardGameFaceDistance - scale);

                    lastShowedFace = col[iCard].IsShowedFace;
                    (col[iCard] as CardImage).Margin = new Thickness(0, cardDistance, 0, 0);
                }

                cardDistance = 0;
                lastShowedFace = false;
            }
        }

        public void AlignDeckSequence(IColumn column)
        {
            int difference = column.Count - StaticGameData.CountVisibleCardsInDeck;
            difference = difference > 0 ? difference : 0;

            for (int index = 0; index < column.Count; index++)
            {
                (column[index] as CardImage).Margin = index < difference
                    ? new Thickness(0, 0, 0, 0)
                    : new Thickness((index - difference) * _gameWindowData.CardDeckDistance,
                        0, -(index - difference) * _gameWindowData.CardDeckDistance, 0);
            }
        }

        /// <summary>
        /// Configure images to new game
        /// </summary>
        public void ConfigureNewGameImages()
        {
            //card deck visible
            DeckColImage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Set allow drop for every card and any other images in the current game
        /// </summary>
        public void SetAllowDrop(bool allowDrop = false)
        {
            foreach (var image in Cards)
                image.AllowDrop = allowDrop;

            foreach (var image in ResultColImages)
                image.AllowDrop = allowDrop;

            foreach (var image in GameColImages)
                image.AllowDrop = allowDrop;

            foreach (var image in AdditionalColImages)
                image.AllowDrop = allowDrop;
        }

        public void SetCardEvents(MouseButtonEventHandler mouseDown, System.Windows.DragEventHandler drop)
        {
            foreach (var image in Cards)
            {
                image.MouseDown += mouseDown;
                image.Drop += drop;
            }

            foreach (var image in ResultColImages)
                image.Drop += drop;

            foreach (var image in GameColImages)
                image.Drop += drop;

            foreach (var image in AdditionalColImages)
                image.Drop += drop;
        }

        #endregion
    }
}
