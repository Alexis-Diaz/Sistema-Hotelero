using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SysHotel.EL.Login;
using System.Web.Mvc;
using System.Web.Routing;

namespace SysHotel.UI.Filtros
{
    //Si no estamos logueados, regresamos al login
    public class AutenticadoAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Este metodo es llamado si el controlador tiene la bandera [Auntentido].
        /// Sirve de filtro para restringir el acceso a usuarios no autorizados
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Se verifica si el usuario esta autenticado
            if (!SessionHelper.ExistUserInSession())
            {
                //Redireccionamos al usurio no autenticado al login
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Index"
                }));
            }
        }
    }

    public class NoLoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Este metodo es llamado si el controlador tiene la bandera [NoLogin].
        /// Sirve para permitir el acceso a solicitudes sin necesidad de loguerse. Si detecta una sesion abierta
        /// redirige al usuario a la pagina de inicio de su cuenta abierta.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Se verifica si el usuario esta autenticado
            if (SessionHelper.ExistUserInSession())
            {
                //Redireccionamos al usurio autenticado a su cuenta abierta
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Admin",
                    action = "Index"
                }));
            }
        }
    }
}