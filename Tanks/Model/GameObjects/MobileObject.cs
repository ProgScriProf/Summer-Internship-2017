using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    // Двигающийся объект
    // Имеет направление движения и координаты
    public class MobileObject : GameObject, ICloneable
    {
        public const ushort left = 0;
        public const ushort up = 1;
        public const ushort right = 2;
        public const ushort down = 3;

        public float speed = 100;

        protected ushort Direction { get; set; }

        public MobileObject(float x, float y, ushort direction, int width, int height)
        {
            Direction = direction;
            _x = x;
            _y = y;
            Width = width;
            Height = height;
        }

        public MobileObject(float x, float y, ushort direction) : this(x, y, direction, 50, 50)
        {
            
        }

        public MobileObject(float x, float y) : this(x, y, up)
        {

        }        

        public void ChangeDirection(ushort dir)
        {
            if (dir < 0 || dir > 3)
                throw new ArgumentOutOfRangeException($"Direction may be (0-3). Current value: {dir}");

            Direction = dir;
        }

        public void ChangeDirection()
        {
            // Меням направление на противоположное
            Direction = (ushort)((2 << (Direction % 2)) - Direction);
        }
        public void Step(float dx)
        {
            float step = speed * dx;

            // Переходим на шаг
            if (Direction % 2 == 0) // Если по горизонтали
            {
                _x += (Direction - 1) * step;
            }
            else
            {
                _y += (Direction - 2) * step ;
            }
        }

        public object Clone()
        {
            return new MobileObject(X, Y, Direction, Width, Height);
        }
    }
}
