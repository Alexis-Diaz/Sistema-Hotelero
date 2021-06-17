//Para hacer funcionar el menu lateral oculto
document.addEventListener('DOMContentLoaded', function () {
    let elems = document.querySelectorAll('.sidenav');
    let instances = M.Sidenav.init(elems);
})

////Para hacer funcionar los dropdown ocultos del menu
document.addEventListener('DOMContentLoaded', function () {
    let elems = document.querySelectorAll('.dropdown-oculto');
    let instances = M.Dropdown.init(elems);
})

//Para hacer funcionar los dropdown del navbar
document.addEventListener('DOMContentLoaded', function () {
    let elems = document.querySelectorAll('.dropdown-trigger');
    let instances = M.Dropdown.init(elems);
})

//Esta funcion sirve para eliminar los mensajes de informacion de una pagina
function closeAlertFunction() {
    let divLoginAlert = document.getElementById("closeAlertLogin");
    divLoginAlert.style.display = "none";
}