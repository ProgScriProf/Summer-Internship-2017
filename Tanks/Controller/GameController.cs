using Model;
using Model.GameObjects;
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
using Model.ViewObjects;

namespace Controller
{
    public class GameController : IGameController
    {
        public int BlockSize = 50;
        private IViewController _view;
        private IGameObjects _objects;

        private Stopwatch timer;
        private Random rnd;

        private bool isGame;

        private void MainLoop()
        {
            float dt = timer.ElapsedMilliseconds / 1000f;
            timer.Restart();

            // Убираем перезарядку
            ResetReload(_objects.Player, dt);
            _objects.Tanks.ForEach(tank => ResetReload(tank, dt));
            

            // Если врагов мало, то с шансом 2% генерируем нового
            CreateTank(2);

            // Если яблок мало, то с шансом 1% генерируем нового
            CreateApple(1);

            // С шансом 1/200 поворачиваем танк
            RotateTank(0.5f);
  
            
            // Проверяем столкновения и сдвигаем объекты
            Collision(dt);

            // Смотрим, если перед танком игрок - стреляем
            ShootTanks();

            // Смещаем спрайты анимации
            SetSprites(dt);

            // Если закнчился взрыв, удаляем его
            _objects.Bangs.Where(bang => bang.OnFinish()).ToList()
                 .ForEach(bang => _objects.Bangs.Remove(bang));

            _view.Render(isGame);
            //Draw(_view.Map, dt);
        }

        private void ResetReload(IShooter shooter, float dt)
        {
            if (shooter.Reload > 0)
            {
                shooter.Reload -= dt;
            }
        }

        delegate void func(float dt);

        private void SetSprites(float dt)
        {
            func f = _objects.Player.SetSprite;
            _objects.Apples.ForEach(i => f += i.SetSprite);
            _objects.Bangs.ForEach(i => f += i.SetSprite);
            _objects.Tanks.ForEach(i => f += i.SetSprite);
            f(dt);
        }

        private void ShootTanks()
        {
            Rectangle p = new Rectangle(_objects.Player.X, _objects.Player.Y, _objects.Player.Width, _objects.Player.Height);

            foreach(var tank in _objects.Tanks)
            {
                bool onTheWay = true;
                ushort lastDir = tank.Direction;

                // Если игрок слева
                if (p.X + p.Width < tank.X && p.Y + p.Height > tank.Y + tank.Height / 2 && p.Y < tank.Y + tank.Height / 2)
                {
                    tank.Direction = MobileObject.left;
                } // справа
                else if (p.X > tank.X + tank.Width && p.Y + p.Height > tank.Y + tank.Height / 2 && p.Y < tank.Y + tank.Height / 2)
                {
                    tank.Direction = MobileObject.right;
                } //сверху
                else if (p.Y + p.Width < tank.Y && p.X + p.Width > tank.X + tank.Width / 2 && p.X < tank.X + tank.Width / 2)
                {
                    tank.Direction = MobileObject.up;
                } // снизу
                else if (p.Y > tank.Y + tank.Width && p.X + p.Width > tank.X + tank.Width / 2 && p.X < tank.X + tank.Width / 2)
                {
                    tank.Direction = MobileObject.down;
                }
                else
                {
                    onTheWay = false;
                }

                if (tank.Direction != lastDir && tank.Reload < 0.1f) // Сменили путь - добавляем паузу, чтобы сразу не стрелял
                {
                    tank.Reload = 0.5f;
                }

                if (onTheWay) // Если на пути - стреляем
                {
                    CreateBullet(tank);
                }
                
            }
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
            if (_objects.Tanks.Count < 3 && rnd.Next(100) <= percent)
            {
                int x = rnd.Next(_view.MapWidth - 30);
                int y = rnd.Next(_view.MapHeight - 30);
                var tank = new TankView(x, y, (ushort)rnd.Next(4), 30, 30);

                // Если он на свободном месте генерируется
                if (_objects.Walls.Find(wall => ObjectCollision(wall, tank)) == null &&
                    ObjectCollision(_objects.Player, tank) == false &&
                    _objects.Water.Find(water => ObjectCollision(water, tank)) == null &&
                    _objects.Tanks.Find(tnk => tnk != tank ? ObjectCollision(tnk, tank) : false) == null)
                {
                    _objects.Tanks.Add(tank);
                }

            }
        }

