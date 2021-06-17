using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL
{
    public class RolUsuarioBL
    {
        //optimizado.
        private RolUsuarioDAL rolUsuarioDAL = new RolUsuarioDAL();

        /// <summary>
        /// Agregar un rol de usuario único.
        /// </summary>
        /// <param name="rol"></param>
        /// <returns>Un entero, donde:
        /// 0:no guardó, 1: guardó, 2: el rol ya existe, 3: el rol está incompleto.</returns>
        public async Task<int>AgregarRolUsuario(RolUsuario rol)
        {
            try
            {
                if (!string.IsNullOrEmpty(rol.Rol))
                {
                    List<RolUsuario> ListaRoles = await rolUsuarioDAL.BuscarRolUsuarioPorNombreRol(rol.Rol);
                    int coincidencia = ListaRoles.Count();
                    if(coincidencia == 0)
                    {
                        rol.Estado = 1;
                        return await rolUsuarioDAL.AgregarRolUsuario(rol);
                    }
                    return 2; //El rol ya existe;
                }
                return 3; // el rol viene incompleto.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita un rol usuario al pasar su estado a 0. Este método no elimina el registro de la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: no existe, 3: id inválido.</returns>
        public async Task<int>EliminarRolUsuario(int id)
        {
            try
            {
                if (id > 0)
                {
                    RolUsuario rolExistente = await rolUsuarioDAL.BuscarRolUsuarioPorId(id);
                    if(rolExistente != null)
                    {
                        rolExistente.Estado = 0;
                        return await rolUsuarioDAL.EditarRolUsuario(rolExistente);
                    }
                    return 2;//no existe.
                }
                return 3;//id invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edita un rol de usuario verificando que no exista.
        /// </summary>
        /// <param name="rol"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: ya existe, 3: no se han hecho cambios, 4: rol incompleto.</returns>
        public async Task<int>EditarRolUsuario(RolUsuario rol)
        {
            try
            {
                if (!string.IsNullOrEmpty(rol.Rol))
                {
                    RolUsuario rolExistente = await rolUsuarioDAL.BuscarRolUsuarioPorId(rol.IdRolUsuario);
                    if(rol.Rol != rolExistente.Rol)
                    {
                        List<RolUsuario> ListaRoles = await rolUsuarioDAL.BuscarRolUsuarioPorNombreRol(rol.IdRolUsuario, rol.Rol);
                        int coincidencia = ListaRoles.Count();
                        if (coincidencia == 0)
                        {
                            return await rolUsuarioDAL.EditarRolUsuario(rol);
                        }
                        return 2; //El rol ya existe;
                    }
                    return 3;//no se han hecho cambios.
                }
                return 4; // el rol viene incompleto.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los roles activos, es decir, cuyo estado es 1.
        /// </summary>
        /// <returns>La lista de roles activos.</returns>
        public async Task<List<RolUsuario>> ListarRolUsuariosActivos()
        {
            try
            {
                List<RolUsuario> listaRolUsuario = await rolUsuarioDAL.ListarRolUsuariosActivos();
                return listaRolUsuario;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Busca un rol por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>El rol encontrado, de lo contrario null.</returns>
        public async Task<RolUsuario>BuscarRolUsuarioPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await rolUsuarioDAL.BuscarRolUsuarioPorId(id);
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
