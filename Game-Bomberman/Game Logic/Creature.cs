using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Bomberman.Game_Logic
{
    abstract class Creature : Object
    {
        private ushort speed;
        private ushort[] buffs;

        public const ushort maxNumberOfBuffs = 7;

        public ushort Speed { get => speed; set => speed = value; }
        public ushort[] Buffs { get => buffs; set => buffs = value; }

        public abstract void ActionWhenMove(object sender, EventArgs e);
        public abstract void ActionWhenAttack(object sender, EventArgs e);
    }
}
