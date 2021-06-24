using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Reservacion
    {
        [Key]
        public int IdReservacion { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Check In (Entrada)")]
        public DateTime DiaEntrada { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Check Out (Salida)")]
        public DateTime DiaSalida { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de reservación")]
        public DateTime FechaReservacion { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Personas")]
        public int NumeroPersonas { get; set; }

        [StringLength(500)]
        [Display(Name = "Comentario")]
        public string Comentarios { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }
        public virtual Cliente cliente { get; set; }

        [Required]
        [Display(Name = "Número de habitación")]
        public int IdHabitacion { get; set; }
        public virtual Habitacion habitacion { get; set; }

        [Required]
        public int IdUsuario { get; set; }
        public virtual Usuario usuario { get; set; }

        public virtual ICollection<Detalle> Detalles { get; set; }
        public virtual ICollection<Factura> Facturas { get; set; }
    }
}
