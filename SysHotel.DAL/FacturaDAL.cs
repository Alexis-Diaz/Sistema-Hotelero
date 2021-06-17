using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class FacturaDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarFactura(Factura factura){
            try
            {
                if(factura != null)
                {
                    db.Facturas.Add(factura);
                    return await db.SaveChangesAsync();
                }
                return 0; //El objeto factura viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int> EliminarFactura(int id)
        {
            try
            {
                if (id > 0)
                {
                    Factura factura = await db.Facturas.FindAsync(id);
                    if(factura != null)
                    {
                        db.Facturas.Remove(factura);
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
        /// <summary>
        /// El metodo editar se programa pero no se utiliza. Una factura una vez emitida
        /// solamente se puede anular. Se hace asi para mantener registros contables 
        /// exactos.
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public async Task<int>EditarFactura(Factura factura)
        {
            try
            {
                if(factura != null)
                {
                    Factura facturaExistente = await db.Facturas.FindAsync(factura.IdFactura);
                    if(facturaExistente != null)
                    {
                        facturaExistente.NumeroFactura = factura.NumeroFactura;
                        facturaExistente.FechaEmision = factura.FechaEmision;
                        facturaExistente.IVA = factura.IVA;
                        facturaExistente.SubTotal = factura.SubTotal;
                        facturaExistente.TotalFactura = factura.TotalFactura;
                        facturaExistente.Estado = factura.Estado;
                        facturaExistente.IdFactura = factura.IdFactura;

                        db.Entry(facturaExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto factura viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<Factura>>ListarFacturas()
        {
            try
            {
                return await db.Facturas.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por numero de factura
        public async Task<List<Factura>> ListarFacturasPorCorrelativo(string correlativo)
        {
            try
            {
                return await db.Facturas.Where(x => x.Estado==1 || x.Estado == 2 && x.NumeroFactura == correlativo).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por numero de factura
        public async Task<List<Factura>> ListarFacturasEmitidasYAnuladas()
        {
            try
            {
                return await db.Facturas.Where(x => x.Estado == 1 || x.Estado == 2).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Factura>> ListarFacturasPorEstado(int estado)
        {
            try
            {
                return await db.Facturas.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        ////listar por id Usuario
        //public async Task<List<Factura>> ListarFacturaPorIdUsuario(int idUsuario)
        //{
        //    try
        //    {
        //        return await db.Facturas.Where(x => x.IdUsuario == idUsuario).ToListAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //buscar por id
        public async Task<Factura> BuscarFacturaPorId(int id)
        {
            try
            {
                return await db.Facturas.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }   

        //buscar última factura según fecha
        public async Task<Factura>BuscarUltimaFacturaIngresada()
        {
            try
            {
                return await db.Facturas.OrderByDescending(t => t.FechaEmision).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
