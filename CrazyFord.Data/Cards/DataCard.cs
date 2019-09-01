using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data
{
    public enum Lear
    {
        None = -1,
        Club,       //треф
        Diamond,    //бубен
        Heart,      //чирва
        Spade       //пика
    }

    public enum Name
    {
        Joker = -1,
        Ace ,    //туз
        Two,    //2
        Three,  //3
        Four,   //4
        Five,   //5
        Six,    //6
        Seven,  //7
        Eight,  //8
        Nine,   //9
        Ten,    //10
        Jack,   //валет
        Queen,  //дама
        King    //король
    }

    public enum Color
    {
        None = -1,
        Red,
        Black
    }

    public class DataCard
    {
        #region Public properties

        public Lear CardLear { get; private set; }
        public Name CardName { get; private set; }
        public Color CardColor { get; private set; }

        #endregion

        #region Initialize

        public DataCard(Lear lear, Name name)
        {
            CardLear = lear;
            CardName = name;

            switch (lear)
            {
                case Lear.Club:
                case Lear.Spade:
                    CardColor = Color.Black;
                    break;

                case Lear.Diamond:
                case Lear.Heart:
                    CardColor = Color.Red;
                    break;

                default:
                    CardColor = Color.None;
                    break;
            }
        }

        #endregion

        #region Public methods

        public Color GetOppositeColor()
        {
            if (CardColor == Color.Red)
            {
                return Color.Black;
            }

            if (CardColor == Color.Black)
            {
                return Color.Red;
            }

            return Color.None;
        }

        #endregion
    }
}
