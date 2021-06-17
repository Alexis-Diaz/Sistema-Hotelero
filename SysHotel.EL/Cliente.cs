using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Es necesario escribir un nombre")]
        [StringLength(30)]
        [Display(Name = "Nombre")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Es necesario escribir un Apellido")]
        [StringLength(30)]
        [Display(Name = "Apellido")]
        public string Apellidos { get; set; }


        [Required(ErrorMessage = "Es necesario ingresar la Fecha de nacimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Es necesario el tipo de documento")]
        [StringLength(10)]
        [Display(Name = "Documento")]
        public string TipoDocumento { get; set; }

        [Required(ErrorMessage = "Es necesario escribir numero de documento")]
        [StringLength(15)]
        [Display(Name = "Número de documento")]
        public string NumeroDocumento { get; set; }

        [Required(ErrorMessage = "Es necesario ingresar un número de telefono")]
        [StringLength(15)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Es necesario escribir un correo")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Es necesario escribir una dirección")]
        [StringLength(100)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        public int Estado { get; set; }

        public virtual ICollection<Reservacion> Reservacions { get; set; }//Llave primaria
    }
}
