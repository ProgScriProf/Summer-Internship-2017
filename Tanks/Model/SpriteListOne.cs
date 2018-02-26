using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SpriteListOne : SpriteList
    {
        private bool _end;

        public SpriteListOne(int x, int y, int w, int h, float speed, int count) : base(x, y, w, h, speed, count)
        {

        }

        public override void SetSprite(float dx)
        {
            _index += dx * _speed;
            if (_index > _count)
            {
                _end = true;
                _index = _count - 1;
            }
        }

        public bool OnFinish()
        {
            return _end;
        }
    }
}
