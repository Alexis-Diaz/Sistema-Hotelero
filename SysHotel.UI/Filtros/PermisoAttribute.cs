using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SysHotel.EL.Login;
using System.Web.Routing;
using System.Web.Mvc;

namespace SysHotel.UI.Filtros
{
    public class PermisoAttribute : ActionFilterAttribute
    {
        public RolesPermisos Permiso { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!FrontUser.TienePermiso(this.Permiso))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Cliente",
                    action = "Index"
                }));
            }
        }
    }
}