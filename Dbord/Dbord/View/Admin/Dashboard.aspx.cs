using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dbord.helpers;

namespace Dbord.View.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private const int PageSize = 5; // Rows per page
        private const int CacheMinutes = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCharts();
                BindCards();
                BindCategoryCompanyGrid();
                BindPoliciesGrid(0, txtSearch.Text.Trim());
            }
        }

        #region Main Policies Grid

        private void BindPoliciesGrid(int pageIndex, string searchText)
        {
            try
            {
                int safePageIndex = pageIndex < 0 ? 0 : pageIndex;
                DataTable dt = GetPoliciesCached(searchText);
                int totalRecords = dt.Rows.Count;
                DataTable pagedDt;
                if (totalRecords > 0)
                {
                    var rows = dt.AsEnumerable()
                                 .Skip(safePageIndex * PageSize)
                                 .Take(PageSize);
                    pagedDt = rows.Any() ? rows.CopyToDataTable() : dt.Clone();
                }
                else{pagedDt = dt.Clone();}
                gvdashboard.DataSource = pagedDt;
                gvdashboard.VirtualItemCount = totalRecords;
                gvdashboard.PageSize = PageSize;
                gvdashboard.DataBind();
                lblMessage.Visible = (totalRecords == 0);
                lblMessage.Text = (totalRecords == 0) ? "No data found." : "";
            }
            catch (Exception ex)
            {
                ShowError("Error loading data: " + ex.Message);
            }}
        private DataTable GetPoliciesCached(string searchText)
        {
            string cacheKey = "AllPolicies_" + (string.IsNullOrEmpty(searchText) ? "All" : searchText);

            if (Cache[cacheKey] != null)
                return (DataTable)Cache[cacheKey];

            SqlParameter[] parameters = {
                new SqlParameter("@PageIndex", 0), // get all rows for caching
                new SqlParameter("@PageSize", int.MaxValue),
                new SqlParameter("@SearchText", string.IsNullOrEmpty(searchText) ? DBNull.Value : (object)searchText),
                new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            DataTable dt = db.ExecuteQuery("sp_GetAllPolicies_Paged", parameters);
            Cache.Insert(cacheKey, dt, null, DateTime.Now.AddMinutes(CacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration);

            return dt;
        }

        protected void gvdashboard_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvdashboard.PageIndex = e.NewPageIndex;
            BindPoliciesGrid(e.NewPageIndex, txtSearch.Text.Trim());
        }

        protected void gvdashboard_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                int totalRecords = gvdashboard.VirtualItemCount;
                e.Row.Cells[0].ColumnSpan = gvdashboard.Columns.Count;

                for (int i = 1; i < gvdashboard.Columns.Count; i++)
                    e.Row.Cells[i].Visible = false;
                e.Row.Cells[0].Text = "Total Records: " + totalRecords;
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Font.Bold = true;
                e.Row.BackColor = System.Drawing.Color.LightGray;
            }
        }

        #endregion

        #region Category-Company Grid

        private void BindCategoryCompanyGrid()
        {
            string cacheKey = "CategoryCompanyData";
            if (Cache[cacheKey] != null)
            {
                gvCategoryCompany.DataSource = (DataTable)Cache[cacheKey];
                gvCategoryCompany.DataBind();
                return;
            }

            DataTable dt = db.ExecuteQuery("sp_GetCategoryCompanyPolicies", null);
            gvCategoryCompany.DataSource = dt;
            gvCategoryCompany.DataBind();
            Cache.Insert(cacheKey, dt, null, DateTime.Now.AddMinutes(CacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        protected void gvCategoryCompany_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategoryCompany.PageIndex = e.NewPageIndex;
            BindCategoryCompanyGrid();
        }

        #endregion

        #region Charts & Cards

        private void BindCharts()
        {
            if (Cache["CompanyChartData"] == null)
                Cache.Insert("CompanyChartData", db.ExecuteQuery("sp_GetCompanyWisePolicyCount", null), null, DateTime.Now.AddMinutes(CacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration);

            if (Cache["CategoryChartData"] == null)
                Cache.Insert("CategoryChartData", db.ExecuteQuery("sp_GetCategoryWisePolicyCount", null), null, DateTime.Now.AddMinutes(CacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration);

            Session["CompanyData"] = Cache["CompanyChartData"];
            Session["CategoryData"] = Cache["CategoryChartData"];
        }

        private void BindCards()
        {
            lbltotal.Text = db.ExecuteScalar("sp_GetTotalPolicies", null)?.ToString() ?? "0";
            lblowner.Text = db.ExecuteScalar("sp_GetDistinctOwnerCount", null)?.ToString() ?? "0";
            lblexpired.Text = db.ExecuteScalar("sp_GetCurrentMonthExpiringPolicies", null)?.ToString() ?? "0";
        }

        #endregion

        #region Search

        protected void btnSearch_dash_Click(object sender, EventArgs e)
        {
            gvdashboard.PageIndex = 0;
            BindPoliciesGrid(0, txtSearch.Text.Trim());
        }

        protected void btnClearSearch_dash_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            gvdashboard.PageIndex = 0;
            BindPoliciesGrid(0, "");
        }

        #endregion

        #region Export to Excel

        private void ExportToExcel(DataTable dt, string fileName, string[] removeCols = null)
        {
            DataTable exportTable = dt.Copy();
            if (removeCols != null)
            {
                foreach (var col in removeCols)
                    if (exportTable.Columns.Contains(col))
                        exportTable.Columns.Remove(col);
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                GridView gvExport = new GridView { DataSource = exportTable };
                gvExport.DataBind();
                gvExport.RenderControl(hw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
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
            DataTable dt = Cache["CategoryCompanyData"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) { ShowError("No data to export."); return; }
            ExportToExcel(dt, "CategoryCompanyWisePolicy.xls", new[] { "CompanyID", "CategoryID" });
        }

        #endregion

        #region Error Handling

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
    }
}
