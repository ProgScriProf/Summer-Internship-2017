using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class BulletView : Bullet
    {
        protected SpriteList _sprite;

        public BulletView(float x, float y, ushort direction, int width, int height, bool kill) : base (x, y, direction, width, height, kill)
        {
            // Тут передаем координаты на спрайте
            _sprite = new SpriteList(180, height * Direction, width, height, 0, 1);
        }

        public void Draw(Graphics g)
        {
            _sprite.Draw(g, X, Y);
        }
    }
}
