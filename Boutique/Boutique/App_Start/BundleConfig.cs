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
            bundles.Add(new ScriptBundle("~/bundles/mainJS").Include(
                      "~/Scripts/Jquery/jquery-3.7.0.min.js",
                      "~/Scripts/Jquery/bootstrap.min.js",
                      "~/Scripts/MyJS/myScript.js",
                       "~/Scripts/MyJS/homeScript.js"
                ));
           
            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/myCSS/base.css",
                      "~/Content/myCSS/mystyle.css",
                      "~/Content/myCSS/responsive.css",
                      "~/Content/Icon/font-awesome.css"
                      ));
            // Login page
            bundles.Add(new StyleBundle("~/bundles/loginCSS").Include(
                "~/Content/Icon/font-awesome.min.css",
                "~/Content/myCSS/login.css"
           ));
            // Cart page
            bundles.Add(new StyleBundle("~/bundles/cartCSS").Include(
                      "~/Content/Bootstrap/bootstrap.min.css",
                      "~/Content/Icon/font-awesome.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/cartJS").Include(
                  "~/Scripts/MyJS/cartScript"
            ));
            //Admin page
            bundles.Add(new ScriptBundle("~/bundles/adminjs").Include(
                     "~/Areas/Admin/Assets/js/jquery-3.2.1.min.js",
                     "~/Areas/Admin/Assets/js/popper.min.js",
                     "~/Areas/Admin/Assets/js/bootstrap.min.js",
                     "~/Areas/Admin/Assets/js/main.js",
                     "~/Areas/Admin/Assets/js/plugins/pace.min.js",
                     "~/Areas/Admin/Assets/js/plugins/chart.js"
               ));
            bundles.Add(new StyleBundle("~/bundles/adminCSS").Include(
                    "~/Areas/Admin/Assets/css/main.css",
                    "~/Areas/Admin/Assets/css/style.css",
                    "~/Areas/Admin/Assets/css/util.css"
                    ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
