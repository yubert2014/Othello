using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    /// <summary>
    /// 色情報を格納
    /// </summary>
    class ColorStorage
    {
        // データ
        private int[] data = new int[3];

        /// <summary>
        /// ゲッター
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
	    public int get(int color)
	    {
		    return data[color+1];
	    }

        /// <summary>
        /// セッター
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value"></param>
	    public void set(int color, int value)
	    {
		    data[color+1] = value;
	    }
    }

}
