using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Bomberman.Game_Logic
{
    class Player : Creature
    {
        private ushort numberOfBombs;
        private ushort rangeOfExplosion;
        private ushort damageOfExplosion;

        public ushort NumberOfBombs { get => numberOfBombs; set => numberOfBombs = value; }
        public ushort RangeOfExplosion { get => rangeOfExplosion; set => rangeOfExplosion = value; }
        public ushort DamageOfExplosion { get => damageOfExplosion; set => damageOfExplosion = value; }

        public Player()
        {
            Speed = 5;
            Health = 3;
            X = 0; Y = 0;
            NumberOfBombs = 1;
            RangeOfExplosion = 1;
            DamageOfExplosion = 10;
            Buffs = new ushort[maxNumberOfBuffs];
            for (int i = 0; i < maxNumberOfBuffs; ++i)
            {
                Buffs[i] = 0;
            }
            Texture = Battle.player;
            Body = new System.Windows.Shapes.Rectangle
            {
                Width = standartSize,
                Height = standartSize,
                Fill = HelpfulFunctions.BitmapToBrush(Texture),
                Focusable = true
            };
        }
        public Player(ushort _speed, ushort _health, double _x, double _y, ushort _numberOfBombs,
            ushort _rangeOfExplosion, ushort _damageOfExplosion, ushort[] _buffs)
        {
            Speed = _speed;
            Health = _health;
            X = _x; Y = _y;
            NumberOfBombs = _numberOfBombs;
            RangeOfExplosion = _rangeOfExplosion;
            DamageOfExplosion = _damageOfExplosion;
            Buffs = new ushort[maxNumberOfBuffs];
            for (int i = 0; i < maxNumberOfBuffs; ++i)
            {
                Buffs[i] = _buffs[i];
            }
            Texture = Battle.player;
            Body = new System.Windows.Shapes.Rectangle
            {
                Width = standartSize,
                Height = standartSize,
                Fill = HelpfulFunctions.BitmapToBrush(Texture),
                Focusable = true
            };
        }
        
        public override void ActionWhenMove(object sender, EventArgs e) { }
        public override void ActionWhenAttack(object sender, EventArgs e) { }
        public override void ActionWhenDamaged(object sender, EventArgs e) { }
        public override void ActionWhenDying(object sender, EventArgs e) { }
    }
}
