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

        public static System.Drawing.Bitmap[][] explosions = new System.Drawing.Bitmap[9][]
        {
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionCenter1, Battle.explosionCenter2, Battle.explosionCenter3, Battle.explosionCenter4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionLeft1, Battle.explosionLeft2, Battle.explosionLeft3, Battle.explosionLeft4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionLeftEnding1, Battle.explosionLeftEnding2, Battle.explosionLeftEnding3, Battle.explosionLeftEnding4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionUp1, Battle.explosionUp2, Battle.explosionUp3, Battle.explosionUp4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionUpEnding1, Battle.explosionUpEnding2, Battle.explosionUpEnding3, Battle.explosionUpEnding4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionRight1, Battle.explosionRight2, Battle.explosionRight3, Battle.explosionRight4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionRightEnding1, Battle.explosionRightEnding2, Battle.explosionRightEnding3, Battle.explosionRightEnding4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionDown1, Battle.explosionDown2, Battle.explosionDown3, Battle.explosionDown4
            },
            new System.Drawing.Bitmap[4]
            {
                Battle.explosionDownEnding1, Battle.explosionDownEnding2, Battle.explosionDownEnding3, Battle.explosionDownEnding4
            }
        };

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
