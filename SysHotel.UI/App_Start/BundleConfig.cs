using System.Web;
using System.Web.Optimization;

namespace SysHotel.UI.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //archivos css
            bundles.Add(new StyleBundle("~/content/css").Include("~/Content/Css/materialize.css",
                                                                 "~/Content/Css/style.css",
                                                                 "~/Content/Css/factura.css"));
            //archivos js
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Content/Script/jquery-{versión}.js"));
            bundles.Add(new ScriptBundle("~/bundles/materialize").Include("~/Content/Script/materialize.js"));
            bundles.Add(new ScriptBundle("~/bundles/hotel").Include("~/Content/Script/layout.js",
                                                                    "~/Content/Script/indexLogin.js"));
        }

    }
}