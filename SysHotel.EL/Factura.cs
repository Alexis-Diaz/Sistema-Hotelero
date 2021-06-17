using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Factura
    {
        [Key]
        public int IdFactura { get; set; }

        [Required(ErrorMessage = "Este valor es requerido")]
        [Display(Name = "Nº Factura")]
        public string NumeroFactura { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de emisión")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        [Display(Name = "IVA")]
        public decimal IVA { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public decimal SubTotal { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public decimal TotalFactura { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        public int IdReservacion { get; set; }
        public virtual Reservacion reservacion { get; set; }

        //[Required]
        //public int IdUsuario { get; set; }
        //public virtual Usuario usuario { get; set; }
    }
}
