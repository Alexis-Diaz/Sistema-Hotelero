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
    public class UsuarioBL
    {
        //optimizado
        private UsuarioDAL usuarioDAL = new UsuarioDAL();

        /// <summary>
        /// Agrega un nuevo usuario.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: DUI tiene letras, 3: DUI inválido, 4: el DUI no tiene 9 digitos
        /// 5: ya existe el usuario, 6: usuario incompleto.</returns>
        public async Task<int>AgregarUsuarioUnico(Usuario usuario)
        {
            try
            {
                if(!string.IsNullOrEmpty(usuario.Nombres) 
                && !string.IsNullOrEmpty(usuario.Apellidos) 
                && usuario.FechaNacimiento != null 
                && !string.IsNullOrEmpty(usuario.Direccion) 
                && !string.IsNullOrEmpty(usuario.Telefono)
                && !string.IsNullOrEmpty(usuario.DUI) 
                && !string.IsNullOrEmpty(usuario.Correo) 
                && !string.IsNullOrEmpty(usuario.NombreUsuario)
                && !string.IsNullOrEmpty(usuario.Contraseña) 
                && usuario.IdRolUsuario > 0) 
                {
                    List<Usuario> ListaUsuario = await usuarioDAL.ListarUsuariosActivosPorDuiCorreoYNombreUsuario(usuario.DUI, usuario.Correo, usuario.NombreUsuario);
                    int coincidencia = ListaUsuario.Count();
                    if (coincidencia == 0)
                    {
                        int y = 0;
                        int x = VerificarDUI.VerificarNumeroDeDUI("dui", usuario.DUI);
                        switch (x)
                        {
                            case 1:
                                y = 2;//dui tiene letras
                                break;
                            case 2:
                                usuario.Estado = 1;
                                 y = await usuarioDAL.AgregarUsuario(usuario);
                                break;
                            case 3:
                                y = 3;//dui invalido
                                break;
                            case 4:
                                y = 4;//el dui no tiene 9 digitos
                                break;
                        }
                        return y;
                    }
                    return 5; //ya existe el usuario.
                }
                return 6;//usuario incompleto.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita un usuario al establecer su estado en 0. Este metodo no elimina el registro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el usuario no existe, 3: el id es inválido.</returns>
        public async Task<int>EliminarUsuario(int id)
        {
            try
            {
                if (id > 0)
                {
                    Usuario usuarioExistente = await usuarioDAL.BuscarUsuarioPorId(id);
                    if(usuarioExistente != null)
                    {
                        usuarioExistente.Estado = 0;
                        return await usuarioDAL.EditarUsuario(usuarioExistente);
                    }
                    return 2;//El usuario no existe.
                }
                return 3;//El id es invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Edita un usuario, verificando que sea único.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns> Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: DUI tiene letras, 3: DUI inválido, 4: el DUI no tiene 9 digitos
        /// 5: ya existe el usuario, 6: no se han hecho cambios , 7: usuario incompleto.</returns>
        public async Task<int>EditarUsuario(Usuario usuario)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuario.Nombres)
                && !string.IsNullOrEmpty(usuario.Apellidos)
                && usuario.FechaNacimiento != null
                && !string.IsNullOrEmpty(usuario.Direccion)
                && !string.IsNullOrEmpty(usuario.Telefono)
                && !string.IsNullOrEmpty(usuario.DUI)
                && !string.IsNullOrEmpty(usuario.Correo)
                && !string.IsNullOrEmpty(usuario.NombreUsuario)
                && !string.IsNullOrEmpty(usuario.Contraseña)
                && usuario.IdRolUsuario > 0)
                {
                    Usuario usuarioExistente = await usuarioDAL.BuscarUsuarioPorId(usuario.IdUsuario);
                    if (usuario.Nombres != usuarioExistente.Nombres
                    || usuario.Apellidos != usuarioExistente.Apellidos
                    || usuario.FechaNacimiento != usuarioExistente.FechaNacimiento
                    || usuario.Direccion != usuarioExistente.Direccion
                    || usuario.Telefono != usuarioExistente.Telefono
                    || usuario.DUI != usuarioExistente.DUI
                    || usuario.Correo != usuarioExistente.Correo
                    || usuario.NombreUsuario != usuarioExistente.NombreUsuario
                    || usuario.Contraseña != usuarioExistente.Contraseña
                    || usuario.IdRolUsuario != usuarioExistente.IdRolUsuario)
                    {
                        List<Usuario> ListaUsuario = await usuarioDAL.ListarUsuariosActivosPorDuiCorreoYNombreUsuario(usuario.IdUsuario, usuario.DUI, usuario.Correo, usuario.NombreUsuario);
                        int coincidencia = ListaUsuario.Count();
                        if (coincidencia == 0)
                        {
                            int y = 0;
                            int x = VerificarDUI.VerificarNumeroDeDUI("dui", usuario.DUI);
                            switch (x)
                            {
                                case 1:
                                    y = 2;//dui tiene letras
                                    break;
                                case 2:
                                    y = await usuarioDAL.EditarUsuario(usuario);
                                    break;
                                case 3:
                                    y = 3;//dui invalido
                                    break;
                                case 4:
                                    y = 4;//el dui no tiene 9 digitos
                                    break;
                            }
                            return y;
                        }
                        return 5; //ya existe el usuario.
                    }
                    return 6;//No se han hecho cambios.
                }
                return 7;//usuario incompleto.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los usuarios activos, es decir cuyo estado = 1.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        public async Task<List<Usuario>> ListarUsuariosActivos()
        {
            try
            {
                return await usuarioDAL.ListarUsuariosActivos();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Busca un usuario por id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Usuario encontrado.</returns>
        public async Task<Usuario>BuscarUsuarioPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await usuarioDAL.BuscarUsuarioPorId(id);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los usuarios por su rol.
        /// </summary>
        /// <param name="idRol"></param>
        /// <returns>Una lista de usuarios.</returns>
        public async Task<List<Usuario>>ListarUsuariosPorRol(int idRol)
        {
            try
            {
                if (idRol > 0)
                {
                    return await usuarioDAL.BuscarUsuarioPorIdRol(idRol);
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
