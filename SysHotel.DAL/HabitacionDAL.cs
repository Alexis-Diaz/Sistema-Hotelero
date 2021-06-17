using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class HabitacionDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int> AgregarHabitacion(Habitacion habitacion)
        {
            try
            {
                if(habitacion != null)
                {
                    db.Habitacions.Add(habitacion);
                    return await db.SaveChangesAsync();
                }
                return 0; //El eobjeto habitacion viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarHabitacion(int id)
        {
            try
            {
                if (id >0)
                {
                    Habitacion habitacionExistente = await db.Habitacions.FindAsync(id);
                    if(habitacionExistente != null)
                    {
                        db.Habitacions.Remove(habitacionExistente);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //editar
        public async Task<int> EditarHabitacion(Habitacion habitacion)
        {
            try
            {
                if (habitacion != null)
                {
                    Habitacion habitacionExistente = await db.Habitacions.FindAsync(habitacion.IdHabitacion);
                    if(habitacionExistente != null)
                    {
                        habitacionExistente.NumeroHabitacion = habitacion.NumeroHabitacion;
                        habitacionExistente.Descripcion = habitacion.Descripcion;
                        habitacionExistente.NumeroCamas = habitacion.NumeroCamas;
                        habitacionExistente.TVCable = habitacion.TVCable;
                        habitacionExistente.Wifi = habitacion.Wifi;
                        habitacionExistente.AireAcondicionado = habitacion.AireAcondicionado;
                        habitacionExistente.Precio = habitacion.Precio;
                        habitacionExistente.Imagen = habitacion.Imagen;
                        habitacionExistente.Estado = habitacion.Estado;
                        habitacionExistente.IdTipoDeHabitacion = habitacion.IdTipoDeHabitacion;

                        db.Entry(habitacionExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto habitacion viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<Habitacion>> ListarHabitaciones()
        {
            try
            {
                return await db.Habitacions.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por numero
        public async Task<List<Habitacion>> ListarHabitacionesPorNumero(int numero)
        {
            try
            {
                return await db.Habitacions.Where(x => x.NumeroHabitacion == numero && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por id y numero
        public async Task<List<Habitacion>> ListarHabitacionesPorNumero(int id, int numero)
        {
            try
            {
                return await db.Habitacions.Where(x =>x.IdHabitacion != id && x.NumeroHabitacion == numero && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Habitacion>> ListarHabitacionesPorEstado(int estado)
        {
            try
            {
                return await db.Habitacions.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por tipo
        public async Task<List<Habitacion>> ListarHabitacionesPorTipoHabitacion(int idTipo)
        {
            try
            {
                return await db.Habitacions.Where(x => x.IdTipoDeHabitacion == idTipo).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Habitacion>BuscarHabitacionPorId(int id)
        {
            try
            {
                return await db.Habitacions.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
