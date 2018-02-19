
// Обертка для кроссбрауерности
var requestAnimFrame = (function(){
    return window.requestAnimationFrame    ||
        window.webkitRequestAnimationFrame ||
        window.mozRequestAnimationFrame    ||
        window.oRequestAnimationFrame      ||
        window.msRequestAnimationFrame     ||
        function(callback){
            window.setTimeout(callback, 1000 / 60);
        };
})();

// Создание канвы
var canvas = document.createElement("canvas");
var ctx = canvas.getContext("2d");
canvas.width = 512;
canvas.height = 480;
document.body.appendChild(canvas);


// Основной цикл игры
var lastTime;
function main() {
    var now = Date.now();
    var dt = (now - lastTime) / 1000.0;

    update(dt);
    render();

    lastTime = now;
    requestAnimFrame(main);
};


// Загрузка текстур
resources.load(['img/sprites.png', 'img/terrain.png']);
// И после загрузки выполнить init
resources.onReady(init);


// Инициализация компонентов
function init() {
    terrainPattern = ctx.createPattern(resources.get('img/terrain.png'), 'repeat');

    loadLevel(1);

    // Событие для кнопки повтора
    document.getElementById('play-again').addEventListener('click', function() {
        reset();
    });

    reset();
    lastTime = Date.now();
    main();
}


// Загрузка уровня
function loadLevel(lvl)
{
    switch (lvl)
    {
        case 1: 
            megaliths = [
            {
                pos: [0.4 * canvas.width , 0.1 * canvas.height],
                sprite: new Sprite('img/sprites.png', [0,210], [60,60], 0, [0])
            }, 
            {
                pos: [0.7 * canvas.width , 0.4 * canvas.height],
                sprite: new Sprite('img/sprites.png', [0,270], [60,60], 0, [0])
            }, 
            {
                pos: [0.5 * canvas.width , 0.75 * canvas.height],
                sprite: new Sprite('img/sprites.png', [0,210], [60,60], 0, [0])
            }]
    }
}


// Состояние игры
var player = {
    pos: [0, 0],
    sprite: new Sprite('img/sprites.png', [0, 0], [39, 39], 16, [0, 1])
};

var bullets = [];
var enemies = [];
var explosions = [];
var megaliths; // Мегалиты
var manna = []; // Манна

var lastFire = Date.now();
var gameTime = 0;
var isGameOver;
var terrainPattern;

// Счет
var score = 0;
var scoreEl = document.getElementsByClassName('value score')[0];
var mannaCount = 0;
var mannaEl = document.getElementsByClassName('value manna')[0];

// скорости
var playerSpeed = 200;
var bulletSpeed = 500;
var enemySpeed = 100;

// другие константы
var walking_distance = 50;
var maxManna = 10;

// Обновление
function update(dt) {
    gameTime += dt;

    handleInput(dt);
    updateEntities(dt);

    // Случайное время появления. Уменьшается с ростом времени
    if(Math.random() < 1 - Math.pow(.993, gameTime)) {
        enemies.push({
            pos: [canvas.width, Math.random() * (canvas.height - 39)],
            dY: 0,
            sprite: new Sprite('img/sprites.png', [0, 78], [80, 39],
                               6, [0, 1, 2, 3, 2, 1])                          
        });
    }

    
    // Случайное время появления маны
    if (manna.length < maxManna && Math.random() > 0.98)
    {
        var _pos = [Math.random() * (canvas.width - 60), Math.random() * (canvas.height - 60)];

        var create = true;
        // Чтобы на мегалитах не спавнилась
        for(var i=0; i<megaliths.length && create; i++)
            if (boxCollides(megaliths[i].pos, megaliths[i].sprite.size, _pos, 60))
                create = false;

        if (create)
            manna.push({
                pos: _pos,
                sprite: new Sprite('img/sprites.png', [0, 154], [60, 60],
                                    0, [0, 1, 2, 3], null, true, 0.08, 10)
            });
    } 

    if (!isGameOver)
        checkCollisions();

    scoreEl.innerHTML = score;
    mannaEl.innerHTML = mannaCount;
};

function isPlayerCollision(x, y) {
    // Новые координаты
    var pos = [player.pos[0] + x, player.pos[1] + y];

     // Бежим по всем мегалитам
    for (var i = 0; i < megaliths.length; i++)
    {   
        if(boxCollides(pos, player.sprite.size, megaliths[i].pos, megaliths[i].sprite.size)) 
            return true;
    }
    return false;
}

