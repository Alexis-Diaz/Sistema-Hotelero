using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Detalle
    {
        [Key]
        public int IdDetalle { get; set; }

        [Required(ErrorMessage = "Es necesario seleccionar una fecha")]
        public DateTime Dia { get; set; }

        //[Required(ErrorMessage = "Es necesario seleccionar una hora")]
        //public DateTime Hora { get; set; }

        [Required(ErrorMessage = "Es necesario escoger un tiempo")]
        [StringLength(10)]
        public string TiempoDeComida { get; set; }

        [Required(ErrorMessage = "Es necesario ingresar una cantidad")]
        public int Cantidad { get; set; }

        [Required]
        public decimal TotalDetalle { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        public int IdReservacion { get; set; }
        public virtual Reservacion reservacion { get; set; }

        [Required]
        public int IdAlimento { get; set; }
        public virtual Alimento alimento { get; set; }
    }
}
