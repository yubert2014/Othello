using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class CoordinatesTransformer
    {
        private int Rotate = 0;
        private bool Mirror = false;

        public CoordinatesTransformer(Point first)
        {
            //if(first.equals(new Point("d3")))
            if (first.equals(new Point(4, 3)))
            {
                Rotate = 1;
                Mirror = true;
            }
            //else if(first.equals(new Point("c4")))
            else if (first.equals(new Point(3, 4)))
            {
                Rotate = 2;
            }
            //else if(first.equals(new Point("e6")))
            else if (first.equals(new Point(5, 6)))
            {
                Rotate = -1;
                Mirror = true;
            }

        }

        // 座標をf5を開始点とする座標系に正規化する
        public Point normalize(Point p)
        {
            Point newp = rotatePoint(p, Rotate);
            if (Mirror) newp = mirrorPoint(newp);

            return newp;
        }

        // f5を開始点とする座標を本来の座標に戻す
        public Point denormalize(Point p)
        {
            Point newp = new Point(p.x, p.y);
            if (Mirror) newp = mirrorPoint(newp);

            newp = rotatePoint(newp, -Rotate);

            return newp;
        }

        private Point rotatePoint(Point old_point, int rotate)
        {
            rotate %= 4;
            if (rotate < 0) rotate += 4;

            Point new_point = new Point();

            switch (rotate)
            {
                case 1:
                    new_point.x = old_point.y;
                    new_point.y = Board.BOARD_SIZE - old_point.x + 1;
                    break;
                case 2:
                    new_point.x = Board.BOARD_SIZE - old_point.x + 1;
                    new_point.y = Board.BOARD_SIZE - old_point.y + 1;
                    break;
                case 3:
                    new_point.x = Board.BOARD_SIZE - old_point.y + 1;
                    new_point.y = old_point.x;
                    break;
                default: // 0
                    new_point.x = old_point.x;
                    new_point.y = old_point.y;
                    break;
            }

            return new_point;
        }

        private Point mirrorPoint(Point point)
        {
            Point new_point = new Point();
            new_point.x = Board.BOARD_SIZE - point.x + 1;
            new_point.y = point.y;

            return new_point;
        }
    }

    /// <summary>
    /// 定石の読み込み
    /// </summary>
    class BookManager
    {
        // 読み込むファイル名
        private static readonly String BOOK_FILE_NAME = "reversi.book";

        class Node
        {
            public Node child = null;
            public Node sibling = null;
            // public int eval = 0;
            public Point point = new Point();
        }

        private Node Root = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookManager()
        {
            Root = new Node();
            //Root.point = new Point("f5");
            Root.point = new Point(6, 5);

            //FileInputStream fis = null;
            //try
            //{
            //    fis = new FileInputStream(BOOK_FILE_NAME);
            //}
            //catch (FileNotFoundException e)
            //{
            //    return;
            //}

            //BufferedReader br = new BufferedReader(new InputStreamReader(fis));

            //String line;
            //try
            //{
            //    while ((line = br.readLine()) != null)
            //    {
            //        List<object> book = new List<object>();
            //        for (int i = 0; i < line.Length; i += 2)
            //        {
            //            Point p = null;
            //            try
            //            {
            //                p = new Point(line.Substring(i));
            //            }
            //            catch (IllegalArgumentException e)
            //            {
            //                break;
            //            }

            //            book.Add(p);
            //        }

            //        add(book);
            //    }
            //}
            //catch (Exception e)
            //{ }
        }

        public List<object> find(Board board)
        {
            Node node = Root;
            List<object> history = board.getHistory();

            if (history.Count == 0)
            {
                return board.getMovablePos();
            }

            Point first = (Point)history[0];
            CoordinatesTransformer transformer = new CoordinatesTransformer(first);

            // 座標を変換してf5から始まるようにする
            List<Point> normalized = new List<Point>();
            for (int i = 0; i < history.Count; i++)
            {
                Point p = (Point)history[i];
                p = transformer.normalize(p);

                normalized.Add(p);
            }


            // 現在までの棋譜リストと定石の対応を取る
            for (int i = 1; i < normalized.Count; i++)
            {
                Point p = (Point)normalized[i];

                node = node.child;
                while (node != null)
                {
                    if (node.point.equals(p)) break;

                    node = node.sibling;
                }
                if (node == null)
                {
                    // 定石を外れている
                    return board.getMovablePos();
                }
            }

            // 履歴と定石の終わりが一致していた場合
            if (node.child == null) return board.getMovablePos();

            Point next_move = getNextMove(node);

            // 座標を元の形に変換する
            next_move = transformer.denormalize(next_move);

            //Vector v = new Vector();
            List<object> v = new List<object>();
            v.Add(next_move);

            return v;

        }

        private Point getNextMove(Node node)
        {
            //Vector candidates = new Vector();
            List<object> candidates = new List<object>();

            for (Node p = node.child; p != null; p = p.sibling)
            {
                candidates.Add(p.point);
            }

            //int index = (int)(Math.random() * candidates.Count);
            Random random = new Random();
            int index = (int)(random.NextDouble() * candidates.Count);
            Point point = (Point)candidates[index];

            return new Point(point.x, point.y);
        }

        //private void add(Vector book)
        private void add(List<object> book)
        {
            Node node = Root;

            for (int i = 1; i < book.Count; i++)
            {
                Point p = (Point)book[i];

                if (node.child == null)
                {
                    // 新しい定石手
                    node.child = new Node();
                    node = node.child;
                    node.point.x = p.x;
                    node.point.y = p.y;
                }
                else
                {
                    // 兄弟ノードの探索に移る
                    node = node.child;

                    while (true)
                    {
                        // 既にこの手はデータベース中にあり、その枝を見つけた
                        if (node.point.equals(p)) break;

                        // 定石木の新しい枝
                        if (node.sibling == null)
                        {
                            node.sibling = new Node();

                            node = node.sibling;
                            node.point.x = p.x;
                            node.point.y = p.y;
                            break;
                        }

                        node = node.sibling;
                    }
                }
            }
        }


    }

}
