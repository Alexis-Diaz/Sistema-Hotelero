using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public partial class RolUsuario
    {
        [Key]
        public int IdRolUsuario { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50)]
        public string Rol { get; set; }

        [Required]
        public int Estado { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }//llave primaria
        public virtual ICollection<Permisos> Permisos { get; set; }//Laves primaria

        //constructor
        public RolUsuario()
        {
            Usuarios = new List<Usuario>();
            Permisos = new List<Permisos>();
        }
    }
}
