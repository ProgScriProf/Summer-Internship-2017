using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    public abstract class StaticObject : GameObject
    {
        public StaticObject(float x, float y, int width, int height)
        {
            _x = x;
            _y = y;
            Width = width;
            Height = height;
        }
    }
}
