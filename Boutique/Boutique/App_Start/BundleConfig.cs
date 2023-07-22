using System.Web;
using System.Web.Optimization;

namespace Boutique
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //main
            bundles.Add(new ScriptBundle("~/bundles/mainjs").Include(
                      "~/Scripts/jquery-3.4.1.js",
                      "~/Scripts/myscript.js",
                      "~/Scripts/bootstrap.min.js"
                ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                       //"~/Content/bootstrap.css",
                       //"~/Content/site.css"
                       "~/Content/base.css",
                      "~/Content/mystyle.css",
                      "~/Content/responsive.css"
                      ));
            //login
            bundles.Add(new ScriptBundle("~/bundles/loginjs").Include(
                    "~/Content/login/jquery.min.js",
                    "~/Content/login/bootstrap.min.js",
                    "~/Content/login/main.js"
              ));
       
            //admin
            bundles.Add(new ScriptBundle("~/bundles/adminjs").Include(
                     "~/Areas/Admin/Assets/js/jquery-3.2.1.min.js",
                     "~/Areas/Admin/Assets/js/popper.min.js",
                     "~/Areas/Admin/Assets/js/bootstrap.min.js",
                     "~/Areas/Admin/Assets/js/main.js",
                     "~/Areas/Admin/Assets/js/plugins/pace.min.js",
                     "~/Areas/Admin/Assets/js/plugins/chart.js"
               ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
