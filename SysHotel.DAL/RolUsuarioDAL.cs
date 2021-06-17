using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class RolUsuarioDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarRolUsuario(RolUsuario rolUsuario)
        {
            try
            {
                if(rolUsuario != null)
                {
                    db.RolUsuarios.Add(rolUsuario);
                    return await db.SaveChangesAsync();
                }
                return 0;//El objeto rolUsuario viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarRolUsuario(int id)
        {
            try
            {
                if (id > 0)
                {
                    RolUsuario rolUsuarioExistente = await db.RolUsuarios.FindAsync(id);
                    if (rolUsuarioExistente != null)
                    {
                        db.RolUsuarios.Remove(rolUsuarioExistente);
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
        public async Task<int>EditarRolUsuario(RolUsuario rolUsuario)
        {
            try
            {
                if(rolUsuario != null)
                {
                    RolUsuario rolUsuarioExistente = await db.RolUsuarios.FindAsync(rolUsuario.IdRolUsuario);
                    if (rolUsuarioExistente != null)
                    {
                        rolUsuarioExistente.Rol = rolUsuario.Rol;
                        rolUsuarioExistente.Estado = rolUsuario.Estado;

                        db.Entry(rolUsuarioExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0; //El objeto rolUsuario viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<RolUsuario>> ListarRolUsuariosActivos()
        {
            try
            {
                return await db.RolUsuarios.Where(x => x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar
        public async Task<List<RolUsuario>> ListarRolUsuariosInactivos()
        {
            try
            {
                return await db.RolUsuarios.Where(x => x.Estado == 0).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<RolUsuario>BuscarRolUsuarioPorId(int id)
        {
            try
            {
                return await db.RolUsuarios.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por rol
        public async Task<List<RolUsuario>> BuscarRolUsuarioPorNombreRol(string rol)
        {
            try
            {
                if(rol != "")
                {
                    return await db.RolUsuarios.Where(x => x.Rol == rol).ToListAsync();
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //buscar por rol
        public async Task<List<RolUsuario>> BuscarRolUsuarioPorNombreRol(int id, string rol)
        {
            try
            {
                if (rol != "")
                {
                    return await db.RolUsuarios.Where(x => x.IdRolUsuario != id && x.Rol == rol).ToListAsync();
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
