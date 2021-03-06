﻿using Model.GameObjects;
using Model.ViewObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ListGameObjects : IGameObjects
    {
        KolobokView _player;
        List<WallView> _walls;
        List<TankView> _tanks;
        List<BulletView> _bullets;
        List<AppleView> _apples;
        List<BangView> _bangs;
        List<WaterView> _water;
        int _score;

        public KolobokView Player { get => _player; set => _player = value; }
        public List<TankView> Tanks { get => _tanks; set => _tanks = value; }
        public List<WallView> Walls { get => _walls; set => _walls = value; }
        public List<BulletView> Bullets { get => _bullets; set => _bullets = value; }
        public List<AppleView> Apples { get => _apples; set => _apples = value; }
        public List<BangView> Bangs { get => _bangs; set => _bangs = value; }
        public List<WaterView> Water { get => _water; set => _water = value; }
        public int Score { get => _score; set => _score = value; }
    }
}
