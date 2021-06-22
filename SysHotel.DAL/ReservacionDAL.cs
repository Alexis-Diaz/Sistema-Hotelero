using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class ReservacionDAL
    {  
        private readonly BDComun db = new BDComun();

        //agregar
        public async Task<int> AgregarReservacion(Reservacion reservacion)
        {
            try
            {
                if(reservacion != null)
                {
                    db.Reservacions.Add(reservacion);
                    return await db.SaveChangesAsync();
                }
                return 0; //El objeto reservacion viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarReservacion(int id)
        {
            try
            {
                if (id > 0)
                {
                    Reservacion reservacion = await db.Reservacions.FindAsync(id);
                    if (reservacion != null)
                    {
                        db.Reservacions.Remove(reservacion);
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
        public async Task<int>EditarReservacion(Reservacion reservacion)
        {
            try
            {
                if(reservacion != null)
                {
                    Reservacion reservacionExistente = await db.Reservacions.FindAsync(reservacion.IdReservacion);
                    if(reservacion != null)
                    {
                        reservacionExistente.DiaEntrada = reservacion.DiaEntrada;
                        reservacionExistente.DiaSalida = reservacion.DiaSalida;
                        reservacionExistente.FechaReservacion = reservacion.FechaReservacion;
                        reservacionExistente.NumeroPersonas = reservacion.NumeroPersonas;
                        reservacionExistente.Comentarios = reservacion.Comentarios;
                        reservacionExistente.Estado = reservacion.Estado;
                        reservacionExistente.IdCliente = reservacion.IdCliente;
                        reservacionExistente.IdHabitacion = reservacion.IdHabitacion;
                        reservacionExistente.IdUsuario = reservacion.IdUsuario;

                        db.Entry(reservacionExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto reservacion viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar todas
        public async Task<List<Reservacion>> ListarTodasLasReservaciones()
        {
            try
            {
                return await db.Reservacions.Where(x => x.Estado > 0).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Reservacion>> ListarReservacionesPorEstado(int estado)
        {
            try
            {
                return await db.Reservacions.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por habitacion
        public async Task<List<Reservacion>> ListarReservacionesDeHabitacion(int idHabitacion)
        {
            try
            {
                return await db.Reservacions.Where(x => x.IdHabitacion == idHabitacion && x.Estado ==1 || x.Estado ==2).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por habitacion
        public async Task<List<Reservacion>> ListarReservacionesDeHabitacion(int idReserva, int idHabitacion)
        {
            try
            {
                List<Reservacion>listaReservacio = await db.Reservacions.Where(x => x.IdReservacion != idReserva 
                                                                                 && x.IdHabitacion == idHabitacion
                                                                                 && x.Estado == 1 || x.Estado == 2)
                                                                                     .ToListAsync();
                return listaReservacio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar reservas actuales
        public async Task<List<Reservacion>> ListarReservacionesActuales( )
        {
            try
            {
                return await db.Reservacions.Where(x => x.DiaEntrada == DateTime.Now && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        /// <summary>
        /// Este método devuelve una lista de reserva filtrada por el idHabitacion y una fecha de entrada.
        /// Se debe tener en cuenta que el método incluirá en la respuesta todas las reservas que cumplan
        /// la condición, si no es esto lo que espera, se puede usar en su lugar, la sobrecarga de método para excluir
        /// una reserva especifica por el id de la reserva.
        /// </summary>
        /// <param name="idHabitacion">El id de la habitación</param>
        /// <param name="fechaEntrada">Una fecha de entrada de una reserva</param>
        /// <returns>La lista de reservas</returns>
        public async Task<List<Reservacion>> ListarReservacionesDeHabitacion(int idHabitacion, DateTime fechaEntrada)
        {
            try
            {
                fechaEntrada = fechaEntrada.AddDays(-1);
                return await db.Reservacions.Where(x => x.IdHabitacion == idHabitacion
                                                     && x.DiaSalida >= fechaEntrada
                                                     && x.Estado == 1 
                                                     || x.Estado == 2 
                                                     || x.Estado == 3)
                                                         .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método devuelve una lista de reserva filtrada por el idHabitacion y una fecha de entrada,
        /// pero excluye de los resultados la reserva que tiene el idReservacion pasado por el parametro.
        /// </summary>
        /// <param name="idHabitacion"></param>
        /// <param name="fechaEntrada"></param>
        /// <param name="idReservacion"></param>
        /// <returns>La lista de reservas</returns>
        public async Task<List<Reservacion>> ListarReservacionesDeHabitacion(int idReservacion, int idHabitacion, DateTime fechaEntrada)
        {
            try
            {
                fechaEntrada = fechaEntrada.AddDays(-1);
                return await db.Reservacions.Where(x => x.IdReservacion != idReservacion 
                                                     && x.IdHabitacion == idHabitacion
                                                     && x.Estado == 1 
                                                     || x.Estado == 2 
                                                     || x.Estado == 3 
                                                     && x.DiaSalida >= fechaEntrada).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Reservacion> BuscarReservacionPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    Reservacion reser = await db.Reservacions.FindAsync(id);
                    return reser;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //buscar habitacion disponible por fecha entrada y salida
        public async Task<List<Reservacion>> BuscarReservacionesPorDiaEntradaYSalida (DateTime checkIn, DateTime checkOut)
        {
            try
            {
                List<Reservacion> ListadeReservaciones = await db.Reservacions.Where(x => x.DiaEntrada >= checkIn && x.DiaEntrada <= checkOut ||
                                                    x.DiaSalida >= checkIn && x.DiaSalida <= checkOut ||
                                                    x.DiaEntrada < checkIn && x.DiaSalida > checkOut).ToListAsync();
                return ListadeReservaciones;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}