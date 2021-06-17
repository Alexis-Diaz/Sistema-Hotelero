using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHotel.EL.Paginador
{
    public class PaginadorGenerico<Tabla>where Tabla:class
    {
        public int PaginaActual { get; set; }
        public int RegistroPorPagina { get; set; }
        public int TotalRegistro { get; set; }
        public int TotalPagina { get; set; }
        public string BusquedaActual { get; set; }
        public IEnumerable<Tabla> Resultado { get; set; }
    }
}
