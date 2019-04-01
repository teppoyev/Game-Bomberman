using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Game_Bomberman.Game_Logic
{
    class Bomb : Object
    {
        private readonly DispatcherTimer timer;
        private int iterator;
        public Bomb()
        {
            Health = 1;
            Texture = Battle.bomb;
            Body = new System.Windows.Shapes.Rectangle
            {
                Width = standartSize,
                Height = standartSize,
                Fill = HelpfulFunctions.BitmapToBrush(Texture)
            };
            Panel.SetZIndex(Body, 50);
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            Iterator = 0;
        }

        public DispatcherTimer Timer => timer;

        public int Iterator { get => iterator; set => iterator = value; }

        public override void ActionWhenDamaged(object sender, EventArgs e)
        {
            
        }
        public override void ActionWhenDying(object sender, EventArgs e)
        {
            
        }
    }
}
