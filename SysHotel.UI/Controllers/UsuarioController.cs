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
using SysHotel.BL;
using SysHotel.UI.Filtros;

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class UsuarioController : Controller
    {
        private UsuarioBL usuarioBL = new UsuarioBL();
        private RolUsuarioBL rolUsuarioBL = new RolUsuarioBL();

        //Variables para el paginador
        private const int registroPorPagina = 15;
        private List<Usuario> usuarios;
        private PaginadorGenerico<Usuario> paginadorUsuario;

        // GET: Usuario
        public async Task<ActionResult> Index(string busqueda, int pagina = 1)
        {
            //Recuperamos la lista completa de usuarios
            usuarios = await usuarioBL.ListarUsuariosActivos();

            //BUSQUEDA
            //Filtramos una nueva lista según la búsqueda
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    usuarios = usuarios.Where(x => x.Nombres.ToUpper().Contains(item) ||
                                                   x.Apellidos.ToUpper().Contains(item) ||
                                                   x.Direccion.ToUpper().Contains(item) ||
                                                   x.Telefono.ToUpper().Contains(item) ||
                                                   x.NombreUsuario.ToUpper().Contains(item) ||
                                                   x.rolUsuario.Rol.ToUpper().Contains(item))
                                                    .ToList();
                }
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el total de registros encontrados
            totalRegistros = usuarios.Count();

            //Se obtiene la lista de registro por pagina
            List<Usuario> listaUsuarios = usuarios.OrderBy(x => x.Nombres)
                                                 .Skip((pagina - 1) * registroPorPagina)
                                                 .Take(registroPorPagina)
                                                 .ToList();

            //Numero total de paginas
            totalPaginas = (int)Math.Ceiling((double)totalPaginas / registroPorPagina);

            //Llenamos la instancia de la clase paginador generico
            paginadorUsuario = new PaginadorGenerico<Usuario>
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaUsuarios
            };

            return View(paginadorUsuario);
        }

        // GET: Usuario/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = await usuarioBL.BuscarUsuarioPorId(Convert.ToInt32(id));
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuario/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.IdRolUsuario = new SelectList(await rolUsuarioBL.ListarRolUsuariosActivos(), "IdRolUsuario", "Rol");
            return View();
        }

        // POST: Usuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdUsuario,Nombres,Apellidos,FechaNacimiento,Direccion,Telefono,DUI,Correo,NombreUsuario,Contraseña,Estado,IdRolUsuario")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await usuarioBL.AgregarUsuarioUnico(usuario);

                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar el nuevo usuario.";
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
                        mensaje = "El usuario ya está registrado.";
                        break;
                    case 6:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.IdRolUsuario = new SelectList(await rolUsuarioBL.ListarRolUsuariosActivos(), "IdRolUsuario", "Rol", usuario.IdRolUsuario);
                return View(usuario);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.IdRolUsuario = new SelectList(await rolUsuarioBL.ListarRolUsuariosActivos(), "IdRolUsuario", "Rol", usuario.IdRolUsuario);
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = await usuarioBL.BuscarUsuarioPorId(Convert.ToInt32(id));
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdRolUsuario = new SelectList(await rolUsuarioBL.ListarRolUsuariosActivos(), "IdRolUsuario", "Rol", usuario.IdRolUsuario);
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdUsuario,Nombres,Apellidos,FechaNacimiento,Direccion,Telefono,DUI,Correo,NombreUsuario,Contraseña,Estado,IdRolUsuario")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await usuarioBL.EditarUsuario(usuario);
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
                        mensaje = "El usuario ya está registrado.";
                        break;
                    case 6:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 7:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                ViewBag.IdRolUsuario = new SelectList(await rolUsuarioBL.ListarRolUsuariosActivos(), "IdRolUsuario", "Rol", usuario.IdRolUsuario);
                return View(usuario);
            }
            ViewBag.Message = "Información incompleta.";
            ViewBag.IdRolUsuario = new SelectList(await rolUsuarioBL.ListarRolUsuariosActivos(), "IdRolUsuario", "Rol", usuario.IdRolUsuario);
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = await usuarioBL.BuscarUsuarioPorId(Convert.ToInt32(id));
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await usuarioBL.EliminarUsuario(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar el usuario.";
                    break;
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    mensaje = "Ocurrió un error, el usuario a eliminar no existe.";
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
