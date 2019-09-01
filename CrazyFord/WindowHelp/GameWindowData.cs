using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.WindowHelp
{
    public class GameWindowData
    {
        public int iGridColDeck { get; set; }
        public int iGridColKing { get; set; }
        public int iGridColJoker { get; set; }
        public double CardWidth { get; set; }
        public double CardHeight { get; set; }
        public double CardGameFaceDistance { get; set; }
        public double CardGameBackDistance { get; set; }
        public double CardDeckDistance { get; set; }
        public bool IsDeckMouseDown { get; set; }
    }
}
