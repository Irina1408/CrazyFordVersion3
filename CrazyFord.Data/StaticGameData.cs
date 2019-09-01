using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data
{
    public static class StaticGameData
    {
        /// <summary>
        /// Count all cards in game
        /// </summary>
        public static int CountCards = 108;

        /// <summary>
        /// Count decks are used in this game
        /// </summary>
        public static int CountDecks = 2;

        /// <summary>
        /// Count cards in every deck
        /// </summary>
        public static int CountCardsInDeck = 54;

        /// <summary>
        /// Count result columns that you need to fill to win
        /// </summary>
        public static int CountResultColumns = 8;

        /// <summary>
        /// Count columns where you play (main columns)
        /// </summary>
        public static int CountGameColumns = 10;

        /// <summary>
        /// Count lears in game (without joker)
        /// </summary>
        public static int CountLears = 4;

        /// <summary>
        /// Count card names in game (without joker)
        /// </summary>
        public static int CountCardNames = 13;

        /// <summary>
        /// Count jokers in one deck
        /// </summary>
        public static int CountJokers = 2;

        /// <summary>
        /// Count cards that user can see
        /// </summary>
        public static int CountVisibleCardsInDeck = 15;

        /// <summary>
        /// Game rules
        /// </summary>
        public const string Rules = @"------------------Сумасшедший Форд------------------
    Цель игры:
    Переместить все карты в базовые ряды

    Правила:

        8 базовых рядов (справа вверху)

    * Заполняются по мастям последовательно от туза до короля.
    * В пустой ряд может быть помещен только туз.
    * Верхнюю карту нельзя перемещать в игровой ряд.

        10 игровых рядов
    * Заполняются в порядке убывания с чередованием цвета.
    * В базовый ряд может быть перемещена только верхняя карта.
    * В другой игровой ряд может быть перемещена верхняя карта либо стопка карт.
    * Карты в перемещаемой стопке должны быть упорядочены в порядке убывания с чередованием цвета.
    * В пустой ряд может быть помещен только король или джокер.
    * Джокер может заменить любую карту.

        Колода (слева внизу)

    * В сток открывается по одной карте за раз.
    * Нет дополнительных пересдач.

        Резерв (внизу по центру)

    * В ряд можно класть только Королей.
    * Верхнего Короля можно перекладывать в другой ряд.

        Ряд Джокера (справа внизу)

    * Заполняется только джокерами.
    * В пустой ряд можно класть только джокера";
    }
}
