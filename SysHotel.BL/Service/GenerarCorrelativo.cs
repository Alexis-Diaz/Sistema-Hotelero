using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;

namespace SysHotel.BL.Service
{
    public class GenerarCorrelativo
    {
        private FacturaDAL facturaDAL = new FacturaDAL();
        /// <summary>
        /// Genera un codigo con un numero correlativo para un documento.
        /// Recibe un parámetros que sirve como 
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns>Una cadena que contiene el nuevo codigo correlativo para un documento</returns>
        public async Task<string> GenerarNumeroCorrelativoDeFactura(string codigo)
        {
            //Buscamos la última factura ingresada
            Factura ultimaFactura = await facturaDAL.BuscarUltimaFacturaIngresada();

            if (ultimaFactura != null)
            {
                //Capturas el numero de la factura
                string NumeroFactura;
                NumeroFactura = ultimaFactura.NumeroFactura;

                //Extraemos una cadena solo con los numeros correlativos.
                int ExtensionCodigo = codigo.Length;
                string CorrelativoFactura = NumeroFactura.Substring(ExtensionCodigo--);

                //Convertimos a numero la cadena de numeros.
                int Numero = Convert.ToInt32(CorrelativoFactura);

                //1 a 9
                if (Numero > 0 && Numero < 9)
                {
                    Numero++;
                    NumeroFactura = codigo + "000" + Numero;
                }   //10 a 99
                else if (Numero >= 9 && Numero < 99)
                {
                    Numero++;
                    NumeroFactura = codigo + "000" + Numero;
                }   //100 a 999
                else if (Numero >= 99 && Numero < 999)
                {
                    Numero++;
                    NumeroFactura = codigo + "000" + Numero;
                }   //1000 a 10000
                else if (Numero >= 999 && Numero < 10000)
                {
                    Numero++;
                    NumeroFactura = codigo + Numero;
                }
                return NumeroFactura;
            }
            return codigo + "0001";
        }
    }
}
