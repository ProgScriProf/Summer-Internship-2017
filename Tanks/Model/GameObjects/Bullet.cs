using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    public class Bullet : MobileObject
    {
        public bool isKill;
        public Bullet(float x, float y, ushort direction, int width, int height, bool kill) : base (x, y, direction, width, height)
        {
            speed = speed * 4;
            isKill = kill;
        }
    }
}
