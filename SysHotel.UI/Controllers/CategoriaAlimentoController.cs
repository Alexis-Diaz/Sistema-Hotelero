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

namespace SysHotel.UI.Controllers
{
    [Autenticado]
    public class CategoriaAlimentoController : Controller
    {
        private CategoriaAlimentoBL categoriaBL = new CategoriaAlimentoBL();

        //Variables para el paginador
        private const int registroPorPagina = 15;
        private List<CategoriaAlimento> categorias;
        private PaginadorGenerico<CategoriaAlimento> paginadorCategoria;


        // GET: CategoriaAlimento
        public async Task<ActionResult> Index(string busqueda, int pagina = 1)
        {
            //Recuperamos la lista completa de categorias
            categorias = await categoriaBL.ListarCategoriasActivas();

            //BUSQUEDA
            //Filtramos una nueva lista segun la busqueda
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToUpper();
                foreach(var item in busqueda.Split(new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    categorias = categorias.Where(x => x.NombreCategoria.ToUpper().Contains(busqueda) ||
                                                       x.Descripcion.ToUpper().Contains(busqueda))
                                                       .ToList();
                }
                
            }

            //PAGINACION
            int totalRegistros = 0;
            int totalPaginas = 0;

            //Se cuenta el total de registros enontrados
            totalRegistros = categorias.Count();

            //Se obtiene la lista de registro por pagina
            List<CategoriaAlimento> listaCategorias = categorias.OrderBy(x => x.NombreCategoria)
                                                                 .Skip((pagina - 1) * registroPorPagina)
                                                                 .Take(registroPorPagina)
                                                                 .ToList();

            //Numero total de paginas
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            //Llenamos la instancia de la clase paginador generico
            paginadorCategoria = new PaginadorGenerico<CategoriaAlimento>()
            {
                RegistroPorPagina = registroPorPagina,
                TotalRegistro = totalRegistros,
                TotalPagina = totalPaginas,
                PaginaActual = pagina,
                Resultado = listaCategorias
            };

            return View(paginadorCategoria);
        }

        // GET: CategoriaAlimento/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaAlimento categoriaAlimento = await categoriaBL.BuscarCategoriaPorId(Convert.ToInt32(id));
            if (categoriaAlimento == null)
            {
                return HttpNotFound();
            }
            return View(categoriaAlimento);
        }

        // GET: CategoriaAlimento/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Message = null;
            List<CategoriaAlimento> listaCategoria = await categoriaBL.ListarCategoriasActivas();
            ViewData["CategoriasExistentes"] = listaCategoria.Take(10).ToList();
            return View();
        }

        // POST: CategoriaAlimento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdCategoriaAlimento,NombreCategoria,Descripcion,Estado")] CategoriaAlimento categoriaAlimento)
        {
            List<CategoriaAlimento> listaCategoria = await categoriaBL.ListarCategoriasActivas();
            ViewData["CategoriasExistentes"] = listaCategoria.Take(10).ToList();
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await categoriaBL.AgregarCategoriaAlimento(categoriaAlimento);

                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, no fué posible guardar la nueva categoría.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "La categoría ya existe.";
                        break;
                    case 3:
                        mensaje = "Datos incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(categoriaAlimento);
            }
            ViewBag.Message = "Información incompleta.";
            return View(categoriaAlimento);
        }

        // GET: CategoriaAlimento/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaAlimento categoria = await categoriaBL.BuscarCategoriaPorId(Convert.ToInt32(id));
            if (categoria == null)
            {
                return HttpNotFound();
            }
            return View(categoria);
        }

        // POST: CategoriaAlimento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdCategoriaAlimento,NombreCategoria,Descripcion,Estado")] CategoriaAlimento categoriaAlimento)
        {
            if (ModelState.IsValid)
            {
                string mensaje = "";
                int res = await categoriaBL.EditarCategoriaAlimentoUnica(categoriaAlimento);
                switch (res)
                {
                    case 0:
                        mensaje = "Ocurrió un error crítico, el cambio no fué posible salvarlo.";
                        break;
                    case 1:
                        return RedirectToAction("Index");

                    case 2:
                        mensaje = "No se han hecho cambios.";
                        break;
                    case 3:
                        mensaje = "Ya existe una categoría con el mismo nombre.";
                        break;
                    case 4:
                        mensaje = "Upss, los datos se fueron incompletos.";
                        break;
                }
                ViewBag.Message = mensaje;
                return View(categoriaAlimento);
            }
            ViewBag.Message = "Información incompleta.";
            return View(categoriaAlimento);
        }

        // GET: CategoriaAlimento/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaAlimento categoria = await categoriaBL.BuscarCategoriaPorId(Convert.ToInt32(id));
            if (categoria == null)
            {
                return HttpNotFound();
            }
            return View(categoria);
        }

        // POST: CategoriaAlimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            int res = await categoriaBL.EliminarCategoriaAlimento(id);
            switch (res)
            {
                case 0:
                    mensaje = "Error crítico, no fué posible eliminar la categoría.";
                    break;
                case 1:
                    return RedirectToAction("Index");
                    
                case 2:
                    mensaje = "Ocurrió un error, la categoría a eliminar no existe.";
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
