using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SysHotel.EL;
using SysHotel.EL.Login;
using SysHotel.UI.Filtros;

namespace SysHotel.UI.Controllers
{
    [NoLogin]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Autenticar (LoginViewModel user)
        {
            Usuario usuario = new Usuario();
            var responseModel = new ResponseModel();
            if (ModelState.IsValid)
            {
                usuario.NombreUsuario = user.Usuario;
                usuario.Contraseña = user.Contraseña;

                responseModel = usuario.Autenticarse();
                //Si el usuario esta autenticado lo dirigimos a la pagina de administracion
                if (responseModel.response)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            else
            {
                responseModel.SetResponse(false, "Debe llenar los campos para auntenticarse");
            }
            ViewBag.Message = responseModel.message;
            return View("Index");
        }
    }
}