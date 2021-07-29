//Para hacer funcionar el slider
let options = {
    indicators: true, //mostrar indicadores
    height: 400,//altura de control deslizante
    duration: 500,//duracion de la animacion
    interval: 6000//duracion de la transicion
}

document.addEventListener('DOMContentLoaded', function () {
    let elems = document.querySelectorAll('.slider');
    let instances = M.Slider.init(elems, options)
})


//Scripta para mostrar los comentarios del avatar
const avatar1 = document.getElementById('avatar1');
avatar1.onmouseover = function () {
    document.getElementById("chat-avatar1").style.display = "block";
    
}
avatar1.onmouseout = function () {
    document.getElementById("chat-avatar1").style.display = "none";

}

const avatar2 = document.getElementById('avatar2');
avatar2.onmouseover = function () {
    document.getElementById("chat-avatar2").style.display = "block";

}
avatar2.onmouseout = function () {
    document.getElementById("chat-avatar2").style.display = "none";

}

const avatar3 = document.getElementById('avatar3');
avatar3.onmouseover = function () {
    document.getElementById("chat-avatar3").style.display = "block";

}
avatar3.onmouseout = function () {
    document.getElementById("chat-avatar3").style.display = "none";

}

const avatar4 = document.getElementById('avatar4');
avatar4.onmouseover = function () {
    document.getElementById("chat-avatar4").style.display = "block";

}
avatar4.onmouseout = function () {
    document.getElementById("chat-avatar4").style.display = "none";

}

const avatar5 = document.getElementById('avatar5');
avatar5.onmouseover = function () {
    document.getElementById("chat-avatar5").style.display = "block";

}
avatar5.onmouseout = function () {
    document.getElementById("chat-avatar5").style.display = "none";

}

const avatar6 = document.getElementById('avatar6');
avatar6.onmouseover = function () {
    document.getElementById("chat-avatar6").style.display = "block";

}
avatar6.onmouseout = function () {
    document.getElementById("chat-avatar6").style.display = "none";

}

const avatar7 = document.getElementById('avatar7');
avatar7.onmouseover = function () {
    document.getElementById("chat-avatar7").style.display = "block";

}
avatar7.onmouseout = function () {
    document.getElementById("chat-avatar7").style.display = "none";

}

const avatar8 = document.getElementById('avatar8');
avatar8.onmouseover = function () {
    document.getElementById("chat-avatar8").style.display = "block";

}
avatar8.onmouseout = function () {
    document.getElementById("chat-avatar8").style.display = "none";

}

const avatar9 = document.getElementById('avatar9');
avatar9.onmouseover = function () {
    document.getElementById("chat-avatar9").style.display = "block";

}
avatar9.onmouseout = function () {
    document.getElementById("chat-avatar9").style.display = "none";

}
