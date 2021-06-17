using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace SysHotel.EL
{
    public class Alimento
    {
        [Key]
        public int IdAlimento { get; set; }

        [Required(ErrorMessage = "Esta campo se debe completar")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Esta campo se debe completar")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Esta campo se debe completar")]
        public decimal Precio { get; set; }


        public string Imagen { get; set; }

        [Required]
        public int Estado { get; set; }

        public virtual Proveedor proveedor { get; set; }
        [Required]
        public int IdProveedor { get; set; }

        public virtual CategoriaAlimento categorialimento { get; set; }//Llave foranea
        [Required(ErrorMessage = "Este dato es requerido")]
        public int IdCategoriaAlimento { get; set; }


        public virtual ICollection<Detalle> Detalles { get; set; }//Llave primaria
    }
}
