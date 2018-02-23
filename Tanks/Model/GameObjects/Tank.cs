using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Tank : MobileObject
    {
        public Tank(int x, int y, ushort direction) : base (x, y, direction)
        {

        }

        public Tank(int x, int y) : this(x, y, up)
        {

        }
    }
}
