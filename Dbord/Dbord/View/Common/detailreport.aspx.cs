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
            if (Request.QueryString["download"] == "1")
            {
                string company = Request.QueryString["company"];
                string category = Request.QueryString["category"];
                ExportExcel(company, category);
                return;
            }

            if (!IsPostBack)
            {
                string company = Request.QueryString["company"];
                string category = Request.QueryString["category"];

                ViewState["Company"] = company;
                ViewState["Category"] = category;

                BindPolicies(company, category);
            }
        }

        private DataTable GetPolicies(string companyName = null, string categoryName = null)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CompanyName", string.IsNullOrEmpty(companyName) ? (object)DBNull.Value : companyName),
                new SqlParameter("@CategoryName", string.IsNullOrEmpty(categoryName) ? (object)DBNull.Value : categoryName)
            };

            return new DatabaseHelper().ExecuteQuery("GetcategoryCompany", parameters.ToArray());
        }

        private void BindPolicies(string companyName = null, string categoryName = null)
        {
            DataTable dt = GetPolicies(companyName, categoryName);
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
            string company = ViewState["Company"] as string;
            string category = ViewState["Category"] as string;
            BindPolicies(company, category);
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

        private void ExportExcel(string company, string category)
        {
            DataTable dt = GetPolicies(company, category);

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
