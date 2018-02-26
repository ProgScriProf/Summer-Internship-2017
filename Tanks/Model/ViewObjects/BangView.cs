using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class BangView : Bang
    {
        protected SpriteListOne _sprite;

        public BangView(float x, float y, int w, int h) : base(x, y, w, h)
        {
            _sprite = new SpriteListOne(0, 200, w, h, 10, 5);
        }

        public void SetSprite(float dx)
        {
            _sprite.SetSprite(dx);
        }

        public void Draw(Graphics g)
        {
            _sprite.Draw(g, X, Y);
        }

        public bool OnFinish()
        {
            return _sprite.OnFinish();
        }
    }
}