// Обработка нажатий клавиш
function handleInput(dt) {
    if(input.isDown('DOWN') || input.isDown('s')) {
        if (!isPlayerCollision(0, playerSpeed * dt))
            player.pos[1] += playerSpeed * dt;
    }

    if(input.isDown('UP') || input.isDown('w')) {
        if (!isPlayerCollision(0, -playerSpeed * dt))
            player.pos[1] -= playerSpeed * dt;
    }

    if(input.isDown('LEFT') || input.isDown('a')) {
        if (!isPlayerCollision(-playerSpeed * dt, 0))
            player.pos[0] -= playerSpeed * dt;
    }

    if(input.isDown('RIGHT') || input.isDown('d')) {
        if (!isPlayerCollision(playerSpeed * dt, 0))
            player.pos[0] += playerSpeed * dt;
    }

    if(input.isDown('SPACE') &&
       !isGameOver &&
       Date.now() - lastFire > 100) {
        var x = player.pos[0] + player.sprite.size[0] / 2;
        var y = player.pos[1] + player.sprite.size[1] / 2;

        bullets.push({ pos: [x, y],
                       dir: 'forward',
                       sprite: new Sprite('img/sprites.png', [0, 39], [18, 8]) });
        bullets.push({ pos: [x, y],
                       dir: 'up',
                       sprite: new Sprite('img/sprites.png', [0, 50], [9, 5]) });
        bullets.push({ pos: [x, y],
                       dir: 'down',
                       sprite: new Sprite('img/sprites.png', [0, 60], [9, 5]) });


        lastFire = Date.now();
    }
}

// Обновление сущностей
function updateEntities(dt) {
    
    // Обновление игрока
    player.sprite.update(dt);

    // Обновление пуль
    for(var i=0; i<bullets.length; i++) {
        var bullet = bullets[i];

        // В зависимости от направления - перемещаем пулю
        switch(bullet.dir) {
            case 'up': bullet.pos[1] -= bulletSpeed * dt; break;
            case 'down': bullet.pos[1] += bulletSpeed * dt; break;
            default:
                bullet.pos[0] += bulletSpeed * dt;
        }

        // Если вышла за границы, удаляем
        if(bullet.pos[1] < 0 || bullet.pos[1] > canvas.height ||
           bullet.pos[0] > canvas.width) {
            bullets.splice(i, 1);
            i--;
        }
    }

    // Обновление врагов
    for(var i=0; i<enemies.length; i++) {
        // Передвигаем влево
        var dx = enemySpeed * dt;
        var dy = 0;

        var pos = enemies[i].pos;
        var size = enemies[i].sprite.size;

        // Облетаем мегалиты

        // Облетаем мегалиты
        for(var j=0; j<megaliths.length; j++) 
        {
            var pos2 = megaliths[j].pos;
            var size2 = megaliths[j].sprite.size;
            
            // Добавляем запас расстояния для облета
            var size2_space = [size2[0] + walking_distance, size2[1]];

            // Проверяем столкновение
            if (boxCollides(pos, size, pos2, size2_space)) {
                // Впереди препятствие! Нужно облететь
                // Вычисляем направление облета

                // Облет выполняем по гипотенузе, где два катета - расстояния по х и у (подобные треугольники)

                /*
                                               _________
                                              |
                _________                     |
                         |'-._                |
                         |     '-._           | enemy
                         |y       | '-._      |
                megalit  |      dy|  dx  '-._ | 
                         |--------'-----------'----------
                         |        x
                _________|
                
               //  */

                // Облетаем вверх
                var y = (pos[1] < pos2[1] || enemies[i].dY > 0) ?
                        -(pos[1] + size[1] - pos2[1]):
                        (pos2[1] + size2[1] - pos[1]);   

                var x = pos[0] - (pos2[0] + size2[0]);  
                dy = y / x * dx;     
                
            }

        }

        if (dy == 0 && Math.abs(enemies[i].dY) > 1) // Если не сталкнулись с мегалитами, пытаемся возвратиться, если уже пора
        {
            
            var isReturn = true;
            for(var j=0; j<megaliths.length; j++) 
            {
                var pos2 = megaliths[j].pos;
                var size2 = megaliths[j].sprite.size;
                
                var r = 5; // радиус облета

                // Добавляем запас расстояния
                var pos2_space = [pos2[0], pos2[1] - r];
                var size2_space = [size2[0], size2[1] + 2*r];

                // Если не пересекает, возвращаемся
                if (boxCollides(pos, size, pos2_space, size2_space)) {
                    isReturn = false;
                }
            }

            if (isReturn)
                dy = enemies[i].dY / 10;
        }

        enemies[i].pos[0] -= dx;     
        enemies[i].pos[1] += dy;  
        enemies[i].dY -= dy;  


        enemies[i].sprite.update(dt);

        // Если совсем вышли за экран, то удаляем
        if(enemies[i].pos[0] + enemies[i].sprite.size[0] < 0) {
            enemies.splice(i, 1);
            i--;
        }
    }

    // Обновляе ВЗРЫВЫ
    for(var i=0; i<explosions.length; i++) {
        explosions[i].sprite.update(dt);

        // Если закончили анимацию, то удаляем
        if(explosions[i].sprite.done) {
            explosions.splice(i, 1);
            i--;
        }
    }

    // Обновляе манну
    for(var i=0; i<manna.length; i++) {
        manna[i].sprite.update(dt, gameTime);
    
        // Если закончили анимацию, то удаляем
        if(manna[i].sprite.done) {
            mannaCount++;
            manna.splice(i, 1);
            i--;
        }
        
    }
}

