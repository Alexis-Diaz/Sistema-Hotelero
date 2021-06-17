using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHotel.EL.Login
{
    /// <summary>
    /// La clase FrontUser sirve para verificar si un usuario loguedo tiene
    /// permiso para ingresar o ver determinado componente o modulos de una
    /// aplicacion web.
    /// </summary>
    public class FrontUser
    {
        /// <summary>
        /// Este metodo recupera el usuario con sesion activa y luego verifica
        /// si tiene permiso para ver un modulo de la aplicacion
        /// </summary>
        /// <param name="valor">El valor es un enum</param>
        /// <returns>Retorna false si no tiene permiso o true si tiene permiso</returns>
        public static bool TienePermiso(RolesPermisos valor)
        {
            var usuario = GetUser();
            return !usuario.rolUsuario.Permisos.Where(x => x.IdPermisos == valor)
                               .Any();
        }
        /// <summary>
        /// El metodo GetUser recupera el usuario que tiene una sesión abierta
        /// </summary>
        /// <returns></returns>
        public static Usuario GetUser()
        {
            return new Usuario().Obtener(SessionHelper.GetUser());
        }
    }
}
