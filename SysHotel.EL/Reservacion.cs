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

    public class ReservacionView 
    {
        public int IdReservacion { get; set; }
        public int IdHabitacion { get; set; }
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string FechaReservacion { get; set; }
        public string DiaEntrada { get; set; }
        public string DiaSalida { get; set; }
        public int NumeroPersonas { get; set; }
        public string Comentarios { get; set; }
        public int Estado { get; set; }

        public ReservacionView(int pIdReservacion, int pIdHabitacion, int pIdCliente, int pIdUsuario, string pFechaReservacion, string pDiaEntrada, string pDiaSalida, int pNumeroPersonas, string pComentarios, int pEstado)
        {
            this.IdReservacion = pIdReservacion;
            this.IdHabitacion = pIdHabitacion;
            this.IdCliente = pIdCliente;
            this.IdUsuario = pIdUsuario;
            this.FechaReservacion = pFechaReservacion;
            this.DiaEntrada = pDiaEntrada;
            this.DiaSalida = pDiaSalida;
            this.NumeroPersonas = pNumeroPersonas;
            this.Comentarios = pComentarios;
            this.Estado = pEstado;
        }
        public ReservacionView()
        {

        }
    }
}
