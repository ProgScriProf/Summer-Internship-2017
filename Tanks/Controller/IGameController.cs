using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller
{
    public interface IGameController
    {
        void NewGame();
        void KeyDown(Keys key);
        void Timer();
    }
}
