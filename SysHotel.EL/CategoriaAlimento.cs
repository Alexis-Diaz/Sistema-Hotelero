using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class CategoriaAlimento
    {
        [Key]
        public int IdCategoriaAlimento { get; set; }

        [Required(ErrorMessage = "Es necesario escribir una categoria")]
        [StringLength(50)]
        [Display(Name = "Categoria")]
        public string NombreCategoria { get; set; }

        [Required(ErrorMessage = "Es necesario agregar una descripción")]
        [StringLength(100)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        public int Estado { get; set; }

        public virtual ICollection<Alimento> Alimentos { get; set; }//Llave primaria
    }
}
