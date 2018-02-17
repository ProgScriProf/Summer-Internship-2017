// Библиотека для загрузки изображений
(function() {
    var resourceCache = {};
    var loading = [];
    var readyCallbacks = [];

    // Загрузка по юрл или по массиву юрл
    function load(urlOrArr) {
        if (urlOrArr instanceof Array) {
            urlOrArr.forEach(function(url) {
                _load(url); // Если массив, то загружаем по отдельности
            });
        }
        else {
            _load(urlOrArr);
        }
    }

    // агружаем новую текстуру по юрл
    function _load(url) {
        // Если это изображение загружено, то возврадаем его
        if(resourceCache[url]) {
            return resourceCache[url];
        }
        else {
            // Иначе создаем новую имагу, даем обработчик события
            var img = new Image();
            img.onload = function() {
                resourceCache[url] = img;
                
                if(isReady()) {
                    readyCallbacks.forEach(function(func) { func(); });
                }
            };
            resourceCache[url] = false;
            img.src = url;
        }
    }

    // Взять изображение по юрл
    function get(url) {
        return resourceCache[url];
    }

    // проверка на то, загрузились ли текстуры
    function isReady() {
        var ready = true;
        for(var k in resourceCache) {
            if(resourceCache.hasOwnProperty(k) &&
               !resourceCache[k]) {
                ready = false;
            }
        }
        return ready;
    }

    // Колбэк, когда все загрузилось
    function onReady(func) {
        readyCallbacks.push(func);
    }

    // Объект для работы с либой
    window.resources = { 
        load: load,
        get: get,
        onReady: onReady,
        isReady: isReady
    };
})();