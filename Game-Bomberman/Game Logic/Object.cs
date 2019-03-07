using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Bomberman.Game_Logic
{
    abstract class Object
    {
        public static int standartSize = 75;

        private double size = standartSize;
        private ushort health;
        private System.Drawing.Bitmap texture;
        private System.Windows.Shapes.Rectangle body = new System.Windows.Shapes.Rectangle()
        {
            Width = standartSize,
            Height = standartSize,
            Fill = System.Windows.Media.Brushes.Magenta
        };

        public Bitmap Texture { get => texture; set => texture = value; }
        public double Size { get => size; set => size = value; }
        public ushort Health { get => health; set => health = value; }
        public System.Windows.Shapes.Rectangle Body { get => body; set => body = value; }

        public abstract void ActionWhenDamaged(object sender, EventArgs e);
        public abstract void ActionWhenDying(object sender, EventArgs e);
    }
}
