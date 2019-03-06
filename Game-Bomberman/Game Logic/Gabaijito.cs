using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Bomberman.Game_Logic
{
    class Gabaijito : Creature
    {
        public Gabaijito()
        {
            Speed = 5;
            Health = 20;
            X = 0; Y = 0;
            Buffs = new ushort[maxNumberOfBuffs];
            for (int i = 0; i < maxNumberOfBuffs; ++i)
            {
                Buffs[i] = 0;
            }
            Texture = Battle.mob;
            Body = new System.Windows.Shapes.Rectangle
            {
                Width = standartSize,
                Height = standartSize,
                Fill = HelpfulFunctions.BitmapToBrush(Texture)
            };
        }
        public override void ActionWhenMove(object sender, EventArgs e) { }
        public override void ActionWhenAttack(object sender, EventArgs e) { }
        public override void ActionWhenDamaged(object sender, EventArgs e) { }
        public override void ActionWhenDying(object sender, EventArgs e) { }
    }
}
