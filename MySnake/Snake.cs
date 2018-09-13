using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MySnake
{
    public class Snake :DependencyObject
    {
        /// <summary>
        /// Grid that represents board
        /// </summary>
        public static Grid grid;
        /// <summary>
        /// Snake's Head
        /// </summary>
        public Piece Head { get; set; }
        /// <summary>
        /// List of Snake's tail pieces
        /// </summary>
        public List<Piece> Tail { get; set; }
        /// <summary>
        /// Number to add to snake's X coordinate every tick
        /// </summary>
        public int DimentionX { get; set; } = 1;
        /// <summary>
        /// Number to add to snake's Y coordinate  every tick
        /// </summary>
        public int DimentionY { get; set; } = 0;
        /// <summary>
        /// Stores last accepted key press by user
        /// </summary>
        private Key LastUsedKey = Key.Right;
        /// <summary>
        /// Stores next snake's move direction
        /// </summary>
        private Key NextKey = Key.Right;
        /// <summary>
        /// Board width in fields
        /// </summary>
        public int GridXSize { get; set; }
        /// <summary>
        /// Board height in fields
        /// </summary>
        public int GridYSize { get; set; }
        /// <summary>
        /// Stores Apples displayed on the board
        /// </summary>
        public List<Apple> apples;
        /// <summary>
        /// Number of pieces left to be added to snake's tail
        /// </summary>
        public int SnakeRest = 0;

        /// <summary>
        /// Dependency property of player Score
        /// </summary>
        public static readonly DependencyProperty ScoreProperty = DependencyProperty.Register("Score", typeof(int), typeof(Snake));
        /// <summary>
        /// Stores current score
        /// </summary>
        public int Score
        {
            get { return (int)GetValue(ScoreProperty); }
            set { SetValue(ScoreProperty,value); }
        }
        

        public Snake()
        {
            Score = 0;
            Head = new Piece(10, 0, Brushes.Red);
            Head.Rect.Width = Head.Rect.Height = 10;
            Grid.SetColumn(Head.Rect, Head.X);
            Tail = new List<Piece>();
            for(int i=9; i>=5; i--) //creates tail at start
            {
                Piece piece = new Piece(i, 0, Brushes.Green);
                Grid.SetColumn(piece.Rect, i);
                Grid.SetRow(piece.Rect, 0);
                Tail.Add(piece);
            }
            apples = new List<Apple>();
        }
        /// <summary>
        /// Changes next snake's move direction if it's not opposite direction
        /// </summary>
        /// <param name="key">Key pressed by player</param>
        public void ChangeKey(Key key)
        {
            switch (key)
            {
                case Key.Right:
                    if (LastUsedKey == Key.Right || LastUsedKey == Key.Left) return;
                    break;
                case Key.Left:
                    if (LastUsedKey == Key.Left || LastUsedKey == Key.Right) return;
                    break;
                case Key.Up:
                    if (LastUsedKey == Key.Up || LastUsedKey == Key.Down) return;
                    break;
                case Key.Down:
                    if (LastUsedKey == Key.Down || LastUsedKey == Key.Up) return;
                    break;
            }
            NextKey = key;
        }
        /// <summary>
        /// Changes values to add to snake's coordinates every move
        /// </summary>
        private void ChangeDimention()
        {
            switch(NextKey)
            {
                case Key.Right:
                    DimentionX = 1;
                    DimentionY = 0;
                    break;
                case Key.Left:
                    DimentionX = -1;
                    DimentionY = 0;
                    break;
                case Key.Up:
                    DimentionX = 0;
                    DimentionY = -1;
                    break;
                case Key.Down:
                    DimentionX = 0;
                    DimentionY = 1;
                    break;
            }
            LastUsedKey = NextKey;
        }
        /// <summary>
        /// Moves snake once
        /// </summary>
        /// <returns>True if there was no collision, false if there was</returns>
        public bool MoveSnake()
        {
            ChangeDimention(); //prepare snake to move in right direction
            if(SnakeRest > 0) //add tail part to snake if there is any left
            {
                Piece piece = new Piece(Tail[Tail.Count - 1].X, Tail[Tail.Count - 1].Y, Brushes.Green);
                Tail.Add(piece);
                grid.Children.Add(piece.Rect);
                SnakeRest--;
            }
            for(int i=Tail.Count-1;i>0;i--)//move tail parts
            {
                Tail[i].X = Tail[i - 1].X;
                Tail[i].Y = Tail[i - 1].Y;
            }
            Tail[0].X = Head.X; //move part before head
            Tail[0].Y = Head.Y; //
            Head.X += DimentionX; //move head
            Head.Y += DimentionY; //
            if (Head.X >= GridXSize || Head.Y >= GridYSize || Head.X < 0 || Head.Y < 0) return false; //if snake hits the wall end the game
                //if (Head.X >= GridXSize) Head.X = 0;    //teleport snake to the opposite side if he moved out of the board
                //if (Head.Y >= GridYSize) Head.Y = 0;    //
                //if (Head.X < 0) Head.X = GridXSize - 1; //
                //if (Head.Y < 0) Head.Y = GridYSize - 1; //
                if (TryToEatApple()) EatAppleHandle(); //eat apple if you can
            if (!CheckPosition(Head.X, Head.Y)) return false; //check if snake hit his tail with head
            return true;
        }
        /// <summary>
        /// Handles apple consume by snake
        /// </summary>
        public void EatAppleHandle()
        {
            SnakeRest += 30; //extend snake's tail length
            Score++;
        }
        /// <summary>
        /// Redraws snake on the board
        /// </summary>
        public void RedrawSnake()
        {
            Grid.SetColumn(Head.Rect, Head.X);
            Grid.SetRow(Head.Rect, Head.Y);
            foreach (var piece in Tail)
            {
                Grid.SetColumn(piece.Rect, piece.X);
                Grid.SetRow(piece.Rect, piece.Y);
            }
        }
        /// <summary>
        /// Checks if snake can eat an apple
        /// </summary>
        /// <returns>true if can eat it, false if not</returns>
        public bool TryToEatApple()
        {
            foreach(var apple in apples)
            {
                if(apple.Piece.X == Head.X && apple.Piece.Y == Head.Y)
                {
                    apple.ChangePosition();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if position is tail free
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        private bool CheckPosition(int x, int y)
        {
            foreach (var piece in Tail)
            {
                if (x == piece.X && y == piece.Y) return false;
            }
            return true;
        }
    }
    
}
