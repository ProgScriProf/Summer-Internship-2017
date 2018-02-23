using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IGameObjects 
    {
        Kolobok Player { get; set; }
        List<Tank> Tanks { get; set; }
    }
}
