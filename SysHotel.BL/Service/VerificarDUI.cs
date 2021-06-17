using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHotel.BL.Service
{
    public static class VerificarDUI
    {
        /// <summary>
        /// Verifica que el número de DUI sea correcto
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="numero"></param>
        /// <returns>Un entero, donde:
        /// 1: número tiene letras, 2: número correcto, 3: DUI inválido, 4: número no tiene 9 digitos, 5: el documento no es DUI</returns>
        public static int VerificarNumeroDeDUI(string documento, string numero)
        {
            //Verificacion de DUI
            if (documento.ToLower() == "dui")
            {
                if (numero.Any(x => !char.IsNumber(x)))
                {
                    return 1;//solo deben ser numeros
                }
                else
                {
                    int x = numero.Length;
                    if (x == 9)
                    {
                        int suma = 0;
                        int posicion = 9;
                        for (int i = 0; i < x - 1; i++)
                        {
                            int digito = Convert.ToInt32(numero.Substring(i, 1));
                            int multiplicar = digito * posicion;
                            suma += multiplicar;
                            posicion--;
                        }

                        int modulo = suma % 10;
                        int verificacion = 10 - modulo;
                        int verificar = Convert.ToInt32(numero.Substring(8, 1));
                        if (verificacion == verificar || verificacion == 0)
                        {
                            return 2;//DUI correcto
                        }
                        else
                        {
                            return 3; //El Numero de DUI es incorrecto
                        }
                    }
                    else
                    {
                        return 4;//la cantidad de numeros del DUI no es 9
                    }
                }
            }
            return 5;//No es DUI
        }
    }
}
