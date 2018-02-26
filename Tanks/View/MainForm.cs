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
using Model;

namespace View
{
    public partial class MainForm : Form, IViewController
    {
        IGameController _controller;
        IGameObjects _objects;

        public bool ActiveTimer { get => timer.Enabled; set => timer.Enabled = value; }
        public int MapWidth => imgMap.Width;
        public int MapHeight => imgMap.Height;

        public MainForm(IGameObjects obj)
        {
            InitializeComponent();
            _objects = obj;
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

        public void Render(bool isGame = true)
        {
            Bitmap bm = new Bitmap(imgMap.Width, imgMap.Height);
            Graphics g = Graphics.FromImage(bm);

            // Закрышиваем
            g.Clear(Color.Black);

            // Рисуем преграды
            _objects.Walls.ForEach(wall => wall.Draw(g));

            // Рисуем воду
            _objects.Water.ForEach(water => water.Draw(g));

            // Рисуем пули
            _objects.Bullets.ForEach(bullet => bullet.Draw(g));

            // Рисуем противников
            _objects.Tanks.ForEach(tank => tank.Draw(g));

            // Рисуем яблоки
            _objects.Apples.ForEach(apple => apple.Draw(g));

            // Рисуем взрывы
            _objects.Bangs.ForEach(bang => bang.Draw(g));

            // Рисуем подсказки
            g.DrawString("Press P for show LOG\nPress R for Restart", new Font(FontFamily.GenericSansSerif, 14),
                                new SolidBrush(Color.White), new PointF(5, 500));

            if (isGame)
            {
                // Рисуем игрока
                _objects.Player.Draw(g);

                // Рисуем очки
                g.DrawImage(Sprite.Image, new Rectangle(5, 8, 30, 30), new Rectangle(0, 170, 30, 30), GraphicsUnit.Pixel);

                g.DrawString(_objects.Score.ToString(), new Font(FontFamily.GenericSansSerif, 22, FontStyle.Bold),
                                new SolidBrush(Color.White), new PointF(35, 5));
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.Black)),
                    new Rectangle(0, 0, MapWidth, MapHeight));

                g.DrawString("Game Over", new Font(FontFamily.GenericSansSerif, 72, FontStyle.Bold),
                                new SolidBrush(Color.LightGray), new PointF(40, 100));

                g.DrawString($"Score: {_objects.Score}", new Font(FontFamily.GenericSansSerif, 32, FontStyle.Bold),
                                new SolidBrush(Color.LightGray), new PointF(240, 220));

                if (DateTime.Now.Millisecond < 500)
                {
                    g.DrawString("Try again", new Font(FontFamily.GenericSansSerif, 28, FontStyle.Bold),
                                    new SolidBrush(Color.LightGray), new PointF(240, 310));
                }
            }

            imgMap.Image = bm;
        }
    }
}
