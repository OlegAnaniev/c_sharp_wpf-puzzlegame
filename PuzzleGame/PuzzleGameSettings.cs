using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamPuzzle
{    
    [Serializable]
    public class PuzzleGameSettings
    {
        public int ColumnsCount { get; set; }
        public int RowsCount { get; set; }
        public int ShuffleRounds { get; set; }
        public string TestID { get { return "This is my test"; } }
        public PuzzleGameSettings(int countX, int countY, int shuffleRounds) //:this()
        {
            ColumnsCount = countX;
            RowsCount = countY;
            ShuffleRounds = shuffleRounds;
        }
    }
}
