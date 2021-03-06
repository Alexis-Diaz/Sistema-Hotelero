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
using SysHotel.EL.Tags;
using System.Text;

namespace SysHotel.UI.Controllers
{
    [AutenticadoAttribute]
    public class AlimentoController : Controller
    {
        private AlimentoBL alimentoBL = new AlimentoBL();
        private CategoriaAlimentoBL categoriaBL = new CategoriaAlimentoBL();
        private ProveedorBL proveedorBL = new ProveedorBL();
        private ReservacionBL reservacionBL = new ReservacionBL();

        //Variables para el paginador generico
        private const int registroPorPagina = 16;
        private List<Alimento> alimentos;
        private PaginadorGenerico<Alimento> paginadorAlimento;

        // GET: Alimento
        public async Task<ActionResult> Index(string busqueda = "", int idCategoria = 0, int pagina = 1)
        {
            //Recuperamos la lista completa de alimentos
            alimentos = await alimentoBL.ListarAlimentosDisponibles();

            //BUSQUEDA
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    alimentos = alimentos.Where(x => x.Nombre.ToUpper().Contains(item) ||
                                                     x.Descripcion.ToUpper().Contains(item) ||
                                                     Convert.ToString(x.Precio).Contains(item))
                                                      .ToList();
                }
            }

            //POR CATEGORIA
            if(idCategoria > 0)
            {
                ViewBag.IdCategoriaGuardada = idCategoria;//salvamos el id para que cuando se cambie de pagina conservemos la categoria seleccionada.
                ViewBag.Categoria = await categoriaBL.BuscarCategoriaPorId(idCategoria) ;//Para mostrar en pantalla en la categoria que nos encontramos.
                alimentos = alimentos.Where(x => x.IdCategoriaAlimento == idCategoria).ToList();
            }

            //PAGINACION

            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el numero de registros encontrados
            totalRegistros = alimentos.Count();

