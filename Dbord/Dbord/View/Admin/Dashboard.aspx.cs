using System;
using System.Data;
using System.Data.SqlClient;
using Dbord.helpers;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dbord.View.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private const int PageSize = 5; // Number of rows per page

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCharts();
                BindCards();
                BindCategoryCompanyGrid();
                BindGrid(0, ""); // Load first page
            }
        }

        #region Grid Bindings

        private void BindGrid(int pageIndex, string searchText)
        {
            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", PageSize),
                    new SqlParameter("@SearchText", string.IsNullOrEmpty(searchText) ? DBNull.Value : (object)searchText),
                    new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                DataTable dt = db.ExecuteQuery("sp_GetAllPolicies_Paged", parameters);

                int totalRecords = (int)parameters[3].Value;
                Session["TotalRecords"] = totalRecords;

                gvdashboard.DataSource = dt;
                gvdashboard.PageSize = PageSize;
                gvdashboard.VirtualItemCount = totalRecords;
                gvdashboard.DataBind();

                lblMessage.Visible = dt.Rows.Count == 0;
                lblMessage.Text = dt.Rows.Count == 0 ? "No data found." : "";
            }
            catch (Exception ex)
            {
                ShowError("Error loading data: " + ex.Message);
            }
        }

        protected void gvdashboard_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvdashboard.PageIndex = e.NewPageIndex;
            BindGrid(e.NewPageIndex, txtSearch.Text.Trim());
        }

        #endregion

        #region Category-Company Grid

        private void BindCategoryCompanyGrid()
        {
            DataTable dt = db.ExecuteQuery("sp_GetCategoryCompanyPolicies", null);
            gvCategoryCompany.DataSource = dt;
            gvCategoryCompany.DataBind();
            Session["CategoryCompanyData"] = dt;
        }

        protected void gvCategoryCompany_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategoryCompany.PageIndex = e.NewPageIndex;

            if (Session["CategoryCompanyData"] != null)
            {
                gvCategoryCompany.DataSource = (DataTable)Session["CategoryCompanyData"];
                gvCategoryCompany.DataBind();
            }
            else
            {
                BindCategoryCompanyGrid();
            }
        }

        #endregion

        #region Charts & Cards

        private void BindCharts()
        {
            Session["CompanyData"] = db.ExecuteQuery("sp_GetCompanyWisePolicyCount", null);
            Session["CategoryData"] = db.ExecuteQuery("sp_GetCategoryWisePolicyCount", null);
        }

        private void BindCards()
        {
            lbltotal.Text = db.ExecuteScalar("sp_GetTotalPolicies", null)?.ToString() ?? "0";
            lblowner.Text = db.ExecuteScalar("sp_GetDistinctOwnerCount", null)?.ToString() ?? "0";
            lblexpired.Text = db.ExecuteScalar("sp_GetCurrentMonthExpiringPolicies", null)?.ToString() ?? "0";
        }

        #endregion

        #region Footer & Error

        protected void gvdashboard_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                int totalRecords = Session["TotalRecords"] != null ? (int)Session["TotalRecords"] : 0;
                e.Row.Cells[0].ColumnSpan = gvdashboard.Columns.Count;

                for (int i = 1; i < gvdashboard.Columns.Count; i++)
                    e.Row.Cells[i].Visible = false;

                e.Row.Cells[0].Text = "Total Records: " + totalRecords;
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Font.Bold = true;
                e.Row.BackColor = System.Drawing.Color.LightGray;
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

        #endregion

        #region Search

        protected void btnSearch_dash_Click(object sender, EventArgs e)
        {
            gvdashboard.PageIndex = 0;
            BindGrid(0, txtSearch.Text.Trim());
        }

        protected void btnClearSearch_dash_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            gvdashboard.PageIndex = 0;
            BindGrid(0, "");
        }

        #endregion

        #region Export to Excel

        private void ExportToExcel(DataTable dt, string fileName, string[] removeCols = null)
        {
            DataTable exportTable = dt.Copy();

            if (removeCols != null)
            {
                foreach (var col in removeCols)
                {
                    if (exportTable.Columns.Contains(col))
                        exportTable.Columns.Remove(col);
                }
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
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

        public override void VerifyRenderingInServerForm(Control control) { }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["CompanyData"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) { ShowError("No data to export."); return; }
            ExportToExcel(dt, "CompanyWisePolicy.xls", new[] { "CompanyID" });
        }

        protected void btnExportCategoryExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["CategoryData"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) { ShowError("No data to export."); return; }
            ExportToExcel(dt, "CategoryWisePolicy.xls", new[] { "CategoryID" });
        }

        protected void lnkcompanywiseCategory_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["CategoryCompanyData"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) { ShowError("No data to export."); return; }
            ExportToExcel(dt, "CategoryCompanyWisePolicy.xls", new[] { "CompanyID", "CategoryID" });
        }

        #endregion
    }
}
