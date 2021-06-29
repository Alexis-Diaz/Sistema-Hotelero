using System.Web;
using System.Web.Optimization;

namespace SysHotel.UI.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //archivos css
            bundles.Add(new StyleBundle("~/bundles/css").Include("~/content/css/materialize.min.css",
                                                                 "~/content/css/style.css",
                                                                 "~/content/css/factura.css"));
            //archivos js
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/content/script/jquery-{versión}.js"));
            bundles.Add(new ScriptBundle("~/bundles/materialize").Include("~/content/script/materialize.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/hotel").Include("~/content/script/layout.js",
                                                                    "~/content/script/indexLogin.js"));
        }

    }
}