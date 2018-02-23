using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // Двигающийся объект
    // Имеет направление движения и координаты
    public abstract class MobileObject
    {
        public const ushort left = 0;
        public const ushort up = 1;
        public const ushort right = 2;
        public const ushort down = 3;

        private ushort _direction;
        private int _x;
        private int _y;

        public int X
        {
            get
            {
                return _x;
            }
        }
        public int Y
        {
            get
            {
                return _y;
            }
        }

        public MobileObject(int x, int y, ushort direction)
        {
            _direction = direction;
            _x = x;
            _y = y;
        }

        public MobileObject(int x, int y) : this(x, y, up)
        {

        }        

        public void ChangeDirection(ushort dir)
        {
            if (dir < 0 || dir > 3)
                throw new ArgumentOutOfRangeException($"Direction may be (0-3). Current value: {dir}");

            _direction = dir;
        }

    }
}
