using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    /// <summary>
    /// ボード
    /// </summary>
    public class Board
    {
	    public static readonly int BOARD_SIZE =  8;
	    public static readonly int MAX_TURNS  = 60;

	    private static readonly int NONE		 =   0;
	    private static readonly int UPPER		 =   1;
	    private static readonly int UPPER_LEFT	 =   2;
	    private static readonly int LEFT		 =   4;
	    private static readonly int LOWER_LEFT	 =   8;
	    private static readonly int LOWER		 =  16;
	    private static readonly int LOWER_RIGHT =  32;
	    private static readonly int RIGHT		 =  64;
	    private static readonly int UPPER_RIGHT = 128;

	    private int[,] RawBoard = new int[BOARD_SIZE+2,BOARD_SIZE+2];
	    private int Turns;          // 手数(0からはじまる)
	    private int CurrentColor;   // 現在のプレイヤー

        private List<List<object>> UpdateLog = new List<List<object>>();

        private List<List<object>> MovablePos = new List<List<object>>();
	    private int[,,] MovableDir = new int[MAX_TURNS+1,BOARD_SIZE+2,BOARD_SIZE+2];
	
	    private ColorStorage Discs = new ColorStorage();
	

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
	    public Board()
	    {
            // MovablePosを初期化
		    for(int i=0; i<=MAX_TURNS; i++)
		    {
                MovablePos.Add(new List<object>());
		    }

		    init();
	    }
	
        /// <summary>
        /// 初期化
        /// </summary>
	    public void init()
	    {
		    // 全マスを空きマスに設定
		    for(int x=1; x <= BOARD_SIZE; x++)
		    {
			    for(int y=1; y <= BOARD_SIZE; y++)
			    {
				    RawBoard[x,y] = Disc.EMPTY;
			    }
		    }

		    // 壁の設定
		    for(int y=0; y < BOARD_SIZE + 2; y++)
		    {
			    RawBoard[0,y] = Disc.WALL;
			    RawBoard[BOARD_SIZE+1,y] = Disc.WALL;
		    }

		    for(int x=0; x < BOARD_SIZE + 2; x++)
		    {
			    RawBoard[x,0] = Disc.WALL;
			    RawBoard[x,BOARD_SIZE+1] = Disc.WALL;
		    }


		    // 初期配置
		    RawBoard[4,4] = Disc.WHITE;
		    RawBoard[5,5] = Disc.WHITE;
		    RawBoard[4,5] = Disc.BLACK;
		    RawBoard[5,4] = Disc.BLACK;

		    // 石数の初期設定
		    Discs.set(Disc.BLACK, 2);
		    Discs.set(Disc.WHITE, 2);
		    Discs.set(Disc.EMPTY, BOARD_SIZE*BOARD_SIZE - 4);

		    Turns = 0; // 手数は0から数える
		    CurrentColor = Disc.BLACK; // 先手は黒

		    initMovable();
	    }
	
        /// <summary>
        /// 次へ
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
	    public bool move(Point point)
	    {
            // ボードの外は無効
            if (point.x <= 0 || point.x > BOARD_SIZE)
            {
                return false;
            }
            if (point.y <= 0 || point.y > BOARD_SIZE) { 
                return false; 
            }
            if (MovableDir[Turns, point.x, point.y] == NONE)
            {
                return false;
            }
		
            // ディスクを裏返す処理
		    flipDiscs(point);

		    Turns++;
		    CurrentColor = -CurrentColor;

		    initMovable();

		    return true;
	    }
	
        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
	    public bool undo()
	    {
		    // ゲーム開始地点ならもう戻れない
            if (Turns == 0) { return false; }

            // 色を入れ替え
		    CurrentColor = -CurrentColor;
		
            var update = UpdateLog[UpdateLog.Count - 1];
            UpdateLog.RemoveAt(UpdateLog.Count - 1);

		    // 前回がパスかどうかで場合分け
		    if(update.Count == 0)
		    {
			    // 前回はパス

			    // MovablePos及びMovableDirを再構築
                MovablePos[Turns].Clear();
			    for(int x=1; x<=BOARD_SIZE; x++)
			    {
				    for(int y=1; y<=BOARD_SIZE; y++)
				    {
					    MovableDir[Turns,x,y] = NONE;
				    }
			    }
		    }
		    else
		    {
			    // 前回はパスでない

			    Turns--;

			    // 石を元に戻す
                Point p = (Point)update[0];
			    RawBoard[p.x,p.y] = Disc.EMPTY;
			    for(int i=1; i<update.Count; i++)
			    {
                    p = (Point)update[i];
				    RawBoard[p.x,p.y] = -CurrentColor;
			    }
			
			    // 石数の更新
                int discdiff = update.Count;
			    Discs.set(CurrentColor, Discs.get(CurrentColor) - discdiff);
			    Discs.set(-CurrentColor, Discs.get(-CurrentColor) + (discdiff - 1));
			    Discs.set(Disc.EMPTY, Discs.get(Disc.EMPTY) + 1);
		    }

		    return true;
	    }
	
        /// <summary>
        /// パス
        /// </summary>
        /// <returns></returns>
	    public bool pass()
	    {
		    // 打つ手があればパスできない
		    if(MovablePos[Turns].Count != 0) return false;
		
		    // ゲームが終了しているなら、パスできない
            if (isGameOver() == true)
            {
                return false;
            }

		    CurrentColor = -CurrentColor;

            UpdateLog.Add(new List<object>());

		    initMovable();
		
		    return true;

	    }
	
        /// <summary>
        /// 色を取得
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
	    public int getColor(Point point)
	    {
		    return RawBoard[point.x,point.y];
	    }
	
        /// <summary>
        /// 現在の色を取得
        /// </summary>
        /// <returns></returns>
	    public int getCurrentColor()
	    {
		    return CurrentColor;
	    }
	
        /// <summary>
        /// 現在のターンを取得
        /// </summary>
        /// <returns></returns>
	    public int getTurns()
	    {
		    return Turns;
	    }
	
        /// <summary>
        /// ゲームオーバーかを判定
        /// </summary>
        /// <returns></returns>
	    public bool isGameOver()
	    {
		    // 60手に達していたらゲーム終了
		    if(Turns == MAX_TURNS) return true;
		
		    // 打てる手があるならゲーム終了ではない
		    if(MovablePos[Turns].Count != 0) return false;
		
		    //
		    //	現在の手番と逆の色が打てるかどうか調べる
		    //
		    Disc disc = new Disc();
		    disc.color = -CurrentColor;
		    for(int x=1; x<=BOARD_SIZE; x++)
		    {
			    disc.x = x;
			    for(int y=1; y<=BOARD_SIZE; y++)
			    {
				    disc.y = y;
				    // 置ける箇所が1つでもあればゲーム終了ではない
				    if(checkMobility(disc) != NONE) return false;
			    }
		    }
		
		    return true;
	    }
	
        /// <summary>
        /// ディスクの個数を数える
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
	    public int countDisc(int color)
	    {
		    return Discs.get(color);
	    }
	
        /// <summary>
        /// 移動可能な場所を返却
        /// </summary>
        /// <returns></returns>
        public List<object> getMovablePos()
	    {
		    return MovablePos[Turns];
	    }
	
        /// <summary>
        /// 履歴を得る
        /// </summary>
        /// <returns></returns>
        public List<object> getHistory()
	    {
            List<object> history = new List<object>();
		
		    for(int i=0; i<UpdateLog.Count; i++)
		    {
                List<object> update = UpdateLog[i];
			    if(update.Count == 0) continue; // パスは飛ばす
			    history.Add(update[0]);
		    }
		
		    return history;
	    }
	
        /// <summary>
        /// アップデートを得る
        /// </summary>
        /// <returns></returns>
        public List<object> getUpdate()
	    {
            if (UpdateLog.Count == 0) return new List<object>();
            else return UpdateLog[UpdateLog.Count - 1];
            //else return UpdateLog.lastElement();
	    }
	
	    public int getLiberty(Point p)
	    {
		    // 仮
		    return 0;
	    }
	
        /// <summary>
        /// 石を置けるかチェック
        /// </summary>
        /// <param name="disc"></param>
        /// <returns></returns>
	    private int checkMobility(Disc disc)
	    {
		    // 既に石があったら置けない
		    if(RawBoard[disc.x,disc.y] != Disc.EMPTY) return NONE;

		    int x, y;
		    int dir = NONE;

		    // 上
		    if(RawBoard[disc.x,disc.y-1] == -disc.color)
		    {
			    x = disc.x; y = disc.y-2;
			    while(RawBoard[x,y] == -disc.color) { y--; }
			    if(RawBoard[x,y] == disc.color) dir |= UPPER;
		    }

		    // 下
		    if(RawBoard[disc.x,disc.y+1] == -disc.color)
		    {
			    x = disc.x; y = disc.y+2;
			    while(RawBoard[x,y] == -disc.color) { y++; }
			    if(RawBoard[x,y] == disc.color) dir |= LOWER;
		    }

		    // 左
		    if(RawBoard[disc.x-1,disc.y] == -disc.color)
		    {
			    x = disc.x-2; y = disc.y;
			    while(RawBoard[x,y] == -disc.color) { x--; }
			    if(RawBoard[x,y] == disc.color) dir |= LEFT;
		    }

		    // 右
		    if(RawBoard[disc.x+1,disc.y] == -disc.color)
		    {
			    x = disc.x+2; y = disc.y;
			    while(RawBoard[x,y] == -disc.color) { x++; }
			    if(RawBoard[x,y] == disc.color) dir |= RIGHT;
		    }


		    // 右上
		    if(RawBoard[disc.x+1,disc.y-1] == -disc.color)
		    {
			    x = disc.x+2; y = disc.y-2;
			    while(RawBoard[x,y] == -disc.color) { x++; y--; }
			    if(RawBoard[x,y] == disc.color) dir |= UPPER_RIGHT;
		    }

		    // 左上
		    if(RawBoard[disc.x-1,disc.y-1] == -disc.color)
		    {
			    x = disc.x-2; y = disc.y-2;
			    while(RawBoard[x,y] == -disc.color) { x--; y--; }
			    if(RawBoard[x,y] == disc.color) dir |= UPPER_LEFT;
		    }

		    // 左下
		    if(RawBoard[disc.x-1,disc.y+1] == -disc.color)
		    {
			    x = disc.x-2; y = disc.y+2;
			    while(RawBoard[x,y] == -disc.color) { x--; y++; }
			    if(RawBoard[x,y] == disc.color) dir |= LOWER_LEFT;
		    }

		    // 右下
		    if(RawBoard[disc.x+1,disc.y+1] == -disc.color)
		    {
			    x = disc.x+2; y = disc.y+2;
			    while(RawBoard[x,y] == -disc.color) { x++; y++; }
			    if(RawBoard[x,y] == disc.color) dir |= LOWER_RIGHT;
		    }

		    return dir;
	    }
	
	    /// <summary>
        /// MovableDir及びMovablePosを初期化する
	    /// </summary>
	    private void initMovable()
	    {
		    Disc disc;
		    int dir;

		    MovablePos[Turns].Clear();

		    for(int x=1; x<=BOARD_SIZE; x++)
		    {
			    for(int y=1; y<=BOARD_SIZE; y++)
			    {
				    disc = new Disc(x, y, CurrentColor);
				    dir = checkMobility(disc);
				    if(dir != NONE)
				    {
					    // 置ける
					    MovablePos[Turns].Add(disc);
				    }
				    MovableDir[Turns,x,y] = dir;
			    }
		    }
	    }
	
        /// <summary>
        /// Discをひっくり返す処理
        /// </summary>
        /// <param name="point"></param>
	    private void flipDiscs(Point point)
	    {
		    int x, y;
		    int dir = MovableDir[Turns,point.x,point.y];

            List<object> update = new List<object>();

		    RawBoard[point.x,point.y] = CurrentColor;
		    update.Add(new Disc(point.x, point.y, CurrentColor));


		    // 上

		    if((dir & UPPER) != NONE) // 上に置ける
		    {
			    y = point.y;
			    while(RawBoard[point.x,--y] != CurrentColor)
			    {
				    RawBoard[point.x,y] = CurrentColor;
                    update.Add(new Disc(point.x, y, CurrentColor));
			    }
		    }


		    // 下

		    if((dir & LOWER) != NONE)
		    {
			    y = point.y;
			    while(RawBoard[point.x,++y] != CurrentColor)
			    {
				    RawBoard[point.x,y] = CurrentColor;
                    update.Add(new Disc(point.x, y, CurrentColor));
			    }
		    }

		    // 左

		    if((dir & LEFT) != NONE)
		    {
			    x = point.x;
			    while(RawBoard[--x,point.y] != CurrentColor)
			    {
				    RawBoard[x,point.y] = CurrentColor;
                    update.Add(new Disc(x, point.y, CurrentColor));
			    }
		    }

		    // 右

		    if((dir & RIGHT) != NONE)
		    {
			    x = point.x;
			    while(RawBoard[++x,point.y] != CurrentColor)
			    {
				    RawBoard[x,point.y] = CurrentColor;
                    update.Add(new Disc(x, point.y, CurrentColor));
			    }
		    }

		    // 右上

		    if((dir & UPPER_RIGHT) != NONE)
		    {
			    x = point.x;
			    y = point.y;
			    while(RawBoard[++x,--y] != CurrentColor)
			    {
				    RawBoard[x,y] = CurrentColor;
                    update.Add(new Disc(x, y, CurrentColor));
			    }
		    }

		    // 左上

		    if((dir & UPPER_LEFT) != NONE)
		    {
			    x = point.x;
			    y = point.y;
			    while(RawBoard[--x,--y] != CurrentColor)
			    {
				    RawBoard[x,y] = CurrentColor;
                    update.Add(new Disc(x, y, CurrentColor));
			    }
		    }

		    // 左下

		    if((dir & LOWER_LEFT) != NONE)
		    {
			    x = point.x;
			    y = point.y;
			    while(RawBoard[--x,++y] != CurrentColor)
			    {
				    RawBoard[x,y] = CurrentColor;
                    update.Add(new Disc(x, y, CurrentColor));
			    }
		    }

		    // 右下

		    if((dir & LOWER_RIGHT) != NONE)
		    {
			    x = point.x;
			    y = point.y;
			    while(RawBoard[++x,++y] != CurrentColor)
			    {
				    RawBoard[x,y] = CurrentColor;
                    update.Add(new Disc(x, y, CurrentColor));
			    }
		    }

		    // 石の数を更新

            int discdiff = update.Count;

		    Discs.set(CurrentColor, Discs.get(CurrentColor) + discdiff);
		    Discs.set(-CurrentColor, Discs.get(-CurrentColor) - (discdiff - 1));
		    Discs.set(Disc.EMPTY, Discs.get(Disc.EMPTY) - 1);
		
		    UpdateLog.Add(update);
	    }

    }

}
