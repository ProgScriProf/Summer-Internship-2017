using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    public class GameObject
    {
        protected float _x;
        protected float _y;

        public int X
        {
            get
            {
                return (int)_x;
            }

            protected set
            {
                _x = value;
            }
        }
        public int Y
        {
            get
            {
                return (int)_y;
            }

            protected set
            {
                _y = value;
            }
        }

        public int Width { get; protected set; }
        public int Height { get; protected set; }
    }
}
