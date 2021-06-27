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
using SysHotel.EL.View;
using SysHotel.BL.Service;
using SysHotel.EL.Login;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class FacturaController : Controller
    {
        //Instancias
        private ReservacionBL reservacionBL = new ReservacionBL();
        private FacturaBL facturaBL = new FacturaBL();
        private DetalleBL detalleBL = new DetalleBL();
        private GenerarCorrelativo generarCorrelativo = new GenerarCorrelativo();
        private UsuarioBL usuarioBL = new UsuarioBL();
        private BDComun db = new BDComun();

        //Variables para el paginador
        private const int registrosPorPagina = 50;
        private List<Reservacion> reservacion;
        private PaginadorGenerico<Reservacion> paginadorReservacion;

        // GET: Factura
        public async Task<ActionResult> Index()
        {
            List<Factura> listaFacturas = await facturaBL.ObtenerTodasLasFacturas();
            return View(listaFacturas);
        }

        public async Task<ActionResult> GenerarFactura(string busqueda, int pagina = 1)
        {
            //recuperamos la lista de reservaciones con estado 2
            reservacion = await reservacionBL.ListarReservacionesPorEstado(2);
            
            //BUSQUEDA
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    reservacion = reservacion.Where(x => x.cliente.Nombres.ToUpper().Contains(item) ||
                                                        x.cliente.Apellidos.ToUpper().Contains(item) ||
                                                        x.DiaEntrada.ToString().Contains(item) ||
                                                        x.DiaSalida.ToString().Contains(item) ||
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

            //Se obtiene la lista de reservaciones por pagina
            List<Reservacion> listaReservaciones = reservacion.OrderBy(x => x.DiaSalida)
                                                              .Skip((pagina-1)*registrosPorPagina)
                                                              .Take(registrosPorPagina)
                                                              .ToList();
            //Numero total de paginas
            //El metodo Ceiling aproxima el numero hacia arriba Floor lo hace hacia abajo
            totalPaginas = (int)Math.Ceiling((double)totalPaginas / registrosPorPagina);
            List<Reservacion> listaReservacionesEnCurso = await reservacionBL.ListarReservacionesPorEstado(2);

            //Llenamos el paginador generico
            paginadorReservacion = new PaginadorGenerico<Reservacion>()
            {
                RegistroPorPagina = registrosPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaReservaciones
            };
            return View(paginadorReservacion);
        }

        // GET: Factura/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = await facturaBL.ObtenerFacturaPorId((int)id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // GET: Factura/Create/5
        public async Task<ActionResult> Create(int? id)
        {
            //Verificamos que el id de la reserva sea correcto
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Buscamos la reserva que queremos facturar
            Reservacion reservacion = await reservacionBL.ObtenerReservaPorId((int)id);
            //Buscamos todos los detalles de gastos adicionales por reservacion
            List<Detalle> detallePorReservacion = await detalleBL.BuscarDetallesPorIdReservacion((int)id);
            //Generamos el numero de la factura
            string correlativo = await generarCorrelativo.GenerarNumeroCorrelativoDeFactura("24DS00F");
            //Obtenemos el usuario logueado para dejar registro en la factura
            int idUser = SessionHelper.GetUser();
            Usuario user = await usuarioBL.BuscarUsuarioPorId(idUser);

            //Calculamos el subtotal, iva y total de la factura
            decimal subtotalFactura = 0;
            decimal ivaFactura = 0;
            decimal totalFactura = 0;
            decimal precioReservacion = reservacion.habitacion.Precio;
            decimal ivaReservacion = precioReservacion * (decimal)0.13;
            decimal totalReservacion = precioReservacion + ivaReservacion;

            foreach(var item in detallePorReservacion)
            {
                decimal subtotalDetalle = item.TotalDetalle;
                decimal ivaDetalle = subtotalDetalle * (decimal)0.13;
                decimal totalDetalle = subtotalDetalle + ivaDetalle;

                subtotalFactura += subtotalDetalle;
                ivaFactura += ivaDetalle;
                totalFactura += totalDetalle;
            }

            subtotalFactura += precioReservacion;
            ivaFactura += ivaReservacion;
            totalFactura += totalReservacion;

            FacturaView<Detalle> facturaView = new FacturaView<Detalle>()
            {
                NumeroFactura = correlativo,
                NumeroDocumento = reservacion.cliente.NumeroDocumento,
                NombreCompleto = reservacion.cliente.Nombres + " " + reservacion.cliente.Apellidos,
                DireccionCliente = reservacion.cliente.Direccion,
                FechaEmision = DateTime.Now,
                Vendedor = user.Nombres + " " + user.Apellidos,
                Cantidad = 1,
                Descripcion = reservacion.habitacion.Descripcion,
                PrecioUnitario = reservacion.habitacion.Precio,
                SubtotalProducto = reservacion.habitacion.Precio,
                SubtotalFactura = subtotalFactura,
                IVA = ivaFactura,
                TotalFactura = totalFactura,
                IdReservacion = reservacion.IdReservacion,
                Detalle = detallePorReservacion
            };
            TempData["factura"] = facturaView;//Esta información la recuperaremos en el metodo de accion Create
            return View(facturaView);
        }

        // POST: Factura/Create/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id)
        {
            if (TempData.ContainsKey("factura"))
            {
                FacturaView<Detalle> fv = TempData["factura"] as FacturaView<Detalle>;
                Reservacion reservacion = await reservacionBL.ObtenerReservaPorId(id);
                if (reservacion != null)
                {
                    int res = await reservacionBL.CambiarEstadoDeReservacion(id, 3);
                    if (res == 1)
                    {
                        Factura nuevaFactura = new Factura()
                        {
                            NumeroFactura = fv.NumeroFactura,
                            Vendedor = fv.Vendedor,
                            FechaEmision = fv.FechaEmision,
                            IVA = fv.IVA,
                            SubTotal = fv.SubtotalFactura,
                            TotalFactura = fv.TotalFactura,
                            Estado = 1,
                            IdReservacion = fv.IdReservacion
                        };
                        res = await facturaBL.AgregarFacturaUnica(nuevaFactura);
                        if (res == 1)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            return View( );
        }

        // GET: Factura/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Factura factura = await facturaBL.ObtenerFacturaPorId((int)id);
        //    if (factura == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(factura);
        //}

        //// POST: Factura/Edit/5
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "IdFactura,NumeroFactura,FechaEmision,IVA,SubTotal,TotalFactura,Estado,IdReservacion")] Factura factura)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(factura).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.IdReservacion = new SelectList(db.Reservacions, "IdReservacion", "Comentarios", factura.IdReservacion);
        //    return View(factura);
        //}

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
