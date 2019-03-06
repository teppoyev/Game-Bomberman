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

namespace Game_Bomberman
{
    /// <summary>
    /// Логика взаимодействия для Battle11.xaml
    /// </summary>
    public partial class Battle11 : Page
    {
        public const double speed = 5.0;
        public const int numberOfDestroyableBlocks = 120;
        private int numberOfEnemies = 3;
        public int widthOfFieldOfBattle = 25;
        public int heightOfFieldOfBattle = 13;
        public int numberOfUnDestroyableBlocks;
        private DispatcherTimer timer, timer1;
        private bool movingDown = false, movingUp = false, movingRight = false, movingLeft = false;
        private int positionOfPlayerInCanvas;
        private double workWidth, workHeight;
        public bool[][] fieldOfBattle;

        public Battle11()
        {
            InitializeComponent();
            Width = MainWindow.width; Height = MainWindow.height;
            numberOfUnDestroyableBlocks = ((widthOfFieldOfBattle - 1) / 2) * ((heightOfFieldOfBattle - 1) / 2);

            var player = new Game_Logic.Player();
            Canvas.SetTop(player.Body, player.Y);
            Canvas.SetLeft(player.Body, player.X);
            player.Body.KeyDown += Player_KeyDown;
            player.Body.KeyUp += Player_KeyUp;
            grid1.Children.Add(player.Body);
            positionOfPlayerInCanvas = grid1.Children.Count - 1;

            fieldOfBattle = new bool[widthOfFieldOfBattle][];
            for (int i = 0; i < widthOfFieldOfBattle; ++i)
            {
                fieldOfBattle[i] = new bool[heightOfFieldOfBattle];
                for (int j = 0; j < heightOfFieldOfBattle; ++j)
                {
                    fieldOfBattle[i][j] = false;
                }
            }

            for (int i = 0; i < (widthOfFieldOfBattle - 1) / 2; ++i)
            {
                for (int j = 0; j < (heightOfFieldOfBattle - 1) / 2; ++j)
                {
                    var block = new Game_Logic.Stone();
                    block.X = block.Size * (i * 2 + 1); block.Y = block.Size * (j * 2 + 1);
                    Canvas.SetLeft(block.Body, block.X); Canvas.SetTop(block.Body, block.Y);
                    fieldOfBattle[i * 2 + 1][j * 2 + 1] = true;
                    grid1.Children.Add(block.Body);
                }
            }

            for (int i = 0; i < numberOfDestroyableBlocks; ++i)
            {
                Random rnd = new Random();
                int x0 , y0;
                do
                {
                    x0 = rnd.Next(widthOfFieldOfBattle);
                    y0 = rnd.Next(heightOfFieldOfBattle);
                } while (fieldOfBattle[x0][y0] || (x0 == 0 && y0 == 0) || (x0 == 1 && y0 == 0) || (x0 == 0 && y0 == 1));
                var block = new Game_Logic.Leaves();
                block.X = block.Size * x0; block.Y = block.Size * y0;
                Canvas.SetLeft(block.Body, block.X); Canvas.SetTop(block.Body, block.Y);
                fieldOfBattle[x0][y0] = true;
                grid1.Children.Add(block.Body);
            }

            for (int i = 0; i < numberOfEnemies; ++i)
            {
                Random rnd = new Random();
                int x0, y0;
                do
                {
                    x0 = rnd.Next(widthOfFieldOfBattle);
                    y0 = rnd.Next(heightOfFieldOfBattle);
                } while (fieldOfBattle[x0][y0] || (x0 < widthOfFieldOfBattle / 2 && y0 < heightOfFieldOfBattle / 2));
                var mob = new Game_Logic.Gabaijito();
                mob.X = mob.Size * x0; mob.Y = mob.Size * y0;
                Canvas.SetLeft(mob.Body, mob.X); Canvas.SetTop(mob.Body, mob.Y);
                fieldOfBattle[x0][y0] = true;
                grid1.Children.Add(mob.Body);
            }

            grid1.Children[positionOfPlayerInCanvas].Focus();

            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void grid0_Loaded(object sender, EventArgs e)
        {
            workWidth = grid0.ColumnDefinitions[1].ActualWidth; workHeight = grid0.RowDefinitions[1].ActualHeight;
        }

        private void Player_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Key == Key.Down)
                {
                    movingDown = true;
                }
                else if (e.Key == Key.Left)
                {
                    movingLeft = true;
                }
                else if (e.Key == Key.Up)
                {
                    movingUp = true;
                }
                else if (e.Key == Key.Right)
                {
                    movingRight = true;
                }
                else if (e.Key == Key.Escape)
                {
                    OnClickQuit();
                }
            }
        }

        private void Player_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                movingDown = false;
            }
            else if (e.Key == Key.Left)
            {
                movingLeft = false;
            }
            else if (e.Key == Key.Up)
            {
                movingUp = false;
            }
            else if (e.Key == Key.Right)
            {
                movingRight = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Rectangle elem = grid1.Children[positionOfPlayerInCanvas] as Rectangle;
            if (movingLeft && !movingRight && (movingUp == movingDown))
            {
                double realSpeed = speed;
                if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                }
                else if (Canvas.GetLeft(elem) != 0.0)
                {
                    Canvas.SetLeft(elem, 0.0);
                }
            }
            else if (movingUp && !movingRight && !movingDown && movingLeft)
            {
                double realSpeed = speed / Math.Sqrt(2);
                if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                }
                else if (Canvas.GetLeft(elem) != 0.0)
                {
                    Canvas.SetLeft(elem, 0.0);
                }
                if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                }
                else if (Canvas.GetTop(elem) != 0.0)
                {
                    Canvas.SetTop(elem, 0.0);
                }
            }
            else if (movingUp && !movingDown && (movingLeft == movingRight))
            {
                double realSpeed = speed;
                if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                }
                else if (Canvas.GetTop(elem) != 0.0)
                {
                    Canvas.SetTop(elem, 0.0);
                }
            }
            else if (movingUp && movingRight && !movingDown && !movingLeft)
            {
                double realSpeed = speed / Math.Sqrt(2);
                if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                }
                else if (Canvas.GetTop(elem) != 0.0)
                {
                    Canvas.SetTop(elem, 0.0);
                }
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width <= workWidth)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                }
                else if (Canvas.GetLeft(elem) + elem.Width != workWidth)
                {
                    Canvas.SetLeft(elem, workWidth - elem.Width);
                }
            }
            else if (movingRight && !movingLeft && (movingUp == movingDown))
            {
                double realSpeed = speed;
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width <= workWidth)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                }
                else if (Canvas.GetLeft(elem) + elem.Width != workWidth)
                {
                    Canvas.SetLeft(elem, workWidth - elem.Width);
                }
            }
            else if (!movingUp && movingRight && movingDown && !movingLeft)
            {
                double realSpeed = speed / Math.Sqrt(2);
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width <= workWidth)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                }
                else if (Canvas.GetLeft(elem) + elem.Width != workWidth)
                {
                    Canvas.SetLeft(elem, workWidth - elem.Width);
                }
                if (Canvas.GetTop(elem) + realSpeed + elem.Height <= workHeight)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                }
                else if (Canvas.GetTop(elem) + elem.Height != workHeight)
                {
                    Canvas.SetTop(elem, workHeight - elem.Height);
                }
            }
            else if (!movingUp && movingDown && (movingLeft == movingRight))
            {
                double realSpeed = speed;
                if (Canvas.GetTop(elem) + realSpeed + elem.Height <= workHeight)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                }
                else if (Canvas.GetTop(elem) + elem.Height != workHeight)
                {
                    Canvas.SetTop(elem, workHeight - elem.Height);
                }
            }
            else if (!movingUp && !movingRight && movingDown && movingLeft)
            {
                double realSpeed = speed / Math.Sqrt(2);
                if (Canvas.GetTop(elem) + realSpeed + elem.Height <= workHeight)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                }
                else if (Canvas.GetTop(elem) + elem.Height != workHeight)
                {
                    Canvas.SetTop(elem, workHeight - elem.Height);
                }
                if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                }
                else if (Canvas.GetLeft(elem) != 0.0)
                {
                    Canvas.SetLeft(elem, 0.0);
                }
            }
        }

        private void OnClickQuit()
        {
            const double width = 250.0, margin = 50.0;
            quitImage.Opacity = 1.0;
            quitButtonsPanel.IsEnabled = true;
            grid1.Children[positionOfPlayerInCanvas].Focusable = false;
            quitButtonsPanel.Width = 800.0;
            quitButtonsPanel.Height = SystemParameters.WorkArea.Height / 8.0;
            Canvas.SetLeft(quitButtonsPanel, (SystemParameters.WorkArea.Width - 800.0) / 2.0);
            Canvas.SetBottom(quitButtonsPanel, SystemParameters.WorkArea.Height * 3.0 / 8.0);
            quitButtonsPanel.Children.Add(new Button()
            {
                Style = (Style)FindResource("StarWarsButtonStyle"),
                Content = Menu.ok,
                IsDefault = true,
                Width = width,
                Margin = new Thickness(margin, 0, 2 * (400.0 - width - margin), 0),
                Focusable = true
            });
            ((Button)quitButtonsPanel.Children[0]).Click += OnClickQuitOk;
            quitButtonsPanel.Children.Add(new Button()
            {
                Style = (Style)FindResource("StarWarsButtonStyle"),
                Content = Menu.cancel,
                IsCancel = true,
                Width = width,
                Focusable = true
            });
            ((Button)quitButtonsPanel.Children[1]).Click += OnClickQuitCancel;
            ((Button)quitButtonsPanel.Children[0]).Focus();
            timer1 = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object obj, EventArgs e)
        {
            timer1.Stop();
        }

        private void OnClickQuitOk(object obj, EventArgs e)
        {
            ((MainWindow)Parent).Content = new MainMenu();
        }

        private void OnClickQuitCancel(object obj, EventArgs e)
        {
            if (timer1.IsEnabled) return;
            quitButtonsPanel.IsEnabled = false;
            quitImage.Opacity = 0.0;
            quitButtonsPanel.Children.RemoveRange(0, 2);
            grid1.Children[positionOfPlayerInCanvas].Focusable = true;
            grid1.Children[positionOfPlayerInCanvas].Focus();
        }
    }
}
