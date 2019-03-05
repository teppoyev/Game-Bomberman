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
        private const double speed = 5.0;
        private const double fixBorderNumber = 0.0;
        private DispatcherTimer timer, timer1;
        private bool movingDown = false, movingUp = false, movingRight = false, movingLeft = false;
        private int positionOfPlayerInCanvas;

        public Battle11()
        {
            InitializeComponent();
            Width = MainWindow.width;
            Height = MainWindow.height;
            var player = new Rectangle()
            {
                Width = 200,
                Height = 200,
                Fill = new ImageBrush(new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Resources/player.jpg"))),
                Focusable = true
            };
            Canvas.SetTop(player, 0);
            Canvas.SetLeft(player, 0);
            player.KeyDown += Player_KeyDown;
            player.KeyUp += Player_KeyUp;
            grid1.Children.Add(player);
            positionOfPlayerInCanvas = grid1.Children.Count - 1;
            grid1.Children[positionOfPlayerInCanvas].Focus();
            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
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
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width <= Width - fixBorderNumber)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                }
                else if (Canvas.GetLeft(elem) + elem.Width != Width - fixBorderNumber)
                {
                    Canvas.SetLeft(elem, Width - fixBorderNumber - elem.Width);
                }
            }
            else if (movingRight && !movingLeft && (movingUp == movingDown))
            {
                double realSpeed = speed;
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width <= Width - fixBorderNumber)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                }
                else if (Canvas.GetLeft(elem) + elem.Width != Width - fixBorderNumber)
                {
                    Canvas.SetLeft(elem, Width - fixBorderNumber - elem.Width);
                }
            }
            else if (!movingUp && movingRight && movingDown && !movingLeft)
            {
                double realSpeed = speed / Math.Sqrt(2);
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width <= Width - fixBorderNumber)
                {
                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                }
                else if (Canvas.GetLeft(elem) + elem.Width != Width - fixBorderNumber)
                {
                    Canvas.SetLeft(elem, Width - fixBorderNumber - elem.Width);
                }
                if (Canvas.GetTop(elem) + realSpeed + elem.Height <= Height - fixBorderNumber)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                }
                else if (Canvas.GetTop(elem) + elem.Height != Height - fixBorderNumber)
                {
                    Canvas.SetTop(elem, Height - fixBorderNumber - elem.Height);
                }
            }
            else if (!movingUp && movingDown && (movingLeft == movingRight))
            {
                double realSpeed = speed;
                if (Canvas.GetTop(elem) + realSpeed + elem.Height <= Height - fixBorderNumber)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                }
                else if (Canvas.GetTop(elem) + elem.Height != Height - fixBorderNumber)
                {
                    Canvas.SetTop(elem, Height - fixBorderNumber - elem.Height);
                }
            }
            else if (!movingUp && !movingRight && movingDown && movingLeft)
            {
                double realSpeed = speed / Math.Sqrt(2);
                if (Canvas.GetTop(elem) + realSpeed + elem.Height <= Height - fixBorderNumber)
                {
                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                }
                else if (Canvas.GetTop(elem) + elem.Height != Height - fixBorderNumber)
                {
                    Canvas.SetTop(elem, Height - fixBorderNumber - elem.Height);
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
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.Yellow,
                FontFamily = new FontFamily("STARWARS"),
                FontSize = SystemParameters.WorkArea.Height / 16.0,
                Content = Menu.ok,
                IsDefault = true,
                Width = width,
                Margin = new Thickness(margin, 0, 2 * (400.0 - width - margin), 0)
            });
            ((Button)quitButtonsPanel.Children[0]).Click += OnClickQuitOk;
            quitButtonsPanel.Children.Add(new Button()
            {
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.Yellow,
                FontFamily = new FontFamily("STARWARS"),
                FontSize = SystemParameters.WorkArea.Height / 16.0,
                Content = Menu.cancel,
                IsCancel = true,
                Width = width
            });
            ((Button)quitButtonsPanel.Children[1]).Click += OnClickQuitCancel;
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
