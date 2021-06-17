using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SysHotel.EL.Login;

namespace SysHotel.EL
{
    public partial class Permisos
    {
       

        [Key]
        public RolesPermisos IdPermisos { get; set; }

        [Required]
        [StringLength(20)]
        public string Modulo { get; set; }

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }

        public virtual ICollection<RolUsuario> RolUsuario { get; set; }
        
        //constructor
        public Permisos()
        {
            RolUsuario = new List<RolUsuario>();
        }
    }
}