// Столкновения
// Кординаты левого верхнего и правого нижнего углов 
function collides(x, y, r, b, x2, y2, r2, b2) {
    return !(r <= x2 || x > r2 ||
             b <= y2 || y > b2);
}

// Позиции и размеры двух сущностей
function boxCollides(pos, size, pos2, size2) {
    return collides(pos[0], pos[1],
                    pos[0] + size[0], pos[1] + size[1],
                    pos2[0], pos2[1],
                    pos2[0] + size2[0], pos2[1] + size2[1]);
}


// Обработка столкновений
function checkCollisions() {

    // Пределы, за которые низя выйти игроку
    checkPlayerBounds();

    // Бежим по всем врагам
    for(var i=0; i<enemies.length; i++) {
        var pos = enemies[i].pos;
        var size = enemies[i].sprite.size;

        // А затем по всем пулям
        for(var j=0; j<bullets.length; j++) {
            var pos2 = bullets[j].pos;
            var size2 = bullets[j].sprite.size;
            

            // Смотрим столкновение каждой пули с каждым врагом
            if(boxCollides(pos, size, pos2, size2)) {
                
                // Если есть столкновение - удаляем врага
                enemies.splice(i, 1);
                i--;

                // Прибавляем очки
                score += 100;

                // Добавляем взрыв
                explosions.push({
                    pos: pos,
                    sprite: new Sprite('img/sprites.png',
                                       [0, 117],
                                       [39, 39],
                                       16,
                                       [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                                       null,
                                       true)
                });

                // Удаляемм пулю и и идем к слеующему врагу
                bullets.splice(j, 1);
                break;
            }
        }

        // Столкновение врага с игроком
        if(boxCollides(pos, size, player.pos, player.sprite.size)) {
            gameOver();
        }


    }

    // Обработка столкновения с мегалитами
    for(var i=0; i<megaliths.length; i++) 
    {
        var pos = megaliths[i].pos;
        var size = megaliths[i].sprite.size;

        // По всем пулям
        for(var j=0; j<bullets.length; j++) 
        {
            var pos2 = bullets[j].pos;
            var size2 = bullets[j].sprite.size;

            // Если текущая пуля столкнулась с мегалитом
            if(boxCollides(pos, size, pos2, size2)) 
            {
                // Удаляем данную пулю
                bullets.splice(j, 1);
                j--;
            }
        }


    }

    // Обработка взятие манны
    for(var i=0; i<manna.length; i++)
    {
        var pos = manna[i].pos;
        var size = manna[i].sprite.size;

        // Если берем манну
        if (boxCollides(pos, size, player.pos, player.sprite.size))
        {
            manna[i].sprite.speed = 8;
        }
    }
}

function checkPlayerBounds() {

    // По горизонтали
    if(player.pos[0] < 0) {
        player.pos[0] = 0;
    }
    else if(player.pos[0] > canvas.width - player.sprite.size[0]) {
        player.pos[0] = canvas.width - player.sprite.size[0];
    }

    // По вертикали
    if(player.pos[1] < 0) {
        player.pos[1] = 0;
    }
    else if(player.pos[1] > canvas.height - player.sprite.size[1]) {
        player.pos[1] = canvas.height - player.sprite.size[1];
    }
}

// рендер
function render() {
    // Заливка фона
    ctx.fillStyle = terrainPattern;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Отрисовываем игрока
    // пули, врагов и взрывы
    if(!isGameOver) {
        renderEntity(player);
    }

    renderEntities(manna);
    renderEntities(bullets);
    renderEntities(enemies);
    renderEntities(explosions);
    renderEntities(megaliths);   
};

// Это для массивов, чтобы отрисовать каждый по отдельности
function renderEntities(list) {
    for(var i=0; i<list.length; i++) {
        renderEntity(list[i]);
    }    
}

// И, по сути, сама отрисовка
function renderEntity(entity) {
    ctx.save();
    ctx.translate(entity.pos[0], entity.pos[1]);
    entity.sprite.render(ctx);
    ctx.restore();
}


// Кнопки управления
function gameOver() {
    document.getElementById('game-over').style.display = 'block';
    document.getElementById('game-over-overlay').style.display = 'block';
    document.getElementById('play-again').focus();
    isGameOver = true;
    document.getElementsByClassName('score')[0].innerHTML = score;
    document.getElementsByClassName('manna')[0].innerHTML = mannaCount;
}

// Рестарт
function reset() {
    document.getElementById('game-over').style.display = 'none';
    document.getElementById('game-over-overlay').style.display = 'none';
    isGameOver = false;
    gameTime = 0;
    score = 0;
    mannaCount = 0;

    enemies = [];
    bullets = [];
    megaliths = [];
    manna = [];

    player.pos = [50, canvas.height / 2];
    loadLevel(1);
};