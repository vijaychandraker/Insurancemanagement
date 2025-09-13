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
                BindCategoryCompanyGrid();
            }
        }
        private void BindCategoryCompanyGrid()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT 
    c.c_id AS CategoryID,
    c.CategoryName, 
    co.c_id AS CompanyID,
    co.CompanyName, 
    COUNT(ip.PolicyID) AS TotalPolicies
FROM InsurancePolicy ip
INNER JOIN mst_category c ON ip.CategoryID = c.c_id
INNER JOIN mst_Company co ON ip.CompanyID = co.c_id
where ip.IsDeleted = 'NO'
GROUP BY c.c_id, c.CategoryName, co.c_id, co.CompanyName
ORDER BY c.CategoryName, co.CompanyName;
";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            gvCategoryCompany.DataSource = dt;
            gvCategoryCompany.DataBind();
            ViewState["CategoryCompanyData"] = dt; // save for export if needed
        }
        private void BindCompanyChart()
        {
            DataTable dtCompany = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                   SELECT c.CompanyName, COUNT(ip.PolicyID) AS Count, ip.CompanyID
                    FROM   InsurancePolicy AS ip INNER JOIN
                    mst_Company AS c ON ip.CompanyID = c.c_id
                    WHERE ip.IsDeleted = 'NO'
                    GROUP BY c.CompanyName, ip.CompanyID
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
                    SELECT COUNT(ip.PolicyID) AS Count, ip.CategoryID, mst_category.CategoryName
                    FROM   InsurancePolicy AS ip LEFT OUTER JOIN
                    mst_category ON ip.CategoryID = mst_category.c_id
                    WHERE ip.IsDeleted = 'NO'
                    GROUP BY ip.CategoryID, mst_category.CategoryName
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
                string sql = "SELECT COUNT(*) AS totalcount FROM InsurancePolicy ip INNER JOIN mst_Company c ON c.c_id = ip.CompanyID INNER JOIN mst_category cat ON ip.CategoryID = cat.c_id WHERE ip.IsDeleted = 'NO';";
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
                string sql = "SELECT COUNT(DISTINCT OwnerName) AS OwnerCount FROM InsurancePolicy WHERE IsDeleted = 'NO'";
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
                string sql = "SELECT COUNT(*) AS Total FROM InsurancePolicy WHERE IsDeleted = 'NO' AND ExpireDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AND ExpireDate < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));";
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
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ViewState["CompanyData"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowError("No data available for export.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideLoader", "HideLoading();", true);
                    return;
                }
                // Make a copy so we don’t alter the ViewState
                DataTable exportTable = dt.Copy();

                // Remove ID columns if they exist
                if (exportTable.Columns.Contains("CompanyID"))
                    exportTable.Columns.Remove("CompanyID");

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=CompanyWisePolicy.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        GridView gvExport = new GridView();
                        gvExport.DataSource = exportTable;
                        gvExport.DataBind();

                        gvExport.RenderControl(hw);
                        Response.Output.Write(sw.ToString());

                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error exporting to Excel: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "hideLoader", "HideLoading();", true);
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // This is required to allow GridView to render during export
        }
        protected void btnExportCategoryExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ViewState["CategoryData"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowError("No data available for export.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideLoader", "HideLoading();", true);
                    return;
                }

                // Make a copy to avoid modifying the ViewState reference directly
                DataTable exportTable2 = dt.Copy();

                // Remove ID columns if they exist

                if (exportTable2.Columns.Contains("CategoryID"))
                    exportTable2.Columns.Remove("CategoryID");
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=CategoryWisePolicy.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        GridView gvExport = new GridView();
                        gvExport.DataSource = exportTable2;
                        gvExport.DataBind();

                        gvExport.RenderControl(hw);
                        Response.Output.Write(sw.ToString());

                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error exporting category data: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "hideLoader", "HideLoading();", true);
            }
        }

        protected void gvCategoryCompany_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategoryCompany.PageIndex = e.NewPageIndex;

            // Rebind from ViewState (avoid hitting DB again if not needed)
            if (ViewState["CategoryCompanyData"] != null)
            {
                gvCategoryCompany.DataSource = (DataTable)ViewState["CategoryCompanyData"];
                gvCategoryCompany.DataBind();
            }
            else
            {
                BindCategoryCompanyGrid(); // fallback if ViewState is empty
            }
        }

        protected void lnkcompanywiseCategory_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ViewState["CategoryCompanyData"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowError("No data available for export.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideLoader", "HideLoading();", true);
                    return;
                }

                // Make a copy to avoid modifying the ViewState reference directly
                DataTable exportTable = dt.Copy();

                // Remove ID columns if they exist
                if (exportTable.Columns.Contains("CompanyID"))
                    exportTable.Columns.Remove("CompanyID");

                if (exportTable.Columns.Contains("CategoryID"))
                    exportTable.Columns.Remove("CategoryID");

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=CategoryCompanyWisePolicy.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        GridView gvExport = new GridView();
                        gvExport.DataSource = exportTable;
                        gvExport.DataBind();

                        gvExport.RenderControl(hw);
                        Response.Output.Write(sw.ToString());

                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error exporting Category-Company data: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "hideLoader", "HideLoading();", true);
            }
        }


    }
}
