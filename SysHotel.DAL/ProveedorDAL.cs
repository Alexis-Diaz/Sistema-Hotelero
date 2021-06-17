using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class ProveedorDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int> AgregarProveedor(Proveedor proveedor)
        {
            try
            {
                if (proveedor != null)
                {
                    db.Proveedors.Add(proveedor);
                    return await db.SaveChangesAsync();
                }
                return 0;// El objeto proveedor esta vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarProveedor(int id)
        {
            try
            {
                if(id > 0)
                {
                    Proveedor proveedorExistente = await db.Proveedors.FindAsync(id);
                    if(proveedorExistente != null)
                    {
                        db.Proveedors.Remove(proveedorExistente);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El id no es valido
            }
            catch (Exception)
            {
                throw;
            }
        }

        //editar
        public async Task<int>EditarProveedor(Proveedor proveedor)
        {
            try
            {
                if(proveedor != null)
                {
                    Proveedor proveedorExistente = await db.Proveedors.FindAsync(proveedor.IdProveedor);
                    if(proveedorExistente != null)
                    {
                        proveedorExistente.NombreEmpresa = proveedor.NombreEmpresa;
                        proveedorExistente.Ubicacion = proveedor.Ubicacion;
                        proveedorExistente.Encargado = proveedor.Encargado;
                        proveedorExistente.Telefono = proveedor.Telefono;
                        proveedorExistente.Correo = proveedor.Correo;
                        proveedorExistente.Estado = proveedor.Estado;

                        db.Entry(proveedorExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0; //El objeto proveedor viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar todo
        public async Task<List<Proveedor>> ListarProveedores()
        {
            try
            {
                return await db.Proveedors.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por nombre de empresa
        public async Task<List<Proveedor>> ListarProveedoresPorIdYNombreEmpresa(string nombreEmpresa)
        {
            try
            {
                return await db.Proveedors.Where(x =>x.NombreEmpresa == nombreEmpresa).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por nombre de empresa
        public async Task<List<Proveedor>> ListarProveedoresPorIdYNombreEmpresa(int id, string nombreEmpresa)
        {
            try
            {
                return await db.Proveedors.Where(x => x.IdProveedor != id && x.NombreEmpresa == nombreEmpresa).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Proveedor>> ListarProveedoresPorEstado(int estado)
        {
            try
            {
                return await db.Proveedors.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Proveedor>BuscaProveedorPorId(int id)
        {
            try
            {
                return await db.Proveedors.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
