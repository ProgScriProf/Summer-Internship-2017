using Model;
using Model.GameObjects;
using Model.ViewObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Controller
{
    public class GameController : IGameController
    {
        public int BlockSize = 50;
        private IViewController _view;
        private IGameObjects _objects;

        private Stopwatch timer;
        private Random rnd;

        private void MainLoop()
        {
            float dt = timer.ElapsedMilliseconds / 1000f;
            timer.Restart();

            // Если врагов мало, то с шансом 1% генерируем нового
            CreateTank(2);

            // С шансом 1/200 поворачиваем танк
            RotateTank(0.5f);
            
            // Проверяем столкновения и сдвигаем объекты
            Collision(dt);
          
            Draw(_view.Map, dt);
        }

        private void RotateTank(float percent)
        {
            _objects.Tanks.ForEach(tank =>
            {
                if (rnd.Next(100) < percent)
                {
                    tank.ChangeDirection((ushort)rnd.Next(4));
                }
            });
        }

        private void CreateTank(int percent)
        {
            if (_objects.Tanks.Count < 5 && rnd.Next(100) <= percent)
            {
                int x = rnd.Next(_view.Map.Width - 30);
                int y = rnd.Next(_view.Map.Height - 30);
                var tank = new TankView(x, y, (ushort)rnd.Next(4), 30, 30);

                // Если он на свободном месте генерируется
                if (_objects.Walls.Find(wall => ObjectCollision(wall, tank)) == null &&
                    ObjectCollision(_objects.Player, tank) == false &&
                    _objects.Tanks.Find(tnk => tnk != tank ? ObjectCollision(tnk, tank) : false) == null)
                {
                    _objects.Tanks.Add(tank);
                }

            }
        }

        private bool ObjectCollision(GameObject obj1, GameObject obj2)
        {
            return (obj1.X + obj1.Width > obj2.X) &&
                (obj1.X <= obj2.X + obj2.Width) &&
                (obj1.Y + obj1.Height > obj2.Y) &&
                (obj1.Y <= obj2.Y + obj2.Height);
        }

        private void Collision(float dt)
        {
            // Создаем нового игрока, смотрим его новое положение
            MobileObject player = _objects.Player.Clone() as MobileObject;
            player.Step(dt);


            // Смотрим, чтобы не вышел за карту и проверяем, и потом проверяем
            // чтобы не было столкновений с объектами "стена"
            if (!(player.X < 0 || player.X + player.Width > _view.Map.Width ||
                player.Y < 0 || player.Y + player.Height > _view.Map.Height ||
                _objects.Walls.Find(wall => ObjectCollision(wall, player)) != null))
            {
                // НЕТ СТОЛКНОВЕНИЙ С ПРЕГРАДАМИ
                _objects.Player.Step(dt);
            }

            // Так же выполняем для каждого врага
            _objects.Tanks.ForEach(t =>
            {
                t.Step(dt);
                // Смотрим столкновения с границей и стенами
                if (t.X < 0 || t.X + t.Width > _view.Map.Width || t.Y < 0 || t.Y + t.Height > _view.Map.Height ||
                    _objects.Walls.Find(wall => ObjectCollision(wall, t)) != null ||
                    _objects.Tanks.Find(tnk => tnk != t ? ObjectCollision(tnk, t) : false) != null)
                {
                    // СТОЛКНОВЕНИЕ СО СТЕНОЙ ИЛИ ДРУГИМ ВРАГОМ
                    // Идем в обратном направлении
                    t.ChangeDirection();
                    t.Step(dt);
                }
            }); 



        }

        private void Draw(PictureBox pb, float dt)
        {
            Bitmap bm = new Bitmap(pb.Width, pb.Height);
            Graphics g = Graphics.FromImage(bm);

            // Закрышиваем
            g.Clear(Color.Black);

            // Рисуем преграды
            _objects.Walls.ForEach(wall => wall.Draw(g));

            // Рисуем противников
            _objects.Tanks.ForEach(tank => tank.Draw(g, dt));

            // Рисуем игрока
            _objects.Player.Draw(g, dt);

            pb.Image = bm;
        }

        public GameController(IViewController view, IGameObjects objects)
        {
            _view = view;
            _objects = objects;
            rnd = new Random();
        }

        public void StartGame()
        {
            LoadLevel(1);
            _view.ActiveTimer = true;
            timer = new Stopwatch();
            timer.Start();
            MainLoop();
        }

        public void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                case Keys.Left:
                    _objects.Player.ChangeDirection(MobileObject.left);
                    break;
                case Keys.W:
                case Keys.Up:
                    _objects.Player.ChangeDirection(MobileObject.up);
                    break;
                case Keys.D:
                case Keys.Right:
                    _objects.Player.ChangeDirection(MobileObject.right);
                    break;
                case Keys.S:
                case Keys.Down:
                    _objects.Player.ChangeDirection(MobileObject.down);
                    break;
            }
        }

        public void NewGame()
        {
            StartGame();
        }

        private void LoadLevel(int lvl)
        {
            // Ищем файл уровня
            string path = Path.Combine(Environment.CurrentDirectory, "levels", lvl.ToString() + ".lvl");
            if (File.Exists(path))
            {
                // Загружаем уровень
                _objects.Walls = new List<WallView>();
                _objects.Tanks = new List<TankView>();

                using (StreamReader sr = File.OpenText(path))
                {
                    int y = 0;

                    // Пока не конец файла
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        for(int x = 0; x < line.Length; x++)
                        {
                            // Смотри возмжные блоки
                            switch (line[x])
                            {
                                case 'p': // игрок
                                    _objects.Player = new KolobokView(BlockSize * x, BlockSize * y, 1, 30, 30);
                                    break;
                                case '*': // непробиваемая стена
                                    _objects.Walls.Add(new WallView(BlockSize * x, BlockSize * y, BlockSize, BlockSize, false));
                                    break;
                                case '=': // пробиваемая стена
                                    _objects.Walls.Add(new WallView(BlockSize * x, BlockSize * y, BlockSize, BlockSize, true));
                                    break;
                            }
                        }

                        y++;
                    }
                }
                  
            }
            else
            {
                throw new FileNotFoundException($"Level {lvl} not found");
            }

        }

        public void Timer()
        {
            MainLoop();
        }
    }
}
