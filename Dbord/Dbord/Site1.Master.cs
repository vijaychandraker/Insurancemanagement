using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dbord
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Session["Username"] != null)
                { 
                lblusername.Text = Session["Username"].ToString();
                if (Session["RoleID"] != null)
                {
                    string roleId = Session["RoleID"].ToString();

                    // Hide all menus by default
                    lnkDashboard.Visible = false;
                    lnkUser.Visible = false;
                    lnkShowData.Visible = false;
                    lnkReport.Visible = false;

                    // Example role-based logic
                    switch (roleId)
                    {
                        case "1": // Admin
                            lnkDashboard.Visible = true;
                            lnkShowData.Visible = true;
                            lnkReport.Visible = true;
                            break;

                        case "2": // Data Entry User
                            lnkDashboard.Visible = true;
                            lnkUser.Visible = true;
                            lnkReport.Visible = true;
                            break;
                    }
                }
                else
                {
                    // If no role, redirect to login
                    Response.Redirect("~/login/Login.aspx");
                }
            }
                else
                {
                    // If no role, redirect to login
                    Response.Redirect("~/login/Login.aspx");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear all session variables
            Session.Clear();

            // Abandon the session
            Session.Abandon();

            // Redirect to login page
            Response.Redirect("~/login/Login.aspx");
        }
    }
}