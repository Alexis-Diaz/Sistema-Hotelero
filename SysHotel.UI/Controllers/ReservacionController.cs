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
using SysHotel.EL.Login;
using SysHotel.BL;
using SysHotel.UI.Filtros;
using SysHotel.EL.Paginador;
using SysHotel.EL.Tags;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class ReservacionController : Controller
    {
        //Instancias
        private ReservacionBL reservacionBL = new ReservacionBL();
        private ClienteBL clienteBL = new ClienteBL();
        private HabitacionBL habitacionBL = new HabitacionBL();
        private UsuarioBL usuarioBL = new UsuarioBL();
        private RolUsuarioBL rolUsuarioBL = new RolUsuarioBL();
        private BDComun db = new BDComun();

        //Variables para el paginador
        private const int registroPorPagina = 50;
        private List<Reservacion> reservacion;
        private PaginadorGenerico<Reservacion> paginadorReservacion;

        // GET: Reservacion
        public async Task<ActionResult> Index(string busqueda, int? estados = null, int pagina = 1)
        {
            int cantidadRegistrosActualizados = 0;//este dato sirve para indicarle al usuario si se han actulizado registros
            //Recuperamos la lista de reservaciones
            if (estados == null || estados == 0)
            {
                reservacion = await reservacionBL.ListarTodasLasReservaciones();
                //Cuando el usuario visite la vista index antes se verificaran las reservas y
                //todas aquellas que no se marcaron como recibidas despues de un dia pasaran 
                //a ser automaticamente vencida con un estado 5.
                foreach (var item in reservacion)
                {
                    DateTime fechaDeVencimiento = DateTime.Now;
                    if (item.DiaEntrada < fechaDeVencimiento && item.Estado == 1)
                    {
                        //Cuando estuve tratando de modificar el estado de manera automatica a 5, se me presento el siguiente problema:
                        //como le cambiaba a la variable item del foreach su estado a 5 (item.Estado = 5;) el objeto contenido en al array
                        //sufria el mismo cambio porque item es una referencia del objeto contenido en el array, el problema era que cuando mas adelante en la capa
                        //ReservacionDAL hacia el edit no funcionaba porque de alguna manera la reserva que se iba a buscar a la base de datos
                        //tambien sufria el cambio y su estado lo mostraba en 5 cuando en realidad era 1 y entonces el metodo me devolvia como
                        //respuesta que no se habian hecho cambios y por tanto no permitia hacer la modificación en la base de datos.
                        //Asi para solucionar ese problema era mejor crear un nuevo objeto y pasar todos sus valores como se ve a continuación.
                        int res = await reservacionBL.EditarEstadoDeLaReserva(item.IdReservacion, 5);

                        if (res == 1)
                        {
                            cantidadRegistrosActualizados++;//cantidad de actualizaciones.
                        }
                    }
                }
            }
            if (estados != null && estados > 0)
            {
                reservacion = await reservacionBL.ListarReservacionesPorEstado((int)estados);
            }

            //BUSQUEDA
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach (var item in busqueda.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    reservacion = reservacion.Where(x => x.cliente.Nombres.ToUpper().Contains(item) ||
                                                         x.cliente.Apellidos.ToUpper().Contains(item) ||
                                                         x.DiaEntrada.ToString().Contains(item) ||
                                                         x.DiaEntrada.ToString().Contains(item) ||
                                                         x.NumeroPersonas.ToString().Contains(item) ||
                                                         x.habitacion.NumeroHabitacion.ToString().Contains(item) ||
                                                         x.usuario.NombreUsuario.ToUpper().Contains(item))
                                                          .ToList();
                }
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el total de registros encontrados
            totalRegistros = reservacion.Count();

            //Se obtiene la lista de registro segun la pagina
            List<Reservacion> listaReservacion = reservacion.OrderByDescending(x => x.DiaEntrada)
                                                            .Skip((pagina - 1) * registroPorPagina)
                                                            .Take(registroPorPagina)
                                                            .ToList();

            //Numero total de paginas
            //El metodo Ceiling aproxima el numero hacia arrioba Floor lo hace hacia abajo
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de clase de paginador generico
            paginadorReservacion = new PaginadorGenerico<Reservacion>
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaReservacion
            };

            string[] Estados = {"Selecciona un estado", "Reservado", "En Curso", "Finalizado", "Cancelado", "Vencido" };
            List<EstadosReservacion> ListaEstados = new List<EstadosReservacion>();
            for (int x = 0; x <= 5; x++)
            {
                EstadosReservacion Estado = new EstadosReservacion()
                {
                    NumeroEstado = x,
                    Estado = Estados[x]
                };
                ListaEstados.Add(Estado);
            }
            ViewBag.Estado = Estados;//Estos estados sirven para enviar a la tabla de la vista un texto en lugar del numero de estado.
            ViewBag.Estados = new SelectList(ListaEstados, "NumeroEstado", "Estado", 0);//Este ViewBag sirve para llenar el dropdown list.
            ViewBag.RegistrosActualizados = cantidadRegistrosActualizados;
            return View(paginadorReservacion);
        }

        // GET: Reservacion/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.Reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            return View(reservacion);
        }

        //GET: Reservacion/ConsultarHabitaciones
        public ActionResult ConsultarHabitaciones()
        {
            return View();
        }

        //POST: Reservacion/ConsultarHabitaciones
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConsultarHabitaciones([Bind(Include ="DiaEntrada, DiaSalida")] Reservacion reservacion)
        {
            string mensaje;
            if (!ModelState.IsValid)
            {
                mensaje = "No ha ingresado ninguna fecha.";
                ViewBag.Message = mensaje;
                return View(reservacion);
            }
            

            if (reservacion.DiaEntrada <= DateTime.Now.AddDays(7))
            {
                mensaje = "La reservación debe hacerse con 7 días de anticipación.";//la reservación debe hacerse con 7 días de anticipación.
                ViewBag.Message = mensaje;
                return View(reservacion);
            }
            if (reservacion.DiaSalida <= reservacion.DiaEntrada)
            {
                mensaje = "La fecha de salida no puede ser menor o igual al día de entrada.";//La fecha de salida es menor o igual a fecha de entrada.
                ViewBag.Message = mensaje;
                return View(reservacion);
            }
            //Creamos una lista de reservaciones que coincidan con los dias en que se pretende realizar la 
            //nueva reserva. Luego se crea otra lista de habitaciones. A esta última lista se le restan
            //las habitaciones que estan reservadas en las fechas indicadas para dejar las habitaciones 
            //disponibles.
            try
            {
                var HabitacionesDisponibles = await reservacionBL.ConsultarHabitacionesPorFechaEntradaYSalida(reservacion.DiaEntrada, reservacion.DiaSalida);
                if(HabitacionesDisponibles.Count() == 0)
                {
                    mensaje = "No hay habitaciones disponibles para estas fechas.";
                    ViewBag.Message = mensaje;
                    return View(reservacion);
                }
                else
                {
                    //Declaramos dos variables temporales para poder a ellas desde el metodo de accion Create
                    //Los valores retenidos seran las fechas que el usuario ha ingresado, de esta manera reutilizamos
                    //la información que el usuario va ingresando al realizar la reserva.
                    TempData["DiaEntrada"] = reservacion.DiaEntrada;
                    TempData["DiaSalida"] = reservacion.DiaSalida;

                    ViewData["ListaHabitacionesDisponibles"] = HabitacionesDisponibles;
                    return View(reservacion);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Reservacion/Create
        public async Task<ActionResult> Create(int id = 0)
        {
            Reservacion reservacion = new Reservacion();
            reservacion.FechaReservacion = DateTime.Now;

            //El parametro recibido es el de la habitación, este parametro es opcional
            //Solo se envia en caso de que el usuario vaya primero ha consultar las 
            //habitaciones disponibles.
            if (id > 0)
            {
                Habitacion habitacionAReservar = await habitacionBL.BuscarHabitacionPorId(id);
                if (habitacionAReservar != null)
                {
                    //Preparamos los datos que se enviaran a la vista para que se autocomplete
                    //Primero verificamos si las variables temporales existen
                    if (TempData.ContainsKey("DiaEntrada") && TempData.ContainsKey("DiaSalida"))
                    {
                         
                        reservacion.DiaEntrada = (DateTime)TempData["DiaEntrada"];
                        reservacion.DiaSalida = (DateTime)TempData["DiaSalida"];
                    }
                   
                    int IdUsuario = SessionHelper.GetUser();//recuperamos el usuario de la session actual
                    Usuario user = await usuarioBL.BuscarUsuarioPorId(IdUsuario);//buscamos la iformacion del usuario
                    RolUsuario rolUser = await rolUsuarioBL.BuscarRolUsuarioPorId(user.IdRolUsuario);//recuperamos el rol del usuario

                    if(rolUser.Rol.ToLower() == "cliente")
                    {
                        //Buscamos el usuario que esta registrado como cliente por medio de su documento.
                        Cliente cliente = await clienteBL.BuscarClientePorDocumentoDeIdentidad(user.DUI);
                        //Lo autoseleccionamos en el dropdownList
                        ViewBag.IdCliente = new SelectList(await clienteBL.ListarClientesActivos(), "IdCliente", "Nombres",cliente.IdCliente);
                        ViewBag.IdHabitacion = new SelectList(await habitacionBL.ListarHabitacionesActivas(), "IdHabitacion", "NumeroHabitacion", habitacionAReservar.IdHabitacion);
                        return View(reservacion);
                    }

                    ViewBag.IdCliente = new SelectList(await clienteBL.ListarClientesActivos(), "IdCliente", "Nombres");
                    ViewBag.IdHabitacion = new SelectList(await habitacionBL.ListarHabitacionesActivas(), "IdHabitacion", "NumeroHabitacion", habitacionAReservar.IdHabitacion);
                    return View(reservacion);
                }
            }

            //Si la reserva se realiza desde cero no se envia nada a la vista.
            ViewBag.IdCliente = new SelectList( await clienteBL.ListarClientesActivos(), "IdCliente", "Nombres");
            ViewBag.IdHabitacion = new SelectList(await habitacionBL.ListarHabitacionesActivas(), "IdHabitacion", "NumeroHabitacion");
            return View(reservacion);
        }

        // POST: Reservacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdReservacion,DiaEntrada,DiaSalida,FechaReservacion,NumeroPersonas,Comentarios,Estado,IdCliente,IdHabitacion,IdUsuario")] Reservacion reservacion)
        {
            string message = "No debe dejar campos vacíos.";
            if (ModelState.IsValid)
            {
                //Recuperamos el usuario con la session actual para dejar evidencia del registro.
                reservacion.IdUsuario = SessionHelper.GetUser();
                int respuesta = await reservacionBL.AgregarReservaNueva(reservacion);
                

                switch (respuesta)
                {
                    case 0:
                        message = "Ocurrio un error crítico, no fué posible guardar la reservación.";
                        break;
                    case 1:
                        return RedirectToAction("Index");
                    case 2:
                        message = "La reservación debe hacerse con 7 días de anticipación.";
                        break;
                    case 3:
                        message = "La fecha de salida no puede ser menor o igual a la fecha de entrada.";
                        break;
                    case 4:
                        message = "La fecha de entrada coincide con el día de limpieza de la habitación.";
                        break;
                    case 5:
                        message = "La reserva coincide con otra reserva.";
                        break;
                    case 6:
                        message = "Los datos estan incompletos.";
                        break;
                }
                ViewBag.Message = message;
                ViewBag.IdCliente = new SelectList(await clienteBL.ListarClientesActivos(), "IdCliente", "Nombres", reservacion.IdCliente);
                ViewBag.IdHabitacion = new SelectList(await habitacionBL.ListarHabitacionesActivas(), "IdHabitacion", "NumeroHabitacion", reservacion.IdHabitacion);
                return View(reservacion);
            }
            ViewBag.Message = message;
            ViewBag.IdCliente = new SelectList(await clienteBL.ListarClientesActivos(), "IdCliente", "Nombres", reservacion.IdCliente);
            ViewBag.IdHabitacion = new SelectList(await habitacionBL.ListarHabitacionesActivas(), "IdHabitacion", "NumeroHabitacion", reservacion.IdHabitacion);
            return View(reservacion);
        }

        // GET: Reservacion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.Reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCliente = new SelectList(db.Clientes, "IdCliente", "Nombres", reservacion.IdCliente);
            ViewBag.IdHabitacion = new SelectList(db.Habitacions, "IdHabitacion", "Descripcion", reservacion.IdHabitacion);
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "Nombres", reservacion.IdUsuario);
            return View(reservacion);
        }

        // POST: Reservacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdReservacion,DiaEntrada,DiaSalida,FechaReservacion,NumeroPersonas,Comentarios,Estado,IdCliente,IdHabitacion,IdUsuario")] Reservacion reservacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdCliente = new SelectList(db.Clientes, "IdCliente", "Nombres", reservacion.IdCliente);
            ViewBag.IdHabitacion = new SelectList(db.Habitacions, "IdHabitacion", "Descripcion", reservacion.IdHabitacion);
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "Nombres", reservacion.IdUsuario);
            return View(reservacion);
        }

        //GET: Reservacion/Marcar/5
        public async Task<ActionResult> Marcar(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await reservacionBL.ObtenerReservaPorId((int)id);
            if(reservacion == null)
            {
                return HttpNotFound();
            }
            return View(reservacion);
        }

        //GET: Reservacion/Cancel/5
        public async Task<ActionResult> Cancel (int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await reservacionBL.ObtenerReservaPorId((int)id);
            if(reservacion == null)
            {
                return HttpNotFound();
            }
            return View(reservacion);
        }

        //POST: Reservacion/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelConfirmed(int id)
        {
            int respuesta = await reservacionBL.CambiarEstadoDeReservacion(id, 4);
            if (respuesta == 0)
            {
                ViewBag.Message = "Error critico, no fué posible cancelar la reserva.";
                return View();
            }
            if(respuesta == 2)
            {
                ViewBag.Message = ("Ocurrió algo inesperado, la reservado ha sido eliminada por otro usuario.");
                return View();
            }
            return RedirectToAction("Index");

        }

        // GET: Reservacion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.Reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            return View(reservacion);
        }

        // POST: Reservacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reservacion reservacion = await db.Reservacions.FindAsync(id);
            db.Reservacions.Remove(reservacion);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
