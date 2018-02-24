using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Model
{
    public class SpriteList
    {
        private static Image _img;
        private int _x;
        private int _y;
        private int _w;
        private int _h;
        private float _speed;
        private int _count;
        private float _index;
        private bool _one;
        private bool _end;


        public static Image Image
        {
            get
            {
                if (_img == null)
                {
                    throw new NullReferenceException("SpriteList not found");
                }
                return _img;
            }
        }

        public SpriteList(int x, int y, int w, int h, float speed = 0, int count = 0, bool one = false)
        {
            // Если карта спрайтов не загружена - загружаем
            if (_img == null)
            {
                LoadFile();
            }

            _x = x;
            _y = y;
            _w = w;
            _h = h;

            _speed = speed;
            _count = count;
            _index = 0;
            _one = one;
            _end = false;
        }

        public void Draw(Graphics g, int x, int y, float dx = 0)
        {
            _index += dx * _speed;
            if (_index > _count)
            {
                if (_one) // Если один раз
                {
                    _end = true;
                    _index = _count - 1;
                }
                else // если циклично
                {
                    _index -= _count;
                }
            }

            g.DrawImage(_img, new Rectangle(x, y, _w, _h), new Rectangle(_x + (int)_index * _w, _y, _w, _h), GraphicsUnit.Pixel);      
        }

        private void LoadFile()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "SpriteList.png");
            if (File.Exists(path))
            {
                _img = Image.FromFile(path);
            }
            else
            {
                throw new FileNotFoundException("File sprites not found");
            }
        }

        public bool OnFinish()
        {
            return _end;
        }

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
