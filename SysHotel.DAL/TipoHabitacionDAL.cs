using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class TipoHabitacionDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarTipoHabitacion(TipoHabitacion tipoHabitacion)
        {
            try
            {
                if (tipoHabitacion != null)
                {
                    db.TipoHabitacions.Add(tipoHabitacion);
                    return await db.SaveChangesAsync();
                }
                return 0;//El objeto tipoHabitaciion viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarTipoHabitacion(int id)
        {
            try
            {
                if (id > 0)
                {
                    TipoHabitacion tipoHabitacionExistente = await db.TipoHabitacions.FindAsync(id);
                    if(tipoHabitacionExistente != null)
                    {
                        db.TipoHabitacions.Remove(tipoHabitacionExistente);
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
        public async Task<int>EditarTipoHabitacion(TipoHabitacion tipoHabitacion)
        {
            try
            {
                if (tipoHabitacion != null)
                {
                    TipoHabitacion tipoHabitacionExistente = await db.TipoHabitacions.FindAsync(tipoHabitacion.IdTipoDeHabitacion);
                    if (tipoHabitacionExistente != null)
                    {
                        tipoHabitacionExistente.TipoDeHabitacion = tipoHabitacion.TipoDeHabitacion;
                        tipoHabitacionExistente.Descripcion = tipoHabitacion.Descripcion;
                        tipoHabitacionExistente.Estado = tipoHabitacion.Estado;

                        db.Entry(tipoHabitacionExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto tipoHabitacion viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<TipoHabitacion>> ListarTipoHabitacionesActivas()
        {
            try
            {
                return await db.TipoHabitacions.Where(x => x.Estado == 1).ToListAsync(); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<TipoHabitacion>> ListarTipoHabitacionesDeshabilitadas()
        {
            try
            {
                return await db.TipoHabitacions.Where(x => x.Estado == 0).ToListAsync(); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<TipoHabitacion> BuscarHabitacionPorId(int id)
        {
            try
            {
                return await db.TipoHabitacions.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por nombre
        public async Task<List<TipoHabitacion>> BuscarHabitacionPorNombre(string nombre)
        {
            try
            {
                if(nombre != "")
                {
                    return await db.TipoHabitacions.Where(x => x.TipoDeHabitacion == nombre).ToListAsync();
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TipoHabitacion>> BuscarHabitacionPorNombre(int id, string nombre)
        {
            try
            {
                if (nombre != "")
                {
                    return await db.TipoHabitacions.Where(x => x.IdTipoDeHabitacion != id && x.TipoDeHabitacion == nombre && x.Estado == 1).ToListAsync();
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
