using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    // Главный игрок
    public class Kolobok : MobileObject, IShooter
    {
        public float _reload;

        public Kolobok(int x, int y, ushort direction, int width, int height) : base(x, y, direction, width, height)
        {
        }

        public float Reload { get => _reload; set => _reload = value; }

        public void OutScreen()
        {
            X = -Width;
            Y = -Height;
        }
    }
}
