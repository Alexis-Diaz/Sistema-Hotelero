//Este codigo hace funcionar el tabs de Admin
let options = {
    duration: 300,//duracion de la transicion en ms
    onShow: null,//callback que se llama al mostrar la nueva pestaña
    swipeable: false,//Pestañas deslizables. Usa responsiveThreshold
    responsiveThreshold: Infinity//ancho maximo de pantalla en px donde inicia la funcion deslizable
}

document.addEventListener('DOMContentLoaded', function () {
    let elems = document.querySelectorAll(".tabs");
    let instances= M.Tabs.init(elems, options)
})