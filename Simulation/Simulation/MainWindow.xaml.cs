using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Simulation
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region メンバ
        // ボード
        Board board;

        // 一マスのサイズ
        private int oneGridSize = 40;

        // AI
        private AIPlayer aiPlayer;

        // タイマー
        DispatcherTimer dispatcherTimer;

        // プレイヤーが操作可能
        bool canMoveByPlayer;
        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// ボードを作る
        /// </summary>
        private void CreateBoard()
        {
            // キャンバス要素をクリア
            Canvas1.Children.Clear();

            // キャンバスサイズ
            Canvas1.Width = Canvas1.Height = oneGridSize * (Board.BOARD_SIZE + 2);

            // 線
            Line line1;
            for (int i = 0; i < (Board.BOARD_SIZE+3); i++)
            {
                line1 = new Line();
                line1.X1 = 0;
                line1.X2 = Canvas1.Width;
                line1.Y1 = i * oneGridSize;
                line1.Y2 = i * oneGridSize;
                line1.HorizontalAlignment = HorizontalAlignment.Center;
                line1.VerticalAlignment = VerticalAlignment.Center;
                line1.Stroke = Brushes.Black;
                line1.StrokeThickness = 2;
                Canvas1.Children.Add(line1);

                line1 = new Line();
                line1.X1 = i * oneGridSize;
                line1.X2 = i * oneGridSize;
                line1.Y1 = 0;
                line1.Y2 = Canvas1.Height;
                line1.HorizontalAlignment = HorizontalAlignment.Left;
                line1.VerticalAlignment = VerticalAlignment.Top;
                line1.Stroke = Brushes.Black;
                line1.StrokeThickness = 2;
                Canvas1.Children.Add(line1);
            }
        }

        /// <summary>
        /// Discを描画する
        /// </summary>
        private void SetDisc()
        {
            // 正直、壁は再描画の必要ないんだけど

            // クリア
            Canvas1.Children.Clear();

            // ループしながら、配置していく
            Rectangle myRect;
            for (int i = 0; i < (Board.BOARD_SIZE + 2); i++)
            {
                for (int j = 0; j < (Board.BOARD_SIZE + 2); j++)
                {
                    switch(board.getColor(new Point(i, j)))
                    {
                        case -1:
                            // 黒色
                            myRect = new System.Windows.Shapes.Rectangle();
                            myRect.Stroke = System.Windows.Media.Brushes.Black;
                            myRect.Fill = System.Windows.Media.Brushes.White;
                            myRect.Height = oneGridSize;
                            myRect.Width = oneGridSize;
                            Canvas1.Children.Add(myRect);
                            Canvas.SetLeft(myRect, i * oneGridSize);
                            Canvas.SetTop(myRect, j * oneGridSize);
                            break;
                        case 0:
                            // 何もしない
                            break;
                        case 1:
                            // 白色
                            myRect = new System.Windows.Shapes.Rectangle();
                            myRect.Stroke = System.Windows.Media.Brushes.Black;
                            myRect.Fill = System.Windows.Media.Brushes.Black;
                            myRect.Height = oneGridSize;
                            myRect.Width = oneGridSize;
                            Canvas1.Children.Add(myRect);
                            Canvas.SetLeft(myRect, i * oneGridSize);
                            Canvas.SetTop(myRect, j * oneGridSize);
                            break;
                        case 2:
                            // 壁
                            myRect = new System.Windows.Shapes.Rectangle();
                            myRect.Stroke = System.Windows.Media.Brushes.Black;
                            myRect.Fill = System.Windows.Media.Brushes.Yellow;
                            myRect.Height = oneGridSize;
                            myRect.Width = oneGridSize;
                            Canvas1.Children.Add(myRect);
                            Canvas.SetLeft(myRect, i * oneGridSize);
                            Canvas.SetTop(myRect, j * oneGridSize);
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 開始ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickStartButton(object sender, RoutedEventArgs e)
        {

            // ボードの状態を格納するオブジェクトを生成
            board = new Board();

            // ボードを描く
            CreateBoard();

            // ディスクを描く
            SetDisc();

            // AIを起動
            aiPlayer = new AIPlayer();

            // 移動可能
            canMoveByPlayer = true;

        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickUndoButton(object sender, RoutedEventArgs e)
        {
            if (board == null)
            {
                MessageBox.Show("開始ボタンをおしてください", "失敗");
                return;
            }


            if (board.undo() == false)
            {
                MessageBox.Show("戻れません", "失敗");
                return;
            }

            // ディスクを描く
            SetDisc();
        }

        #region イベント
        /// <summary>
        /// 左クリック処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas1_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {

            // マウス位置を取得
            var mousePosition = e.GetPosition(Canvas1);

            // インデックスを計算
            int xIndex = (int)(mousePosition.X / oneGridSize);
            int yIndex = (int)(mousePosition.Y / oneGridSize);

            Point point = new Point(xIndex, yIndex);
            if(board.move(point) == false)
            {
                MessageBox.Show("その場所にはおけません", "失敗");
                return;

            }

            // ボードにセット
            board.move(new Point(xIndex, yIndex));

            // 色を描画
            SetDisc();

            // 終了かの判定
            if (board.isGameOver())
            {
                board.countDisc(Disc.BLACK);
                StringBuilder sb1 = new StringBuilder();
                sb1.Append("黒 : ");
                sb1.Append(Disc.BLACK);
                sb1.Append("\n");
                sb1.Append("白");
                sb1.Append(Disc.WHITE);
                sb1.Append("\n");
                if (Disc.BLACK == Disc.WHITE)
                {
                    sb1.Append("引き分け");
                }
                else if (Disc.BLACK < Disc.WHITE)
                {
                    sb1.Append("白の勝ち");
                }
                else
                {
                    sb1.Append("黒の勝ち");
                }

                MessageBox.Show(String.Format("黒 : {0}\n白 : {1}", Disc.BLACK, Disc.WHITE), "ゲーム終了");
            }

            // テスト ------
            // AIの番
            aiPlayer.onTurn(board);

            // タイマースタート
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
            // -------------

        }

        /// <summary>
        /// タイマーイベントを処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            // 色を描画
            SetDisc();

            dispatcherTimer.Stop();

            // 終了かの判定
            if (board.isGameOver())
            {
                board.countDisc(Disc.BLACK);
                StringBuilder sb1 = new StringBuilder();
                sb1.Append("黒 : ");
                sb1.Append(Disc.BLACK);
                sb1.Append("\n");
                sb1.Append("白");
                sb1.Append(Disc.WHITE);
                sb1.Append("\n");
                if (Disc.BLACK == Disc.WHITE)
                {
                    sb1.Append("引き分け");
                }
                else if (Disc.BLACK < Disc.WHITE)
                {
                    sb1.Append("白の勝ち");
                }
                else
                {
                    sb1.Append("黒の勝ち");
                }

                MessageBox.Show(String.Format("黒 : {0}\n白 : {1}", Disc.BLACK, Disc.WHITE), "ゲーム終了");
            }
        }
        #endregion

        /// <summary>
        /// パス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPassButton(object sender, RoutedEventArgs e)
        {
            if (board == null)
            {
                MessageBox.Show("開始ボタンをおしてください", "失敗");
                return;
            }

            if (board.pass() == false)
            {
                MessageBox.Show("パスできません", "失敗");
                return;
            }

            // AIにもう一回動いてもらう
            aiPlayer.onTurn(board);

            // 色を描画
            SetDisc();


            // 終了かの判定
            if (board.isGameOver())
            {
                board.countDisc(Disc.BLACK);
                StringBuilder sb1 = new StringBuilder();
                sb1.Append("黒 : ");
                sb1.Append(Disc.BLACK);
                sb1.Append("\n");
                sb1.Append("白");
                sb1.Append(Disc.WHITE);
                sb1.Append("\n");
                if (Disc.BLACK == Disc.WHITE)
                {
                    sb1.Append("引き分け");
                }
                else if (Disc.BLACK < Disc.WHITE)
                {
                    sb1.Append("白の勝ち");
                }
                else
                {
                    sb1.Append("黒の勝ち");
                }

                MessageBox.Show(String.Format("黒 : {0}\n白 : {1}", Disc.BLACK, Disc.WHITE), "ゲーム終了");
            }


        }

        /// <summary>
        /// 終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
