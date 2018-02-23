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
    public class GameController : IGameController
    {
        private IViewController _view;
        private IGameObjects _objects;

        public GameController(IViewController view, IGameObjects objects)
        {
            _view = view;
            _objects = objects;
        }

        public void StartGame()
        {
            Graphics g = _view.Map.CreateGraphics();
            g.DrawRectangle(new Pen(Color.Black), 30, 30, 50, 70);
        }

        
        public void KeyDown(Keys key)
        {
            throw new NotImplementedException();
        }

        public void NewGame()
        {
            StartGame();
        }
    }
}
