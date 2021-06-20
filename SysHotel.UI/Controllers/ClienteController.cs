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
using SysHotel.EL.Paginador;


namespace SysHotel.UI.Controllers
{
    public class ClienteController : Controller
    {
        private ClienteBL clienteBL = new ClienteBL();
        private BDComun db = new BDComun();

        //Variables para el paginador
        private const int registroPorPagina = 15;
        private List<Cliente> clientes;
        private PaginadorGenerico<Cliente> paginadorCliente;

       

        // GET: Cliente
        public async Task<ActionResult> Index(string busqueda, int pagina = 1)
        {
            //Recuperamos la lista completa de categorias
            clientes = await clienteBL.ListarClientesActivos();

            //BUSQUEDA
            //Filtramos una nueva lista segun la busqueda
            if(!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    clientes = clientes.Where(x => x.Nombres.ToUpper().Contains(item) ||
                                                   x.Apellidos.ToUpper().Contains(item) ||
                                                   x.FechaNacimiento.ToString().Contains(item) ||
                                                   x.TipoDocumento.ToUpper().Contains(item) ||
                                                   x.NumeroDocumento.ToString().Contains(item) ||
                                                   x.Telefono.Contains(item) ||
                                                   x.Correo.ToUpper().Contains(item) ||
                                                   x.Direccion.ToUpper().Contains(item))
                                                    .ToList();
                }                      
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el total de registros encontrados
            totalRegistros = clientes.Count();

            //Se obtiene la lista de registro por pagina
            List<Cliente> listaClientes = clientes.OrderBy(x => x.Nombres)
                                                  .Skip((pagina - 1) * registroPorPagina)
                                                  .Take(registroPorPagina)
                                                  .ToList();
            //Numero total de paginas
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de la clase paginador generico
            paginadorCliente = new PaginadorGenerico<Cliente>
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaClientes
            };

            return View(paginadorCliente);
        }

        // GET: Cliente/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = await clienteBL.BuscarClientePorId(Convert.ToInt32(id));
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public ActionResult Create()
        {
            //Se crea el array para el dropdown tipo documento de la vista.
            string[] TipoDocumento = { "DUI", "PASAPORTE" };
            ViewBag.TipoDocumento = new SelectList(TipoDocumento);
            return View();
        }

        // POST: Cliente/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdCliente,Nombres,Apellidos,FechaNacimiento,TipoDocumento,NumeroDocumento,Telefono,Correo,Direccion,Estado")] Cliente cliente)
        {
            string[] TipoDocumento = { "DUI", "NIT", "PASAPORTE" };

            if (ModelState.IsValid)
            {
                int x = await clienteBL.AgregarClienteUnico(cliente);
                string mensaje = "";
                switch (x)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el nuevo cliente.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El número del DUI no debe tener letras ni guiones.";
                        break;
                    case 3:
                        mensaje = "DUI inválido.";
                        break;
                    case 4:
                        mensaje = "Entrada de DUI incorrecta.";
                        break;
                    case 5:
                        mensaje = "El cliente ya está registrado.";
                        break;
                    case 6:
                        mensaje = "Datos incompletos.";
                        break;
                }
                //Se crea el array para el dropdown tipo documento de la vista.
                ViewBag.TipoDocumento = new SelectList(TipoDocumento);
                ViewBag.Message = mensaje;
                return View(cliente);
            }
            //Se crea el array para el dropdown tipo documento de la vista.
            ViewBag.TipoDocumento = new SelectList(TipoDocumento);
            ViewBag.Message = "Información incompleta.";
            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = await clienteBL.BuscarClientePorId(Convert.ToInt32(id));
            if (cliente == null)
            {
                return HttpNotFound();
            }
            //Se crea el array para el dropdown tipo documento de la vista
            string[] TipoDocumento = { "DUI", "NIT", "PASAPORTE" };
            ViewBag.TipoDocumento = new SelectList(TipoDocumento);
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdCliente,Nombres,Apellidos,FechaNacimiento,TipoDocumento,NumeroDocumento,Telefono,Correo,Direccion,Estado")] Cliente cliente)
        {
            //Se crea el array para el dropdown tipo documento de la vista.
            string[] TipoDocumento = { "DUI", "PASAPORTE" };
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await clienteBL.EditarCliente(cliente);
                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el cambio.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El número del DUI no debe tener letras ni guiones.";
                        break;
                    case 3:
                        mensaje = "DUI inválido.";
                        break;
                    case 4:
                        mensaje = "Entrada de DUI incorrecta.";
                        break;
                    case 5:
                        mensaje = "El cliente ya está registrado.";
                        break;
                    case 6:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 7:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.TipoDocumento = new SelectList(TipoDocumento);
                return View(cliente);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.TipoDocumento = new SelectList(TipoDocumento);
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = await clienteBL.BuscarClientePorId(Convert.ToInt32(id));
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await clienteBL.EliminarCliente(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar el cliente.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, el cliente a eliminar no existe.";
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
