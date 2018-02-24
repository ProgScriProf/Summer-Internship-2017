using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    public class Tank : MobileObject, IShooter
    {
        public float _reload;

        public Tank(int x, int y, ushort direction, int width, int height) : base(x, y, direction, width, height)
        {
            _reload = 1f;
        }

        public float Reload { get => _reload; set => _reload = value; }
    }
        
}
