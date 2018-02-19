// Класс управления анимациями

(function() {
    function Sprite(url, pos, size, speed, frames, dir, once, radiusScale, speedScale) {
        this.pos = pos;
        this.size = size;
        this.speed = typeof speed === 'number' ? speed : 0;
        this.frames = frames;
        this._index = 0;
        this.url = url;
        this.dir = dir || 'horizontal';
        this.once = once;
        this.radiusScale = +radiusScale;
        this.speedScale = +speedScale;
        this.scale = 1;
    };

    Sprite.prototype = {
        update: function(dt, time) {
            
            this._index += this.speed*dt;
            // Изменяем масштаб
            if (this.radiusScale > 0)
                this.scale = 1 + this.radiusScale * Math.sin(time * this.speedScale);

        },

        render: function(ctx) {
            var frame;

            // определяем номер фрейма
            if(this.speed > 0) {
                var max = this.frames.length;
                var idx = Math.floor(this._index);
                frame = this.frames[idx % max];

                if(this.once && idx >= max) {
                    this.done = true;
                    return;
                }
            }
            else {
                frame = 0;
            }


            var x = this.pos[0];
            var y = this.pos[1];

            // сдвигаем координаты 

            if(this.dir == 'vertical') {
                y += frame * this.size[1];
            }
            else {
                x += frame * this.size[0];
            }


            // отрисовываем
            ctx.drawImage(resources.get(this.url),
                          x, y,
                          this.size[0], this.size[1],
                          this.size[0] * (1 - this.scale) / 2, this.size[1] * (1 - this.scale) / 2,
                          this.size[0] * this.scale, this.size[1] * this.scale);

            
        }
    };

    window.Sprite = Sprite;
})();