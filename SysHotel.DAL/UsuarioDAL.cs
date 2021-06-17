using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class UsuarioDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarUsuario(Usuario usuario)
        {
            try
            {
                if(usuario != null)
                {
                    db.Usuarios.Add(usuario);
                    return await db.SaveChangesAsync();
                }
                return 0;//El objeto usuario viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int> EliminarUsuario(int id)
        {
            try
            {
                if (id > 0)
                {
                    Usuario usuarioExistente = await db.Usuarios.FindAsync(id);
                    if (usuarioExistente != null)
                    {
                        db.Usuarios.Remove(usuarioExistente);
                    }
                }
                return 0;//El id es invalido.
            }
            catch (Exception)
            {
                throw;
            }
        }

        //editar
        public async Task<int>EditarUsuario(Usuario usuario)
        {
            try
            {
                if(usuario != null)
                {
                    Usuario usuarioExistente = await db.Usuarios.FindAsync(usuario.IdUsuario);
                    if(usuarioExistente != null)
                    {
                        usuarioExistente.Nombres = usuario.Nombres;
                        usuarioExistente.Apellidos = usuario.Apellidos;
                        usuarioExistente.FechaNacimiento = usuario.FechaNacimiento;
                        usuarioExistente.Direccion = usuario.Direccion;
                        usuarioExistente.Telefono = usuario.Telefono;
                        usuarioExistente.DUI = usuario.DUI;
                        usuarioExistente.Correo = usuario.Correo;
                        usuarioExistente.NombreUsuario = usuario.NombreUsuario;
                        usuarioExistente.Contraseña = usuario.Contraseña;
                        usuarioExistente.Estado = usuario.Estado;
                        usuarioExistente.IdRolUsuario = usuario.IdRolUsuario;

                        db.Entry(usuarioExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto usuario viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<Usuario>> ListarUsuariosActivos()
        {
            try
            {
                return await db.Usuarios.Where(x => x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<Usuario>> ListarUsuariosActivosPorDuiCorreoYNombreUsuario(int id, string dui, string correo, string nombreUsuario)
        {
            try
            {
                List<Usuario> usuario = await db.Usuarios.Where(x => x.IdUsuario != id
                                                                  && x.Estado == 1
                                                                  && x.DUI == dui
                                                                  || x.IdUsuario != id
                                                                  && x.Estado == 1
                                                                  && x.Correo == correo
                                                                  || x.IdUsuario != id
                                                                  && x.Estado == 1
                                                                  && x.NombreUsuario == nombreUsuario)
                                                                      .ToListAsync();
                return usuario;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Usuario>> ListarUsuariosActivosPorDuiCorreoYNombreUsuario(string dui, string correo, string nombreUsuario)
        {
            try
            {
                return await db.Usuarios.Where(x => x.DUI == dui
                                                 || x.Correo == correo
                                                 || x.NombreUsuario == nombreUsuario
                                                 && x.Estado == 1)
                                                     .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Usuario>BuscarUsuarioPorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await db.Usuarios.FindAsync(id);
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //buscar por idRol
        public async Task<List<Usuario>> BuscarUsuarioPorIdRol(int idRol)
        {
            try
            {
                if (idRol > 0)
                {
                    return await db.Usuarios.Where(x => x.IdRolUsuario == idRol).ToListAsync();
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
