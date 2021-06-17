using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using SysHotel.BL;
using SysHotel.UI.Filtros;
using SysHotel.EL.Login;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class AdminController : Controller
    {
        private ReservacionBL reservacionBL = new ReservacionBL();
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            return View(await reservacionBL.ListarReservacionesActuales());
        }

        public ActionResult Salir()
        {
            SessionHelper.DestroyUserSession();
            return RedirectToAction("Index","Login");
        }
    }
}