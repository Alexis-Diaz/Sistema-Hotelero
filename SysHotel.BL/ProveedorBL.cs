using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class ProveedorBL
    {
        //optimizado
        private ProveedorDAL proveedorDAL = new ProveedorDAL();
        
        /// <summary>
        /// Se agregar un nuevo proveedor.
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: ya existe, 3: se recibe información incompleta.</returns>
        public async Task<int> AgregarNuevoProveedor(Proveedor proveedor)
        {
            try
            {
                if(!string.IsNullOrEmpty(proveedor.NombreEmpresa) && !string.IsNullOrEmpty(proveedor.Ubicacion)
                && !string.IsNullOrEmpty(proveedor.Encargado) && !string.IsNullOrEmpty(proveedor.Telefono)
                && !string.IsNullOrEmpty(proveedor.Correo))
                {
                    List<Proveedor> ListaProveedores = await proveedorDAL.ListarProveedoresPorIdYNombreEmpresa(proveedor.NombreEmpresa);
                    int coincidencia = ListaProveedores.Count();
                    if(coincidencia == 0)
                    {
                        proveedor.Estado = 1;
                        return await proveedorDAL.AgregarProveedor(proveedor);
                    }
                    return 2; //El proveedor ya existe.
                }
                return 3; //La información está incompleta.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita al proveedor al pasar su estado en 0. Este método no elimina al proveedor.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el proveedor no existe, 3: el id es inválido</returns>
        public async Task<int>EliminarProveedor(int id)
        {
            try
            {
                if (id > 0)
                {
                    Proveedor proveedorExistente = await proveedorDAL.BuscaProveedorPorId(id);
                    if(proveedorExistente != null)
                    {
                        proveedorExistente.Estado = 0;
                        return await proveedorDAL.EditarProveedor(proveedorExistente);
                    }
                    return 2;//el proveedor no existe.
                }
                return 3;//el id es invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita la información de un proveedor.
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el proveedor no existe, 3: no se han hecho cambios, 4: se recibe información incompleta.</returns>
        public async Task<int>EditarProveedor(Proveedor proveedor)
        {
            try
            {
                //Verificamos que el proveedor esté completo
                if (!string.IsNullOrEmpty(proveedor.NombreEmpresa) && !string.IsNullOrEmpty(proveedor.Ubicacion)
                && !string.IsNullOrEmpty(proveedor.Encargado) && !string.IsNullOrEmpty(proveedor.Telefono)
                && !string.IsNullOrEmpty(proveedor.Correo))
                {
                    //Verificamos que se hayan hecho cambios
                    Proveedor proveedorExistente = await proveedorDAL.BuscaProveedorPorId(proveedor.IdProveedor);
                    if (proveedor.NombreEmpresa != proveedorExistente.NombreEmpresa
                    || proveedor.Ubicacion != proveedorExistente.Ubicacion
                    ||  proveedor.Encargado != proveedorExistente.Encargado
                    || proveedor.Telefono != proveedorExistente.Telefono
                    || proveedor.Correo != proveedorExistente.Correo)
                    {
                        //Verificamos que no exista
                        List<Proveedor> ListaProveedores = await proveedorDAL.ListarProveedoresPorIdYNombreEmpresa(proveedor.IdProveedor, proveedor.NombreEmpresa);
                        int coincidencia = ListaProveedores.Count();
                        if (coincidencia == 0)
                        {
                            proveedor.Estado = 1;
                            return await proveedorDAL.EditarProveedor(proveedor);
                        }
                        return 2; //El proveedor ya existe.
                    }
                    return 3;//No se han hecho cambios.
                }
                return 4; //La información está incompleta.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los proveedores activos, es decir con estado = 1.
        /// </summary>
        /// <returns>La lista de proveedores.</returns>
        public async Task<List<Proveedor>> ListarProveedoresActivos()
        {
            try
            {
                List<Proveedor> ListaProveedores = await proveedorDAL.ListarProveedoresPorEstado(1);
                return ListaProveedores;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca un proveedor por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>El proveedor encontrado.</returns>
        public async Task<Proveedor>BuscarProveedorPorId(int id)
        {
            try
            {
                if(id > 0)
                {
                    return await proveedorDAL.BuscaProveedorPorId(id);
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