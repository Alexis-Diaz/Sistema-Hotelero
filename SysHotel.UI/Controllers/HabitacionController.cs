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

using SysHotel.EL.Tags;
using SysHotel.EL.Paginador;
using SysHotel.BL;
using SysHotel.UI.Filtros;
using SysHotel.EL.View;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class HabitacionController : Controller
    {
        private BDComun db = new BDComun();
        private HabitacionBL habitacionBL = new HabitacionBL();
        private TipoHabitacionBL tipoHabitacionBL = new TipoHabitacionBL();

        //Variables para el paginador generico
        private List<Habitacion> habitaciones;
        private const int registroPorPagina = 12;
        private PaginadorGenerico<Habitacion> paginadorHabitacion;

        // GET: Habitacion
        public async Task<ActionResult> Index(string busqueda = "", int idCategoria = 0, int pagina = 0)
        {
            //Recuperamos la lista de habitaciones completa
            habitaciones = await habitacionBL.ListarHabitacionesActivas();

            //BUSQUEDA
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach (var item in busqueda.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    habitaciones = habitaciones.Where(x => x.NumeroHabitacion.ToString().Contains(item) ||
                                                           x.Descripcion.ToUpper().Contains(item) ||
                                                           x.NumeroCamas.ToString().Contains(item) ||
                                                           x.Precio.ToString().Contains(item))
                                                            .ToList();
                }
            }

            //POR CATEGORIA
            if (idCategoria > 0)
            {
                ViewBag.IdCategoriaGuardada = idCategoria;//salvamos el id para que cuando se cambie la pagina se conserve la categoria seleccionada.
                ViewBag.Categoria = await tipoHabitacionBL.BuscarTipoDeHabitacionPorId(idCategoria);//Para mostrar en pantalla la categoria en la que nos encontramos.
                habitaciones = habitaciones.Where(x => x.IdTipoDeHabitacion == idCategoria).ToList();
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //se encuentra el numero de registros
            totalRegistros = habitaciones.Count();

            //se crea la pagina de habitaciones
            List<Habitacion> listaHabitaciones = habitaciones.OrderBy(x => x.NumeroHabitacion)
                                                             .Skip((pagina - 1) * registroPorPagina)
                                                             .Take(registroPorPagina)
                                                             .ToList();

            //Numero total de paginas
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de paginador generico
            paginadorHabitacion = new PaginadorGenerico<Habitacion>()
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaHabitaciones
            };

            //Le agregamos una categoria geneica "Seleccionar una categoría"
            List<TipoHabitacion> listaCategoria = await tipoHabitacionBL.ListarTipoDeHabitacionesActivas();
            TipoHabitacion tipo = new TipoHabitacion()
            {
                IdTipoDeHabitacion = 0,
                TipoDeHabitacion = "Selecciones una categoría."
            };

            listaCategoria.Add(tipo);
            ViewBag.IdCategoria = new SelectList(listaCategoria, "IdTipoDeHabitacion", "TipoDeHabitacion", idCategoria);
            return View(paginadorHabitacion);
        }

        // GET: Habitacion/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Habitacion habitacion = await habitacionBL.BuscarHabitacionPorId(Convert.ToInt32(id));
            if (habitacion == null)
            {
                return HttpNotFound();
            }
            return View(habitacion);
        }

        // GET: Habitacion/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.IdTipoDeHabitacion = new SelectList(await tipoHabitacionBL.ListarTipoDeHabitacionesActivas(), "IdTipoDeHabitacion", "TipoDeHabitacion");
            return View();
        }

        // POST: Habitacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HabitacionView habitacionView)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                var picture = string.Empty;
                var folder = "~/Content/Img/Habitaciones";

                if (habitacionView.Foto != null)
                {
                    picture = RutaFoto.GuardarRutaFoto(habitacionView.Foto, folder);
                    picture = string.Format("{0}/{1}", folder, picture);
                }
                //ahora debemos convertir la clase AlimentoView a la clase Alimento para que nos permita guardar en la BD
                var habitacion = ToHabitacion(habitacionView);
                habitacion.Imagen = picture;

                var valor = await habitacionBL.AgregarHabitacionUnica(habitacion);
                switch (valor)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar la nueva habitación.";
                        break;

                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El número de habitación ya existe.";
                        break;
                    case 3:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.IdTipoDeHabitacion = new SelectList(await tipoHabitacionBL.ListarTipoDeHabitacionesActivas(), "IdTipoDeHabitacion", "TipoDeHabitacion", habitacionView.IdTipoDeHabitacion);
                return View(habitacionView);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.IdTipoDeHabitacion = new SelectList(await tipoHabitacionBL.ListarTipoDeHabitacionesActivas(), "IdTipoDeHabitacion", "TipoDeHabitacion", habitacionView.IdTipoDeHabitacion);
            return View(habitacionView);
        }

        // GET: Habitacion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Habitacion habitacion = await habitacionBL.BuscarHabitacionPorId(Convert.ToInt32(id));
            if (habitacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTipoDeHabitacion = new SelectList(await tipoHabitacionBL.ListarTipoDeHabitacionesActivas(), "IdTipoDeHabitacion", "TipoDeHabitacion", habitacion.IdTipoDeHabitacion);
            //Convertimos la habitacion a habitacionView
            HabitacionView habitacionView = ToHabitacionView(habitacion);
            return View(habitacionView);
        }

        // POST: Habitacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HabitacionView habitacionView)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                var picture = string.Empty;
                var folder = "~/Content/Img/Habitaciones";

                if (habitacionView.Foto != null)
                {
                    picture = RutaFoto.GuardarRutaFoto(habitacionView.Foto, folder);
                    picture = string.Format("{0}/{1}", folder, picture);
                }
                //ahora debemos convertir la clase AlimentoView a la clase Alimento para que nos permita guardar en la BD
                var habitacion = ToHabitacion(habitacionView);
                //Se verifica si ha cambiado la imagen
                if (!string.IsNullOrEmpty(picture))
                {
                    habitacion.Imagen = picture;
                }

                var valor = await habitacionBL.EditarHabitacion(habitacion);
                switch (valor)
                {
                    case 0:
                        mensaje = "Ocurrió un error, el cambio no fué posible salvarlo.";
                        break;

                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "Ya existe una habitación con el mismo número.";
                        break;
                    case 3:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 4:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.IdTipoDeHabitacion = new SelectList(await tipoHabitacionBL.ListarTipoDeHabitacionesActivas(), "IdTipoDeHabitacion", "TipoDeHabitacion", habitacionView.IdTipoDeHabitacion);
                return View(habitacionView);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.IdTipoDeHabitacion = new SelectList(await tipoHabitacionBL.ListarTipoDeHabitacionesActivas(), "IdTipoDeHabitacion", "TipoDeHabitacion", habitacionView.IdTipoDeHabitacion);
            return View(habitacionView);
        }

        // GET: Habitacion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Habitacion habitacion = await habitacionBL.BuscarHabitacionPorId(Convert.ToInt32(id));
            if (habitacion == null)
            {
                return HttpNotFound();
            }
            return View(habitacion);
        }

        // POST: Habitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await habitacionBL.EliminarHabitación(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar la habitación.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, la habitación a eliminar no existe.";
                    break;
                case 3:
                    mensaje = "Se recibió un identificador incorrecto.";
                    break;
            }
            ViewBag.Message = mensaje;
            return View();
        }

        //Metodo para convertir de HabitacionView a Habitacion
        public Habitacion ToHabitacion(HabitacionView habitacionView)
        {
            return new Habitacion()
            {
                IdHabitacion = habitacionView.IdHabitacion,
                NumeroHabitacion = habitacionView.NumeroHabitacion,
                Descripcion = habitacionView.Descripcion,
                NumeroCamas = habitacionView.NumeroCamas,
                TVCable = habitacionView.TVCable,
                Wifi = habitacionView.Wifi,
                AireAcondicionado = habitacionView.AireAcondicionado,
                Precio = habitacionView.Precio,
                Imagen = habitacionView.Imagen,
                IdTipoDeHabitacion = habitacionView.IdTipoDeHabitacion,
                Estado = habitacionView.Estado
            };
        }

        //Metodo para convertir de Habitacion a HabitacionView
        public HabitacionView ToHabitacionView (Habitacion habitacion)
        {
            return new HabitacionView()
            {
                IdHabitacion = habitacion.IdHabitacion,
                NumeroHabitacion = habitacion.NumeroHabitacion,
                Descripcion = habitacion.Descripcion,
                NumeroCamas = habitacion.NumeroCamas,
                TVCable = habitacion.TVCable,
                Wifi = habitacion.Wifi,
                AireAcondicionado = habitacion.AireAcondicionado,
                Precio = habitacion.Precio,
                Imagen = habitacion.Imagen,
                Estado = habitacion.Estado,
                IdTipoDeHabitacion = habitacion.IdTipoDeHabitacion
            };
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
