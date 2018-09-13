using System.Windows.Media;
using System.Windows.Shapes;

namespace MySnake
{
    /// <summary>
    /// Represents object that can be displayed on the board
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// X coordinate of object
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y coordinate of object
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Rectangle to be displayed on the board
        /// </summary>
        public Rectangle Rect { get; set; }

        public Piece(Brush color)
        {
            Rect = new Rectangle();
            Rect.Fill = color;
            Rect.Width = Rect.Height = MainWindow.SQUARE_SIZE - 1;
        }

        public Piece(int x, int y, Brush color)
        {
            X = x;
            Y = y;
            Rect = new Rectangle();
            Rect.Fill = color;
            Rect.Width = Rect.Height = MainWindow.SQUARE_SIZE - 1;
        }
    }
}
