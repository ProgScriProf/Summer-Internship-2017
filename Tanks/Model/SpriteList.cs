using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Model
{
    public class SpriteList : Sprite
    {       
        protected float _speed;
        protected int _count;
        protected float _index;        

        public SpriteList(int x, int y, int w, int h, float speed, int count) : base(x, y, w, h)
        {
            _speed = speed;
            _count = count;
            _index = 0;
        }

        public virtual void SetSprite(float dx)
        {
            _index += dx * _speed;
            if (_index > _count)
            {
                _index -= _count;
            }             
        }

        public override void Draw(Graphics g, int x, int y)
        {
            g.DrawImage(_img, new Rectangle(x, y, _w, _h), new Rectangle(_x + (int)_index * _w, _y, _w, _h), GraphicsUnit.Pixel);      
        }

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
