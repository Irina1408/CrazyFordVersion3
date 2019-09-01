using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppStyle.Controls;
using CrazyFord.Data.Cards;

namespace CrazyFord.WindowHelp.WindowLogic
{
    using CrazyFord.Data;
    using CrazyFord.Logic;
    using CrazyFord.WindowHelp.WindowData;

    public class CustomGame : Game
    {
        protected override void InitializeCards()
        {
            Cards = new CardImage[StaticGameData.CountCards];

            int index = 0;
            //loop for every deck
            for (int iDeck = 1; iDeck <= StaticGameData.CountDecks; iDeck++)
            {
                //loop for set lear, name and place in file settings of the cards
                for (int iLear = 0; iLear < StaticGameData.CountLears; iLear++)
                {
                    for (int iName = 0; iName < StaticGameData.CountCardNames; iName++)
                    {
                        Cards[index] = new CardImage(new DataCard((Lear)iLear, (Name)iName));

                        var currentCard = ((CardImage)Cards[index]);
                        currentCard.ImageSourceCardFace = Helper.GetImageSourceCardFaceByCardData(Cards[index].Data);

                        Cards[index].Hide();

                        index++;
                    }
                }

                //loop for set card settings if this card is joker
                for (int iJoker = index; iJoker < index + StaticGameData.CountJokers; iJoker++)
                {
                    Cards[iJoker] = new CardImage(new DataCard(Lear.None, CrazyFord.Data.Name.Joker));
                    var currentCard = ((CardImage)Cards[iJoker]);
                    currentCard.ImageSourceCardFace = Helper.GetImageSourceFromResource("Resources/CardFace/Joker.png");

                    Cards[iJoker].Hide();
                }

                index += StaticGameData.CountJokers;
            }

            // set default card back for cards
            CardImage.ImageSourceCardBack = Helper.GetImageSourceFromResource("Resources/CardBack/2.png");
        }

        protected override bool StartNewGameAfterWin()
        {
            return MessageBox.Show("Вы выиграли! \n Хотите начать новую игру?", "Поздравляем!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes;
        }
    }
}
