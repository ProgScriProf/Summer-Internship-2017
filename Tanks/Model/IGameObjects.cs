using Model.GameObjects;
using Model.ViewObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IGameObjects 
    {
        KolobokView Player { get; set; }
        List<WallView> Walls { get; set; }
        List<TankView> Tanks { get; set; }
        List<BulletView> Bullets { get; set; }
        List<AppleView> Apples { get; set; }
        List<BangView> Bangs { get; set; }
        List<WaterView> Water { get; set; }
    }
}
