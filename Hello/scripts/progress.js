// Добавление рядом с скроллбарами значение в процентах
// А так же черненькой полоски

var list = document.getElementsByClassName('progress');
for(var i = 0; i < list.length; i++)
{   
    // Добавляем значение процентов
    var percent = list[i].getElementsByTagName('progress')[0].value;
    list[i].getElementsByClassName('progress-percent')[0].innerHTML = percent + "%";

    // добавляем разделитель
    var pos = document.createElement('img');
    pos.src = 'img/pos.png';    
    pos.style = 'position: absolute; left:' + (percent * 0.7 + 2) + '%; top:-5px;';
    list[i].appendChild(pos);
}