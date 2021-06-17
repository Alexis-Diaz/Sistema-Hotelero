using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class CategoriaAlimentoBL
    {
        //optimizado
        private CategoriaAlimentoDAL categoriaDAL = new CategoriaAlimentoDAL();

        /// <summary>
        /// Agregar una categoría única.
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: categoría ya existe, 3: categoría recibida incompleta.</returns>
        public async Task<int> AgregarCategoriaAlimento(CategoriaAlimento categoria)
        {
            try
            {
                if (!string.IsNullOrEmpty(categoria.NombreCategoria) && !string.IsNullOrEmpty(categoria.Descripcion))
                {
                    List<CategoriaAlimento> listaCategoria = await categoriaDAL.ListarCategoriaAlimentoPorNombre(categoria.NombreCategoria);
                    int coincidencia = listaCategoria.Count();
                    if (coincidencia == 0)
                    {
                        categoria.Estado = 1;
                        return await categoriaDAL.AgregarCategoriaDeAlimento(categoria);
                    }
                    return 2; //Categoria ya existe.
                }
                return 3;//El objeto categoria no viene completo.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita la categoría al pasar su estado en 0. Este metodo no elimina el registro en la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: la categoría no existe en la base de datos, 3: el id recibido no es válido.</returns>
        public async Task<int>EliminarCategoriaAlimento(int id)
        {
            try
            {
                if(id > 0)
                {
                    CategoriaAlimento categoriaExistente = await categoriaDAL.BuscarCategoriaAlimentoPorId(id);
                    if(categoriaExistente != null)
                    {
                        categoriaExistente.Estado = 0;
                        return await categoriaDAL.EditarCategoriaAlimento(categoriaExistente);
                    }
                    return 2;//No se encontró la categoria.
                }
                return 3; //El id es invalido
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Edita una categoría de alimento asegurando que no se repita en la base de datos.
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns>Un entero, donde:
        /// o: no guardó, 1: guardó, 2: no se han hecho cambios, 3: ya existe, 4: el objeto se recibe incompleto.</returns>
        public async Task<int> EditarCategoriaAlimentoUnica(CategoriaAlimento categoria)
        {
            try
            {
                //Verificamos que no venga vacia
                if (!string.IsNullOrEmpty(categoria.NombreCategoria) || !string.IsNullOrEmpty(categoria.Descripcion))
                {
                    CategoriaAlimento categoriaExistente = await categoriaDAL.BuscarCategoriaAlimentoPorId(categoria.IdCategoriaAlimento);

                    //Verifiamos cambios
                    if (categoriaExistente.NombreCategoria == categoria.NombreCategoria && categoriaExistente.Descripcion == categoria.Descripcion)
                    {
                        return 2;//no se ha hecho cambios.
                    }
                    else
                    {
                        //Verificamos que sea única
                        List<CategoriaAlimento> listaCategoria = await categoriaDAL.ListarCategoriaAlimentoPorNombre(categoria.IdCategoriaAlimento, categoria.NombreCategoria);
                        int coincidencia = listaCategoria.Count();
                        if (coincidencia == 0)
                        {

                            return await categoriaDAL.EditarCategoriaAlimento(categoria);
                        }
                        return 3;//La categoría ya existe
                    }
                }
                return 4;//El objeto categoria no viene completo.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista las categorías activas
        /// </summary>
        /// <returns>La lista de categorías</returns>
        public async Task<List<CategoriaAlimento>> ListarCategoriasActivas()
        {
            try
            {
                List<CategoriaAlimento> ListaCategoria = await categoriaDAL.ListarCategoriaAlimentoPorEstado(1);
                return ListaCategoria;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca una categoría de alimento por id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>La categoría encontrada.</returns>
        public async Task<CategoriaAlimento> BuscarCategoriaPorId(int id)
        {
            try
            {
                if(id > 0)
                {
                    return await categoriaDAL.BuscarCategoriaAlimentoPorId(id);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
