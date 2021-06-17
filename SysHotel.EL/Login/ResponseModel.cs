using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHotel.EL.Login
{
	/// <summary>
	/// La clase ResponseModel tiene dos propiedades y un metodo.
	/// Sirve para devolver una respuesta al usuario que intenta iniciar una sesion.
	/// </summary>
	public class ResponseModel
    {
		public bool response { get; set; }
		public string message { get; set; }

		/// <summary>
		/// ResponseModel constructor pertenece a la clase ResponseModel que tiene dos propiedades y un metodo.
		/// Sirve para devolver una respuesta al usuario que intenta iniciar una sesion.
		/// </summary>
		public ResponseModel()
		{
			this.response = false;
			this.message = "Ocurrio un error inesperado";
		}

		/// <summary>
		/// Este metodo sirve para establecer si el usuario tiene
		/// permiso para ingresar al sistema (true) o no (false).
		/// r = booleano donde true es permitido y false es denegado.
		/// m = string es el mensaje que se le puede enviar al usuario
		/// </summary>
		/// <param name="r"></param>
		/// <param name="m"></param>
		public void SetResponse(bool r, string m = "")
		{
			this.response = r;
			this.message = m;

			if (!r && m == "") this.message = "Ocurrio un error inesperado";
		}
	}
}
