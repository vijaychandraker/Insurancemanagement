using Dbord.helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.IO;

namespace Dbord.View.Common
{
    public partial class detailreport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string companyId = Request.QueryString["CompanyID"];
            string categoryId = Request.QueryString["CategoryID"];
            string defValue = Request.QueryString["defaultvalue"];

            if (Request.QueryString["download"] == "1")
            {
                ExportExcel(companyId, categoryId, defValue);
                return;
            }
            if (!IsPostBack)
            {
                Session["DefValue"] = defValue;
                Session["Company"] = companyId;
                Session["Category"] = categoryId;
                BindPolicies(companyId, categoryId, defValue);
            }
        }
        private DataTable GetPolicies(string companyId = null, string categoryId = null, string defVal = null)
        {
            if (defVal == "1")
            {
                return new DatabaseHelper().ExecuteQuery("GetCurrentMonthExpiringPolicies", new SqlParameter[] { });
            }
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CompanyID", string.IsNullOrEmpty(companyId) ? (object)DBNull.Value : companyId),
                new SqlParameter("@CategoryID", string.IsNullOrEmpty(categoryId) ? (object)DBNull.Value : categoryId)
            };

            return new DatabaseHelper().ExecuteQuery("GetcategoryCompany", parameters.ToArray());
        }
        private void BindPolicies(string companyId, string categoryId, string defValue, int pageIndex = 0)
        {
            int pageSize = Gvrepot.PageSize;
            int totalCount = 0;

            DataTable dt = GetPoliciesPaged(companyId, categoryId, defValue, pageIndex, pageSize, out totalCount);

            Gvrepot.DataSource = dt;
            Gvrepot.VirtualItemCount = totalCount; // important for correct paging
            Gvrepot.DataBind();

            if (dt.Rows.Count > 0)
                SetFooterTotal(totalCount);
        }

        private DataTable GetPoliciesPaged(string companyId, string categoryId, string defVal, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;

            // Prepare parameters for both procedures
            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@CompanyID", string.IsNullOrEmpty(companyId) ? (object)DBNull.Value : companyId),
        new SqlParameter("@CategoryID", string.IsNullOrEmpty(categoryId) ? (object)DBNull.Value : categoryId),
        new SqlParameter("@PageIndex", pageIndex),
        new SqlParameter("@PageSize", pageSize),
        new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
    };

            DataTable dt;

            if (defVal == "1")
            {
                // Call current month expiring policies stored procedure
                dt = new DatabaseHelper().ExecuteQuery("GetCurrentMonthExpiringPolicies", parameters.ToArray());
            }
            else
            {
                // Call category-company policies stored procedure
                dt = new DatabaseHelper().ExecuteQuery("GetcategoryCompany", parameters.ToArray());
            }

            // Read the output parameter for total count
            totalCount = Convert.ToInt32(parameters[4].Value);

            return dt;
        }

        private void SetFooterTotal(int total)
        {
            if (Gvrepot.FooterRow != null)
            {
                TableCell footerCell = Gvrepot.FooterRow.Cells[0];
                footerCell.ColumnSpan = Gvrepot.Columns.Count;
                footerCell.Text = "Total Records: " + total;

                for (int i = 1; i < Gvrepot.FooterRow.Cells.Count; i++)
                    Gvrepot.FooterRow.Cells[i].Visible = false;

                footerCell.CssClass = "grid-footer";
            }
        }
        protected void Gvrepot_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Gvrepot.PageIndex = e.NewPageIndex;

            string companyId = Session["Company"] as string;
            string categoryId = Session["Category"] as string;
            string defValue = Session["DefValue"] as string;

            BindPolicies(companyId, categoryId, defValue, e.NewPageIndex);
        }

        protected void Gvrepot_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int serial = e.Row.RowIndex + 1 + (Gvrepot.PageIndex * Gvrepot.PageSize);
                Label lblSerial = (Label)e.Row.FindControl("lblSerial");
                if (lblSerial != null)
                    lblSerial.Text = serial.ToString();
            }
        }
        private void ExportExcel(string companyId, string categoryId, string defValue)
        {
            DataTable dt = GetPolicies(companyId, categoryId, defValue);
            if (dt != null && dt.Rows.Count > 0)
            {
                using (XLWorkbook wb = new XLWorkbook())
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.Worksheets.Add(dt, "Policies");
                    wb.SaveAs(ms);
                    string token = Request.QueryString["token"];
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=PoliciesReport.xlsx");
                    if (!string.IsNullOrEmpty(token))
                    {
                        Response.Cookies.Add(new HttpCookie("downloadToken", token)
                        {
                            Path = "/",
                            HttpOnly = false
                        });
                    }
                    Response.BinaryWrite(ms.ToArray());
                    Response.Flush();
                    Response.End();
                }
            }
            else
            {
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.Write("No data available for export.");
                Response.End();
            }
        }
    }
}
