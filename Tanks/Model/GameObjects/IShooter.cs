using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GameObjects
{
    public interface IShooter
    {
        float Reload { get; set; } // Перезарядка
    }
}
