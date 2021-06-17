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
