using System.Web.Optimization;

namespace CMS.API
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/ui.jqgrid.css",
                "~/Content/jquery-ui.css",
                "~/Content/jBoxReal.css",
                "~/Content/font-awesome.min.css",
                "~/Content/common.css",
                "~/Content/jBox.css",
                "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/css/schedule").Include(
                "~/Content/schedule/jquery-ui-custom-1.11.2.min.css",
                "~/Content/schedule/calenstyle.css",
                "~/Content/schedule/calenstyle-jquery-ui-override.css",
                "~/Content/schedule/calenstyle-iconfont.css",
                "~/Content/schedule/CalEventList.css"));

            bundles.Add(new ScriptBundle("~/bundles").Include(
                "~/Scripts/jquery-3.3.1.min.js",
                "~/Scripts/jquery-ui-1.12.1.min.js",
                "~/Scripts/grid.locale-en.js",
                "~/Scripts/jquery.jqGrid.min.js",
                "~/Scripts/jBox.min.js",
                "~/Scripts/jquery-ias.min.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/cleave.min.js",
                "~/Scripts/datepicker.js",
                "~/Scripts/datepicker-ko.js",
                "~/Scripts/common.js",
                "~/Scripts/cms-common.js"));

            bundles.Add(new ScriptBundle("~/bundles/schedule").Include(
                //"~/Scripts/schedule/jquery-ui-custom-1.11.2.min.js",
                "~/Scripts/schedule/calenstyle.js",
                "~/Scripts/Views/manage-schedule.js"));
        }
    }
}