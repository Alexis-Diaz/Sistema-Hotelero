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
    public class HabitacionView
    {
        public HttpPostedFileBase Foto { get; set; }

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
        [DisplayFormat(DataFormatString = "{0:n}")]
        //[DisplayFormat(DataFormatString = "{0:0,0}")]
        [Required(ErrorMessage = "Este dato es necesario")]
        public decimal Precio { get; set; }

        //[Required(ErrorMessage = "Es necesario agregar una cantidad")]
        //public int Cantidad { get; set; }
        [Display(Name = "Foto")]
        public string Imagen { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int IdTipoDeHabitacion { get; set; }
    }
}
