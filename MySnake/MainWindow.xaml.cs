using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MySnake
{
    /// <summary>
    /// Creates main window
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Size of one grid field in pixels
        /// </summary>
        public static int SQUARE_SIZE = 10;
        /// <summary>
        /// Represents Snake moved by player
        /// </summary>
        public Snake Snake;
        /// <summary>
        /// Timer used to move Snake through the time
        /// </summary>
        private DispatcherTimer Timer;
        /// <summary>
        /// File name to save highscore
        /// </summary>
        private const string HighscoreFilename = @"highscore.txt";
        /// <summary>
        /// Dependency property of highscore
        /// </summary>
        public static readonly DependencyProperty HighscoreProperty = DependencyProperty.Register("Highscore", typeof(int), typeof(MainWindow));
        /// <summary>
        /// Stores Highest score
        /// </summary>
        private int Highscore
        {
            get { return (int)GetValue(HighscoreProperty); }
            set { SetValue(HighscoreProperty, value); }
        }
        public MainWindow()
        {
            
            InitializeComponent();
            border.Width = grid.Width + 2;
            border.Height = grid.Height + 2;

            InitBoard();
            NewGame();
            InitTimer();
            ReadHighscore();
        }

        /// <summary>
        /// Initializes new game
        /// </summary>
        private void NewGame()
        {
            grid.Children.Clear();
            InitSnake();
            InitApple();
            InitTexts();
        }
        /// <summary>
        /// Binds data with labels
        /// </summary>
        private void InitTexts()
        {
            ScoreLabel.DataContext = Snake;
            HighscoreLabel.DataContext = this;
        }
        /// <summary>
        /// Reads highscore from file
        /// </summary>
        private void ReadHighscore()
        {
            try
            {
                Highscore = File.ReadAllLines(HighscoreFilename).Select(scorline => int.Parse(scorline)).Max();
            }
            catch (FileNotFoundException)
            {
                File.Create(HighscoreFilename);
            }
            catch(InvalidOperationException)
            {
                Highscore = 0;
            }
        }
        /// <summary>
        /// Initiates timer moving player through the time
        /// </summary>
        private void InitTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(TickHandler);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
        }
        /// <summary>
        /// Moves Snake to next position and redraws it in window
        /// Called every timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TickHandler(object sender, EventArgs e)
        {
            if(!Snake.MoveSnake()) //if snake hits is tail
            {
                Timer.Stop();
                MessageBox.Show("You lost");
                CheckScore();
                NewGame();
                //this.Close();
            }
            Snake.RedrawSnake();
        }
        /// <summary>
        /// Checks if player reached high score
        /// </summary>
        private void CheckScore()
        {
            if (Snake.Score > Highscore)
            {
                MessageBox.Show("New Highscore!");
                Highscore = Snake.Score;
                SaveScore();
            }
        }
        /// <summary>
        /// Saves highscore to file
        /// </summary>
        private void SaveScore()
        {
            using (FileStream fs = new FileStream(HighscoreFilename, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(Highscore);
                }
            }
        }
        /// <summary>
        /// Initiates board that displays all game elements
        /// </summary>
        public void InitBoard()
        {
            for(int i=0; i<grid.Width/SQUARE_SIZE; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                grid.ColumnDefinitions.Add(columnDefinition);
            }
            for (int i = 0; i < grid.Height / SQUARE_SIZE; i++)
            {
                RowDefinition RowDefinition = new RowDefinition();
                grid.RowDefinitions.Add(RowDefinition);
            }
        }
        /// <summary>
        /// Initiates Snake moved by player
        /// </summary>
        public void InitSnake()
        {
            Snake.grid = grid;
            Snake = new Snake();
            Snake.GridXSize = (int)grid.Width/SQUARE_SIZE;
            Snake.GridYSize = (int)grid.Height/SQUARE_SIZE;
            grid.Children.Add(Snake.Head.Rect);
            foreach (var piece in Snake.Tail)
            {
                grid.Children.Add(piece.Rect);
            }
            Snake.RedrawSnake();
        }
        /// <summary>
        /// Initiates Apples
        /// </summary>
        public void InitApple()
        {
            Apple.Snake = Snake;
            Apple.MainWindow = this;
            for(int i=0;i<2;i++)
            {
                Apple apple = new Apple();
                grid.Children.Add(apple.Piece.Rect);
                apple.RedrawApple();
                Snake.apples.Add(apple);
            }
        }
        /// <summary>
        /// Handles keyboard key press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
            Snake.ChangeKey(e.Key);
            TickHandler(null, null); //call next tick right after key press so game won't miss any key press
                                     //makes game more playable

            Timer.Stop();            //restart timer so snake won't jump
            Timer.Start();           //by any frame
        }
        /// <summary>
        /// Calls MessageBos.Show() in main window
        /// </summary>
        /// <param name="s">Message</param>
        public static void ShowMessage(String s)
        {
            MessageBox.Show(s);
        }
    }
}
