using Dbord.helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.Web;

namespace Dbord.View.User
{
    public partial class report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindPolicies();
        }

        private void BindPolicies()
        {
            DataTable dt = new DatabaseHelper().ExecuteQuery("GetAllInsurancePolicies", new SqlParameter[] { });
            GridView1.DataSource = dt;
            GridView1.DataBind();
            if (dt.Rows.Count > 0)
                SetFooterTotal(dt.Rows.Count);
        }

        private void BindPoliciesWithSearch()
        {
            Dictionary<string, string> searchValues = ViewState["SearchValues"] as Dictionary<string, string> ?? new Dictionary<string, string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (var kvp in searchValues)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                    parameters.Add(new SqlParameter("@" + kvp.Key, kvp.Value));
            }

            DataTable dt = new DatabaseHelper().ExecuteQuery("sp_getsearch", parameters.ToArray());
            GridView1.DataSource = dt;
            GridView1.DataBind();
            if (dt.Rows.Count > 0)
                SetFooterTotal(dt.Rows.Count);
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
            BindPoliciesWithSearch();
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
                ["InsuredDateSearch"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchStartDate"))?.Text.Trim() ?? "",
                ["ExpireDateSearch"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchEndDate"))?.Text.Trim() ?? "",
                ["CompanyName"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchCompany"))?.Text.Trim() ?? "",
                ["CategoryName"] = ((TextBox)GridView1.HeaderRow.FindControl("txtSearchCategory"))?.Text.Trim() ?? ""
            };

            ViewState["SearchValues"] = searchValues;
            GridView1.PageIndex = 0;
            BindPoliciesWithSearch();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // Get data (search or full list)
            DataTable dt;
            if (ViewState["SearchValues"] != null)
                dt = new DatabaseHelper().ExecuteQuery("sp_getsearch", BuildSearchParameters());
            else
                dt = new DatabaseHelper().ExecuteQuery("GetAllInsurancePolicies", new SqlParameter[] { });

            if (dt == null || dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No records to export');", true);
                return;
            }

            using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "Policies");

                // Format header
                var headerRow = ws.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                headerRow.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                // Auto adjust column widths
                ws.Columns().AdjustToContents();

                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    wb.SaveAs(stream);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=PolicyReport.xlsx");

                    Response.BinaryWrite(stream.ToArray());
                    Response.Flush();

                    // Important: avoids ThreadAbortException
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }

        // Helper to reuse search params
        private SqlParameter[] BuildSearchParameters()
        {
            Dictionary<string, string> searchValues = ViewState["SearchValues"] as Dictionary<string, string>;
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (searchValues != null)
            {
                foreach (var kvp in searchValues)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value))
                        parameters.Add(new SqlParameter("@" + kvp.Key, kvp.Value));
                }
            }

            return parameters.ToArray();
        }


        private void ShowMessage(string msg)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{msg}');", true);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            // Clear search filters
            ViewState["SearchValues"] = null;

            // Clear all header textboxes only if HeaderRow exists
            if (GridView1.HeaderRow != null)
            {
                foreach (TableCell cell in GridView1.HeaderRow.Cells)
                {
                    foreach (Control ctl in cell.Controls)
                    {
                        if (ctl is TextBox txt)
                            txt.Text = string.Empty;
                    }
                }
            }

            // Reset page and bind full data
            GridView1.PageIndex = 0;
            BindPolicies();
        }

    }
}
