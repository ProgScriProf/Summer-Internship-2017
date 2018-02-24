using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class TankView : Tank
    {
        protected SpriteList _sprite;

        public TankView(int x, int y, ushort direction, int width, int height) : base(x, y, direction, width, height)
        {
            // Тут передаем координаты на спрайте
            _sprite = new SpriteList(120, 0, width, height, 15, 2);
        }

        public void Draw(Graphics g, float dx)
        {
            _sprite.SetPosition(120, Direction * Height);
            _sprite.Draw(g, X, Y, dx);
        }

    }
}
