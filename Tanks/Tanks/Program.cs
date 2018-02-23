using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using View;

namespace Tanks
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Форма игры - вьюха
            MainForm form = new MainForm(); // IViewController

            // Объекты игры (в List) - модель
            IGameObjects objects = new ListGameObjects();
            
            // Контроллер
            IGameController gameController = new GameController(form, objects);

            form.SetController(gameController);
            form.ShowDialog();
        }
    }
}
