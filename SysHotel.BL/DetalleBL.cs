using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class DetalleBL
    {
        //optimizado
        private DetalleDAL detalleDAL = new DetalleDAL();
        private AlimentoDAL alimentoDAL = new AlimentoDAL();

        /// <summary>
        /// Agrega un nuevo detalle del pedido de comida
        /// </summary>
        /// <param name="detalle"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: no existe el alimento, 3: fecha de comida incorrecta, 4: La reservación no está activa, 5: El detalle es incompleto.</returns>
        public async Task<int> AgregarNuevoDetalle(Detalle detalle)
        {
            try
            {
                if (detalle.Dia != null && !string.IsNullOrEmpty(detalle.TiempoDeComida)
                && detalle.Cantidad > 0 && detalle.IdReservacion > 0 && detalle.IdAlimento > 0)
                {
                   if(detalle.reservacion.Estado == 2)
                    {
                        //validamos que la fecha sea futura
                        //La hora de anticipacion es personalizable
                        if (VerificarTiempo(detalle.TiempoDeComida, detalle.Dia, 6))
                        {
                            //Calculamos el total del detalle
                            Alimento alimento = await alimentoDAL.BuscarAlimentoPorId(detalle.IdAlimento);
                            if (alimento == null)
                            {
                                return 2;
                            }
                            detalle.TotalDetalle = alimento.Precio * detalle.Cantidad;
                            detalle.Estado = 1;//Detalle activo
                            return await detalleDAL.AgregarDetalle(detalle);
                        }
                        return 3;//Indica que la fecha de comida es incorrecta, debe hacerse con anticipación
                    }
                    return 4;//La reservación no está en curso.
                }
                return 5;//El objeto detalle se recibe incompleto.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cancela un detalle de pedido de comida asegurando que se haga
        /// según el tiempo de anticipación.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: Ya no se puede cancelar, 3: El detalle no existe, 4: El id es inválido.</returns>
        public async Task<int> CancelarDetalle(int id)
        {
            try
            {
                if (id > 0)
                {
                    Detalle detalleExistente = await detalleDAL.BuscarDetallePorId(id);
                    if (detalleExistente != null)
                    {
                        //Verificamos que la cancelación se haga con anticipación.
                        //La hora de cancelación se puede personalizar.
                        if (VerificarTiempo(detalleExistente.TiempoDeComida, detalleExistente.Dia, 6)) {
                            detalleExistente.Estado = 3;//El detalle esta cancelado.
                            return await detalleDAL.EditarDetalle(detalleExistente);
                        }
                        return 2; //La comida ya no se puede cancelar.
                    }
                    return 3; //El detalle no existe.
                }
                return 4; //El id es invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cobra un detalle activo
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el detalle no está activo, 3: el detalle no existe, 4: el id es inválido.</returns>
        public async Task<int> CobrarDetalle(int id)
        {
            try
            {
                if (id > 0)
                {
                    Detalle detalleExistente = await detalleDAL.BuscarDetallePorId(id);
                    if (detalleExistente != null)
                    {
                        //Verificamos que sea un detalle activo
                        if (detalleExistente.Estado == 1)
                        {
                            detalleExistente.Estado = 2;//El detalle está cobrado.
                            return await detalleDAL.EditarDetalle(detalleExistente);
                        }
                        return 2;//El detalle no esta activo.
                    }
                    return 3;//El detalle no existe.
                }
                return 4;//El id es invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Elimina un detalle cancelado o cobrado. Este método no lo elimina de la base de datos, solo 
        /// vuelve su estado en 0.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el detalle está activo, 3: el detalle no existe, 4: el id es inválido.</returns>
        public async Task<int> EliminarDetalle(int id)
        {
            try
            {
                if (id > 0)
                {
                    Detalle detalleExistente = await detalleDAL.BuscarDetallePorId(id);
                    if (detalleExistente != null)
                    {
                        //Verificamos que sea un detalle cancelado o cobrado
                        if (detalleExistente.Estado == 2 || detalleExistente.Estado == 3)
                        {
                            detalleExistente.Estado = 0;//El detalle está eliminado.
                            return await detalleDAL.EditarDetalle(detalleExistente);
                        }
                        return 2;//El detalle está activo.
                    }
                    return 3;//El detalle no existe.
                }
                return 4;//El id es invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita un detalle, tomando en cuenta que la fecha sea correcta
        /// </summary>
        /// <param name="detalle"></param>
        /// <returns>Un entero, dende:
        /// 0: no guardo, 1: guardó, 2: fecha de comida incorrecta, 3: no se han hecho cambios,
        /// 4: el detalle no existe, 5: el objeto detalle se recibe incompleto.</returns>
        public async Task<int> EditarDetalle(Detalle detalle) {
            try
            {
                if (detalle.Dia != null 
                || !string.IsNullOrEmpty(detalle.TiempoDeComida)
                || detalle.Cantidad > 0 
                || detalle.IdReservacion > 0 
                || detalle.IdAlimento > 0)
                {
                    Detalle detalleExistente = await detalleDAL.BuscarDetallePorId(detalle.IdDetalle);
                    if (detalleExistente != null)
                    {
                        if (detalle.Dia != detalleExistente.Dia 
                        || detalle.TiempoDeComida != detalleExistente.TiempoDeComida
                        || detalle.Cantidad != detalleExistente.Cantidad 
                        || detalle.IdReservacion != detalleExistente.IdReservacion
                        || detalle.IdAlimento != detalleExistente.IdAlimento)
                        {
                            //validamos que la fecha sea futura
                            //La hora de anticipacion es personalizable
                            if (VerificarTiempo(detalle.TiempoDeComida, detalle.Dia, 6))
                            {
                                detalle.Estado = 1;//Detalle activo
                                return await detalleDAL.EditarDetalle(detalle);
                            }
                            return 2;//La modificacion debe hacerse con anticipación
                        }
                        return 3; //no se han hecho cambios
                    }
                    return 4; //el detalle no existe.
                }
                return 5;//El objeto detalle se recibe incompleto.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los detalles de acuerdo al estado recibido.
        /// </summary>
        /// <param name="estado"></param>
        /// <returns>Una lista de detalles filtrada.</returns>
        public async Task<List<Detalle>> ListarDetallesPorEstado(int estado)
        {
            try
            {
                if(estado >= 0)
                {
                    List<Detalle> ListaDetalles = await detalleDAL.ListarDetallesPorEstado(estado);
                    return ListaDetalles;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca un detalle por id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Devuelve el detalle encontrado.</returns>
        public async Task<Detalle> BuscarDetallePorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    Detalle detalle = await detalleDAL.BuscarDetallePorId(id);
                    return detalle;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los detalles segun el id de la reservación.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Devuelve una lista de detalles segun el id de la reservación.</returns>
        public async Task<List<Detalle>>BuscarDetallesPorIdReservacion(int id)
        {
            try
            {
                if (id > 0)
                {
                    List<Detalle> ListaDetalle = await detalleDAL.ListarDetallesPorIdReservacion(id);
                    return ListaDetalle;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método sirve para saber si el pedido o cancelación de la comida se está haciendo de acuerdo 
        /// al tiempo de anticipación requerido.
        /// </summary>
        /// <param name="TiempoComida"></param>
        /// <param name="DiaComida"></param>
        /// <param name="HoraAnticipacion"></param>
        /// <returns>Un booleano, donde:
        /// true: la fecha de pedido o cancelación es correcta, false: la fecha de pedido o cancelación es incorrecta.</returns>
        public bool VerificarTiempo(string TiempoComida, DateTime DiaComida, int HoraAnticipacion)
        {
            TiempoComida = TiempoComida.ToLower();//convertimos el string a minúsculas para manejarlo mejor.
            DateTime FechaActual = DateTime.Now;//La fecha actual.
            bool Respuesta = false;

            //Se verifica que el tiempo del pedido de comida
            //se haga según las horas de anticipación requerida.
            switch (TiempoComida)
            {
                case "desayuno":
                    DiaComida = DiaComida.AddHours(6);//El dia se captura con hora 0, aquí se establece la hora que se sirve el desayuno.
                    //Se comprueba que la fecha de la comida sea mayor a la fecha actual, 
                    //tomando en cuenta las horas de anticipación;
                    if(DiaComida > FechaActual.AddHours(HoraAnticipacion))
                    {
                        Respuesta = true;//fecha valida para el desayuno
                    }
                    break;

                case "almuerzo":
                    DiaComida = DiaComida.AddHours(12);//El dia se captura con hora 0, aquí se establece la hora que se sirve el almuerzo.
                    //Se comprueba que la fecha de la comida sea mayor a la fecha actual, 
                    //tomando en cuenta las horas de anticipación;
                    if (DiaComida > FechaActual.AddHours(HoraAnticipacion))
                    {
                        Respuesta = true;//fecha valida para el almuerzo
                    }
                    break;

                case "cena":
                    DiaComida = DiaComida.AddHours(18);//El dia se captura con hora 0, aquí se establece la hora que se sirve la cena.
                    //Se comprueba que la fecha de la comida sea mayor a la fecha actual, 
                    //tomando en cuenta las horas de anticipación;
                    if (DiaComida > FechaActual.AddHours(HoraAnticipacion))
                    {
                        Respuesta = true;//fecha valida para la cena
                    }
                    break;
            }
            return Respuesta;
        }
    }
}
