using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL.View
{
    public class FacturaView<Tabla> where Tabla:class
    {
        [Required(ErrorMessage = "Este valor es obligatorio")]
        public int IdFactura { get; set; }

        [Required(ErrorMessage = "Este valor es requerido")]
        [Display(Name = "Nº Factura")]
        public string NumeroFactura { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public string NumeroDocumento { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public string NombreCompleto { get; set; }

        //[Required(ErrorMessage = "Este valor es obligatorio")]
        //public string ApellidoCliente { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public string DireccionCliente { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de emisión")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public string Vendedor { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public decimal SubtotalProducto { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public decimal SubtotalFactura { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        [Display(Name = "IVA")]
        public decimal IVA { get; set; }

        [Required(ErrorMessage = "Este valor es obligatorio")]
        public decimal TotalFactura { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        public int IdReservacion { get; set; }

        public IEnumerable<Tabla> Detalle { get; set; }
    }
}