        private void CreateApple(int percent)
        {
            if (_objects.Apples.Count < 5 && rnd.Next(100) <= percent)
            {
                int x = rnd.Next(_view.MapWidth - 30);
                int y = rnd.Next(_view.MapHeight - 30);
                var apple = new AppleView(x, y, 30, 30);

                // Если он на свободном месте генерируется
                if (_objects.Walls.Find(wall => ObjectCollision(wall, apple)) == null &&
                    _objects.Water.Find(water => ObjectCollision(water, apple)) == null &&
                    ObjectCollision(_objects.Player, apple) == false)
                {
                    _objects.Apples.Add(apple);
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

        private bool ObjectInScreen(GameObject obj)
        {
            return (obj.X >= 0) && (obj.X + obj.Width <= _view.MapWidth) &&
                (obj.Y >= 0) && (obj.Y + obj.Height <= _view.MapHeight);
        }

        private void CreateBang(GameObject obj)
        {
            _objects.Bangs.Add(new BangView(obj.X + (obj.Width - 30) / 2, obj.Y + (obj.Height - 30) / 2, 30, 30));
        }

        private void Collision(float dt)
        {
            var delWalls = new List<WallView>();
            var delBullets = new List<BulletView>();
            var delTanks = new List<TankView>();
            var delApples = new List<AppleView>();

            // Создаем нового игрока, смотрим его новое положение
            MobileObject player = _objects.Player.Clone() as MobileObject;
            player.Step(dt);

            // Смотрим, чтобы не вышел за карту и проверяем,
            // чтобы не было столкновений с объектами "стена"
            // А еще проверяем, чтобы с танками не встречался
            if (ObjectInScreen(player) && 
                _objects.Walls.Find(wall => ObjectCollision(wall, player)) == null &&
                _objects.Water.Find(water => ObjectCollision(water, player)) == null)
            {
                bool isFree = true;

                foreach(var tank in _objects.Tanks)
                {
                    if (ObjectCollision(tank, player))
                    {
                        isFree = false;
                       // GameOver();
                    }
                }
                if (isFree)
                {
                    _objects.Player.Step(dt);
                }
            }

            // Так же выполняем для каждого врага
            _objects.Tanks.ForEach(t =>
            {
                t.Step(dt);
                // Смотрим столкновения с границей и стенами
                if (!ObjectInScreen(t) ||
                    _objects.Walls.Find(wall => ObjectCollision(wall, t)) != null ||
                    _objects.Tanks.Find(tnk => tnk != t ? ObjectCollision(tnk, t) : false) != null ||
                    _objects.Water.Find(water => ObjectCollision(water, t)) != null ||
                    ObjectCollision(t, player))
                {
                    // СТОЛКНОВЕНИЕ СО СТЕНОЙ ИЛИ ДРУГИМ ВРАГОМ
                    // Идем в обратном направлении
                    t.ChangeDirection();
                    t.Step(dt);
                }
            });

            // Смотрим столкновение пуль
            _objects.Bullets.ForEach(bullet =>
            {
                bool delBullet = false;

                if (!ObjectInScreen(bullet))
                {
                    delBullet = true;
                }

                // Смотрим столкновения пули со стенами
                _objects.Walls.ForEach(wall =>
                {
                    if (ObjectCollision(bullet, wall))
                    {
                        if (wall.Destroyable)
                        {
                            delWalls.Add(wall);
                        }
                        delBullet = true;
                        CreateBang(bullet);
                    }
                });

                // Смотрим столкновение пули с врагом
                _objects.Tanks.ForEach(tank =>
                {
                    if (ObjectCollision(tank, bullet) && bullet.Sender != tank)
                    {
                        delBullet = true;
                        delTanks.Add(tank);
                        CreateBang(tank);
                    }
                });

                // Столкновение пуль с игроком
                if (ObjectCollision(bullet, _objects.Player) && bullet.Sender != _objects.Player)
                {
                    CreateBang(_objects.Player);
                    GameOver(); // Конец игры
                }

                if (delBullet)
                {
                    delBullets.Add(bullet);
                }
                else
                {
                    bullet.Step(dt);
                }

            });

            // Собираем яблоки
            _objects.Apples.ForEach(apple =>
            {
                if (ObjectCollision(player, apple))
                {
                    delApples.Add(apple);
                    _objects.Score++;
                }
            });


            // Удаляем элементы
            delBullets.ForEach(bullet => _objects.Bullets.Remove(bullet));
            delWalls.ForEach(wall => _objects.Walls.Remove(wall));
            delTanks.ForEach(tank => _objects.Tanks.Remove(tank));
            delApples.ForEach(apple => _objects.Apples.Remove(apple));
        }

        public GameController(IViewController view, IGameObjects objects)
        {
            _view = view;
            _objects = objects;
            rnd = new Random();
        }

        private void CreateBullet(MobileObject obj)
        {
            // Если у объекта еще перезарядка, то не стреляем
            if ((obj as IShooter).Reload > 0)
            {
                return;
            }

            float x = obj.X + obj.Width / 2 - 2;
            float y = obj.Y + obj.Height / 2 - 2;

            int w = obj.IsHorizontal() ? 8 : 4;
            int h = obj.IsVertical() ? 8 : 4;

            _objects.Bullets.Add(new BulletView(x, y, obj.Direction, w, h, obj));
            (obj as IShooter).Reload = 0.3f;
        }

        public void StartGame()
        {
            LoadLevel(1);
            _objects.Tanks = new List<TankView>();
            _objects.Bullets = new List<BulletView>();
            _objects.Apples = new List<AppleView>();
            _objects.Bangs = new List<BangView>();
            _objects.Score = 0;
            timer = new Stopwatch();
            timer.Start();
            _view.ActiveTimer = true;
            isGame = true;
            MainLoop();           
        }

        public void KeyDown(Keys key)
        {
            if (!isGame)
            {
                StartGame();
                return;
            }
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
                case Keys.Space:
                    CreateBullet(_objects.Player);
                    break;
                case Keys.R:
                    StartGame();
                    break;
                case Keys.P:
                    // Ставим на паузу
                    _view.ActiveTimer = false;
                    timer.Stop();

                    // Запускаем форму лога
                    LogForm form = new LogForm();
                    ShowInfo(form);
                    form.ShowDialog();
                    // продолжаем игру

                    timer.Start();
                    _view.ActiveTimer = true;
                    break;
            }
        }

        private void GameOver()
        {
            isGame = false;
            _objects.Player.OutScreen();
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
                _objects.Water = new List<WaterView>();

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
                                case '*': // пробиваемая стена
                                    _objects.Walls.Add(new WallView(BlockSize * x, BlockSize * y, BlockSize, BlockSize, true));
                                    break;
                                case '=': // непробиваемая стена
                                    _objects.Walls.Add(new WallView(BlockSize * x, BlockSize * y, BlockSize, BlockSize, false));
                                    break;
                                case '0': // вода
                                    _objects.Water.Add(new WaterView(BlockSize * x, BlockSize * y, BlockSize, BlockSize, rnd.Next(3)));
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

        // LOG

        private void AddItem(ListBox lb, GameObject obj)
        {
            lb.Items.Add($"X = {obj.X}, Y = {obj.Y}");
        }

        private void AddItems(ListBox lb, List<GameObject> obj)
        {
            lb.Items.Clear();
            obj.ForEach(i => AddItem(lb, i));
        }

        public void ShowInfo(LogForm form)
        {
            form.lblPlayer.Text = $"X = {_objects.Player.X}, Y = {_objects.Player.Y}";

            AddItems(form.lbTanks, _objects.Tanks.Select(i => i as GameObject).ToList());
            AddItems(form.lbBullets, _objects.Bullets.Select(i => i as GameObject).ToList());
            AddItems(form.lbApples, _objects.Apples.Select(i => i as GameObject).ToList());
            AddItems(form.lbWalls, _objects.Walls.Select(i => i as GameObject).ToList());
            AddItems(form.lbBangs, _objects.Bangs.Select(i => i as GameObject).ToList());

        }
    }
}
