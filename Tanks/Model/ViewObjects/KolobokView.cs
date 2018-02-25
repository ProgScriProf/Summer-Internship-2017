using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class KolobokView : Kolobok
    {
        protected SpriteList _sprite;
       
        public KolobokView(int x, int y, ushort direction, int height, int width) : base(x, y, direction, height, width)
        {
            // Тут передаем координаты на спрайте
            _sprite = new SpriteList(0, 0, width, height, 15, 4);
        }

        public void SetSprite(float dx)
        {
            _sprite.SetSprite(dx);
        }

        public void Draw(Graphics g)
        {
            _sprite.SetPosition(0, Direction * Height);
            _sprite.Draw(g, X, Y);
        }

    }
}
