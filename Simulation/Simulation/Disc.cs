using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    /// <summary>
    /// Disc
    /// </summary>
    public class Disc : Point
    {

        #region メンバ
        public static readonly int BLACK = 1;       // 黒
	    public static readonly int EMPTY = 0;       // 何もない
	    public static readonly int WHITE = -1;      // 白
        public static readonly int WALL = 2;        // 壁
	
	    public int color;                           // 色
        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public Disc() : base()
	    {
		    this.color = EMPTY;
	    }
	
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
	    public Disc(int x, int y, int color) : base(x, y)
	    {
		    this.color = color;
	    }
    }
}
