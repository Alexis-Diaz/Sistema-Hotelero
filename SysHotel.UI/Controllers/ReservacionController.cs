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

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class ReservacionController : Controller
    {
        private ReservacionBL reservacionBL = new ReservacionBL();
        private ClienteBL clienteBL = new ClienteBL();
        private HabitacionBL habitacionBL = new HabitacionBL();
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
                return View();
            }
            if (reservacion.DiaSalida <= reservacion.DiaEntrada)
            {
                mensaje = "La fecha de salida no puede ser menor o igual al día de entrada.";//La fecha de salida es menor o igual a fecha de entrada.
                ViewBag.Message = mensaje;
                return View();
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
                    return View();
                }
                else
                {
                    ViewData["ListaHabitacionesDisponibles"] = HabitacionesDisponibles;
                    return View();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Reservacion/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.IdCliente = new SelectList( await clienteBL.ListarClientesActivos(), "IdCliente", "Nombres");
            ViewBag.IdHabitacion = new SelectList(await habitacionBL.ListarHabitacionesActivas(), "IdHabitacion", "NumeroHabitacion");
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "Nombres");
            return View();
        }

        // POST: Reservacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdReservacion,DiaEntrada,DiaSalida,FechaReservacion,NumeroPersonas,Comentarios,Estado,IdCliente,IdHabitacion,IdUsuario")] Reservacion reservacion)
        {
            if (ModelState.IsValid)
            {
                db.Reservacions.Add(reservacion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdCliente = new SelectList(db.Clientes, "IdCliente", "Nombres", reservacion.IdCliente);
            ViewBag.IdHabitacion = new SelectList(db.Habitacions, "IdHabitacion", "Descripcion", reservacion.IdHabitacion);
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "Nombres", reservacion.IdUsuario);
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
