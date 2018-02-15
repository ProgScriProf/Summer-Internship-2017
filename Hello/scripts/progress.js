// Добавление рядом с скроллбарами значение в процентах
var list = document.getElementsByClassName('progress');
for(var i = 0; i < list.length; i++)
{
    var percent = list[i].getElementsByTagName('progress')[0].value;
    list[i].getElementsByClassName('progress-percent')[0].innerHTML = percent + "%";
}