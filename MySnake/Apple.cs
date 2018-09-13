using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace MySnake
{
    /// <summary>
    /// Represents pple eatable by snake
    /// </summary>
    public class Apple
    {
        /// <summary>
        /// Object that represents Apple on the board
        /// </summary>
        public Piece Piece;
        /// <summary>
        /// Snake moved by player
        /// Provides informations about it's coordinates
        /// </summary>
        public static Snake Snake;
        /// <summary>
        /// Main game window
        /// </summary>
        public static MainWindow MainWindow;
        /// <summary>
        /// Number of tries to find correct apple coordinates
        /// </summary>
        private int RandsCount = 0;
        /// <summary>
        /// Maximal number of tries to find correct apple coordinates
        /// </summary>
        private int MaxRandsCount = 10000;
        public Apple()
        {
            Piece = new Piece(Brushes.Blue);
            ChangePosition();
        }
        /// <summary>
        /// Changes Apple to new random position
        /// </summary>
        public void ChangePosition()
        {
            RandsCount = 0;
            Tuple<int, int> xy = RandPosition();
            Piece.X = xy.Item1;
            Piece.Y = xy.Item2;
            Grid.SetColumn(Piece.Rect, Piece.X);
            Grid.SetRow(Piece.Rect, Piece.Y);
        }
        /// <summary>
        /// Randomizes coordinates not taken by any object
        /// </summary>
        /// <returns>x,y</returns>
        private Tuple<int,int> RandPosition()
        {
            RandsCount++;
            if(RandsCount > MaxRandsCount) //if program fails too many times to find right apple position
            {
                MainWindow.ShowMessage("You win");
                MainWindow.Close();
                return null;
            }
            Random rand = new Random();
            int x = rand.Next(0, Snake.GridXSize);
            int y = rand.Next(0, Snake.GridYSize);
            if (!CheckPosition(x, y)) return RandPosition();
            else return Tuple.Create(x, y);
                
        }
        /// <summary>
        /// Checks if coordinates are taken by another object
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>true if position is free, false if taken</returns>
        private bool CheckPosition(int x, int y)
        {
            foreach(var piece in Snake.Tail)
            {
                if (x == piece.X && y == piece.Y) return false;
            }
            if (x == Snake.Head.X && y == Snake.Head.Y) return false;
            foreach(var apple in Snake.apples)
            {
                if (x == apple.Piece.X && y == apple.Piece.Y) return false;
            }
            return true;
        }
        /// <summary>
        /// Redraws apple on the board
        /// </summary>
        public void RedrawApple()
        {
            Grid.SetColumn(Piece.Rect, Piece.X);
            Grid.SetRow(Piece.Rect, Piece.Y);
        }
    }

}
