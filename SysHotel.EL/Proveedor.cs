using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50)]
        public string NombreEmpresa { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100)]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50)]
        public string Encargado { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(15)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Required]
        public int Estado { get; set; }

        public virtual ICollection<Alimento> alimento { get; set; }//Llave primaria
    }
}
