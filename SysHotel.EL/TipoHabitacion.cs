using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class TipoHabitacion
    {
        [Key]
        public int IdTipoDeHabitacion { get; set; }

        [Required(ErrorMessage = "Es necesario agregar un tipo de habitacion")]
        [Display(Name = "Tipo de habitación")]
        [StringLength(50)]
        public string TipoDeHabitacion { get; set; }

        [Required(ErrorMessage = "Es necesario agregar una descripción")]
        [Display(Name = "Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required]
        public int Estado { get; set; }

        public virtual ICollection<Habitacion> Habitacions { get; set; }//Llave primaria
    }
}
