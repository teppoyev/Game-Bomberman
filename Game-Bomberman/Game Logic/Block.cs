using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Bomberman.Game_Logic
{
    abstract class Block : Object
    {
        private ushort minDamageToHit;

        public ushort MinDamageToHit { get => minDamageToHit; set => minDamageToHit = value; }
    }
}
