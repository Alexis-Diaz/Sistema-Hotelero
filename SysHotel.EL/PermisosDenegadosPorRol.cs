using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class PermisosDenegadosPorRol
    {
        [Key]
        public int IdPermisosDeneadosPorId { get; set; }

        [Required]
        public virtual Permisos Permisos { get; set; }

        [Required]
        public virtual RolUsuario RolUsuario { get; set; }
    }
}
