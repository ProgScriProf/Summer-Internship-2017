using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class MainForm : Form, IViewController
    {
        IGameController _controller;

        //public Graphics Map { get => imgMap.CreateGraphics(); }
        public PictureBox Map { get => imgMap; }
        public bool ActiveTimer { get => timer.Enabled; set => timer.Enabled = value; }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        { 
            this._controller.KeyDown(e.KeyCode);
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            this._controller.NewGame();
        }


        public void SetController(IGameController controller)
        {
            this._controller = controller;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this._controller.Timer();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
