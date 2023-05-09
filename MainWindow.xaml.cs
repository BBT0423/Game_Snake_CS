using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Dictionary<Gridevalue, ImageSource> gridValToImage = new Dictionary<Gridevalue, ImageSource>()
        {
            { Gridevalue.Empty, images.Empty },
            { Gridevalue.Snake, images.Body },
            { Gridevalue.Food, images.Food },
        };
        private readonly Dictionary<Directon, int> dirToRotation = new Dictionary<Directon, int>()
        {
            {Directon.Up,0}, {Directon.Right, 90}, {Directon.Down,180} , {Directon.Left,270}
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private gamestate gamestate;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gamestate = new gamestate(rows, cols);
        }

        private async Task Rungame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gamestate = new gamestate(rows,cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await Rungame();
                gameRunning = false;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gamestate.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gamestate.ChangeDirection(Directon.Left);
                    break;
                case Key.Right:
                    gamestate.ChangeDirection(Directon.Right);
                    break;
                case Key.Up:
                    gamestate.ChangeDirection(Directon.Up);
                    break;
                case Key.Down:
                    gamestate.ChangeDirection(Directon.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gamestate.GameOver)
            {
                await Task.Delay(100);
                gamestate.Move();
                Draw();
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] image = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols/(double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image1 = new Image
                    {
                        Source = images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                    };

                    image[r, c] = image1;
                    GameGrid.Children.Add(image1);
                }
            }
            return image;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gamestate.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Gridevalue gridval = gamestate.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridval];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gamestate.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Column];
            image.Source = images.Head;

            int rotation = dirToRotation[gamestate.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDaedSnake()
        {
            List<Position> positions = new List<Position>(gamestate.SnakePositions());

            for(int i=0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i==0) ? images.DeadHead : images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                await Task.Delay(50);
            }
        }
        private async Task ShowCountDown()
        {
            for (int i = 3; i > 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDaedSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press Any Key To Start";
        }
    }
}
