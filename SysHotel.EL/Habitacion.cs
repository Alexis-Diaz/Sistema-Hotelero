using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Habitacion
    {
        [Key]
        public int IdHabitacion { get; set; }

        [Required(ErrorMessage = "Este dato es necesario")]
        [Display(Name = "Habitación N°")]
        public int NumeroHabitacion { get; set; }

        [Required(ErrorMessage = "Este dato es necesario")]
        [StringLength(100)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Este dato es necesario")]
        [Display(Name = "Camas")]
        public int NumeroCamas { get; set; }

        [Required(ErrorMessage = "Este dato es necesario")]
        [Display(Name = "TV cable")]
        public bool TVCable { get; set; }

        [Required(ErrorMessage = "Este dato es necesario")]
        [Display(Name = "WIFI")]
        public bool Wifi { get; set; }

        [Required(ErrorMessage = "Este dato es necesario")]
        [Display(Name = "Aire acondicionado")]
        public bool AireAcondicionado { get; set; }

        //[DisplayFormat(DataFormatString = "{C}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:n}")]
        //[DisplayFormat(DataFormatString = "{0:0,0}")]
        [Required(ErrorMessage = "Este dato es necesario")]
        public decimal Precio { get; set; }

        [Display(Name = "Foto")]
        public string Imagen { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int IdTipoDeHabitacion { get; set; }
        public virtual TipoHabitacion TipoDeHabitacion { get; set; }//Llave foranea

        public virtual ICollection<Reservacion> Reservacions { get; set; }//Llave primaria
    }
}
