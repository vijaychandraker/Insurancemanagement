using System.Web.Optimization;

namespace Dbord
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // =====================
            // CORE CSS
            // =====================
            var css = new StyleBundle("~/bundles/css")
                .Include("~/Assets/Admin/fontawesome-free/css/all.min.css", new CssRewriteUrlTransform())
                .Include("~/Assets/Admin/tempusdominus-bootstrap-4/css/tempusdominus-bootstrap-4.min.css", new CssRewriteUrlTransform())
                .Include("~/Assets/Admin/icheck-bootstrap/icheck-bootstrap.min.css")
                .Include("~/Assets/Admin/jqvmap/jqvmap.min.css", new CssRewriteUrlTransform())
                .Include("~/Assets/dist/css/adminlte.min.css", new CssRewriteUrlTransform())
                .Include("~/Assets/Admin/overlayScrollbars/css/OverlayScrollbars.min.css", new CssRewriteUrlTransform())
                .Include("~/Assets/Admin/summernote/summernote-bs4.min.css", new CssRewriteUrlTransform())
                .Include("~/Assets/Scripts/sweetalert.css")
                .Include("~/Assets/css/glassmorphism.css");
            bundles.Add(css);

            // =====================
            // jQuery (standalone bundle to force first load)
            // =====================
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Assets/Admin/jquery/jquery.min.js"
            ));

            // =====================
            // CORE JS (jQuery FIRST!)
            // =====================
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Assets/Admin/jquery-ui/jquery-ui.min.js",
                "~/Assets/Admin/bootstrap/js/bootstrap.bundle.min.js",
                "~/Assets/Admin/overlayScrollbars/js/jquery.overlayScrollbars.min.js",
                "~/Assets/dist/js/adminlte.min.js"
            ));

            // =====================
            // DATATABLES JS + CSS (Optional, use only on needed pages)
            // =====================
            bundles.Add(new StyleBundle("~/bundles/datatables-css").Include(
                "~/Assets/plugins/datatables-bs4/css/dataTables.bootstrap4.min.css",
                "~/Assets/plugins/datatables-responsive/css/responsive.bootstrap4.min.css",
                "~/Assets/plugins/datatables-buttons/css/buttons.bootstrap4.min.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datatables-js").Include(
                "~/Assets/plugins/datatables/jquery.dataTables.min.js",
                "~/Assets/plugins/datatables-bs4/js/dataTables.bootstrap4.min.js",
                "~/Assets/plugins/datatables-responsive/js/dataTables.responsive.min.js",
                "~/Assets/plugins/datatables-responsive/js/responsive.bootstrap4.min.js",
                "~/Assets/plugins/datatables-buttons/js/dataTables.buttons.min.js",
                "~/Assets/plugins/datatables-buttons/js/buttons.bootstrap4.min.js",
                "~/Assets/plugins/jszip/jszip.min.js"
            ));

            // Force bundling in debug mode
            BundleTable.EnableOptimizations = true;
        }
    }
}
