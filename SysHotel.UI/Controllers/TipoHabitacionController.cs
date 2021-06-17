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

using SysHotel.EL.Paginador;
using SysHotel.UI.Filtros;
using SysHotel.BL;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class TipoHabitacionController : Controller
    {
        private TipoHabitacionBL tipoHabitacionBL = new TipoHabitacionBL();
        private BDComun db = new BDComun();

        //Variables para el paginador
        private const int registroPorPagina = 25;
        private List<TipoHabitacion> tipoHabitaciones;
        private PaginadorGenerico<TipoHabitacion> paginadorTipoHabitacion;

        // GET: TipoHabitacion
        public async Task<ActionResult> Index(string busqueda, int pagina = 1)
        {
            //Recuperamops la lista completa de tipo de habitaciones
            tipoHabitaciones = await tipoHabitacionBL.ListarTipoDeHabitacionesActivas();

            //BUSQUEDA 
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    tipoHabitaciones = tipoHabitaciones.Where(x => x.TipoDeHabitacion.ToUpper().Contains(item) ||
                                                                   x.Descripcion.ToUpper().Contains(item))
                                                                    .ToList();
                }
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el total de registros encontrados
            totalRegistros = tipoHabitaciones.Count();

            //Se obtienen la lista de registro por pagina
            List<TipoHabitacion> listaTipoHabitacion = tipoHabitaciones.OrderBy(x => x.TipoDeHabitacion)
                                                                      .Skip((pagina - 1) * registroPorPagina)
                                                                      .Take(registroPorPagina)
                                                                      .ToList();

            //Numero de paginas
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de paginador generico
            paginadorTipoHabitacion = new PaginadorGenerico<TipoHabitacion>()
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaTipoHabitacion
            };
            return View(paginadorTipoHabitacion);
        }

        // GET: TipoHabitacion/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoHabitacion tipoHabitacion = await tipoHabitacionBL.BuscarTipoDeHabitacionPorId(Convert.ToInt32(id));
            if (tipoHabitacion == null)
            {
                return HttpNotFound();
            }
            return View(tipoHabitacion);
        }

        // GET: TipoHabitacion/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Message = null;
            List<TipoHabitacion> listaTipoHabitaciones = await tipoHabitacionBL.ListarTipoDeHabitacionesActivas();
            ViewData["TipoDeHabitacionesExistentes"] = listaTipoHabitaciones.Take(10).ToList();
            return View();
        }

        // POST: TipoHabitacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdTipoDeHabitacion,TipoDeHabitacion,Descripcion,Estado")] TipoHabitacion tipoHabitacion)
        {
            List<TipoHabitacion> listaTipoHabitaciones = await tipoHabitacionBL.ListarTipoDeHabitacionesActivas();
            ViewData["TipoDeHabitacionesExistentes"] = listaTipoHabitaciones.Take(10).ToList();
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await tipoHabitacionBL.AgregarTipoDeHabitacionUnico(tipoHabitacion);

                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el nuevo tipo de habitación.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El tipo de habitación ya existe.";
                        break;
                    case 3:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(tipoHabitacion);
            }
            ViewBag.Message = "Información incompleta.";
            return View(tipoHabitacion);
        }

        // GET: TipoHabitacion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoHabitacion tipoHabitacion = await tipoHabitacionBL.BuscarTipoDeHabitacionPorId(Convert.ToInt32(id));
            if (tipoHabitacion == null)
            {
                return HttpNotFound();
            }
            return View(tipoHabitacion);
        }

        // POST: TipoHabitacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdTipoDeHabitacion,TipoDeHabitacion,Descripcion,Estado")] TipoHabitacion tipoHabitacion)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await tipoHabitacionBL.EditarTipoDeHabitacion(tipoHabitacion);
                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, el cambio no fué posible salvarlo.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "Ya existe un tipo de habitación con el mismo nombre.";
                        break;
                    case 3:
                        mensaje = "No se han hecho cambios."; 
                        break;
                    case 4:
                        mensaje = "Upss, los datos se fueron incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(tipoHabitacion);
            }
            ViewBag.Message = "Información incompleta.";
            return View(tipoHabitacion);
        }

        // GET: TipoHabitacion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoHabitacion tipoHabitacion = await tipoHabitacionBL.BuscarTipoDeHabitacionPorId(Convert.ToInt32(id));
            if (tipoHabitacion == null)
            {
                return HttpNotFound();
            }
            return View(tipoHabitacion);
        }

        // POST: TipoHabitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await tipoHabitacionBL.EliminarTipoDeHabitacion(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar el tipo de habitación.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, el tipo de habitación a eliminar no existe.";
                    break;
                case 3:
                    mensaje = "Se recibió un identificador incorrecto.";
                    break;
            }
            ViewBag.Message = mensaje;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
