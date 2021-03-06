﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Game_Bomberman.Game_Logic
{
    class Stone : Block
    {
        public Stone()
        {
            Health = 1000;
            MinDamageToHit = 1000;
            Texture = Battle.block;
            Body = new System.Windows.Shapes.Rectangle
            {
                Width = standartSize,
                Height = standartSize,
                Fill = HelpfulFunctions.BitmapToBrush(Texture)
            };
            Panel.SetZIndex(Body, 10);
        }

        public override void ActionWhenDamaged(object sender, EventArgs e) { }
        public override void ActionWhenDying(object sender, EventArgs e) { }
    }
}
