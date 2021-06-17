using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class TipoHabitacionBL
    {
        //optimizado
        private TipoHabitacionDAL tipoHabitacionDAL = new TipoHabitacionDAL();
        
        /// <summary>
        /// Se agrega un nuevo tipo de habitación.
        /// </summary>
        /// <param name="tipoHabitacion"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: ya existe, 3: la información recibida es incompleta.</returns>
        public async Task<int> AgregarTipoDeHabitacionUnico(TipoHabitacion tipoHabitacion)
        {
            try
            {
                if(!string.IsNullOrEmpty(tipoHabitacion.TipoDeHabitacion) && !string.IsNullOrEmpty(tipoHabitacion.Descripcion))
                {
                    List<TipoHabitacion> ListaTipoHabitaciones = await tipoHabitacionDAL.BuscarHabitacionPorNombre(tipoHabitacion.TipoDeHabitacion);
                    int coincidencia = ListaTipoHabitaciones.Count();
                    if(coincidencia == 0)
                    {
                        tipoHabitacion.Estado = 1;
                        return await tipoHabitacionDAL.AgregarTipoHabitacion(tipoHabitacion);
                    }
                    return 2;//Ya existe.
                }
                return 3;//La información está incompleta.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita un tipo de habitación al pasar su estado a 0. Este método no elimina el registro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: la habitación no existe, 3: el id es invalido.</returns>
        public async Task<int> EliminarTipoDeHabitacion(int id)
        {
            try
            {
                if (id > 0)
                {
                    TipoHabitacion tipoHabitacionExistente = await tipoHabitacionDAL.BuscarHabitacionPorId(id);
                    if(tipoHabitacionExistente != null)
                    {
                        tipoHabitacionExistente.Estado = 0;
                        return await tipoHabitacionDAL.EditarTipoHabitacion(tipoHabitacionExistente);
                    }
                    return 2;//La habitación no exite.
                }
                return 3;//El id no es valido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita un tipo de habitación asegurando que este no se repita.
        /// </summary>
        /// <param name="tipoHabitacion"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: ya existe, 3: no se han hecho cambios, 4: se recibe información incompleta.</returns>
        public async Task<int>EditarTipoDeHabitacion(TipoHabitacion tipoHabitacion)
        {
            try
            {
                if (!string.IsNullOrEmpty(tipoHabitacion.TipoDeHabitacion) && !string.IsNullOrEmpty(tipoHabitacion.Descripcion))
                {
                    //Control de cambios.
                    TipoHabitacion tipoHabitacionExistente = await tipoHabitacionDAL.BuscarHabitacionPorId(tipoHabitacion.IdTipoDeHabitacion);
                    if(tipoHabitacion.TipoDeHabitacion != tipoHabitacionExistente.TipoDeHabitacion || tipoHabitacion.Descripcion != tipoHabitacionExistente.Descripcion)
                    {
                        //Verificamos que sea único.
                        List<TipoHabitacion> ListaTipoHabitaciones = await tipoHabitacionDAL.BuscarHabitacionPorNombre(tipoHabitacion.IdTipoDeHabitacion, tipoHabitacion.TipoDeHabitacion);
                        int coincidencia = ListaTipoHabitaciones.Count();
                        if (coincidencia == 0)
                        {
                            return await tipoHabitacionDAL.EditarTipoHabitacion(tipoHabitacion);
                        }
                        return 2;//Ya existe.
                    }
                    return 3;//No se han hecho cambios.
                }
                return 4;//La información está incompleta.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los tipos de habitaciones activos, es decir, cuyo estado = 1.
        /// </summary>
        /// <returns>La lista encontrada.</returns>
        public async Task<List<TipoHabitacion>>ListarTipoDeHabitacionesActivas()
        {
            try
            {
                return await tipoHabitacionDAL.ListarTipoHabitacionesActivas();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca un tipo de habitación  por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>El tipo de habitación encontrado.</returns>
        public async Task<TipoHabitacion>BuscarTipoDeHabitacionPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await tipoHabitacionDAL.BuscarHabitacionPorId(id);
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
