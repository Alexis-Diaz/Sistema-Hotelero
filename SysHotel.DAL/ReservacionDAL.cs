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

        //listar reservas actuales
        public async Task<List<Reservacion>> ListarReservacionesActuales()
        {
            try
            {
                DateTime fechaActual = DateTime.Now.Date;
                return await db.Reservacions.Where(x => x.DiaEntrada == fechaActual && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método devuelve una lista de reserva filtrada por el idHabitacion y que su estado es 1 0 2.
        /// Se debe tener en cuenta que el método incluirá en la respuesta todas las reservas que cumplan
        /// la condición, si no es esto lo que espera, se puede usar en su lugar, la sobrecarga de método para excluir
        /// una reserva especifica por el id de la reserva.
        /// </summary>
        /// <param name="idHabitacion"></param>
        /// <returns>Las reservas filtradas por la habitación</returns>
        public async Task<List<Reservacion>> ListarReservacionesPorHabitacion(int idHabitacion)
        {
            try
            {
                return await db.Reservacions.Where(x => x.IdHabitacion == idHabitacion && (x.Estado ==1 || x.Estado ==2)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método devuelve una lista de reserva filtrada por el idHabitacion y que su estado es 1 0 2..
        /// pero excluye de los resultados la reserva que tiene el idReservacion pasado por el parametro.
        /// </summary>
        /// <param name="idReserva"></param>
        /// <param name="idHabitacion"></param>
        /// <returns></returns>
        public async Task<List<Reservacion>> ListarReservacionesPorHabitacion(int idReserva, int idHabitacion)
        {
            try
            {
                List<Reservacion>listaReservacio = await db.Reservacions.Where(x => x.IdReservacion != idReserva 
                                                                                &&  x.IdHabitacion == idHabitacion
                                                                                && (x.Estado == 1 || x.Estado == 2))
                                                                                     .ToListAsync();
                return listaReservacio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método devuelve una lista de reserva filtrada por el idHabitacion, que a su vez estrán en limpieza las fechas indicadas y que su estado es 1 0 2.
        /// Se debe tener en cuenta que el método incluirá en la respuesta todas las reservas que cumplan
        /// la condición, si no es esto lo que espera, se puede usar en su lugar, la sobrecarga de método para excluir
        /// una reserva especifica por el id de la reserva.
        /// </summary>
        /// <param name="idHabitacion">El id de la habitación</param>
        /// <param name="fechaEntrada">Una fecha de entrada de una reserva</param>
        /// <returns>La lista de reservas que su idHabitacion y fecha de entrada sean iguales a los parámetros.</returns>
        public async Task<List<Reservacion>> ListarReservacionesPorHabitacion(int idHabitacion, DateTime LimpiezaAnterior, DateTime LimpiezaPosterior)
        {
            try
            {
                LimpiezaAnterior = LimpiezaAnterior.AddDays(-1);
                LimpiezaPosterior = LimpiezaPosterior.AddDays(1);
                return await db.Reservacions.Where(x => x.IdHabitacion == idHabitacion
                                                    && (x.DiaSalida == LimpiezaAnterior || x.DiaEntrada == LimpiezaPosterior)
                                                    && (x.Estado == 1 || x.Estado == 2)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método devuelve una lista de reserva filtrada por el idHabitacion y que se encuentre en limpieza los dias indicados,
        /// pero excluye de los resultados la reserva que tiene el idReservacion pasado por el parametro.
        /// </summary>
        /// <param name="idHabitacion"></param>
        /// <param name="fechaEntrada"></param>
        /// <param name="idReservacion"></param>
        /// <returns>La lista de reservas</returns>
        public async Task<List<Reservacion>> ListarReservacionesPorHabitacion(int idReservacion, int idHabitacion, DateTime LimpiezaAnterior, DateTime LimpiezaPosterior)
        {
            try
            {
                LimpiezaAnterior = LimpiezaAnterior.AddDays(-1);
                LimpiezaPosterior = LimpiezaPosterior.AddDays(1);
                return await db.Reservacions.Where(x => x.IdReservacion != idReservacion
                                                    &&  x.IdHabitacion  == idHabitacion
                                                    && (x.DiaSalida     == LimpiezaAnterior || x.DiaEntrada == LimpiezaPosterior)
                                                    && (x.Estado        == 1                || x.Estado     == 2)).ToListAsync();
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

        //buscar habitacion disponible por fecha entrada y salida y que su estado sea 1 0 2
        public async Task<List<Reservacion>> BuscarReservacionesPorDiaEntradaYSalida (DateTime checkIn, DateTime checkOut)
        {
            try
            {
                List<Reservacion> ListadeReservaciones = await db.Reservacions.Where(x => (x.DiaEntrada >= checkIn && x.DiaEntrada <= checkOut)
                                                                                       || (x.DiaSalida  >= checkIn && x.DiaSalida  <= checkOut)
                                                                                       || (x.DiaEntrada <  checkIn && x.DiaSalida  >  checkOut)
                                                                                       && (x.Estado == 1 || x.Estado == 2)).ToListAsync();
                return ListadeReservaciones;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método busca todas las reservas donde su habitación recibirá limpieza antes o después de la reservación según
        /// las fechas enviadas por parametros. La lista retornada tiene estado 1 0 2.
        /// Parámetro 1: corresponde a la fecha de limpieza antes de iniciar la reservación.
        /// Parámetro 2: corresponde a la fecha de limpieza después de finalizar la reservación.
        /// </summary>
        /// <param name="LimpiezaAnterior"></param>
        /// <param name="LimpiezaPosterior"></param>
        /// <returns>Las reservas cuyas habitaciones estarán en limpieza en culquiera de las fechas indicadas.</returns>
        public async Task<List<Reservacion>> BuscarReservasPorDiaDeLimpieza (DateTime LimpiezaAnterior, DateTime LimpiezaPosterior)
        {
            try
            {
                LimpiezaAnterior = LimpiezaAnterior.AddDays(-1);
                LimpiezaPosterior = LimpiezaPosterior.AddDays(1);
                List<Reservacion> ListaReservaciones = await db.Reservacions.Where(x => x.DiaSalida == LimpiezaAnterior 
                                                                                     || x.DiaEntrada == LimpiezaPosterior
                                                                                     && x.Estado == 1 
                                                                                     || x.Estado == 2)
                                                                                         .ToListAsync();
                return ListaReservaciones;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}