using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SysHotel.EL.View
{
    public class AlimentoView
    {
        public HttpPostedFileBase Foto { get; set; }

        public int IdAlimento { get; set; }

        [Required(ErrorMessage = "Este campo se debe completar")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo se debe completar")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Este campo se debe completar")]
        public decimal Precio { get; set; }


        public string Imagen { get; set; }

        [Required]
        public int Estado { get; set; }

        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "Este dato es requerido")]
        public int IdProveedor { get; set; }

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "Este dato es requerido")]
        public int IdCategoriaAlimento { get; set; }
    }
}
