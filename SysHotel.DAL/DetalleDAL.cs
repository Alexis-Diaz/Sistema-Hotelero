using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class DetalleDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarDetalle(Detalle detalle)
        {
            try
            {
                if (detalle != null)
                {
                    db.Detalles.Add(detalle);
                    return await db.SaveChangesAsync();
                }
                return 0;//El objeto detalle viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarDetalle(int id)
        {
            try
            {
                if (id > 0)
                {
                    Detalle detalle = await db.Detalles.FindAsync(id);
                    if(detalle != null)
                    {
                        db.Detalles.Remove(detalle);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0; //El id es invalido
            }
            catch (Exception)
            {
                throw;
            }
        }

        //editar
        public async Task<int>EditarDetalle(Detalle detalle)
        {
            try
            {
                if (detalle != null)
                {
                    Detalle detalleExistente = await db.Detalles.FindAsync(detalle.IdDetalle);
                    if (detalleExistente != null)
                    {
                        detalleExistente.Dia = detalle.Dia;
                        //detalleExistente.Hora = detalle.Hora;
                        detalleExistente.TiempoDeComida = detalle.TiempoDeComida;
                        detalleExistente.Cantidad = detalle.Cantidad;
                        detalleExistente.TotalDetalle = detalle.TotalDetalle;
                        detalleExistente.Estado = detalle.Estado;
                        detalleExistente.IdReservacion = detalle.IdReservacion;
                        detalleExistente.IdAlimento = detalle.IdAlimento;

                        db.Entry(detalleExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto detalle viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar todo
        public async Task<List<Detalle>> ListarDetalles()
        {
            try
            {
                return await db.Detalles.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Detalle>> ListarDetallesPorEstado(int estado)
        {
            try
            {
                return await db.Detalles.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por id reservacion
        public async Task<List<Detalle>> ListarDetallesPorIdReservacion(int idReservacion)
        {
            try
            {
                return await db.Detalles.Where(x => x.IdReservacion == idReservacion).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Detalle> BuscarDetallePorId(int id)
        {
            try
            {
                return await db.Detalles.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
