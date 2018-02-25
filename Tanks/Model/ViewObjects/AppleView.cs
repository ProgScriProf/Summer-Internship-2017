﻿using Model.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewObjects
{
    public class AppleView : Apple
    {
        protected SpriteList _sprite;

        public AppleView(float x, float y, int w, int h) : base(x, y, w, h)
        {
            _sprite = new SpriteList(0, 170, w, h, 20, 9);
        }

        public void SetSprite(float dx)
        {
            _sprite.SetSprite(dx);
        }

        public void Draw(Graphics g)
        {
            _sprite.Draw(g, X, Y);
        }
    }
}
