using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    /// <summary>
    /// 評価関数
    /// </summary>
    interface Evaluator
    {
        // 評価
        int evaluate(Board board);
    }

    /// <summary>
    /// 全評価
    /// </summary>
    class PerfectEvaluator : Evaluator
    {
        // 評価
	    public int evaluate(Board board)
	    {
		    int discdiff
			    = board.getCurrentColor()
			    * (board.countDisc(Disc.BLACK) - board.countDisc(Disc.WHITE));
		
		    return discdiff;
	    }
    }

    /// <summary>
    /// 必勝読み
    /// </summary>
    class WLDEvaluator : Evaluator
    {
	    public static readonly int WIN  =  1;
	    public static readonly int DRAW =  0;
        public static readonly int LOSE = -1;

        // 評価
	    public int evaluate(Board board)
	    {
		    int discdiff
			    = board.getCurrentColor()
			    * (board.countDisc(Disc.BLACK) - board.countDisc(Disc.WHITE));
		
		    if(discdiff > 0) return WIN;
		    else if(discdiff < 0) return LOSE;
		    else return DRAW;
	    }
    }


}
