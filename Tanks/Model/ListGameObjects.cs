using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ListGameObjects : IGameObjects
    {
        Kolobok _player;
        List<Tank> _tanks;

        public Kolobok Player { get => _player; set => _player = value; }
        public List<Tank> Tanks { get => _tanks; set => _tanks = value; }
    }
}
