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

        bool ActiveTimer { get; set; }
        // Graphics Map { get; }
        PictureBox Map { get; }
    }
}
