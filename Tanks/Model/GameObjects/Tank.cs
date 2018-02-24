using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    public class Tank : MobileObject
    {
        public Tank(int x, int y, ushort direction, int width, int height) : base(x, y, direction, width, height)
        {

        }
    }
        
}
