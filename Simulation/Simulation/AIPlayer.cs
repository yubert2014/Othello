using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class AIPlayer
    {

        // AI
        private AI Ai = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AIPlayer()
        {
            Ai = new AlphaBetaAI();
        }

        /// <summary>
        /// 着手
        /// </summary>
        /// <param name="board"></param>
        public void onTurn(Board board)
        {
            Ai.move(board);
        }
    }
}
