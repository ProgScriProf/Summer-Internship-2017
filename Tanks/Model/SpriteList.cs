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

        public SpriteList(int x, int y, int w, int h, float speed = 0, int count = 0)
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
        }

        public void Draw(Graphics g, int x, int y, float dx = 0)
        {
            _index += dx * _speed;
            if (_index > _count)
            {
                _index -= _count;
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

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

       
    }
}
