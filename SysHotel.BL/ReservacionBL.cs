using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class ReservacionBL
    {
        //optimizado
        private ReservacionDAL reservacionDAL = new ReservacionDAL();
        private HabitacionDAL habitacionDAL = new HabitacionDAL();

        /// <summary>
        /// Agrega una nueva reservación. El metodo verifica que la reserva de la habitación no
        /// coincida con otra, que se deje un día entre reservaciones y que se haga con 7 días
        /// de anticipación.
        /// </summary>
        /// <param name="reservacion"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: se debe hacer con 7 días de anticipación, 3: la fecha salida es anterior a la fecha entrada,
        /// 4: debe dejar un dia entre reservación, 5: la reserva coincide con otra reserva, 6: datos incompletos.</returns>
        public async Task<int> AgregarReservaNueva(Reservacion reservacion)
        {
            try
            {
                if (reservacion.DiaEntrada > DateTime.Now && reservacion.DiaSalida > DateTime.Now
                && reservacion.NumeroPersonas > 0 && reservacion.IdCliente > 0 
                && reservacion.IdHabitacion > 0 && reservacion.IdUsuario > 0 )
                {
                    //Buscamos las reservaciones para la habitación solicitada y verificamos que la nueva reserva no choque con las que estan hechas.
                    List<Reservacion> ReservacionDeHabitacion = await reservacionDAL.ListarReservacionesPorHabitacion(reservacion.IdHabitacion);
                    int coincidencia = ReservacionDeHabitacion.Where(r => (reservacion.DiaEntrada >= r.DiaEntrada && reservacion.DiaEntrada <= r.DiaSalida)
                                                                       || (reservacion.DiaSalida  >= r.DiaEntrada && reservacion.DiaSalida  <= r.DiaSalida)
                                                                       || (reservacion.DiaEntrada <  r.DiaEntrada && reservacion.DiaSalida  >  r.DiaSalida))
                                                                                      .Count();
                    if (coincidencia == 0)
                    {
                        //Aqui se indica que se debe dejar un dia entre reservación y reservación del la misma habitación.
                        List<Reservacion> re = await reservacionDAL.ListarReservacionesPorHabitacion(reservacion.IdHabitacion, reservacion.DiaEntrada, reservacion.DiaSalida);
                        int resul = re.Count();
                        if (resul == 0)
                        {
                            //Aqui se establece los dias de anticipacion de una reserva.
                            DateTime FechaActual = DateTime.Now.Date.AddDays(7);

                            if (reservacion.DiaEntrada <= FechaActual)
                            {
                                return 2;//la debe hacerse con 7 dias de anticipacion
                            }
                            else
                            {
                                if (reservacion.DiaEntrada < reservacion.DiaSalida)
                                {
                                    reservacion.FechaReservacion = DateTime.Now;
                                    reservacion.Estado = 1;// 0 eliminadas; 1 reservas futuras; 2 en curso; 3 finalizadas; 4 canceladas; 5 vencidas
                                    return await reservacionDAL.AgregarReservacion(reservacion);//retorna 1 El registro se guardó
                                }
                                return 3; //La fecha de salida es menor o igual que la fecha de entrada.
                            }
                        }
                        return 4; //La reserva no se puede realizar porque la habitación esta sucia.
                    }
                    return 5; //La reserva no se puede realizar porque coincide con los dias de otra reserva.
                }
                return 6; //los datos no estan completos.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita una reserva al pasar su estado a 0. Este método no elimina el registro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: la reserva no existe.</returns>
        public async Task<int> EliminarReserva(int id)
        {
            try
            {
                Reservacion reservacion = await reservacionDAL.BuscarReservacionPorId(id);
                if (reservacion != null)
                {
                    reservacion.Estado = 0;
                    return await reservacionDAL.EditarReservacion(reservacion); 
                }
                return 2;//La reservación no existe.
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Edita una reservación. El metodo verifica que la reserva de la habitación no
        /// coincida con otra, que se deje un día entre reservaciones y que se haga con 7 días
        /// de anticipación.
        /// </summary>
        /// <param name="reservacion"></param>
        /// <returns>0: no guardó, 1: guardó, 2: se debe hacer con 7 días de anticipación, 3: la fecha salida es anterior a la fecha entrada,
        /// 4: debe dejar un dia entre reservación, 5: la reserva coincide con otra reserva, 6: no se han hecho cambios,
        /// 7: datos incompletos.</returns>
        public async Task<int> EditarReserva(Reservacion reservacion)
        {
            try
            {
                //Se comprueba que la información este completa
                if (reservacion.DiaEntrada != null && reservacion.DiaSalida != null
                && reservacion.NumeroPersonas > 0 && reservacion.IdCliente > 0
                && reservacion.IdHabitacion > 0 && reservacion.IdUsuario > 0)
                {
                    //Control de cambios
                    var reservacionExistente = new Reservacion();
                    reservacionExistente = await reservacionDAL.BuscarReservacionPorId(reservacion.IdReservacion);
                    if (reservacion.DiaEntrada != reservacionExistente.DiaEntrada
                    || reservacion.DiaSalida != reservacionExistente.DiaSalida
                    || reservacion.NumeroPersonas != reservacionExistente.NumeroPersonas
                    || reservacion.Comentarios != reservacionExistente.Comentarios
                    || reservacion.IdCliente != reservacionExistente.IdCliente
                    || reservacion.IdHabitacion != reservacionExistente.IdHabitacion
                    || reservacion.Estado != reservacionExistente.Estado)
                    {
                        //Buscamos las reservaciones para la habitación solicitada y verificamos que la nueva reserva no choque con las que estan hechas.
                        List<Reservacion> ReservacionDeHabitacion = await reservacionDAL.ListarReservacionesPorHabitacion(reservacion.IdReservacion, reservacion.IdHabitacion);
                        int coincidencia = ReservacionDeHabitacion.Where(r => (reservacion.DiaEntrada >= r.DiaEntrada && reservacion.DiaEntrada <= r.DiaSalida)
                                                                           || (reservacion.DiaSalida  >= r.DiaEntrada && reservacion.DiaSalida  <= r.DiaSalida)
                                                                           || (reservacion.DiaEntrada <  r.DiaEntrada && reservacion.DiaSalida  >  r.DiaSalida))
                                                                                          .Count();
                        if (coincidencia == 0)
                        {
                            //Aqui se indica que se debe dejar un dia entre reservación y reservación del la misma habitación.
                            List<Reservacion> re = await reservacionDAL.ListarReservacionesPorHabitacion(reservacion.IdReservacion, reservacion.IdHabitacion, reservacion.DiaEntrada, reservacion.DiaSalida);
                            int resul = re.Count();
                            if (resul == 0)
                            {
                                //Aqui se establece los dias de anticipacion de una reserva.
                                DateTime FechaActual = DateTime.Now.Date.AddDays(7);

                                if (reservacion.DiaEntrada <= FechaActual)
                                {
                                    return 2;//la debe hacerse con 7 dias de anticipacion
                                }
                                else
                                {
                                    if (reservacion.DiaEntrada < reservacion.DiaSalida)
                                    {
                                        return await reservacionDAL.EditarReservacion(reservacion);//retorna 1 El registro se guardó
                                    }
                                    return 3; //La fecha de salida es menor o igual que la fecha de entrada.
                                }
                            }
                            return 4; //La reserva no se puede realizar porque la habitación esta sucia.
                        }
                        return 5; //La reserva no se puede realizar porque coincide con los dias de otra reserva.
                    }
                    return 6;//no se han hecho cambios.
                }
                return 7; //los datos no estan completos.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita el estado de una reservación. Los estado validos son de 0 a 5. Donde:
        /// 0 eliminadas; 1 reservas futuras; 2 en curso; 3 finalizadas; 4 canceladas; 5 vencidas
        /// </summary>
        /// <param name="estado"></param>
        /// <returns>0: no guardó, 1: guardó.</returns>
        public async Task<int> EditarEstadoDeLaReserva(int idReservacion, int estado)
        {
            try
            {
                //Se comprueba que la información este completa
                if (estado >= 0 && estado <= 5 && idReservacion > 0)
                {
                    Reservacion reser = await reservacionDAL.BuscarReservacionPorId(idReservacion);
                    reser.Estado = estado;
                    return await reservacionDAL.EditarReservacion(reser);//retorna 1 El registro se guardó
                }
                return 0;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista las reservaciones segun su estado. Donde, el estado es:
        /// 0: eliminadas, 1: reservas futuras, 2: en curso, 3: finalizadas, 4: canceladas, 5: vencidas
        /// </summary>
        /// <param name="estado"></param>
        /// <returns>Lista de reserva ordenada segun la fecha de entrada.</returns>
        public async Task<List<Reservacion>> ListarReservacionesPorEstado(int estado)
        {
            try
            {
                List<Reservacion> ListaReservaciones = await reservacionDAL.ListarReservacionesPorEstado(estado);
                ListaReservaciones.Sort((x, y) => DateTime.Compare(x.DiaEntrada, y.DiaEntrada));//para ordenar por fecha de entrada.
                return ListaReservaciones;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Lista todas las reservaciones en curso, futuras, canceladas, finalizadass y vencidas.
        /// </summary>
        /// <returns>Lista de todas las reservaciones</returns>
        public async Task<List<Reservacion>> ListarTodasLasReservaciones()
        {
            try
            {
                return await reservacionDAL.ListarTodasLasReservaciones();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca las reservacion actuales.
        /// </summary>
        /// <returns>Una lista con las reservaciones actuales, de lo contrario null.</returns>
        public async Task<List<Reservacion>> ListarReservacionesActuales()
        {
            try
            {
                List<Reservacion> reservacionesActuales = await reservacionDAL.ListarReservacionesActuales();
                reservacionesActuales.Sort((x, y) => DateTime.Compare(x.DiaEntrada, y.DiaEntrada));//ordenamos segun fecha.
                return reservacionesActuales;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cambia el estado de la reservación.Donde, el estado es:
        /// 0: eliminadas, 1: reservas futuras, 2: en curso, 3: finalizadas, 4: canceladas, 5: vencidas
        /// </summary>
        /// <param name="idReservacion"></param>
        /// <param name="estado"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: no existe la reserva.</returns>
        public async Task<int> CambiarEstadoDeReservacion(int idReservacion, int estado)
        {
            try
            {
                Reservacion reservacionExistente = await reservacionDAL.BuscarReservacionPorId(idReservacion);
                if(reservacionExistente != null)
                {
                    reservacionExistente.Estado = estado;// 0 eliminadas; 1 reservas futuras; 2 en curso; 3 finalizadas; 4 canceladas; 5 vencidas
                    return await reservacionDAL.EditarReservacion(reservacionExistente);
                }
                return 2;//La reservacion no existe.
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Busca la reservación por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>La reservación encontrada.</returns>
        public async Task<Reservacion> ObtenerReservaPorId(int id)
        {
            try
            {
                if (id >0)
                {
                    return await reservacionDAL.BuscarReservacionPorId(id);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Hace una consulta para saber que habitaciones estan disponibles entre las fechas 
        /// de entrada y salida.
        /// </summary>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns>Una lista de las habitaciones disponibles.
        /// En caso de que las entradas de fecha esten incorrectas o no se encuentre ninguna habitación
        /// disponible, devuelve null.</returns>
        public async Task<List<Habitacion>>ConsultarHabitacionesDisponiblesPorFechaEntradaYSalida(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn != null && checkOut != null)
            {
                if (checkIn <= DateTime.Now.AddDays(7))
                {
                    return null;//la reservación debe hacerse con 7 días de anticipación.
                }
                if (checkOut <= checkIn)
                {
                    return null;//La fecha de salida es menor o igual a fecha de entrada.
                }
                //Creamos una lista de reservaciones que coincidan con los dias en que se pretende realizar la 
                //nueva reserva. Luego se crea otra lista de habitaciones. A esta última lista se le restan
                //las habitaciones que estan reservadas en las fechas indicadas para dejar las habitaciones 
                //disponibles.
                try
                {
                    List<Reservacion> ListaRerservaciones = await reservacionDAL.BuscarReservacionesPorDiaEntradaYSalida(checkIn, checkOut);
                    List<Habitacion> HabitacionesDisponibles = await habitacionDAL.ListarHabitaciones();
                    foreach (var item in ListaRerservaciones)
                    {
                        Habitacion habitacion = await habitacionDAL.BuscarHabitacionPorId(item.IdHabitacion);
                        HabitacionesDisponibles.Remove(habitacion);
                    }
                    //Ahora restamos las habitaciones que estaran en limpieza cualquiera de las dos fechas.
                    ListaRerservaciones = await reservacionDAL.BuscarReservasPorDiaDeLimpieza(checkIn, checkOut);
                    foreach (var item in ListaRerservaciones)
                    {
                        Habitacion habitacion = await habitacionDAL.BuscarHabitacionPorId(item.IdHabitacion);
                        HabitacionesDisponibles.Remove(habitacion);
                    }
                    return HabitacionesDisponibles;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return null;
        }
    }
}
