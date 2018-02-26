using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Sprite
    {
        protected static Image _img;
        protected int _x;
        protected int _y;
        protected int _w;
        protected int _h;

        public Sprite(int x, int y, int w, int h)
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

        public virtual void Draw(Graphics g, int x, int y)
        {
            g.DrawImage(_img, new Rectangle(x, y, _w, _h), new Rectangle(_x, _y, _w, _h), GraphicsUnit.Pixel);
        }
     
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
       
    }
}
