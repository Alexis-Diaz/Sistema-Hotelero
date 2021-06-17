using System;
using System.Web;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHotel.EL.Login
{
    /// <summary>
    /// La clase SessionHelper tiene metodos que sirven para crear cookies
    /// de autenticacion para usuarios autenticados, verificar si existe 
    /// una sesion abierta, recuperar el id de una sesión abierta y destruirlas.
    /// </summary>
    public class SessionHelper
    {

        /// <summary>
        /// Este metodo sirve para verificar que un usuario este autenticado.
        /// </summary>
        /// <returns>Si el usuario está autenticado devuelve true, si no está autenticado retorna false.</returns>
        public static bool ExistUserInSession()
        {
            //Para que obtener el usuario autenticado funcione es importante agregar en el
            //archivo Web.config del proyecto LoginPersonalizado.UI las siguientes lineas 
            //de codigo dentro de la etiqueta system.web:
            // <authentication mode="Forms">
            //   < forms name = "prueba" cookieless = "UseCookies" protection = "All" />
            // </ authentication >
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Metodo que sirve para cerrar una sesión abierta.
        /// </summary>
        public static void DestroyUserSession()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Metodo estatico que permite recuperar el id de usuario de una sesion abierta 
        /// y que no ha expirado.
        /// </summary>
        /// <returns>El id del usario con sesión abierta</returns>
        public static int GetUser()
        {
            int user_id = 0;
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null)
                {
                    user_id = Convert.ToInt32(ticket.UserData);
                }
            }
            return user_id;
        }

        /// <summary>
        /// Este metodo de la clase SessionHelper crea una cookie de autenticación 
        /// para un usuario, para ello solo necesida su id de usario.
        /// </summary>
        /// <param name="id"></param>
        public static void AddUserToSession(string id)
        {
            //La persistencia con valor true sirve para crear una cookie duradera(una que
            //se guarda en todas las sesiones del navegador. De los contrario el valor es false.
            bool persist = true;
            var cookie = FormsAuthentication.GetAuthCookie("usuario", persist);

            cookie.Name = FormsAuthentication.FormsCookieName;
            //Se establece el tiempo para que la cookie de autenticacion expire
            cookie.Expires = DateTime.Now.AddMonths(3);

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, id);

            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
