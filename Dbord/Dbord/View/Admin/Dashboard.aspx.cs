using System;
using System.Data;
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
                BindonmentExp();
            }
        }

        private void BindCompanyChart()
        {
            DataTable dtCompany = new DataTable();
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
            ViewState["CompanyData"] = dtCompany;
        }

        private void BindCategoryChart()
        {
            DataTable dtcategory = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT c.CategoryName, COUNT(ip.PolicyID) AS Count
                    FROM   InsurancePolicy AS ip 
                    INNER JOIN mst_category AS c ON ip.CategoryID = c.c_id
                    GROUP BY c.CategoryName
                    ORDER BY Count DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtcategory);
                }
            }
            ViewState["CategoryData"] = dtcategory;
        }

        private void BindTotalPolicyCard()
        {
            int totalowner = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(PolicyID) FROM InsurancePolicy";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    object resultowner = cmd.ExecuteScalar();
                    if (resultowner != null && resultowner != DBNull.Value)
                        totalowner = Convert.ToInt32(resultowner);
                }
            }
            lbltotal.Text = totalowner.ToString();
        }

        private void Bindmorethenone()
        {
            int totalPolicies = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(DISTINCT OwnerName) AS OwnerCount FROM InsurancePolicy";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        totalPolicies = Convert.ToInt32(result);
                }
            }
            lblowner.Text = totalPolicies.ToString();
        }


        private void BindonmentExp()
        {
            int totalExp = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) AS Total FROM   InsurancePolicy WHERE (ExpireDate BETWEEN GETDATE() AND DATEADD(MONTH, 1, GETDATE()))";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        totalExp = Convert.ToInt32(result);
                }
            }
            lblexpired.Text = totalExp.ToString();
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

                if (dt.Rows.Count > 0)
                {
                    lblMessage.Visible = false;
                    SetFooterTotal(dt.Rows.Count);
                }
                else
                {
                    lblMessage.Text = "No data found.";
                    lblMessage.Visible = true;
                }

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
                if (dt.Rows.Count > 0)
                    SetFooterTotal(dt.Rows.Count);
            }
            else
            {
                BindGrid();
            }
        }

        // ✅ Footer showing only total record count
        private void SetFooterTotal(int total)
        {
            if (gvdashboard.FooterRow != null)
            {
                gvdashboard.FooterRow.Cells[0].Text = "Total Records: " + total;
                gvdashboard.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Center;

                // Merge all columns into one cell
                gvdashboard.FooterRow.Cells[0].ColumnSpan = gvdashboard.Columns.Count;

                // Hide remaining cells
                for (int i = 1; i < gvdashboard.FooterRow.Cells.Count; i++)
                    gvdashboard.FooterRow.Cells[i].Visible = false;

                // Style footer
                gvdashboard.FooterRow.Font.Bold = true;
                gvdashboard.FooterRow.BackColor = System.Drawing.Color.LightGray;
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
        protected void gvdashboard_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].ColumnSpan = gvdashboard.Columns.Count;
                for (int i = 1; i < gvdashboard.Columns.Count; i++)
                {
                    e.Row.Cells[i].Visible = false;
                }
                e.Row.Cells[0].Text = "Total Records: " + gvdashboard.Rows.Count;
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
            }
        }
    }
}
