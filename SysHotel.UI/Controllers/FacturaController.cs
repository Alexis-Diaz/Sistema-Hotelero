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

namespace SysHotel.UI.Controllers
{
    public class FacturaController : Controller
    {
        private ReservacionBL reservacionBL = new ReservacionBL();
        private BDComun db = new BDComun();

        // GET: Factura
        public async Task<ActionResult> Index()
        {
            var facturas = db.Facturas.Include(f => f.reservacion);
            return View(await facturas.ToListAsync());
        }

        public async Task<ActionResult> GenerarFactura()
        {
            List<Reservacion> listaReservacionesEnCurso = await reservacionBL.ListarReservacionesPorEstado(2);
            return View(listaReservacionesEnCurso);
        }

        // GET: Factura/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = await db.Facturas.FindAsync(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // GET: Factura/Create
        public ActionResult Create()
        {
            ViewBag.IdReservacion = new SelectList(db.Reservacions, "IdReservacion", "Comentarios");
            return View();
        }

        // POST: Factura/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdFactura,NumeroFactura,FechaEmision,IVA,SubTotal,TotalFactura,Estado,IdReservacion")] Factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Facturas.Add(factura);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdReservacion = new SelectList(db.Reservacions, "IdReservacion", "Comentarios", factura.IdReservacion);
            return View(factura);
        }

        // GET: Factura/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = await db.Facturas.FindAsync(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdReservacion = new SelectList(db.Reservacions, "IdReservacion", "Comentarios", factura.IdReservacion);
            return View(factura);
        }

        // POST: Factura/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdFactura,NumeroFactura,FechaEmision,IVA,SubTotal,TotalFactura,Estado,IdReservacion")] Factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdReservacion = new SelectList(db.Reservacions, "IdReservacion", "Comentarios", factura.IdReservacion);
            return View(factura);
        }

        // GET: Factura/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = await db.Facturas.FindAsync(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: Factura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Factura factura = await db.Facturas.FindAsync(id);
            db.Facturas.Remove(factura);
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
