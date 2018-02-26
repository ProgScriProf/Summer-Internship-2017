using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class WallView : Wall
    {
        protected Sprite _sprite;

        public WallView(int x, int y, int width, int height, bool destroyable = false) :  base(x, y, width, height, destroyable)
        {
            if (!destroyable)
            {
                _sprite = new Sprite(50, 120, width, height);
            }
            else
            {
                _sprite = new Sprite(0, 120, width, height);
            }
        }

        public void Draw(Graphics g)
        {
            _sprite.Draw(g, X, Y);
        }
    }
}
