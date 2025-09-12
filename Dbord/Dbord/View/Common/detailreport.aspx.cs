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

            if (Request.QueryString["download"] == "1")
            {
              
                ExportExcel(companyId, categoryId);
                return;
            }

            if (!IsPostBack)
            {
                ViewState["Company"] = companyId;
                ViewState["Category"] = categoryId;

                BindPolicies(companyId, categoryId);
            }
        }

        private DataTable GetPolicies(string companyId = null, string categoryId = null)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CompanyID", string.IsNullOrEmpty(companyId) ? (object)DBNull.Value : companyId),
                new SqlParameter("@CategoryID", string.IsNullOrEmpty(categoryId) ? (object)DBNull.Value : categoryId)
            };

            return new DatabaseHelper().ExecuteQuery("GetcategoryCompany", parameters.ToArray());
        }

        private void BindPolicies(string companyId = null, string categoryId = null)
        {
            DataTable dt = GetPolicies(companyId, categoryId);
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
            BindPolicies(companyId, categoryId);
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

        private void ExportExcel(string companyId, string categoryId)
        {
            DataTable dt = GetPolicies(companyId, categoryId);

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
