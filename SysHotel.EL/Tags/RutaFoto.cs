using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using System.IO;

namespace SysHotel.EL.Tags
{
    public class RutaFoto
    {
        public static string GuardarRutaFoto(HttpPostedFileBase archivo, string folder)
        {
            string path = string.Empty;
            string pic = string.Empty;

            if (archivo != null)
            {
                pic = Path.GetFileName(archivo.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath(folder), pic);
                archivo.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    archivo.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }
            return pic;
        }
    }
}
