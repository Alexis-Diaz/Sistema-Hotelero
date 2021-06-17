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
    public class ProveedorController : Controller
    {
        private ProveedorBL proveedorBL = new ProveedorBL();

        //Variables para el paginador
        private const int registroPorPagina = 15;
        private List<Proveedor> proveedores;
        private PaginadorGenerico<Proveedor> paginadorProveedor;

        // GET: Proveedor
        public async Task<ActionResult> Index(string busqueda, int pagina = 1)
        {
            //Recuperamos la lista de proveedores
            proveedores = await proveedorBL.ListarProveedoresActivos();

            //BUSQUEDA
            //Filtramos la lista según la busqueda
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    proveedores = proveedores.Where(x => x.NombreEmpresa.ToUpper().Contains(item) ||
                                                         x.Ubicacion.ToUpper().Contains(item) ||
                                                         x.Encargado.ToUpper().Contains(item) ||
                                                         x.Telefono.ToUpper().Contains(item) ||
                                                         x.Correo.ToUpper().Contains(item))
                                                          .ToList();
                }
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cueta el total de registros encontrados
            totalRegistros = proveedores.Count();

            //Se obtiene la lista de regisros por pagina
            List<Proveedor> listaProveedores = proveedores.OrderBy(x => x.NombreEmpresa)
                                                         .Skip((pagina - 1) * registroPorPagina)
                                                         .Take(registroPorPagina)
                                                         .ToList();

            //Numero total de paginas 
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de la clase paginador generico
            paginadorProveedor = new PaginadorGenerico<Proveedor>
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaProveedores
            };
            return View(paginadorProveedor);
        }

        // GET: Proveedor/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = await proveedorBL.BuscarProveedorPorId(Convert.ToInt32(id));
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // GET: Proveedor/Create
        public async Task< ActionResult> Create()
        {
            ViewBag.Message = null;
            List<Proveedor>listaProveedores = await proveedorBL.ListarProveedoresActivos();
            ViewData["Proveedores"] = listaProveedores.Take(10).ToList();
            return View();
        }

        // POST: Proveedor/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdProveedor,NombreEmpresa,Ubicacion,Encargado,Telefono,Correo,Estado")] Proveedor proveedor)
        {
            List<Proveedor> listaProveedores = await proveedorBL.ListarProveedoresActivos();
            ViewData["Proveedores"] = listaProveedores.Take(10).ToList();
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await proveedorBL.AgregarNuevoProveedor(proveedor);

                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el proveedor.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El proveedor ya existe.";
                        break;
                    case 3:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(proveedor);
            }
            ViewBag.Message = "Información incompleta.";
            return View(proveedor);
        }

        // GET: Proveedor/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = await proveedorBL.BuscarProveedorPorId(Convert.ToInt32(id));
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedor/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdProveedor,NombreEmpresa,Ubicacion,Encargado,Telefono,Correo,Estado")] Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await proveedorBL.EditarProveedor(proveedor);
                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, el cambio no fué posible salvarlo.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "Ya existe un proveedor con el mismo nombre.";
                        break;
                    case 3:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 4:
                        mensaje = "Upss, los datos se fueron incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(proveedor);
            }
            ViewBag.Message = "Información incompleta.";
            return View(proveedor);
        }

        // GET: Proveedor/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = await proveedorBL.BuscarProveedorPorId(Convert.ToInt32(id));
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await proveedorBL.EliminarProveedor(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar el proveedor.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, el proveedor a eliminar no existe.";
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
