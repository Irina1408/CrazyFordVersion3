using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyFord.Data;

namespace CrazyFord.GameProcess
{
    public class GameWindowData
    {
        #region Init

        public GameWindowData()
        {
            CardSequence = new int[AdditionalData.CountCards];
            IsGame = false;
            IsDeckMouseDown = false;
        }

        #endregion

        #region Public properties

        public int iGridColDeck { get; set; }
        public int iGridColKing { get; set; }
        public int iGridColJoker { get; set; }
        public double CardWidth { get; set; }
        public double CardHeight { get; set; }
        public double CardGameFaceDistance { get; set; }
        public double CardGameBackDistance { get; set; }
        public double CardDeckDistance { get; set; }
        public int[] CardSequence { get; set; }
        public int CurCardIndex { get; set; }
        public bool IsGame { get; set; }
        public bool IsDeckMouseDown { get; set; }

        #endregion
    }
}
