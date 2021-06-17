using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class ComentarioBL
    {
        //optimizado
        private ComentarioDAL comentarioDAL = new ComentarioDAL();

        /// <summary>
        /// Agrega un nuevo comentario a la base de datos.
        /// </summary>
        /// <param name="comentario"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: El comentario viene vacío o nulo.</returns>
        public async Task<int> AgregarNuevoComentario(Comentario comentario)
        {
            try
            {
                if (!string.IsNullOrEmpty(comentario.Nota))
                {
                    return await comentarioDAL.AgreagarComentario(comentario);
                }
                return 2;//El objeto comentario no viene completo.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita el comentario al pasar su estado en 0. Este método no elimina el registro en la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        ///0: no guardó, 1: guardó, 2: el cliente no existe en la base de datos, 3: el id recibido no es válido</returns>
        public async Task<int>EliminarComentario(int id)
        {
            try
            {
                if (id > 0)
                {
                    Comentario comentarioExistente = await comentarioDAL.BuscarComentarioPorId(id);
                    if (comentarioExistente != null)
                    {
                        comentarioExistente.Estado = 0;
                        return await comentarioDAL.EditarComentario(comentarioExistente);
                    }
                    return 2;//El comentario no existe.
                }
                return 3;//El id es invalido.
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Solicita una lista de todos los comentarios
        /// </summary>
        /// <returns></returns>
        public async Task<List<Comentario>> ListarComentarios()
        {
            try
            {
                List<Comentario>listaComentarios = await comentarioDAL.ListarComentariosPorEstado(1);
                return listaComentarios;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
