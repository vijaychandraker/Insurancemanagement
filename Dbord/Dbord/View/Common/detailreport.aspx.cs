using Dbord.helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
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

            // Handle download BEFORE binding
            if (Request.QueryString["download"] == "1")
            {
                // ✅ Pass query string values directly
                ExportExcel(companyId, categoryId, defValue);
                return;
            }

            if (!IsPostBack)
            {
                // Store values in ViewState for reuse in paging
                ViewState["DefValue"] = defValue;
                ViewState["Company"] = companyId;
                ViewState["Category"] = categoryId;

                // Initial bind
                BindPolicies(companyId, categoryId, defValue);
            }
        }

        private DataTable GetPolicies(string companyId = null, string categoryId = null, string defVal = null)
        {
            if (defVal == "1")
            {
                // Expiring policies SP
                return new DatabaseHelper().ExecuteQuery("GetCurrentMonthExpiringPolicies", new SqlParameter[] { });
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CompanyID", string.IsNullOrEmpty(companyId) ? (object)DBNull.Value : companyId),
                new SqlParameter("@CategoryID", string.IsNullOrEmpty(categoryId) ? (object)DBNull.Value : categoryId)
            };

            return new DatabaseHelper().ExecuteQuery("GetcategoryCompany", parameters.ToArray());
        }

        private void BindPolicies(string companyId, string categoryId, string defValue)
        {
            DataTable dt = GetPolicies(companyId, categoryId, defValue);
            Gvrepot.DataSource = dt;
            Gvrepot.DataBind();

            if (dt.Rows.Count > 0)
                SetFooterTotal(dt.Rows.Count);
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

            string companyId = ViewState["Company"] as string;
            string categoryId = ViewState["Category"] as string;
            string defValue = ViewState["DefValue"] as string;

            BindPolicies(companyId, categoryId, defValue);
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
            // ✅ Always fetch fresh data using query string values
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
