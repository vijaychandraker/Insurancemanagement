using System;
using System.Data;                // Required for DataTable
using System.Data.SqlClient;
using System.Configuration;
using Dbord.helpers;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dbord.View.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        // Get connection string from Web.config
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCompanyChart();
                BindCategoryChart();
                BindTotalPolicyCard();
                Bindmorethenone();
                    BindGrid();
            }
        }

        private void BindCompanyChart()
        {
            // Create a DataTable to store company-wise policy counts
            System.Data.DataTable dtCompany = new System.Data.DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT c.CompanyName, COUNT(ip.PolicyID) AS Count
                    FROM InsurancePolicy ip
                    INNER JOIN mst_Company c ON ip.CompanyID = c.c_id
                    GROUP BY c.CompanyName
                    ORDER BY Count DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtCompany);
                }
            }
         // Store data in ViewState for use in ASPX Chart.js script
            ViewState["CompanyData"] = dtCompany;
        }
        private void BindCategoryChart()
        {
            // Create a DataTable to store company-wise policy counts
            System.Data.DataTable dtcategory = new System.Data.DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT c.CategoryName, COUNT(ip.PolicyID) AS Count
                    FROM   InsurancePolicy AS ip INNER JOIN
                    mst_category AS c ON ip.CategoryID = c.c_id
                    GROUP BY c.CategoryName
                    ORDER BY Count DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtcategory);
                }
            }
            // Store data in ViewState for use in ASPX Chart.js script
            ViewState["CategoryData"] = dtcategory;
        }

        private void BindTotalPolicyCard()
        {
            int totalowner = 0; // default value

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = "SELECT COUNT(PolicyID) FROM InsurancePolicy";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    object resultowner = cmd.ExecuteScalar();
                    if (resultowner != null && resultowner != DBNull.Value)
                    {
                        totalowner = Convert.ToInt32(resultowner);
                    }
                }
            }

            // Store the total in ViewState or directly bind to a Label on the card
            lbltotal.Text = totalowner.ToString();
        }
        private void Bindmorethenone()
        {
            int totalPolicies = 0; // default value

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = "SELECT COUNT(DISTINCT OwnerName) AS OwnerCount FROM   InsurancePolicy";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalPolicies = Convert.ToInt32(result);
                    }
                }
            }

            // Store the total in ViewState or directly bind to a Label on the card
            lblowner.Text = totalPolicies.ToString();
        }

        private void BindGrid(string searchText = "")
        {
            try
            {
                DataTable dt = db.ExecuteQuery("sp_GetAllPolicies", null);

                // 🔎 Apply search filter
                if (!string.IsNullOrEmpty(searchText))
                {
                    string filter = $"Convert(PolicyNo, 'System.String') LIKE '%{searchText}%' OR " +
                                    $"Name LIKE '%{searchText}%' OR " +
                                    $"Convert(VehicleNo, 'System.String') LIKE '%{searchText}%' OR " +
                                    $"CompanyName LIKE '%{searchText}%' OR " +
                                    $"CategoryName LIKE '%{searchText}%'";
                    DataRow[] filtered = dt.Select(filter);
                    dt = filtered.Length > 0 ? filtered.CopyToDataTable() : dt.Clone();
                }

                gvdashboard.DataSource = dt;
                gvdashboard.DataBind();

                // ✅ Save filtered data in ViewState
                ViewState["GridDatadashboard"] = dt;
            }
            catch (Exception ex)
            {
                ShowError("Error loading data: " + ex.Message);
            }
        }
        private void ShowError(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $@"
                Swal.fire({{
                    icon: 'error',
                    title: 'Error',
                    text: '{message}'
                }});", true);
        }
        protected void gvdashboard_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvdashboard.PageIndex = e.NewPageIndex;
            RebindFromViewState();
        }

        private void RebindFromViewState()
        {
            if (ViewState["GridDatadashboard"] != null)
            {
                DataTable dt = (DataTable)ViewState["GridDatadashboard"];
                gvdashboard.DataSource = dt;
                gvdashboard.DataBind();
            }
            else
            {
                BindGrid();
            }
        }

        protected void btnSearch_dash_Click(object sender, EventArgs e)
        {
            BindGrid(txtSearch.Text.Trim());
        }

        protected void btnClearSearch_dash_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            BindGrid();
        }


    }
}
