using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{

    /// <summary>
    /// AI
    /// </summary>
    abstract class AI
    {
        abstract public void move(Board board);

        // 事前読みの深さ
        public int presearch_depth = 3;

        // 通常の読み込み深さ
        public int normal_depth = 5;

        public int wld_depth = 15;
        public int perfect_depth = 13;

    }

    /// <summary>
    /// α-β法によるAI
    /// </summary>
    class AlphaBetaAI : AI
    {
        // テストバリュー ----------
        int MAX_VALUE = 1000000000;
        int MIN_VALUE = -1000000000;
        // -------------------------

        /// <summary>
        /// 動く
        /// </summary>
        class Move : Point
        {
            // 評価
            public int eval = 0;
            public Move() : base(0, 0) { }

            public Move(int x, int y, int e)
                : base(x, y)
            {
                eval = e;
            }
        };

        // 評価関数
        private Evaluator Eval = null;

        /// <summary>
        /// ボード情報を渡して、最適な手を探索
        /// </summary>
        /// <param name="board"></param>
        public override void move(Board board)
        {
            BookManager book = new BookManager();
            List<object> movables = book.find(board);

            if (movables.Count == 0)
            {
                // 打てる箇所がなければパスする
                board.pass();
                return;
            }

            if (movables.Count == 1)
            {
                // 打てる箇所が一カ所だけなら探索は行わず、即座に打って返る
                board.move((Point)movables[0]);
                return;
            }

            int limit;
            Eval = new MidEvaluator();
            sort(board, movables, presearch_depth); // 事前に手を良さそうな順にソート

            if (Board.MAX_TURNS - board.getTurns() <= wld_depth)
            {
                //limit = Integer.MAX_VALUE;
                limit = MAX_VALUE;
                if (Board.MAX_TURNS - board.getTurns() <= perfect_depth)
                {
                    Eval = new PerfectEvaluator();
                }
                else
                {
                    Eval = new WLDEvaluator();
                }
            }
            else
            {
                limit = normal_depth;
            }

            //int eval, eval_max = Integer.MIN_VALUE;
            int eval, eval_max = MIN_VALUE;

            // 着手場所を決定
            Point p = null;
            for (int i = 0; i < movables.Count; i++)
            {
                board.move((Point)movables[i]);
                //eval = -alphabeta(board, limit - 1, -Integer.MAX_VALUE, -Integer.MIN_VALUE);

                // アルファベータ評価がうまくいってない
                eval = -alphabeta(board, limit - 1, - MAX_VALUE, - MIN_VALUE);
                board.undo();

                if (eval > eval_max)
                {
                    p = (Point)movables[i];
                }
            }

            board.move(p);

        }

        /// <summary>
        /// α-β法
        /// </summary>
        /// <param name="board"></param>
        /// <param name="limit"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int alphabeta(Board board, int limit, int alpha, int beta)
        {
            // 深さ制限に達したら評価値を返す
            if (board.isGameOver() || limit == 0)
            {
                return evaluate(board);
            }

            List<object> pos = board.getMovablePos();
            int eval;

            if (pos.Count == 0)
            {
                // パス
                board.pass();
                eval = -alphabeta(board, limit, -beta, -alpha);
                board.undo();
                return eval;
            }

            for (int i = 0; i < pos.Count; i++)
            {
                board.move((Point)pos[i]);
                eval = -alphabeta(board, limit - 1, -beta, -alpha);
                board.undo();

                alpha = Math.Max(alpha, eval);

                if (alpha >= beta)
                {
                    // β刈り
                    return alpha;
                }
            }

            return alpha;

        }

        /// <summary>
        /// 並び替え
        /// </summary>
        /// <param name="board"></param>
        /// <param name="movables"></param>
        /// <param name="limit"></param>
        private void sort(Board board, List<object> movables, int limit)
        {
            List<object> moves = new List<object>();

            for (int i = 0; i < movables.Count; i++)
            {
                int eval;
                Point p = (Point)movables[i];

                board.move(p);
                //eval = -alphabeta(board, limit - 1, -Integer.MAX_VALUE, Integer.MAX_VALUE);
                eval = -alphabeta(board, limit - 1, - MAX_VALUE, MAX_VALUE);
                board.undo();

                Move move = new Move(p.x, p.y, eval);
                moves.Add(move);
            }

            // 評価値の大きい順にソート(選択ソート)

            int begin, current;
            for (begin = 0; begin < moves.Count - 1; begin++)
            {
                for (current = 1; current < moves.Count; current++)
                {
                    Move b = (Move)moves[begin];
                    Move c = (Move)moves[current];
                    if (b.eval < c.eval)
                    {
                        // 交換
                        //moves.set(begin, c);
                        //moves.set(current, b);
                        moves[begin] = c;
                        moves[current] = b;
                    }
                }
            }
            // 結果の書き戻し

            movables.Clear();
            for (int i = 0; i < moves.Count; i++)
            {
                movables.Add(moves[i]);
            }

            return;

        }

        /// <summary>
        /// 評価を行う
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private int evaluate(Board board)
        {
            return 0;
        }
    }


}
