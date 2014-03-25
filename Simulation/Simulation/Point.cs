using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    /// <summary>
    /// 場所
    /// </summary>
    public class Point
    {
        #region メンバ
        public int x;       // x
	    public int y;       // y
        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
	    public Point()
	    {
            this.x = 0;
            this.y = 0;
	    }
	
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
	    public Point(int x, int y)
	    {
		    this.x = x;
		    this.y = y;
	    }
	
        /// <summary>
        /// マス目を返却
        /// </summary>
        /// <returns></returns>
	    public String toString()
	    {
            StringBuilder coord = new StringBuilder();
            coord.Append((char)('a' + x - 1));
            coord.Append((char)('1' + y - 1));

            return coord.ToString();
	    }
	
        /// <summary>
        /// 打った場所が同じかどうかを判定
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
	    public bool equals(Point p)
	    {
            // x座標が異なる
            if (this.x != p.x)
            {
                return false;
            }

            // y座標が異なる
            if (this.y != p.y)
            {
                return false;
            }
		
		    return true;
	    }


    }

}
