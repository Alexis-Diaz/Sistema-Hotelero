using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class AlimentoBL
    {
        //optimizado
        private AlimentoDAL alimentoDAL = new AlimentoDAL();

        /// <summary>
        /// Agrega un nuevo alimento donde su nombre no existe en la base de datos
        /// </summary>
        /// <param name="alimento"></param>
        /// <returns>Un entero:
        /// 0: no guardó, 1: guardó, 2: ya existe, 3: El argumento recibido no esta completo.</returns>
        public async Task<int> AgregarAlimentoUnico(Alimento alimento)
        {
            try
            {
                //Comprobar que la información del alimento venga completa
                if (!string.IsNullOrEmpty(alimento.Nombre) && !string.IsNullOrEmpty(alimento.Descripcion) && alimento.Precio > 0 && alimento.Estado >= 0 && alimento.IdProveedor > 0 && alimento.IdCategoriaAlimento > 0)
                {
                    //Comprobamos que el alimento sea único
                    List<Alimento> ListaAlimento = await alimentoDAL.ListarAlimentosPorNombre(alimento.Nombre);
                    int coincidencia = ListaAlimento.Count();
                    if (coincidencia == 0)
                    {
                        alimento.Estado = 1;
                        return await alimentoDAL.AgregarAlimento(alimento);
                    }
                    return 2;//Ya existe
                }
                return 3;//El objeto alimento no está completo
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita un alimento al pasar su estado en 0. Este metodo no elimina el registro en la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: al alimento no existe en la base de datos, 3: el id no es válido.</returns>
        public async Task<int>EliminarAlimento(int id)
        {
            try
            {
                if(id > 0)
                {
                    Alimento alimentoExistente = await alimentoDAL.BuscarAlimentoPorId(id);
                    if(alimentoExistente != null)
                    {
                        alimentoExistente.Estado = 0;//El cero indica que el alimento está deshabilitado.
                        return await alimentoDAL.EditarAlimento(alimentoExistente);
                    }
                    return 2; //El alimento a borrar no se encontró en la base de datos.
                }
                return 3; //El id no es valido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita un alimento asegurando que no se repita en la base de datos.
        /// </summary>
        /// <param name="alimento"></param>
        /// <returns>Un entero, donde:
        /// o: no guardó, 1: guardó, 2: no se han hecho cambios, 3: ya existe, 4: el objeto se recibe incompleto.</returns>
        public async Task<int> EditarAlimento(Alimento alimento)
        {
            try
            {
                //Comprobar que la información del alimento venga completa
                if (!string.IsNullOrEmpty(alimento.Nombre) && !string.IsNullOrEmpty(alimento.Descripcion)
                    && alimento.Precio > 0 && alimento.Estado >= 0 && alimento.IdProveedor > 0
                    && alimento.IdCategoriaAlimento > 0)
                {
                    //Comprobar que se han hecho cambios.
                    Alimento alimentoExistente = await alimentoDAL.BuscarAlimentoPorId(alimento.IdAlimento);
                    if (alimentoExistente.Nombre == alimento.Nombre && alimentoExistente.Descripcion == alimento.Descripcion
                       && alimentoExistente.Precio == alimento.Precio && alimentoExistente.Imagen == alimento.Imagen
                       && alimentoExistente.Estado == alimento.Estado && alimentoExistente.IdProveedor == alimento.IdProveedor
                       && alimentoExistente.IdCategoriaAlimento == alimento.IdCategoriaAlimento)
                    {
                        return 2;//No se ha hecho cambios
                    }
                    else
                    {
                        //Comprobar que el nombre del alimento editado no exista.
                        List<Alimento> ListaAlimento = await alimentoDAL.ListarAlimentosPorNombre(alimento.IdAlimento, alimento.Nombre);
                        int coincidencia = ListaAlimento.Count();
                        if (coincidencia == 0)
                        {
                            return await alimentoDAL.EditarAlimento(alimento);
                        }
                        return 3; //ya existe
                    }
                }
                return 4;//El objeto alimento no viene completo.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista todos los alimentos que estan disponibles y cuyo estado es 1.
        /// </summary>
        /// <returns>Una lista de los alimentos disponibles</returns>
        public async Task<List<Alimento>> ListarAlimentosDisponibles()
        {
            try
            {
                List<Alimento>ListaAlimentos = await alimentoDAL.ListarAlimentosPorEstado(1);
                return ListaAlimentos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista todos los alimentos que estan no disponibles y cuyo estado es 2.
        /// </summary>
        /// <returns>Una lista de los alimentos disponibles</returns>
        public async Task<List<Alimento>> ListarAlimentosNoDisponibles()
        {
            try
            {
                List<Alimento> ListaAlimentos = await alimentoDAL.ListarAlimentosPorEstado(2);
                return ListaAlimentos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los alimentos por la categoria indicada.
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <returns>Una lista de alimentos por categoria.</returns>
        public async Task<List<Alimento>> ListarAlimentosPorCategoria(int idCategoria)
        {
            try
            {
                if(idCategoria > 0)
                {
                    List<Alimento> ListaAlimentos = await alimentoDAL.ListarAlimentosPorCategoria(idCategoria);
                    return ListaAlimentos;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca un alimento por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Alimento encontrado.</returns>
        public async Task<Alimento> BuscarAlimentoPorId(int id)
        {
            try
            {
                if(id > 0)
                {
                    return await alimentoDAL.BuscarAlimentoPorId(id);
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
