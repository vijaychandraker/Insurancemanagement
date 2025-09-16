using Dbord.helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

namespace Dbord.View.User
{
    public partial class report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindPolicies(1, 5); // Load first page
        }

        private void BindPolicies(int pageIndex = 1, int pageSize = 5)
        {
            Dictionary<string, string> searchValues = Session["SearchValues"] as Dictionary<string, string> ?? new Dictionary<string, string>();

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@PageNumber", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };

            // Add all filters (if empty pass DBNull)
            string[] keys = { "Name","OwnerName","Address","VehicleNo","Particular","SumInsured","Premium",
                              "NCB","PolicyNo","CompanyName","CategoryName",
                              "StartDateFrom","StartDateTo","EndDateFrom","EndDateTo" };

            foreach (var key in keys)
            {
                string value = searchValues.ContainsKey(key) ? searchValues[key] : null;
                parameters.Add(new SqlParameter("@" + key, string.IsNullOrWhiteSpace(value) ? DBNull.Value : (object)value));
            }

            DataTable dt = new DatabaseHelper().ExecuteQuery("GetAllInsurancePolicies", parameters.ToArray());

            if (dt.Rows.Count > 0)
            {
                int totalRecords = Convert.ToInt32(dt.Rows[0]["TotalCount"]);
                GridView1.DataSource = dt;
                GridView1.VirtualItemCount = totalRecords;
                GridView1.DataBind();
                SetFooterTotal(totalRecords);
            }
            else
            {
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
        }

        private void SetFooterTotal(int total)
        {
            if (GridView1.FooterRow != null)
            {
                TableCell footerCell = GridView1.FooterRow.Cells[0];
                footerCell.ColumnSpan = GridView1.Columns.Count;
                footerCell.Text = "Total Records: " + total;
                for (int i = 1; i < GridView1.FooterRow.Cells.Count; i++)
                    GridView1.FooterRow.Cells[i].Visible = false;
                footerCell.CssClass = "grid-footer";
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindPolicies(e.NewPageIndex + 1, GridView1.PageSize);
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int serial = e.Row.RowIndex + 1 + (GridView1.PageIndex * GridView1.PageSize);
                Label lblSerial = (Label)e.Row.FindControl("lblSerial");
                if (lblSerial != null) lblSerial.Text = serial.ToString();
            }
        }

        protected void SearchTextChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> searchValues = new Dictionary<string, string>
            {
                ["Name"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchName"))?.Text.Trim() ?? "",
                ["OwnerName"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchOwner"))?.Text.Trim() ?? "",
                ["Address"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchAddress"))?.Text.Trim() ?? "",
                ["VehicleNo"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchVehicle"))?.Text.Trim() ?? "",
                ["Particular"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchParticular"))?.Text.Trim() ?? "",
                ["SumInsured"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchSumInsured"))?.Text.Trim() ?? "",
                ["Premium"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchPremium"))?.Text.Trim() ?? "",
                ["NCB"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchNCB"))?.Text.Trim() ?? "",
                ["PolicyNo"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchPolicyNo"))?.Text.Trim() ?? "",
                ["CompanyName"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchCompany"))?.Text.Trim() ?? "",
                ["CategoryName"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchCategory"))?.Text.Trim() ?? "",
                ["StartDateFrom"] = txtSearchStartDateFrom.Text.Trim(),
                ["StartDateTo"] = txtSearchStartDateTo.Text.Trim(),
                ["EndDateFrom"] = txtSearchEndDateFrom.Text.Trim(),
                ["EndDateTo"] = txtSearchEndDateTo.Text.Trim()
            };

            Session["SearchValues"] = searchValues;
            GridView1.PageIndex = 0;
            BindPolicies(1, GridView1.PageSize);
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> searchValues = Session["SearchValues"] as Dictionary<string, string> ?? new Dictionary<string, string>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            string[] keys = { "Name","OwnerName","Address","VehicleNo","Particular","SumInsured","Premium",
                              "NCB","PolicyNo","CompanyName","CategoryName",
                              "StartDateFrom","StartDateTo","EndDateFrom","EndDateTo" };

            foreach (var key in keys)
            {
                string value = searchValues.ContainsKey(key) ? searchValues[key] : null;
                parameters.Add(new SqlParameter("@" + key, string.IsNullOrWhiteSpace(value) ? DBNull.Value : (object)value));
            }

            // Use large number for export to get all records
            parameters.Add(new SqlParameter("@PageNumber", 1));
            parameters.Add(new SqlParameter("@PageSize", 1000000));

            DataTable dt = new DatabaseHelper().ExecuteQuery("GetAllInsurancePolicies", parameters.ToArray());

            if (dt == null || dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No records to export');", true);
                return;
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "Policies");

                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=PolicyReport.xlsx");
                    Response.BinaryWrite(stream.ToArray());
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearchStartDateFrom.Text = string.Empty;
            txtSearchStartDateTo.Text = string.Empty;
            txtSearchEndDateFrom.Text = string.Empty;
            txtSearchEndDateTo.Text = string.Empty;

            Session["SearchValues"] = null;

            if (GridView1.HeaderRow != null)
            {
                foreach (TableCell cell in GridView1.HeaderRow.Cells)
                    foreach (Control ctl in cell.Controls)
                        if (ctl is TextBox txt) txt.Text = string.Empty;
            }

            GridView1.PageIndex = 0;
            BindPolicies(1, GridView1.PageSize);
        }
    }
}
