using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyFord.Data;
using CrazyFord.Data.Clumns.BaseClasses;

namespace CrazyFord
{
    public class Action
    {
        //private List<Card> cards = new List<Card>();
        public List<Card> Cards { get; set; }
        public CardPlace ActionFrom { get; set; }
        public CardPlace ActionTo { get; set; }
        public ColumnBase ColumnFrom { get; set; }
        public ColumnBase ColumnTo { get; set; }

        public Action()
        {
            Cards = new List<Card>();
        }

        public Action(List<Card> cards, CardPlace actionFrom, CardPlace actionTo, ColumnBase columnFrom, ColumnBase columnTo)
        {
            Cards = cards;
            ActionFrom = actionFrom;
            ActionTo = actionTo;
            ColumnFrom = columnFrom;
            ColumnTo = columnTo;
        }
    }
}
