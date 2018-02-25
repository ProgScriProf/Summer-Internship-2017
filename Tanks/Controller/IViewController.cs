using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller
{
    public interface IViewController
    {
        void SetController(IGameController controller);
        void Render(bool isGame = true);

        bool ActiveTimer { get; set; }
        int MapWidth { get; }
        int MapHeight { get; }
    }
}
