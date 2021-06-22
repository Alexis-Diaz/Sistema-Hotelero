using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHotel.EL.Tags
{
    /// <summary>
    /// Esta clase solo me sirve para enviar
    /// los estados a la vista index del controlardor
    /// resercacion.
    /// </summary>
    public class EstadosReservacion
    {
        public int NumeroEstado { get; set; }
        public string Estado { get; set; }
    }
}
