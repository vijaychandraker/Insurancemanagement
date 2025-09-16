using System;
using System.Web.UI;

namespace Dbord.Helpers
{
    public static class AlertHelper
    {
        public static void ShowSuccess(Page page, string message)
        {
            ScriptManager.RegisterStartupScript(page, page.GetType(), "successAlert", $@"
                Swal.fire({{
                    icon: 'success',
                    title: 'Success',
                    text: '{message}'
                }});", true);
        }

        public static void ShowError(Page page, string message)
        {
            ScriptManager.RegisterStartupScript(page, page.GetType(), "errorAlert", $@"
                Swal.fire({{
                    icon: 'error',
                    title: 'Error',
                    text: '{message}'
                }});", true);
        }

    }
}
