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

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class ReservacionController : Controller
    {
        private ReservacionBL reservacionBL = new ReservacionBL();
        private ClienteBL clienteBL = new ClienteBL();
        private HabitacionBL habitacionBL = new HabitacionBL();
        private UsuarioBL usuarioBL = new UsuarioBL();
        private RolUsuarioBL rolUsuarioBL = new RolUsuarioBL();
        private BDComun db = new BDComun();

        // GET: Reservacion
        public async Task<ActionResult> Index()
        {
            var reservacions = db.Reservacions.Include(r => r.cliente).Include(r => r.habitacion).Include(r => r.usuario);
            return View(await reservacions.ToListAsync());
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
            string mensaje = "No ha ingresado ninguna fecha.";

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
