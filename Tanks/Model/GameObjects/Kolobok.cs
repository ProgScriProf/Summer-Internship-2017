using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    // Главный игрок
    public class Kolobok : MobileObject
    {
        public Kolobok(int x, int y, ushort direction, int width, int height) : base(x, y, direction, width, height)
        {
        }

    }
}
