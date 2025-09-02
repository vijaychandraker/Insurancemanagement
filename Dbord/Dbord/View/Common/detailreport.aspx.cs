using Dbord.helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dbord.View.Common
{
    public partial class detailreport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            {
                if (!IsPostBack)
                    BindPolicies();
            }

        }
        private void BindPolicies()
        {
            DataTable dt = new DatabaseHelper().ExecuteQuery("GetPoliciesExpiringInOneMonth", new SqlParameter[] { });
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
            BindPolicies();


        }
        protected void Gvrepot_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int serial = e.Row.RowIndex + 1 + (Gvrepot.PageIndex * Gvrepot.PageSize);
                Label lblSerial = (Label)e.Row.FindControl("lblSerial");
                if (lblSerial != null) lblSerial.Text = serial.ToString();
            }
        }

    }
}