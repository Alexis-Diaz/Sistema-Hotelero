using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;
using SysHotel.BL.Service;

namespace SysHotel.BL
{
    public class FacturaBL
    {
        //optimizado
        private FacturaDAL facturaDAL = new FacturaDAL();
        private GenerarCorrelativo generar = new GenerarCorrelativo();

        /// <summary>
        /// Guarda la información de una factura.
        /// </summary>
        /// <param name="factura"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: ya existe factura con el mismo número, 3: falta información. </returns>
        public async Task<int> AgregarFacturaUnica(Factura factura)
        {
            try
            {
                if (!string.IsNullOrEmpty(factura.NumeroFactura) && factura.FechaEmision != null
                && factura.IVA > 0 && factura.SubTotal > 0 && factura.TotalFactura > 0 
                && factura.IdReservacion > 0)
                {
                    //Se verifica que el número de la factura sea único.
                    List<Factura> facturas = await facturaDAL.ListarFacturasPorCorrelativo(factura.NumeroFactura);
                    int resultado = facturas.Count();

                    if (resultado == 0)
                    {
                        factura.Estado = 1;//0 eliminada, 1 emitida, 2 anulada.
                        return await facturaDAL.AgregarFactura(factura);
                    }
                    else
                    {
                        //En caso de que exista el número de la factura le generamos uno nuevo.
                        factura.NumeroFactura = await generar.GenerarNumeroCorrelativoDeFactura("24DS00F");
                        factura.Estado = 1;//0 eliminada, 1 emitida, 2 anulada.
                        return await facturaDAL.AgregarFactura(factura);
                    }
                }
                return 3;//falta información
            }
            catch (Exception)
            {
                return 0;//no guardó
            }
        }

        /// <summary>
        /// Anula una factura pasando su estado a 2.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: factura no existe.</returns>
        public async Task<int> AnularFactura(int id)
        {
            try
            {
                Factura factura = await facturaDAL.BuscarFacturaPorId(id);
                if (factura != null)
                {
                    factura.Estado = 2;//2: anulada
                    return await facturaDAL.EditarFactura(factura);
                }
                return 2;//no existe la factura
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// Lista todas las facturas en orden de emisión. La más reciente queda primero.
        /// </summary>
        /// <returns>Una lista de facturas.</returns>
        public async Task<List<Factura>> ObtenerTodasLasFacturas()
        {
            try
            {
                List<Factura> Lista = await facturaDAL.ListarFacturasEmitidasYAnuladas();
                return Lista;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Lista las facturas emitidas correctamente en orden de emisión. La más reciente queda primero.
        /// </summary>
        /// <returns>Una lista de facturas.</returns>
        public async Task<List<Factura>> ObtenerTodasLasFacturasEmitidas()
        {
            try
            {
                List<Factura> Lista = await facturaDAL.ListarFacturasPorEstado(1);
                return Lista;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Lista las facturas anuladas en orden de emisión. La más reciente queda primero.
        /// </summary>
        /// <returns>Una lista de facturas.</returns>
        public async Task<List<Factura>> ObtenerTodasLasFacturasAnuladas()
        {
            try
            {
                List<Factura> Lista = await facturaDAL.ListarFacturasPorEstado(2);
                return Lista;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Busca una factura por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>La factura encontrada, caso contrario devuelve null.</returns>
        public async Task<Factura> ObtenerFacturaPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await facturaDAL.BuscarFacturaPorId(id);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Lista las facturas por usuario en orden de emisión. La más reciente queda primero.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Lista de facturas por usuario.</returns>
        //        public async Task<List<Factura>> ObtenerFacturasPorIdUsuario(int id)
        //        {
        //            try
        //            {
        //                if (id > 0)
        //                {
        //                    List<Factura> ListaFacturas = await facturaDAL.ListarFacturaPorIdUsuario(id);
        //                    return ListaFacturas;
        //                }
        //                return null;
        //            }
        //            catch (Exception)
        //            {
        //                throw;
        //            }

        //        }
    }
}
