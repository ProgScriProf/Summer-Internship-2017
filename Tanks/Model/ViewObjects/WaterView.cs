using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class WaterView : Water
    {
        protected SpriteList _sprite;

        public WaterView(float x, float y, int w, int h, int index) : base(x, y, w, h)
        {
            _sprite = new SpriteList(100 + index * w, 120, w, h);
        }

        public void Draw(Graphics g)
        {
            _sprite.Draw(g, X, Y);
        }
    }
}
