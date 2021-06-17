using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SysHotel.EL;

using SysHotel.BL;
using SysHotel.UI.Filtros;
using SysHotel.EL.Paginador;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class RolUsuarioController : Controller
    {
        private RolUsuarioBL rolBL = new RolUsuarioBL();

        //Variables para el paginador
        private const int registroPorPagina = 15;
        private List<RolUsuario> rolUsuario;
        private PaginadorGenerico<RolUsuario> paginadorRoles;
        
        // GET: RolUsuario
        public async Task<ActionResult> Index(string busqueda, int pagina = 1)
        {
            //Recuperamos la lista completa de roles
            rolUsuario = await rolBL.ListarRolUsuariosActivos();
           
            //BUSQUEDA
            //Filtramos una nueva lista segun la busqueda
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    rolUsuario = rolUsuario.Where(x => x.Rol.ToUpper().Contains(item)).ToList();
                }
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el total de registros encontrados
            totalRegistros = rolUsuario.Count();

            //Se obtiene la lista de registro por pagina
            List<RolUsuario> listaRolUsuario = rolUsuario.OrderBy(x => x.Rol)
                                                         .Skip((pagina - 1) * registroPorPagina)
                                                         .Take(registroPorPagina)
                                                         .ToList();
            //Numero total de paginas
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de la clase paginador generico
            paginadorRoles = new PaginadorGenerico<RolUsuario>
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaRolUsuario
            };
            return View(paginadorRoles);
        }

        // GET: RolUsuario/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolUsuario rolUsuario = await rolBL.BuscarRolUsuarioPorId(Convert.ToInt32(id));
            if (rolUsuario == null)
            {
                return HttpNotFound();
            }
            return View(rolUsuario);
        }

        // GET: RolUsuario/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Message = null;
            List<RolUsuario>listaRoles = await rolBL.ListarRolUsuariosActivos();
            ViewData["RolesExistentes"] = listaRoles.Take(10).ToList();
            return View();
        }

        // POST: RolUsuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdRolUsuario,Rol,Estado")] RolUsuario rolUsuario)
        {
            List<RolUsuario> listaRoles = await rolBL.ListarRolUsuariosActivos();
            ViewData["RolesExistentes"] = listaRoles.Take(10).ToList();
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await rolBL.AgregarRolUsuario(rolUsuario);

                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el nuevo rol.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El rol ya existe.";
                        break;
                    case 3:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(rolUsuario);
            }
            ViewBag.Message = "Información incompleta.";
            return View(rolUsuario);
        }

        // GET: RolUsuario/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolUsuario rolUsuario = await rolBL.BuscarRolUsuarioPorId(Convert.ToInt32(id));
            if (rolUsuario == null)
            {
                return HttpNotFound();
            }
            return View(rolUsuario);
        }

        // POST: RolUsuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdRolUsuario,Rol,Estado")] RolUsuario rolUsuario)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await rolBL.EditarRolUsuario(rolUsuario);
                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, el cambio no fué posible salvarlo.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "Ya existe un rol con el mismo nombre."; 
                        break;
                    case 3:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 4:
                        mensaje = "Upss, los datos se fueron incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(rolUsuario);
            }
            ViewBag.Message = "Información incompleta.";
            return View(rolUsuario);
        }

        // GET: RolUsuario/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolUsuario rolUsuario = await rolBL.BuscarRolUsuarioPorId(Convert.ToInt32(id));
            if (rolUsuario == null)
            {
                return HttpNotFound();
            }
            return View(rolUsuario);
        }

        // POST: RolUsuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await rolBL.EliminarRolUsuario(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar el rol.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, el usuaio a eliminar no existe.";
                    break;
                case 3:
                    mensaje = "Se recibió un identificador incorrecto.";
                    break;
            }
            ViewBag.Message = mensaje;
            return View();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
