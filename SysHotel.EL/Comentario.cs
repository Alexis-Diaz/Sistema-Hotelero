using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SysHotel.EL
{
    public class Comentario
    {
        [Key]
        public int IdComentario { get; set; }

        [Required]
        public string Nota { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        public int IdUsuario { get; set; }
        public virtual Usuario usuario { get; set; }
    }
}
