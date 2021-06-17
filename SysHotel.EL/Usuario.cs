using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SysHotel.EL.Login;

namespace SysHotel.EL
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(50)]
        [Display(Name = "Apellido")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]//Se usa para que el calendario absorva la fecha y la muestre en el modo editar
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(100)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(15)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(10)]
        public string DUI { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(25)]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(25)]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        [Display(Name = "Rol Usuario")]
        public int IdRolUsuario { get; set; }
        public virtual RolUsuario rolUsuario { get; set; }//llave foranea

        public virtual ICollection<Reservacion> Reservacions { get; set; }//Llave primaria
        public virtual ICollection<Comentario> Comentarios { get; set; }//Llave primaria
        //public virtual ICollection<Factura> Facturas { get; set; }//Llave primaria

        /// <summary>
        /// Este metodo sirve para autenticar un usuario que ya existe en la base
        /// de datos. Simplemente verifica que el usuario exista y crea una cookie
        /// de autenticacion. En caso de no existir un usuario lo deniega el acceso
        /// al sistema.
        /// </summary>
        /// <returns>Retorna un objeto de la clase ResponseModel</returns>
        public ResponseModel Autenticarse()
        {
            var rm = new ResponseModel();

            try
            {
                using (var ctx = new BDComun())
                {                                                           
                    var usuario = ctx.Usuarios.Where(x => x.NombreUsuario == this.NombreUsuario && x.Contraseña == this.Contraseña).SingleOrDefault();
                    if (usuario != null)
                    {
                        string id = Convert.ToString(usuario.IdUsuario);

                        SessionHelper.AddUserToSession(id);
                        rm.SetResponse(true);
                    }
                    else
                    {
                        rm.SetResponse(false, "Acceso denegado al sistema");
                    }
                }
            }
            catch (Exception e)
            {
                rm.SetResponse(false, "Error crítico, perdona los inconvenientes.");
            }
            return rm;
        }

        /// <summary>
        /// Busca un usario por medio de su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Usuario Obtener(int id)
        {
            var usuario = new Usuario();

            try
            {
                using (var ctx = new BDComun())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    usuario = ctx.Usuarios.Include("IdRolUsuario")
                                         .Include("IdRolUsuario.Permisos")
                                         .Where(x => x.IdUsuario == id).SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                throw;//controloar el error cuando la base de datos no responda o el servicion no este iniciado
            }

            return usuario;
        }
    }
}
