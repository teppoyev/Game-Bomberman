using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Bomberman.Game_Logic
{
    abstract class Enemy : Creature
    {
        private int direction;

        public int Direction { get => direction; set => direction = value; }
    }
}