            //Se crea la pagina de alimentos
            List<Alimento> listaAlimentos = alimentos.OrderBy(x => x.Nombre)
                                                    .Skip((pagina - 1) * registroPorPagina)
                                                    .Take(registroPorPagina)
                                                    .ToList();
            //Numero total de paginas
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de paginador generico
            paginadorAlimento = new PaginadorGenerico<Alimento>()
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaAlimentos
            };
            //Le agregamos una categoria generica "Seleccionar una categoria"
            List<CategoriaAlimento> listaCategoria = await categoriaBL.ListarCategoriasActivas();
            CategoriaAlimento cat = new CategoriaAlimento()
            {
                IdCategoriaAlimento = 0,
                NombreCategoria = "Todas las categorías.",
            };
            listaCategoria.Add(cat);
            ViewBag.IdCategoria = new SelectList(listaCategoria, "IdCategoriaAlimento", "NombreCategoria", idCategoria);
            return View(paginadorAlimento);
        }

        //GET: ObtenerReseravacio
        public async Task<JsonResult>Reservacion(string id)
        {
            string mensaje = "Id inválido.";
            if(!string.IsNullOrEmpty(id))
            {
                //verificamos que el id sea un numero
                int idReservacion;
                bool res = int.TryParse(id, out idReservacion);
                if (res)
                {
                    //buscamos la reserva por el id
                    Reservacion reservacion = await reservacionBL.ObtenerReservaPorId(idReservacion);
                    
                    if(reservacion != null && (reservacion.Estado == 1 || reservacion.Estado == 2))
                    {
                        var r = new ReservacionView
                        {
                            IdReservacion = reservacion.IdReservacion,
                            IdHabitacion = reservacion.IdHabitacion,
                            IdUsuario = reservacion.IdUsuario,
                            IdCliente = reservacion.IdCliente,
                            NombreCliente = reservacion.cliente.Nombres + " " + reservacion.cliente.Apellidos,
                            NumeroHabitacion = reservacion.habitacion.NumeroHabitacion,
                            FechaReservacion = reservacion.FechaReservacion.ToShortDateString(),
                            Comentarios = reservacion.Comentarios,
                            NumeroPersonas = reservacion.NumeroPersonas,
                            DiaEntrada = reservacion.DiaEntrada.ToShortDateString(),
                            DiaSalida = reservacion.DiaSalida.ToShortDateString(),
                            Estado = reservacion.Estado

                        };
                        return Json(r, JsonRequestBehavior.AllowGet);//enviamos el objeto en formato json.
                    }
                    mensaje = "No existe una reserva activa con este id";
                }
            }
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        //POST:SetCookie
        [HttpPost]
        public JsonResult SetCookie(string idReservacion, string nombreCliente, string numeroHabitacion, DateTime diaEntrada, DateTime diaSalida, string idAlimento, string nombreAlimento, string precio, DateTime dia, string tiempoComida, string cantidad, string subtotal)
        {
            string key = string.Format("UY#9G#HF{0}",idReservacion);//Llave que identifica la cookie
            string message = "";//mensaje que se enviara al cliente
            if(dia >= diaEntrada && dia <= diaSalida)
            {
                if(tiempoComida == "1" || tiempoComida == "2" || tiempoComida == "3")
                {
                    int Cantidad;
                    int.TryParse(cantidad, out Cantidad);
                    if (Cantidad > 0)
                    {
                        string value = "";
                        string[] detallesDelPedido = { idReservacion, nombreCliente, numeroHabitacion, diaEntrada.ToString(), diaSalida.ToString(), idAlimento, nombreAlimento, precio, dia.ToString(), tiempoComida, cantidad, subtotal };
                        //verificamos si ya existe una cookie creada
                        var cookies = ControllerContext.HttpContext.Request.Cookies[key];
                        if (cookies!=null)
                        {
                            //le agregamos los nuevos detalles
                            cookies.Path = "/";
                            cookies.Expires = DateTime.Now.AddDays(1);
                            cookies.Value += "?";
                            foreach (var item in detallesDelPedido)
                            {
                                value += item + "||";
                            }
                            cookies.Value += value;
                            ControllerContext.HttpContext.Response.SetCookie(cookies);
                            message = "Nuevo pedido agregado al carrito%1";
                           
                        }
                        else
                        {
                            HttpCookie carrito = new HttpCookie(key);
                           
                            foreach (var item in detallesDelPedido)
                            {
                                value += item + "||";
                            }
                            carrito.Value = value;
                            carrito.Expires = DateTime.Now.AddDays(1);
                            ControllerContext.HttpContext.Response.SetCookie(carrito);
                            message = "Nuevo pedido agregado al carrito%1";
                        }
                    }
                    else
                    {
                        message = "Agregue una cantidad válida%0";
                    }
                }
                else
                {
                    message = "Debe elegir el tiempo de comida%0";
                }
            }
            else
            {
                message = "El día para el pedido no coincide con los días de la reserva%0";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerCesta(string id)
        {
            int idReserva;
            bool res = int.TryParse(id, out idReserva);
            if (res)
            {
                string key = string.Format("UY#9G#HF{0}", idReserva);//Llave que identifica la cookie
                var cookie = ControllerContext.HttpContext.Request.Cookies[key];
                if (cookie != null)
                {
                    ViewBag.InformacionCompra = cookie.Value;
                    return View();
                }
            }
            ViewBag.InformacionCompra = "No hay ningún artículo agregado";
            return View();
        }

        // GET: Alimento/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alimento alimento = await alimentoBL.BuscarAlimentoPorId(Convert.ToInt32(id));
            if (alimento == null)
            {
                return HttpNotFound();
            }
            return View(alimento);
        }

        // GET: Alimento/Create
        public async Task <ActionResult> Create()
        {
            ViewBag.IdCategoriaAlimento = new SelectList(await categoriaBL.ListarCategoriasActivas(), "IdCategoriaAlimento", "NombreCategoria");
            ViewBag.IdProveedor = new SelectList(await proveedorBL.ListarProveedoresActivos(), "IdProveedor", "NombreEmpresa");
            return View();
        }

        // POST: Alimento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AlimentoView alimentoView)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                var picture = string.Empty;
                var folder = "~/content/img/alimentos";

                if (alimentoView.Foto != null)
                {
                    picture = RutaFoto.GuardarRutaFoto(alimentoView.Foto, folder);
                    picture = string.Format("{0}/{1}", folder, picture);
                }
                //ahora debemos convertir la clase AlimentoView a la clase Alimento para que nos permita guardar en la BD
                var alimento = ToAlimento(alimentoView);
                alimento.Imagen = picture;

                var valor = await alimentoBL.AgregarAlimentoUnico(alimento);
                switch (valor)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el nuevo alimento.";
                        break;

                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "El alimento ya existe.";
                        break;
                    case 3:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.IdCategoriaAlimento = new SelectList(await categoriaBL.ListarCategoriasActivas(), "IdCategoriaAlimento", "NombreCategoria", alimentoView.IdCategoriaAlimento);
                ViewBag.IdProveedor = new SelectList(await proveedorBL.ListarProveedoresActivos(), "IdProveedor", "NombreEmpresa", alimentoView.IdCategoriaAlimento);
                return View(alimentoView);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.IdCategoriaAlimento = new SelectList(await categoriaBL.ListarCategoriasActivas(), "IdCategoriaAlimento", "NombreCategoria", alimentoView.IdCategoriaAlimento);
            ViewBag.IdProveedor = new SelectList(await proveedorBL.ListarProveedoresActivos(), "IdProveedor", "NombreEmpresa", alimentoView.IdCategoriaAlimento);
            return View(alimentoView);
        }

        // GET: Alimento/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alimento alimento = await alimentoBL.BuscarAlimentoPorId(Convert.ToInt32(id));
            if (alimento == null)
            {
                return HttpNotFound();
            }
            //Convertimos Alimento a AlimentoView
            var alimentoView = ToAlimentoView(alimento);
            ViewBag.IdCategoriaAlimento = new SelectList(await categoriaBL.ListarCategoriasActivas(), "IdCategoriaAlimento", "NombreCategoria", alimento.IdCategoriaAlimento);
            ViewBag.IdProveedor = new SelectList(await proveedorBL.ListarProveedoresActivos(), "IdProveedor", "NombreEmpresa", alimento.IdProveedor);
            return View(alimentoView);
        }

        // POST: Alimento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AlimentoView alimentoView)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                var picture = string.Empty;
                var folder = "~/content/img/alimentos";

                if (alimentoView.Foto != null)
                {
                    picture = RutaFoto.GuardarRutaFoto(alimentoView.Foto, folder);
                    picture = string.Format("{0}/{1}", folder, picture);
                }
                //ahora debemos convertir la clase AlimentoView a la clase Alimento para que nos permita guardar en la BD
                var alimento = ToAlimento(alimentoView);
                //Se verifica si ha cambiado la imagen
                if (!string.IsNullOrEmpty(picture))
                {
                    alimento.Imagen = picture;
                }
              
                var valor = await alimentoBL.EditarAlimento(alimento);
                switch (valor)
                {
                    case 0:
                        mensaje = "Ocurrió un error, el cambio no fué posible salvarlo.";
                        break;

                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 3:
                        mensaje = "Ya existe un alimento con el mismo nombre.";
                        break;
                    case 4:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.IdCategoriaAlimento = new SelectList(await categoriaBL.ListarCategoriasActivas(), "IdCategoriaAlimento", "NombreCategoria", alimentoView.IdCategoriaAlimento);
                ViewBag.IdProveedor = new SelectList(await proveedorBL.ListarProveedoresActivos(), "IdProveedor", "NombreEmpresa", alimentoView.IdProveedor);
                return View(alimentoView);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.IdCategoriaAlimento = new SelectList(await categoriaBL.ListarCategoriasActivas(), "IdCategoriaAlimento", "NombreCategoria", alimentoView.IdCategoriaAlimento);
            ViewBag.IdProveedor = new SelectList(await proveedorBL.ListarProveedoresActivos(), "IdProveedor", "NombreEmpresa", alimentoView.IdProveedor);
            return View(alimentoView);
        }

        // GET: Alimento/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alimento alimento = await alimentoBL.BuscarAlimentoPorId(Convert.ToInt32(id));
            if (alimento == null)
            {
                return HttpNotFound();
            }
            return View(alimento);
        }

        // POST: Alimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await alimentoBL.EliminarAlimento(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar el alimento.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, el alimento a eliminar no existe.";
                    break;
                case 3:
                    mensaje = "Se recibió un identificador incorrecto.";
                    break;
            }
            ViewBag.Message = mensaje;
            return View();
        }

        //metodo de conversion de HabitacionView a Habitacion
        private Alimento ToAlimento(AlimentoView alimentoView)
        {
            return new Alimento
            {
                IdAlimento = alimentoView.IdAlimento,
                Nombre = alimentoView.Nombre,
                Descripcion = alimentoView.Descripcion,
                Precio = alimentoView.Precio,
                Imagen = alimentoView.Imagen,
                Estado = alimentoView.Estado,
                IdProveedor = alimentoView.IdProveedor,
                IdCategoriaAlimento = alimentoView.IdCategoriaAlimento
            };
        }

        //metodo de conversion de Habitacion a HabitacionView
        private AlimentoView ToAlimentoView(Alimento alimento)
        {
            return new AlimentoView
            {
                IdAlimento = alimento.IdAlimento,
                Nombre = alimento.Nombre,
                Descripcion = alimento.Descripcion,
                Precio = alimento.Precio,
                Imagen = alimento.Imagen,
                Estado = alimento.Estado,
                IdProveedor = alimento.IdProveedor,
                IdCategoriaAlimento = alimento.IdCategoriaAlimento
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
