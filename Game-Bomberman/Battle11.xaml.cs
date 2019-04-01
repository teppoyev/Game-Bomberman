using Game_Bomberman.Game_Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_Bomberman
{
    /// <summary>
    /// Логика взаимодействия для Battle11.xaml
    /// </summary>
    public partial class Battle11 : Page
    {
        private const double coefOfDeath = 0.25;
        private const double kspeed = 1.1;
        private int numberOfEnemies = 6;
        private int widthOfFieldOfBattle = 25;
        private int heightOfFieldOfBattle = 13;
        private int numberOfUnDestroyableBlocks;
        private int numberOfDestroyableBlocks;
        private DispatcherTimer timer, timer1;
        private bool movingDown = false, movingUp = false, movingRight = false, movingLeft = false;
        private bool lastUR = false, lastRD = false, lastDL = false, lastLU = false;
        private int positionOfPlayerInCanvas;
        private double workWidth, workHeight;
        private bool[][] fieldOfBattle;
        private bool[][] fieldOfBombs;
        private Queue<Bomb> bombs;
        private Player player;
        private List<Enemy> enemies;
        private Block[][] blocks;

        public Battle11()
        {
            InitializeComponent();
            Width = MainWindow.width; Height = MainWindow.height;
            numberOfUnDestroyableBlocks = ((widthOfFieldOfBattle - 1) / 2) * ((heightOfFieldOfBattle - 1) / 2);
            numberOfDestroyableBlocks = Convert.ToInt32(Math.Round((widthOfFieldOfBattle * heightOfFieldOfBattle - numberOfUnDestroyableBlocks) * 0.26));

            //put the player's character to the canvas
            player = new Player
            {
                NumberOfBombs = 4,
                RangeOfExplosion = 5
            };
            Canvas.SetTop(player.Body, 0.0);
            Canvas.SetLeft(player.Body, 0.0);
            player.Body.KeyDown += Player_KeyDown;
            player.Body.KeyUp += Player_KeyUp;
            grid1.Children.Add(player.Body);
            positionOfPlayerInCanvas = grid1.Children.Count - 1;

            //init the field of the battle and the field of bombs
            fieldOfBattle = new bool[widthOfFieldOfBattle][];
            fieldOfBombs = new bool[widthOfFieldOfBattle][];
            for (int i = 0; i < widthOfFieldOfBattle; ++i)
            {
                fieldOfBattle[i] = new bool[heightOfFieldOfBattle];
                fieldOfBombs[i] = new bool[heightOfFieldOfBattle];
                for (int j = 0; j < heightOfFieldOfBattle; ++j)
                {
                    fieldOfBattle[i][j] = false;
                    fieldOfBombs[i][j] = false;
                }
            }

            //put undestroyable blocks to the field and to the canvas
            blocks = new Block[widthOfFieldOfBattle][];
            for (int i = 0; i < widthOfFieldOfBattle; ++i)
            {
                blocks[i] = new Block[heightOfFieldOfBattle];
            }
            for (int i = 0; i < (widthOfFieldOfBattle - 1) / 2; ++i)
            {
                for (int j = 0; j < (heightOfFieldOfBattle - 1) / 2; ++j)
                {
                    var block = new Game_Logic.Stone();
                    Canvas.SetLeft(block.Body, HelpfulFunctions.AbsoluteCoord(i * 2 + 1));
                    Canvas.SetTop(block.Body, HelpfulFunctions.AbsoluteCoord(j * 2 + 1));
                    fieldOfBattle[i * 2 + 1][j * 2 + 1] = true;
                    blocks[i * 2 + 1][j * 2 + 1] = block;
                    grid1.Children.Add(block.Body);
                }
            }

            //put destroyable blocks to field and to the canvas
            for (int i = 0; i < numberOfDestroyableBlocks; ++i)
            {
                Random rnd = new Random();
                int x0 , y0;
                do
                {
                    x0 = rnd.Next(widthOfFieldOfBattle);
                    y0 = rnd.Next(heightOfFieldOfBattle);
                } while (fieldOfBattle[x0][y0] || (x0 == 0 && y0 == 0) || (x0 == 1 && y0 == 0) || (x0 == 0 && y0 == 1));
                var block = new Leaves();
                Canvas.SetLeft(block.Body, HelpfulFunctions.AbsoluteCoord(x0));
                Canvas.SetTop(block.Body, HelpfulFunctions.AbsoluteCoord(y0));
                fieldOfBattle[x0][y0] = true;
                blocks[x0][y0] = block;
                grid1.Children.Add(block.Body);
            }

            //put ememies to field
            enemies = new List<Enemy>(numberOfEnemies);
            for (int i = 0; i < numberOfEnemies; ++i)
            {
                Random rnd = new Random(i);
                int x0, y0;
                do
                {
                    x0 = rnd.Next(widthOfFieldOfBattle);
                    y0 = rnd.Next(heightOfFieldOfBattle);
                } while (fieldOfBattle[x0][y0] || (x0 < widthOfFieldOfBattle / 2 && y0 < heightOfFieldOfBattle / 2));
                enemies.Add(new Gabaijito());
                Canvas.SetLeft(enemies[i].Body, HelpfulFunctions.AbsoluteCoord(x0));
                Canvas.SetTop(enemies[i].Body, HelpfulFunctions.AbsoluteCoord(y0));
                enemies[i].Direction = rnd.Next(4) + 1;
                grid1.Children.Add(enemies[i].Body);
            }

            //put buffs to the canvas???
            //...

            //put the focus to the player's character
            grid1.Children[positionOfPlayerInCanvas].Focus();

            //start a timer to check keyboard's events
            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            timer.Tick += Player_Moving;
            timer.Tick += Enemies_Moving;
            timer.Tick += Enemies_Attacking;
            timer.Start();

            //init bombs;
            bombs = new Queue<Bomb>(10);
        }

        private void Grid0_Loaded(object sender, EventArgs e)
        {
            workWidth = grid0.ColumnDefinitions[1].ActualWidth; workHeight = grid0.RowDefinitions[1].ActualHeight;
            BattleMusic.MediaEnded += (object obj, RoutedEventArgs ev) => { BattleMusic.Stop(); BattleMusic.Play(); };
            BlastSound.MediaEnded += (object obj, RoutedEventArgs ev) => { BlastSound.Stop(); };
            BattleMusic.Play();
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
                else if (e.Key == Key.Space)
                {
                    SetBomb();
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

        private void Player_Moving(object sender, EventArgs e)
        {
            Rectangle elem = grid1.Children[positionOfPlayerInCanvas] as Rectangle;
            int x0, y0;
            double x, y;
            double realSpeed = kspeed * player.Speed;
            if (movingLeft && !movingRight && (movingUp == movingDown))
            {
                x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                x0 = HelpfulFunctions.RelativeCoord(x - realSpeed); y0 = HelpfulFunctions.RelativeCoord(y);
                if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                {
                    if (!fieldOfBattle[x0][y0] && y == HelpfulFunctions.AbsoluteCoord(y0))
                    {
                        if (!fieldOfBombs[x0][y0] || x0 == HelpfulFunctions.RelativeCoord(x)) Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                    }
                    else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0][y0 + 1])
                    {
                        if (Canvas.GetTop(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(y0))
                        {
                            Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                        }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0));
                    }
                    else if (fieldOfBattle[x0][y0] && y != HelpfulFunctions.AbsoluteCoord(y0) && !fieldOfBattle[x0][y0 + 1])
                    {
                        if (Canvas.GetTop(elem) + elem.Height + realSpeed <= HelpfulFunctions.AbsoluteCoord(y0 + 2))
                        {
                            Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                        }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 2) - elem.Height);
                    }
                    else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                }
                else Canvas.SetLeft(elem, 0.0);
            }
            else if (movingUp && !movingRight && !movingDown && movingLeft)
            {
                realSpeed *= 2.0;
                if (lastLU)
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x - realSpeed); y0 = HelpfulFunctions.RelativeCoord(y);
                    if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                    {
                        if (!fieldOfBattle[x0][y0] && y == HelpfulFunctions.AbsoluteCoord(y0))
                        {
                            if (!fieldOfBombs[x0][y0] || x0 == HelpfulFunctions.RelativeCoord(x))
                            {
                                if (x <= HelpfulFunctions.AbsoluteCoord(x0 + 1))
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                                else
                                {
                                    double difference = x - HelpfulFunctions.AbsoluteCoord(x0 + 1);
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - difference);
                                    if (y0 != 0 && !fieldOfBattle[x0 + 1][y0 - 1]) Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed + difference);
                                }
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0][y0 + 1])
                        {
                            if (Canvas.GetTop(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(y0))
                            {
                                Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0));
                        }
                        else if (fieldOfBattle[x0][y0] && y != HelpfulFunctions.AbsoluteCoord(y0) && !fieldOfBattle[x0][y0 + 1]) { }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                    }
                    else Canvas.SetLeft(elem, 0.0);
                    lastLU = false;
                }
                else
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y - realSpeed);
                    if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                    {
                        if (!fieldOfBattle[x0][y0] && x == HelpfulFunctions.AbsoluteCoord(x0))
                        {
                            if (!fieldOfBombs[x0][y0] || y0 == HelpfulFunctions.RelativeCoord(y))
                            {
                                if (y <= HelpfulFunctions.AbsoluteCoord(y0 + 1))
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                                else
                                {
                                    double difference = y - HelpfulFunctions.AbsoluteCoord(y0 + 1);
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) - difference);
                                    if (x0 != 0 && !fieldOfBattle[x0 - 1][y0 + 1]) Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed + difference);
                                }
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0])
                        {
                            if (Canvas.GetLeft(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(x0))
                            {
                                Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0));
                        }
                        else if (fieldOfBattle[x0][y0] && x != HelpfulFunctions.AbsoluteCoord(x0) && !fieldOfBattle[x0 + 1][y0]) { }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                    }
                    else Canvas.SetTop(elem, 0.0);
                    lastLU = true;
                }
            }
            else if (movingUp && !movingDown && (movingLeft == movingRight))
            {
                x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y - realSpeed);
                if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                {
                    if (!fieldOfBattle[x0][y0] && x == HelpfulFunctions.AbsoluteCoord(x0))
                    {
                        if (!fieldOfBombs[x0][y0] || y0 == HelpfulFunctions.RelativeCoord(y)) Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                    }
                    else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0])
                    {
                        if (Canvas.GetLeft(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(x0))
                        {
                            Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                        }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0));
                    }
                    else if (fieldOfBattle[x0][y0] && x != HelpfulFunctions.AbsoluteCoord(x0) && !fieldOfBattle[x0 + 1][y0])
                    {
                        if (Canvas.GetLeft(elem) + elem.Width + realSpeed <= HelpfulFunctions.AbsoluteCoord(x0 + 2))
                        {
                            Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                        }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 2) - elem.Width);
                    }
                    else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                }
                else Canvas.SetTop(elem, 0.0);
            }
            else if (movingUp && movingRight && !movingDown && !movingLeft)
            {
                realSpeed *= 2.0;
                if (lastUR)
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y - realSpeed);
                    if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                    {
                        if (!fieldOfBattle[x0][y0] && x == HelpfulFunctions.AbsoluteCoord(x0))
                        {
                            if (!fieldOfBombs[x0][y0] || y0 == HelpfulFunctions.RelativeCoord(y))
                            {
                                if (y <= HelpfulFunctions.AbsoluteCoord(y0 + 1))
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                                else
                                {
                                    double difference = y - HelpfulFunctions.AbsoluteCoord(y0 + 1);
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) - difference);
                                    if (x0 != widthOfFieldOfBattle - 1 && !fieldOfBattle[x0 + 1][y0 + 1]) Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed - difference);
                                }
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0]) { }
                        else if (fieldOfBattle[x0][y0] && x != HelpfulFunctions.AbsoluteCoord(x0) && !fieldOfBattle[x0 + 1][y0])
                        {
                            if (Canvas.GetLeft(elem) + elem.Width + realSpeed <= HelpfulFunctions.AbsoluteCoord(x0 + 2))
                            {
                                Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 2) - elem.Width);
                        }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                    }
                    else Canvas.SetTop(elem, 0.0);
                    lastUR = false;
                }
                else
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x + realSpeed + elem.Width); y0 = HelpfulFunctions.RelativeCoord(y);
                    if (Canvas.GetLeft(elem) + realSpeed + elem.Width < workWidth)
                    {
                        if (!fieldOfBattle[x0][y0] && y == HelpfulFunctions.AbsoluteCoord(y0))
                        {
                            if (!fieldOfBombs[x0][y0] || x0 == HelpfulFunctions.RelativeCoord(x + elem.Width - 1))
                            {
                                if (x >= HelpfulFunctions.AbsoluteCoord(x0 - 1))
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                                else
                                {
                                    double difference = HelpfulFunctions.AbsoluteCoord(x0 - 1) - x;
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + difference);
                                    if (y0 != 0 && !fieldOfBattle[x0 - 1][y0 - 1]) Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed + difference);
                                }
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0][y0 + 1])
                        {
                            if (Canvas.GetTop(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(y0))
                            {
                                Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0));
                        }
                        else if (fieldOfBattle[x0][y0] && y != HelpfulFunctions.AbsoluteCoord(y0) && !fieldOfBattle[x0][y0 + 1]) { }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                    }
                    else Canvas.SetLeft(elem, workWidth - elem.Width);
                    lastUR = true;
                }
            }
            else if (movingRight && !movingLeft && (movingUp == movingDown))
            {
                x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                x0 = HelpfulFunctions.RelativeCoord(x + realSpeed + elem.Width); y0 = HelpfulFunctions.RelativeCoord(y);
                if (Canvas.GetLeft(elem) + realSpeed + elem.Width < workWidth)
                {
                    if (!fieldOfBattle[x0][y0] && y == HelpfulFunctions.AbsoluteCoord(y0))
                    {
                        if (!fieldOfBombs[x0][y0] || x0 == HelpfulFunctions.RelativeCoord(x + elem.Width - 1)) Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                    }
                    else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0][y0 + 1])
                    {
                        if (Canvas.GetTop(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(y0))
                        {
                            Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                        }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0));
                    }
                    else if (fieldOfBattle[x0][y0] && y != HelpfulFunctions.AbsoluteCoord(y0) && !fieldOfBattle[x0][y0 + 1])
                    {
                        if (Canvas.GetTop(elem) + elem.Height + realSpeed <= HelpfulFunctions.AbsoluteCoord(y0 + 2))
                        {
                            Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                        }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 2) - elem.Height);
                    }
                    else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                }
                else Canvas.SetLeft(elem, workWidth - elem.Width);
            }
            else if (!movingUp && movingRight && movingDown && !movingLeft)
            {
                realSpeed *= 2.0;
                if (lastRD)
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x + realSpeed + elem.Width); y0 = HelpfulFunctions.RelativeCoord(y);
                    if (Canvas.GetLeft(elem) + realSpeed + elem.Width < workWidth)
                    {
                        if (!fieldOfBattle[x0][y0] && y == HelpfulFunctions.AbsoluteCoord(y0))
                        {
                            if (!fieldOfBombs[x0][y0] || x0 == HelpfulFunctions.RelativeCoord(x + elem.Width - 1))
                            {
                                if (x >= HelpfulFunctions.AbsoluteCoord(x0 - 1))
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                                else
                                {
                                    double difference = HelpfulFunctions.AbsoluteCoord(x0 - 1) - x;
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + difference);
                                    if (y0 != heightOfFieldOfBattle - 1 && !fieldOfBattle[x0 - 1][y0 + 1]) Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed - difference);
                                }
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0][y0 + 1]) { }
                        else if (fieldOfBattle[x0][y0] && y != HelpfulFunctions.AbsoluteCoord(y0) && !fieldOfBattle[x0][y0 + 1])
                        {
                            if (Canvas.GetTop(elem) + elem.Height + realSpeed <= HelpfulFunctions.AbsoluteCoord(y0 + 2))
                            {
                                Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 2) - elem.Height);
                        }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                    }
                    else Canvas.SetLeft(elem, workWidth - elem.Width);
                    lastRD = false;
                }
                else
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y + realSpeed + elem.Height);
                    if (Canvas.GetTop(elem) + realSpeed + elem.Height < workHeight)
                    {
                        if (!fieldOfBattle[x0][y0] && x == HelpfulFunctions.AbsoluteCoord(x0))
                        {
                            if (!fieldOfBombs[x0][y0] || y0 == HelpfulFunctions.RelativeCoord(y + elem.Width - 1))
                            {
                                if (y >= HelpfulFunctions.AbsoluteCoord(y0 - 1))
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                                else
                                {
                                    double difference = HelpfulFunctions.AbsoluteCoord(y0 - 1) - y;
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) + difference);
                                    if (x0 != widthOfFieldOfBattle - 1 && !fieldOfBattle[x0 + 1][y0 - 1]) Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed - difference);
                                }
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0]) { }
                        else if (fieldOfBattle[x0][y0] && x != HelpfulFunctions.AbsoluteCoord(x0) && !fieldOfBattle[x0 + 1][y0])
                        {
                            if (Canvas.GetLeft(elem) + elem.Width + realSpeed <= HelpfulFunctions.AbsoluteCoord(x0 + 2))
                            {
                                Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 2) - elem.Width);
                        }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                    }
                    else Canvas.SetTop(elem, workHeight - elem.Height);
                    lastRD = true;
                }
            }
            else if (!movingUp && movingDown && (movingLeft == movingRight))
            {
                x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y + realSpeed + elem.Height);
                if (Canvas.GetTop(elem) + realSpeed + elem.Height < workHeight)
                {
                    if (!fieldOfBattle[x0][y0] && x == HelpfulFunctions.AbsoluteCoord(x0))
                    {
                        if (!fieldOfBombs[x0][y0] || y0 == HelpfulFunctions.RelativeCoord(y + elem.Height - 1)) Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                    }
                    else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0])
                    {
                        if (Canvas.GetLeft(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(x0))
                        {
                            Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                        }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0));
                    }
                    else if (fieldOfBattle[x0][y0] && x != HelpfulFunctions.AbsoluteCoord(x0) && !fieldOfBattle[x0 + 1][y0])
                    {
                        if (Canvas.GetLeft(elem) + elem.Width + realSpeed <= HelpfulFunctions.AbsoluteCoord(x0 + 2))
                        {
                            Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                        }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 2) - elem.Width);
                    }
                    else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                }
                else Canvas.SetTop(elem, workHeight - elem.Height);
            }
            else if (!movingUp && !movingRight && movingDown && movingLeft)
            {
                realSpeed *= 2.0;
                if (lastDL)
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y + realSpeed + elem.Height);
                    if (Canvas.GetTop(elem) + realSpeed + elem.Height < workHeight)
                    {
                        if (!fieldOfBattle[x0][y0] && x == HelpfulFunctions.AbsoluteCoord(x0))
                        {
                            if (!fieldOfBombs[x0][y0] || y0 == HelpfulFunctions.RelativeCoord(y + elem.Width - 1))
                            {
                                if (y >= HelpfulFunctions.AbsoluteCoord(y0 - 1))
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                                else
                                {
                                    double difference = HelpfulFunctions.AbsoluteCoord(y0 - 1) - y;
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) + difference);
                                    if (x0 != 0 && !fieldOfBattle[x0 - 1][y0 - 1]) Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed + difference);
                                }
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0])
                        {
                            if (Canvas.GetLeft(elem) - realSpeed >= HelpfulFunctions.AbsoluteCoord(x0))
                            {
                                Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0));
                        }
                        else if (fieldOfBattle[x0][y0] && x != HelpfulFunctions.AbsoluteCoord(x0) && !fieldOfBattle[x0 + 1][y0]) { }
                        else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                    }
                    else Canvas.SetTop(elem, workHeight - elem.Height);
                    lastDL = false;
                }
                else
                {
                    x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                    x0 = HelpfulFunctions.RelativeCoord(x - realSpeed); y0 = HelpfulFunctions.RelativeCoord(y);
                    if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                    {
                        if (!fieldOfBattle[x0][y0] && y == HelpfulFunctions.AbsoluteCoord(y0))
                        {
                            if (!fieldOfBombs[x0][y0] || x0 == HelpfulFunctions.RelativeCoord(x))
                            {
                                if (x <= HelpfulFunctions.AbsoluteCoord(x0 + 1))
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                                else
                                {
                                    double difference = x - HelpfulFunctions.AbsoluteCoord(x0 + 1);
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - difference);
                                    if (y0 != heightOfFieldOfBattle - 1 && !fieldOfBattle[x0 + 1][y0]) Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed - difference);
                                }
                            }
                            else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                        }
                        else if (!fieldOfBattle[x0][y0] && fieldOfBattle[x0 + 1][y0 + 1]) { }
                        else if (fieldOfBattle[x0][y0] && y != HelpfulFunctions.AbsoluteCoord(y0) && !fieldOfBattle[x0][y0 + 1])
                        {
                            if (Canvas.GetTop(elem) + elem.Height + realSpeed <= HelpfulFunctions.AbsoluteCoord(y0 + 2))
                            {
                                Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                            }
                            else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 2) - elem.Height);
                        }
                        else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                    }
                    else Canvas.SetLeft(elem, 0.0);
                    lastDL = true;
                }
            }
        }

        private void Enemies_Moving(object sender, EventArgs e)
        {
            for (int i = 0; i < numberOfEnemies; ++i)
            {
                Rectangle elem = enemies[i].Body;
                Random rnd = new Random();
                int x0, y0;
                double x, y;
                double realSpeed = kspeed * enemies[i].Speed;

                switch (enemies[i].Direction)
                {
                    case 1:
                        {
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            x0 = HelpfulFunctions.RelativeCoord(x - realSpeed); y0 = HelpfulFunctions.RelativeCoord(y);
                            if (Canvas.GetLeft(elem) - realSpeed >= 0.0)
                            {
                                if (!fieldOfBattle[x0][y0] && !fieldOfBombs[x0][y0])
                                {
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) - realSpeed);
                                    break;
                                }
                                else if (fieldOfBombs[x0][y0] && HelpfulFunctions.RelativeCoord(x) == x0) break;
                                else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 + 1));
                            }
                            else Canvas.SetLeft(elem, 0.0);
                            int newDirection;
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            do
                            {
                                newDirection = rnd.Next(100);
                                if (newDirection % 2 == 0) newDirection = newDirection > 50 ? 2 : 4;
                                else newDirection = 3;
                            } while (!CheckDirection(newDirection, HelpfulFunctions.RelativeCoord(x), HelpfulFunctions.RelativeCoord(y)));
                            enemies[i].Direction = newDirection;
                            break;
                        }
                    case 2:
                        {
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y - realSpeed);
                            if (Canvas.GetTop(elem) - realSpeed >= 0.0)
                            {
                                if (!fieldOfBattle[x0][y0] && !fieldOfBombs[x0][y0])
                                {
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) - realSpeed);
                                    break;
                                }
                                else if (fieldOfBombs[x0][y0] && HelpfulFunctions.RelativeCoord(y) == y0) break;
                                else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 + 1));
                            }
                            else Canvas.SetTop(elem, 0.0);
                            int newDirection;
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            do
                            {
                                newDirection = rnd.Next(100);
                                if (newDirection % 2 == 0) newDirection = newDirection > 50 ? 1 : 3;
                                else newDirection = 4;
                            } while (!CheckDirection(newDirection, HelpfulFunctions.RelativeCoord(x), HelpfulFunctions.RelativeCoord(y)));
                            enemies[i].Direction = newDirection;
                            break;
                        }
                    case 3:
                        {
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            x0 = HelpfulFunctions.RelativeCoord(x + realSpeed + elem.Width); y0 = HelpfulFunctions.RelativeCoord(y);
                            if (Canvas.GetLeft(elem) + realSpeed + elem.Width < workWidth)
                            {
                                if (!fieldOfBattle[x0][y0] && !fieldOfBombs[x0][y0])
                                {
                                    Canvas.SetLeft(elem, Canvas.GetLeft(elem) + realSpeed);
                                    break;
                                }
                                else if (fieldOfBombs[x0][y0] && HelpfulFunctions.RelativeCoord(x + elem.Width - 1) == x0) break;
                                else Canvas.SetLeft(elem, HelpfulFunctions.AbsoluteCoord(x0 - 1));
                            }
                            else Canvas.SetLeft(elem, workWidth - elem.Width);
                            int newDirection;
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            do
                            {
                                newDirection = rnd.Next(100);
                                if (newDirection % 2 == 0) newDirection = newDirection > 50 ? 2 : 4;
                                else newDirection = 1;
                            } while (!CheckDirection(newDirection, HelpfulFunctions.RelativeCoord(x), HelpfulFunctions.RelativeCoord(y)));
                            enemies[i].Direction = newDirection;
                            break;
                        }
                    case 4:
                        {
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            x0 = HelpfulFunctions.RelativeCoord(x); y0 = HelpfulFunctions.RelativeCoord(y + realSpeed + elem.Height);
                            if (Canvas.GetTop(elem) + realSpeed + elem.Height < workHeight)
                            {
                                if (!fieldOfBattle[x0][y0] && !fieldOfBombs[x0][y0])
                                {
                                    Canvas.SetTop(elem, Canvas.GetTop(elem) + realSpeed);
                                    break;
                                }
                                else if (fieldOfBombs[x0][y0] && HelpfulFunctions.RelativeCoord(y + elem.Width - 1) == y0) break;
                                else Canvas.SetTop(elem, HelpfulFunctions.AbsoluteCoord(y0 - 1));
                            }
                            else Canvas.SetTop(elem, workHeight - elem.Height);
                            int newDirection;
                            x = Canvas.GetLeft(elem); y = Canvas.GetTop(elem);
                            do
                            {
                                newDirection = rnd.Next(100);
                                if (newDirection % 2 == 0) newDirection = newDirection > 50 ? 1 : 3;
                                else newDirection = 2;
                            } while (!CheckDirection(newDirection, HelpfulFunctions.RelativeCoord(x), HelpfulFunctions.RelativeCoord(y)));
                            enemies[i].Direction = newDirection;
                            break;
                        }
                }
            };
        }

        private void Enemies_Attacking(object sender, EventArgs e)
        {
            if (player.Body != null)
            {
                Rectangle elem; double x1, y1; double x = Canvas.GetLeft(player.Body), y = Canvas.GetTop(player.Body);
                for (int i = 0; i < numberOfEnemies; ++i)
                {
                    elem = enemies[i].Body; x1 = Canvas.GetLeft(elem); y1 = Canvas.GetTop(elem);
                    if (x1 + elem.Width < x + elem.Width * coefOfDeath)
                    {
                        if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                        {
                            if (x1 + elem.Width >= x + elem.Width * coefOfDeath)
                            {
                                Death(player); break;
                            }
                        }
                    }
                    else if (x1 < x + elem.Width - elem.Width * coefOfDeath)
                    {
                        if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                        {
                            Death(player); break;
                        }
                    }
                    else
                    {
                        if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                        {
                            if (x1 < x + elem.Width - elem.Width * coefOfDeath)
                            {
                                Death(player); break;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckDirection(int direction, int x, int y)
        {
            if (x < widthOfFieldOfBattle - 1)
            {
                if (x > 0)
                {
                    if (y < heightOfFieldOfBattle - 1)
                    {
                        if (y > 0)
                        {
                            if (!fieldOfBattle[x - 1][y] || !fieldOfBattle[x][y - 1] || !fieldOfBattle[x + 1][y] || !fieldOfBattle[x][y + 1])
                            {
                                switch (direction)
                                {
                                    case 1:
                                        return !fieldOfBattle[x - 1][y] ? true : false;
                                    case 2:
                                        return !fieldOfBattle[x][y - 1] ? true : false;
                                    case 3:
                                        return !fieldOfBattle[x + 1][y] ? true : false;
                                    case 4:
                                        return !fieldOfBattle[x][y + 1] ? true : false;
                                }
                            }
                            else return true;
                        }
                        else
                        {
                            if (!fieldOfBattle[x - 1][y] || !fieldOfBattle[x + 1][y] || !fieldOfBattle[x][y + 1])
                            {
                                switch (direction)
                                {
                                    case 1:
                                        return !fieldOfBattle[x - 1][y] ? true : false;
                                    case 2:
                                        return false;
                                    case 3:
                                        return !fieldOfBattle[x + 1][y] ? true : false;
                                    case 4:
                                        return !fieldOfBattle[x][y + 1] ? true : false;
                                }
                            }
                            else return true;
                        }
                    }
                    else
                    {
                        if (!fieldOfBattle[x - 1][y] || !fieldOfBattle[x][y - 1] || !fieldOfBattle[x + 1][y])
                        {
                            switch (direction)
                            {
                                case 1:
                                    return !fieldOfBattle[x - 1][y] ? true : false;
                                case 2:
                                    return !fieldOfBattle[x][y - 1] ? true : false;
                                case 3:
                                    return !fieldOfBattle[x + 1][y] ? true : false;
                                case 4:
                                    return false;
                            }
                        }
                        else return true;
                    }
                }
                else
                {
                    if (y < heightOfFieldOfBattle - 1)
                    {
                        if (y > 0)
                        {
                            if (!fieldOfBattle[x][y - 1] || !fieldOfBattle[x + 1][y] || !fieldOfBattle[x][y + 1])
                            {
                                switch (direction)
                                {
                                    case 1:
                                        return false;
                                    case 2:
                                        return !fieldOfBattle[x][y - 1] ? true : false;
                                    case 3:
                                        return !fieldOfBattle[x + 1][y] ? true : false;
                                    case 4:
                                        return !fieldOfBattle[x][y + 1] ? true : false;
                                }
                            }
                            else return true;
                        }
                        else
                        {
                            if (!fieldOfBattle[x + 1][y] || !fieldOfBattle[x][y + 1])
                            {
                                switch (direction)
                                {
                                    case 1:
                                        return false;
                                    case 2:
                                        return false;
                                    case 3:
                                        return !fieldOfBattle[x + 1][y] ? true : false;
                                    case 4:
                                        return !fieldOfBattle[x][y + 1] ? true : false;
                                }
                            }
                            else return true;
                        }
                    }
                    else
                    {
                        if (!fieldOfBattle[x][y - 1] || !fieldOfBattle[x + 1][y])
                        {
                            switch (direction)
                            {
                                case 1:
                                    return false;
                                case 2:
                                    return !fieldOfBattle[x][y - 1] ? true : false;
                                case 3:
                                    return !fieldOfBattle[x + 1][y] ? true : false;
                                case 4:
                                    return false;
                            }
                        }
                        else return true;
                    }
                }
            }
            else
            {
                if (y < heightOfFieldOfBattle - 1)
                {
                    if (y > 0)
                    {
                        if (!fieldOfBattle[x - 1][y] || !fieldOfBattle[x][y - 1] || !fieldOfBattle[x][y + 1])
                        {
                            switch (direction)
                            {
                                case 1:
                                    return !fieldOfBattle[x - 1][y] ? true : false;
                                case 2:
                                    return !fieldOfBattle[x][y - 1] ? true : false;
                                case 3:
                                    return false;
                                case 4:
                                    return !fieldOfBattle[x][y + 1] ? true : false;
                            }
                        }
                        else return true;
                    }
                    else
                    {
                        if (!fieldOfBattle[x - 1][y] || !fieldOfBattle[x][y + 1])
                        {
                            switch (direction)
                            {
                                case 1:
                                    return !fieldOfBattle[x - 1][y] ? true : false;
                                case 2:
                                    return false;
                                case 3:
                                    return false;
                                case 4:
                                    return !fieldOfBattle[x][y + 1] ? true : false;
                            }
                        }
                        else return true;
                    }
                }
                else
                {
                    if (!fieldOfBattle[x - 1][y] || !fieldOfBattle[x][y - 1])
                    {
                        switch (direction)
                        {
                            case 1:
                                return !fieldOfBattle[x - 1][y] ? true : false;
                            case 2:
                                return !fieldOfBattle[x][y - 1] ? true : false;
                            case 3:
                                return false;
                            case 4:
                                return false;
                        }
                    }
                    else return true;
                }
            }
            return true;
        }

        private void OnClickQuit()
        {
            timer.Stop();
            BattleMusic.Pause();
            for (int i = 0; i < bombs.Count(); ++i)
            {
                bombs.ElementAt(i).Timer.Stop();
            }
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
            timer1.Tick += (object obj, EventArgs e) => { timer1.Stop(); };
            timer1.Start();
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
            timer.Start();
            BattleMusic.Play();
            for (int i = 0; i < bombs.Count(); ++i)
            {
                bombs.ElementAt(i).Timer.Start();
            }
        }

        private void SetBomb()
        {
            if (player.NumberOfBombs > 0)
            {
                //put a bomb to player position
                double x0, y0;
                x0 = Canvas.GetLeft(player.Body) % 75 > 37.5 ?
                    HelpfulFunctions.AbsoluteCoord(HelpfulFunctions.RelativeCoord(Canvas.GetLeft(player.Body)) + 1) :
                    HelpfulFunctions.AbsoluteCoord(HelpfulFunctions.RelativeCoord(Canvas.GetLeft(player.Body)));
                y0 = Canvas.GetTop(player.Body) % 75 > 37.5 ?
                    HelpfulFunctions.AbsoluteCoord(HelpfulFunctions.RelativeCoord(Canvas.GetTop(player.Body)) + 1) :
                    HelpfulFunctions.AbsoluteCoord(HelpfulFunctions.RelativeCoord(Canvas.GetTop(player.Body)));
                if (!fieldOfBombs[HelpfulFunctions.RelativeCoord(x0)][HelpfulFunctions.RelativeCoord(y0)] && !fieldOfBattle[HelpfulFunctions.RelativeCoord(x0)][HelpfulFunctions.RelativeCoord(y0)])
                {
                    var bomb = new Game_Logic.Bomb();
                    Canvas.SetLeft(bomb.Body, x0);
                    Canvas.SetTop(bomb.Body, y0);
                    fieldOfBombs[HelpfulFunctions.RelativeCoord(x0)][HelpfulFunctions.RelativeCoord(y0)] = true;
                    bombs.Enqueue(bomb);
                    bomb.Timer.Tick += (object o, EventArgs events) => TimerTillExpload(o, events, bomb);
                    bomb.Timer.Start();

                    grid1.Children.Add(bomb.Body);

                    --player.NumberOfBombs;
                }
                else return;
            }
        }

        private void TimerTillExpload(object obj, EventArgs e, Bomb bomb)
        {
            if (bomb.Iterator < 175) ++bomb.Iterator;
            else
            {
                bombs.Dequeue();
                var x = HelpfulFunctions.RelativeCoord(Canvas.GetLeft(bomb.Body));
                var y = HelpfulFunctions.RelativeCoord(Canvas.GetTop(bomb.Body));
                fieldOfBombs[x][y] = false;
                ++player.NumberOfBombs;
                grid1.Children.Remove(bomb.Body);
                BlastSound.Stop();
                BlastSound.Play();
                bomb.Iterator = 0;
                ((DispatcherTimer)obj).Stop();
                Explosion(x, y, player.RangeOfExplosion);
            }
        }

        private void Explosion(int x, int y, int r)
        {
            var texture = Battle.explosionCenter1;
            List<Rectangle> recs = new List<Rectangle>
            {
                new Rectangle
                {
                    Width = Game_Logic.Object.standartSize,
                    Height = Game_Logic.Object.standartSize
                }
            };
            List<Rectangle> toDelete = new List<Rectangle>();
            Canvas.SetLeft(recs.ElementAt(0), HelpfulFunctions.AbsoluteCoord(x));
            Canvas.SetTop(recs.ElementAt(0), HelpfulFunctions.AbsoluteCoord(y));
            int a = 1, b = 1, c = 1;
            if (y % 2 == 0) {
                int x0 = x - 1, r0 = r;
                while (x0 >= 0 && r0 > 0)
                {
                    if (!fieldOfBattle[x0][y])
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x0));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y));
                        if (fieldOfBombs[x0][y])
                        {
                            for (int k = 0; k < bombs.Count; ++k)
                            {
                                if (Canvas.GetLeft(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(x0) &&
                                    Canvas.GetTop(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(y))
                                {
                                    bombs.ElementAt(k).Iterator = 175;
                                }
                            }
                        }
                        --x0; --r0;
                    } else
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x0));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y));
                        toDelete.Add(blocks[x0][y].Body);
                        break;
                    }
                }
                a = recs.Count;
                x0 = x + 1; r0 = r;
                while (x0 < widthOfFieldOfBattle && r0 > 0)
                {
                    if (!fieldOfBattle[x0][y])
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x0));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y));
                        if (fieldOfBombs[x0][y])
                        {
                            for (int k = 0; k < bombs.Count; ++k)
                            {
                                if (Canvas.GetLeft(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(x0) &&
                                    Canvas.GetTop(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(y))
                                {
                                    bombs.ElementAt(k).Iterator = 175;
                                }
                            }
                        }
                        ++x0; --r0;
                    }
                    else
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x0));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y));
                        toDelete.Add(blocks[x0][y].Body);
                        break;
                    }
                }
                b = recs.Count;
            }

            if (x % 2 == 0)
            {
                int y0 = y - 1, r0 = r;
                while (y0 >= 0 && r0 > 0)
                {
                    if (!fieldOfBattle[x][y0])
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y0));
                        if (fieldOfBombs[x][y0])
                        {
                            for (int k = 0; k < bombs.Count; ++k)
                            {
                                if (Canvas.GetLeft(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(x) &&
                                    Canvas.GetTop(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(y0))
                                {
                                    bombs.ElementAt(k).Iterator = 175;
                                }
                            }
                        }
                        --y0; --r0;
                    }
                    else
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y0));
                        toDelete.Add(blocks[x][y0].Body);
                        break;
                    }
                }
                c = recs.Count;
                y0 = y + 1; r0 = r;
                while (y0 < heightOfFieldOfBattle && r0 > 0)
                {
                    if (!fieldOfBattle[x][y0])
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y0));
                        if (fieldOfBombs[x][y0])
                        {
                            for (int k = 0; k < bombs.Count; ++k)
                            {
                                if (Canvas.GetLeft(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(x) &&
                                    Canvas.GetTop(bombs.ElementAt(k).Body) == HelpfulFunctions.AbsoluteCoord(y0))
                                {
                                    bombs.ElementAt(k).Iterator = 175;
                                }
                            }
                        }
                        ++y0; --r0;
                    }
                    else
                    {
                        recs.Add(new Rectangle
                        {
                            Width = Game_Logic.Object.standartSize,
                            Height = Game_Logic.Object.standartSize
                        });
                        Canvas.SetLeft(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(x));
                        Canvas.SetTop(recs.ElementAt(recs.Count - 1), HelpfulFunctions.AbsoluteCoord(y0));
                        toDelete.Add(blocks[x][y0].Body);
                        break;
                    }
                }
            }
            else c = b;

            for (int i = 0; i < recs.Count; ++i)
            {
                Panel.SetZIndex(recs.ElementAt(i), 30);
                grid1.Children.Add(recs.ElementAt(i));
            }

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(40)
            };
            int j = 0;
            timer.Tick += (object obj, EventArgs e) => ExplosingTextureChanging(obj, e, j++, recs, a, b, c, toDelete);
            timer.Tick += (object obj, EventArgs e) => CheckDamage(obj, e, HelpfulFunctions.AbsoluteCoord(x), HelpfulFunctions.AbsoluteCoord(y),
                HelpfulFunctions.AbsoluteCoord(a - 1), HelpfulFunctions.AbsoluteCoord(c - b), HelpfulFunctions.AbsoluteCoord(b - a),
                HelpfulFunctions.AbsoluteCoord(recs.Count - c));
            timer.Start();
        }

        private void ExplosingTextureChanging(object obj, EventArgs e, int i, List<Rectangle> recs, int a, int b, int c, List<Rectangle> toDelete)
        {
            if (i == 0)
            {
                for (int k = 0; k < toDelete.Count; ++k) toDelete.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Battle.burnedLeaves);
            }
            if (i == 0 || i == 6)
            {
                recs.ElementAt(0).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[0][0]);
                for (int k = 1; k < a - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[1][0]);
                if (a > 1) recs.ElementAt(a - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[2][0]);
                for (int k = a; k < b - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[5][0]);
                if (b > a) recs.ElementAt(b - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[6][0]);
                for (int k = b; k < c - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[3][0]);
                if (c > b) recs.ElementAt(c - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[4][0]);
                for (int k = c; k < recs.Count - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[7][0]);
                if (recs.Count > c) recs.ElementAt(recs.Count - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[8][0]);
            }
            else if (i == 1 || i == 5)
            {
                recs.ElementAt(0).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[0][1]);
                for (int k = 1; k < a - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[1][1]);
                if (a > 1) recs.ElementAt(a - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[2][1]);
                for (int k = a; k < b - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[5][1]);
                if (b > a) recs.ElementAt(b - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[6][1]);
                for (int k = b; k < c - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[3][1]);
                if (c > b) recs.ElementAt(c - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[4][1]);
                for (int k = c; k < recs.Count - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[7][1]);
                if (recs.Count > c) recs.ElementAt(recs.Count - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[8][1]);
            }
            else if (i == 2 || i == 4)
            {
                recs.ElementAt(0).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[0][2]);
                for (int k = 1; k < a - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[1][2]);
                if (a > 1) recs.ElementAt(a - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[2][2]);
                for (int k = a; k < b - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[5][2]);
                if (b > a) recs.ElementAt(b - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[6][2]);
                for (int k = b; k < c - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[3][2]);
                if (c > b) recs.ElementAt(c - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[4][2]);
                for (int k = c; k < recs.Count - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[7][2]);
                if (recs.Count > c) recs.ElementAt(recs.Count - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[8][2]);
            }
            else if (i == 3)
            {
                recs.ElementAt(0).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[0][3]);
                for (int k = 1; k < a - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[1][3]);
                if (a > 1) recs.ElementAt(a - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[2][3]);
                for (int k = a; k < b - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[5][3]);
                if (b > a) recs.ElementAt(b - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[6][3]);
                for (int k = b; k < c - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[3][3]);
                if (c > b) recs.ElementAt(c - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[4][3]);
                for (int k = c; k < recs.Count - 1; ++k) recs.ElementAt(k).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[7][3]);
                if (recs.Count > c) recs.ElementAt(recs.Count - 1).Fill = HelpfulFunctions.BitmapToBrush(Bomb.explosions[8][3]);
            }
            else
            {
                int x1, y1;
                for (int k = 0; k < recs.Count; ++k) grid1.Children.Remove(recs.ElementAt(k));
                for (int k = 0; k < toDelete.Count; ++k)
                {
                    x1 = HelpfulFunctions.RelativeCoord(Canvas.GetLeft(toDelete.ElementAt(k)));
                    y1 = HelpfulFunctions.RelativeCoord(Canvas.GetTop(toDelete.ElementAt(k)));
                    fieldOfBattle[x1][y1] = false;
                    grid1.Children.Remove(toDelete.ElementAt(k));
                }
                ((DispatcherTimer)obj).Stop();
            }
        }

        private void CheckDamage(object obj, EventArgs e, double x, double y, double left, double up, double right, double down)
        {
            Rectangle elem; double x1, y1;
            if (player.Body != null)
            {
                elem = player.Body; x1 = Canvas.GetLeft(elem); y1 = Canvas.GetTop(elem);
                if (x1 + elem.Width < x + elem.Width * coefOfDeath)
                {
                    if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                    {
                        if (x1 + elem.Width >= x - left + elem.Width * coefOfDeath)
                        {
                            Death(player);
                        }
                    }
                }
                else if (x1 < x + elem.Width - elem.Width * coefOfDeath)
                {
                    if (y1 + elem.Height >= y - up + elem.Height * coefOfDeath && y1 < y + down + elem.Height - elem.Height * coefOfDeath)
                    {
                        Death(player);
                    }
                }
                else
                {
                    if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                    {
                        if (x1 < x + right + elem.Width - elem.Width * coefOfDeath)
                        {
                            Death(player);
                        }
                    }
                }
            }

            for (int i = 0; i < numberOfEnemies; ++i)
            {
                elem = enemies[i].Body; x1 = Canvas.GetLeft(elem); y1 = Canvas.GetTop(elem);
                if (x1 + elem.Width < x + elem.Width * coefOfDeath)
                {
                    if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                    {
                        if (x1 + elem.Width >= x - left + elem.Width * coefOfDeath)
                        {
                            Death(enemies[i]);
                        }
                    }
                }
                else if (x1 < x + elem.Width - elem.Width * coefOfDeath)
                {
                    if (y1 + elem.Height >= y - up + elem.Height * coefOfDeath && y1 < y + down + elem.Height - elem.Height * coefOfDeath)
                    {
                        Death(enemies[i]);
                    }
                }
                else
                {
                    if (y1 + elem.Height >= y + elem.Height * coefOfDeath && y1 < y + elem.Height - elem.Height * coefOfDeath)
                    {
                        if (x1 < x + right + elem.Width - elem.Width * coefOfDeath)
                        {
                            Death(enemies[i]);
                        }
                    }
                }
            }
        }

        private void Death(Creature creature)
        {
            Rectangle rec = new Rectangle
            {
                Width = creature.Body.Width,
                Height = creature.Body.Height,
                Fill = HelpfulFunctions.BitmapToBrush(Creature.texturesOfDeath[0])
            };
            Canvas.SetLeft(rec, Canvas.GetLeft(creature.Body));
            Canvas.SetTop(rec, Canvas.GetTop(creature.Body));
            Panel.SetZIndex(rec, Panel.GetZIndex(creature.Body));
            grid1.Children.Add(rec);


            DispatcherTimer _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };
            int k = 0;

            if (creature.Equals(player))
            {
                timer.Tick -= Player_Moving;
                if (numberOfEnemies != 0) _timer.Tick += (object o, EventArgs ev) => DeathTextureChanging(o, ev, rec, k++, true, false);
                else _timer.Tick += (object o, EventArgs ev) => DeathTextureChanging(o, ev, rec, k++, true, true);
            }
            else
            {
                for (int i = 0; i < numberOfEnemies; ++i)
                {
                    if (creature.Equals(enemies[i]))
                    {
                        enemies.RemoveAt(i);
                        break;
                    }
                }
                --numberOfEnemies;
                if (numberOfEnemies != 0) _timer.Tick += (object o, EventArgs ev) => DeathTextureChanging(o, ev, rec, k++, false, false);
                else _timer.Tick += (object o, EventArgs ev) => DeathTextureChanging(o, ev, rec, k++, false, true);
            }
            grid1.Children.Remove(creature.Body);
            creature.Body = null;
            _timer.Start();
        }

        private void DeathTextureChanging(object obj, EventArgs e, Rectangle rec, int counter, bool restart, bool isClear)
        {
            if (counter < 10) rec.Fill = HelpfulFunctions.BitmapToBrush(Creature.texturesOfDeath[counter]);
            else
            {
                grid1.Children.Remove(rec);
                ((DispatcherTimer)obj).Stop();
                if (restart)
                {
                    --player.Health;
                    var _timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(2)
                    };
                    _timer.Tick += (object o, EventArgs ev) =>
                    {
                        ((DispatcherTimer)o).Stop();
                        ((MainWindow)Parent).Content = new Battle11();
                    };
                    _timer.Start();
                } else if (isClear)
                {
                    ((MainWindow)Parent).Content = new MainMenu();
                }
            }
        }
    }
}
