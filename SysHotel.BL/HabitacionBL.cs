using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class HabitacionBL
    {
        //optimizado
        private HabitacionDAL habitacionDAL = new HabitacionDAL();

        /// <summary>
        /// Agregar una nueva habitación.
        /// </summary>
        /// <param name="habitacion"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el número de la habitaciíon ya existe, 3: la información se recibe incompleta.</returns>
        public async Task<int>AgregarHabitacionUnica(Habitacion habitacion)
        {
            try
            {
                if (habitacion.NumeroHabitacion > 0 && !string.IsNullOrEmpty(habitacion.Descripcion) && habitacion.NumeroCamas > 0
                && habitacion.Precio > 0 && habitacion.IdTipoDeHabitacion > 0)
                {
                    List<Habitacion> ListaHabitacion = await habitacionDAL.ListarHabitacionesPorNumero(habitacion.NumeroHabitacion);
                    int coincidencia = ListaHabitacion.Count();

                    if(coincidencia == 0)
                    {
                        habitacion.Estado = 1;
                        return await habitacionDAL.AgregarHabitacion(habitacion);
                    }
                    return 2;// El número de la habitación ya existe.
                }
                return 3; //La información de la habitación viene incompleta.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita una habitación al pasar su estado a 0. Este método no elimina la habitación.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: la habitación no existe, 3: el id es inválido.</returns>
        public async Task<int>EliminarHabitación(int id)
        {
            try
            {
                if (id > 0)
                {
                    Habitacion habitacionExistente = await habitacionDAL.BuscarHabitacionPorId(id);
                    if (habitacionExistente != null)
                    {
                        habitacionExistente.Estado = 0;
                        return await habitacionDAL.EditarHabitacion(habitacionExistente);
                    }
                    return 2; //La habitacion no existe.
                }
                return 3;//El id no es valido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita una habitación tomando en cuenta que sea única.
        /// </summary>
        /// <param name="habitacion"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el número de la habitación ya existe,
        /// 3: no se han hecho cambios, 4: se recibe información incompleta.</returns>
        public async Task<int>EditarHabitacion(Habitacion habitacion)
        {
            try
            {
                //Se comprueba que la información se recibe correcta
                if (habitacion.NumeroHabitacion > 0 && !string.IsNullOrEmpty(habitacion.Descripcion) && habitacion.NumeroCamas > 0
                && habitacion.Precio > 0 && habitacion.IdTipoDeHabitacion > 0)
                {
                    //Control de cambios
                    Habitacion habitacionExistente = await habitacionDAL.BuscarHabitacionPorId(habitacion.IdHabitacion);
                    if (habitacion.NumeroHabitacion != habitacionExistente.NumeroHabitacion
                       || habitacion.Descripcion != habitacionExistente.Descripcion
                       || habitacion.NumeroCamas != habitacionExistente.NumeroCamas
                       || habitacion.TVCable != habitacionExistente.TVCable
                       || habitacion.Wifi != habitacionExistente.Wifi
                       || habitacion.AireAcondicionado != habitacionExistente.AireAcondicionado
                       || habitacion.Precio != habitacionExistente.Precio
                       || habitacion.Imagen != habitacionExistente.Imagen
                       || habitacion.Estado != habitacionExistente.Estado
                       || habitacion.IdHabitacion != habitacionExistente.IdHabitacion
                       || habitacion.IdTipoDeHabitacion != habitacionExistente.IdTipoDeHabitacion)
                    {
                        List<Habitacion> ListaHabitacion = await habitacionDAL.ListarHabitacionesPorNumero(habitacion.IdHabitacion, habitacion.NumeroHabitacion);
                        int coincidencia = ListaHabitacion.Count();

                        if (coincidencia == 0)
                        {
                            return await habitacionDAL.EditarHabitacion(habitacion);
                        }
                        return 2;// El número de la habitación ya existe.
                    }
                    return 3;//No se han hecho cambios.
                }
                return 4; //La información de la habitación viene incompleta.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista las habitaciones cuyo estado es 1, es decir activas.
        /// </summary>
        /// <returns>Lista de habitaciones.</returns>
        public async Task<List<Habitacion>> ListarHabitacionesActivas()
        {
            try
            {
                List<Habitacion> ListaHabitaciones = await habitacionDAL.ListarHabitacionesPorEstado(1);
                return ListaHabitaciones;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista las habitaciones por tipo de habitación.
        /// </summary>
        /// <returns>Lista de habitaciones.</returns>
        public async Task<List<Habitacion>> ListarHabitacionesPorTipo(int id)
        {
            try
            {
               if(id > 0)
                {
                    List<Habitacion> ListaHabitaciones = await habitacionDAL.ListarHabitacionesPorTipoHabitacion(id);
                    return ListaHabitaciones;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca una habitación por id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>La habitación encontrada, de los contrario null.</returns>
        public async Task<Habitacion> BuscarHabitacionPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await habitacionDAL.BuscarHabitacionPorId(id);
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
