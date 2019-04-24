using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_Bomberman
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        const int numberOfSaves = 2;
        DispatcherTimer timer1;
        public MainMenu()
        {
            InitializeComponent();
            logoImage.Width = quitImage.Width = MainWindow.width;
            logoImage.Height = logoImage.Height = MainWindow.height;
            ButtonsPanel.Width = MainWindow.width / 2.0;
            Canvas.SetLeft(ButtonsPanel, MainWindow.width / 4.0);
            Canvas.SetBottom(ButtonsPanel, MainWindow.height / 4.0);
            gamePanel.Width = MainWindow.width / 2.0;
            gamePanel.Height = MainWindow.height * 7.0 / 8.0;
            Canvas.SetLeft(gamePanel, MainWindow.width / 4.0);
            Canvas.SetTop(gamePanel, MainWindow.height / 16.0);
            quitButtonsPanel.Width = MainWindow.width / 2.4;
            quitButtonsPanel.Height = MainWindow.height / 8.0;
            Canvas.SetLeft(quitButtonsPanel, (MainWindow.width - quitButtonsPanel.Width) / 2.0);
            Canvas.SetTop(quitButtonsPanel, MainWindow.height / 2.0);
            ShowLogo();
        }

        private void ShowLogo()
        {
            var logoImageAnimation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(1.5))
            {
                BeginTime = TimeSpan.FromSeconds(1),
                AccelerationRatio = 0.5
            };
            logoImageAnimation.Completed += IncludingLabel;
            logoImage.BeginAnimation(OpacityProperty, logoImageAnimation);
        }

        private void IncludingLabel(object obj, EventArgs e)
        {
            menuMusic.MediaEnded += (object o, RoutedEventArgs ev) => { menuMusic.Play(); };
            menuMusic.Play();
            ((AnimationClock) obj).Completed -= IncludingLabel;
            ButtonsPanel.IsEnabled = true;
            ButtonsPanel.Children.Add(new Label()
            {
                Style = (Style)FindResource("StarWarsStyle"),
                Content = Menu.pressAnyButton,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Opacity = 0.0
            });
            //Keyboard.FocusedElement.KeyDown += IncludingButtons;
            ((MainWindow)Parent).KeyDown += IncludingButtons;
            Tmr_Tick(((Label)ButtonsPanel.Children[0]).Opacity);
        }
        
        private async void Tmr_Tick(double opacity)
        {
            ((Label)ButtonsPanel.Children[0]).Opacity = await IsOpacityFull(((Label)ButtonsPanel.Children[0]).Opacity);
            if (ButtonsPanel.Children[0].GetType() == typeof(Label)) Tmr_Tick(((Label)ButtonsPanel.Children[0]).Opacity);
        }

        private Task<double> IsOpacityFull(double opacity) => Task.Run(() =>
        {
            Thread.Sleep(500);
            return opacity == 1.0 ? 0.0 : 1.0;
        });

        private void IncludingButtons(object obj, KeyEventArgs e)
        {
            Keyboard.FocusedElement.KeyDown -= IncludingButtons;
            ButtonsPanel.Children.RemoveAt(0);
            for (int i = 0; i < 4; ++i)
            {
                var btn = new Button()
                {
                    Style = (Style)FindResource("StarWarsButtonStyle")
                };
                ButtonsPanel.Children.Add(btn);
            }
            ((Button)(ButtonsPanel.Children[0])).Content = Menu.newGame;
            ((Button)(ButtonsPanel.Children[0])).Click += OnClickNew;
            ((Button)(ButtonsPanel.Children[0])).IsDefault = true;
            ((Button)(ButtonsPanel.Children[0])).Focus();
            ((Button)(ButtonsPanel.Children[1])).Content = Menu.loadGame;
            ((Button)(ButtonsPanel.Children[1])).Click += OnClickLoad;
            ((Button)(ButtonsPanel.Children[2])).Content = Menu.settings;
            ((Button)(ButtonsPanel.Children[2])).Click += OnClickSettings;
            ((Button)(ButtonsPanel.Children[3])).Content = Menu.quitGame;
            ((Button)(ButtonsPanel.Children[3])).Click += OnClickQuit;
            ((Button)(ButtonsPanel.Children[3])).IsCancel = true;
            timer1 = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            timer1.Tick += (object sender, EventArgs ev) => { timer1.Stop(); };
            timer1.Start();
        }

        private void OnClickNew(object obj, EventArgs e)
        {
            if (timer1.IsEnabled) return;
            const double marginW = 20.0, marginH = 20.0, heightOfBack = 100.0;
            double width = gamePanel.Width - 2 * marginW, height = (gamePanel.Height - heightOfBack) / numberOfSaves - 2 * marginH;
            ButtonsPanel.IsEnabled = false;
            ButtonsPanel.Opacity = 0.0;
            gamePanel.IsEnabled = true;
            gamePanel.Opacity = 1.0;
            for (int i = 0; i < numberOfSaves; ++i)
            {
                gamePanel.Children.Add(new Button()
                {
                    Style = (Style)FindResource("StarWarsButtonStyle"),
                    Content = "SAVE #" + (i + 1),
                    Width = width,
                    Height = height,
                    Margin = new Thickness(marginW, marginH, marginW, 0)
                });
                ((Button)gamePanel.Children[i]).Click += (object sender, RoutedEventArgs ev) =>
                {
                    ((MainWindow)Parent).Content = new Battle11();
                };
            }
            gamePanel.Children.Add(new Button()
            {
                Style = (Style)FindResource("StarWarsButtonStyle"),
                Content = "BACK",
                Width = width,
                Height = heightOfBack,
                IsCancel = true,
                Margin = new Thickness(marginW, marginH, marginW, marginH)
            });
            ((Button)gamePanel.Children[numberOfSaves]).Click += (object sender, RoutedEventArgs ev) =>
            {
                gamePanel.IsEnabled = false;
                quitImage.Opacity = 0.0;
                gamePanel.Children.RemoveRange(0, gamePanel.Children.Count);
                gamePanel.Opacity = 0.0;
                ButtonsPanel.Opacity = 1.0;
                ButtonsPanel.IsEnabled = true;
                ((Button)(ButtonsPanel.Children[0])).Focus();
            };
            ((Button)gamePanel.Children[0]).Focus();
        }

        private void OnClickLoad(object obj, EventArgs e)
        {
            if (timer1.IsEnabled) return;
            const double marginW = 20.0, marginH = 20.0, heightOfBack = 100.0;
            double width = gamePanel.Width - 2 * marginW, height = (gamePanel.Height - heightOfBack) / numberOfSaves - 2 * marginH;
            ButtonsPanel.IsEnabled = false;
            ButtonsPanel.Opacity = 0.0;
            gamePanel.IsEnabled = true;
            gamePanel.Opacity = 1.0;
            for (int i = 0; i < numberOfSaves; ++i)
            {
                gamePanel.Children.Add(new Button()
                {
                    Style = (Style)FindResource("StarWarsButtonStyle"),
                    Content = "SAVE #" + (i + 1),
                    Width = width,
                    Height = height,
                    Margin = new Thickness(marginW, marginH, marginW, 0)
                });
                ((Button)gamePanel.Children[i]).Click += (object sender, RoutedEventArgs ev) =>
                {
                    ((MainWindow)Parent).Content = new Battle11();
                };
            }
            gamePanel.Children.Add(new Button()
            {
                Style = (Style)FindResource("StarWarsButtonStyle"),
                Content = "BACK",
                Width = width,
                Height = heightOfBack,
                IsCancel = true,
                Margin = new Thickness(marginW, marginH, marginW, marginH)
            });
            ((Button)gamePanel.Children[numberOfSaves]).Click += (object sender, RoutedEventArgs ev) =>
            {
                gamePanel.IsEnabled = false;
                quitImage.Opacity = 0.0;
                gamePanel.Children.RemoveRange(0, gamePanel.Children.Count);
                gamePanel.Opacity = 0.0;
                ButtonsPanel.Opacity = 1.0;
                ButtonsPanel.IsEnabled = true;
                ((Button)(ButtonsPanel.Children[0])).Focus();
            };
            ((Button)gamePanel.Children[0]).Focus();
        }

        private void OnClickSettings(object obj, EventArgs e)
        {

        }

        private void OnClickQuit(object obj, EventArgs e)
        {
            if (timer1.IsEnabled) return;
            const double width = 250.0, margin = 50.0;
            ButtonsPanel.IsEnabled = false;
            ButtonsPanel.Opacity = 0.0;
            quitImage.Opacity = 1.0;
            quitButtonsPanel.IsEnabled = true;
            quitButtonsPanel.Children.Add(new Button()
            {
                Style = (Style)FindResource("StarWarsButtonStyle"),
                Content = Menu.ok,
                IsDefault = true,
                Width = width,
                Margin = new Thickness(margin, 0, 2 * (quitButtonsPanel.Width / 2 - width - margin), 0)
            });
            ((Button)quitButtonsPanel.Children[0]).Click += (object sender, RoutedEventArgs ev) =>
            {
                ((MainWindow)Parent).Close();
            };
            quitButtonsPanel.Children.Add(new Button()
            {
                Style = (Style)FindResource("StarWarsButtonStyle"),
                Content = Menu.cancel,
                IsCancel = true,
                Width = width,
                Focusable = true
            });
            ((Button)quitButtonsPanel.Children[1]).Click += (object sender, RoutedEventArgs ev) =>
            {
                quitButtonsPanel.IsEnabled = false;
                quitImage.Opacity = 0.0;
                quitButtonsPanel.Children.RemoveRange(0, quitButtonsPanel.Children.Count);
                ButtonsPanel.Opacity = 1.0;
                ButtonsPanel.IsEnabled = true;
                ((Button)(ButtonsPanel.Children[0])).Focus();
            };
            ((Button)quitButtonsPanel.Children[0]).Focus();
        }
    }
}
