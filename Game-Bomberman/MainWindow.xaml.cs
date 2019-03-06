using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Game_Bomberman
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public double width = 1920.0, height = 1080.0;
        public MainWindow()
        {
            InitializeComponent();
            Width = width;
            Height = height;
            ShowLogos();
        }

        private void ShowLogos()
        {
            splashImage.BeginAnimation(OpacityProperty, InitAnimationStart());
        }

        private DoubleAnimation InitAnimationStart()
        {
            var splashImageAnimationStart = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(1.5))
            {
                BeginTime = TimeSpan.FromSeconds(1),
                AccelerationRatio = 0.5
            };
            splashImageAnimationStart.Completed += SplashImageAnimationStart_Completed;

            return splashImageAnimationStart;
        }

        private void SplashImageAnimationStart_Completed(object sender, EventArgs e)
        {
            var splashImageAnimationFinish = new DoubleAnimation(1.0, 0.0, TimeSpan.FromSeconds(1.5))
            {
                BeginTime = TimeSpan.FromSeconds(1),
                DecelerationRatio = 0.5
            };
            splashImageAnimationFinish.Completed += SplashImageAnimationFinish_Completed;
            splashImage.BeginAnimation(OpacityProperty, splashImageAnimationFinish);
        }

        private void SplashImageAnimationFinish_Completed(object sender, EventArgs e)
        {
            var image = new ImageBrush(new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Resources/c_.png")))
            {
                Stretch = Stretch.Uniform
            };
            splashImage.Background = image;

            var splashImageAnimationStart = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(1.5))
            {
                BeginTime = TimeSpan.FromSeconds(1),
                AccelerationRatio = 0.5
            };
            splashImageAnimationStart.Completed += SplashImageAnimationStart2_Completed;

            splashImage.BeginAnimation(OpacityProperty, splashImageAnimationStart);
        }

        private void SplashImageAnimationStart2_Completed(object sender, EventArgs e)
        {
            var splashImageAnimationFinish = new DoubleAnimation(1.0, 0.0, TimeSpan.FromSeconds(1.5))
            {
                BeginTime = TimeSpan.FromSeconds(1),
                DecelerationRatio = 0.5
            };
            splashImageAnimationFinish.Completed += SplashImageAnimationFinish2_Completed;
            splashImage.BeginAnimation(OpacityProperty, splashImageAnimationFinish);
        }

        private void SplashImageAnimationFinish2_Completed(object sender, EventArgs e)
        {
            Content = new MainMenu();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            ((Control)sender).Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            ((Control)sender).Background = Brushes.Transparent;
        }
    }
}

/*<EventTrigger RoutedEvent="Mouse.MouseEnter">
                    <BeginStoryboard>
                        <Storyboard >
                            <ColorAnimation From="Red" To="#FF7F7F" Duration="0:0:0" Storyboard.TargetProperty="Background.Color"/>
                        </Storyboard>
                    </BeginStoryboard>
                </EventTrigger>
                <EventTrigger RoutedEvent="Mouse.MouseLeave">
                    <BeginStoryboard>
                        <Storyboard>
                            <ColorAnimation From="#FF7F7F" To="Red" Duration="0:0:0.5" Storyboard.TargetProperty="Background.Color"/>
                        </Storyboard>
                    </BeginStoryboard>
                </EventTrigger>*/
