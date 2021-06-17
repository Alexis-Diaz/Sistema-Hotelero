using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class ComentarioDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgreagarComentario(Comentario comentario)
        {
            try
            {
                if(comentario != null)
                {
                    db.Comentarios.Add(comentario);
                    return await db.SaveChangesAsync();
                }
                return 0;//El objeto comentario viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int> EliminarComentario(int id)
        {
            try
            {
                if(id > 0)
                {
                    Comentario comentarioExistente = await db.Comentarios.FindAsync(id);
                    if(comentarioExistente != null)
                    {
                        db.Comentarios.Remove(comentarioExistente);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El id es invalido
            }
            catch (Exception)
            {
                throw;
            }
        }

        //editar
        public async Task<int> EditarComentario(Comentario comentario)
        {
            try
            {
                if(comentario != null)
                {
                    Comentario comentarioExistente = await db.Comentarios.FindAsync(comentario.IdComentario);
                    if(comentarioExistente != null)
                    {
                        comentarioExistente.Nota = comentario.Nota;
                        comentarioExistente.Fecha = comentario.Fecha;
                        comentarioExistente.Estado = comentario.Estado;
                        comentarioExistente.IdComentario = comentario.IdComentario;

                        db.Entry(comentarioExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto comentario viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar todo
        public async Task<List<Comentario>> ListarComentarios()
        {
            try
            {
                return await db.Comentarios.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Comentario>> ListarComentariosPorEstado(int estado)
        {
            try
            {
                return await db.Comentarios.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Comentario> BuscarComentarioPorId(int id)
        {
            try
            {
                return await db.Comentarios.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
