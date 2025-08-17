using Dbord.helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dbord.View.Admin
{
    public partial class showdata : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                DatabaseHelper db = new DatabaseHelper();
                DataTable dt = db.ExecuteQuery("sp_GetAllPolicies", new SqlParameter[] { });
                gvPolicies.DataSource = dt;
                gvPolicies.DataBind();
            }
            catch (Exception ex)
            {
                ShowError("Error loading data: " + ex.Message);
            }
        }

        protected void gvPolicies_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPolicies.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvPolicies_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPolicies.EditIndex = -1;
            BindGrid();
        }

        protected void gvPolicies_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvPolicies.DataKeys[e.RowIndex].Value);

                GridViewRow row = gvPolicies.Rows[e.RowIndex];

                string policyNumber = ((TextBox)row.FindControl("txtPolicyNumber")).Text;
                string customerName = ((TextBox)row.FindControl("txtCustomerName")).Text;
                decimal premium = Convert.ToDecimal(((TextBox)row.FindControl("txtPremium")).Text);
                DateTime startDate = Convert.ToDateTime(((TextBox)row.FindControl("txtStartDate")).Text);
                DateTime endDate = Convert.ToDateTime(((TextBox)row.FindControl("txtEndDate")).Text);
                string policyType = ((TextBox)row.FindControl("txtPolicyType")).Text;
                string agentName = ((TextBox)row.FindControl("txtAgentName")).Text;
                decimal coverage = Convert.ToDecimal(((TextBox)row.FindControl("txtCoverage")).Text);
                string status = ((DropDownList)row.FindControl("ddlStatus")).SelectedValue;
                string remarks = ((TextBox)row.FindControl("txtRemarks")).Text;

                DatabaseHelper db = new DatabaseHelper();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PolicyId", id),
                    new SqlParameter("@PolicyNumber", policyNumber),
                    new SqlParameter("@CustomerName", customerName),
                    new SqlParameter("@PremiumAmount", premium),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@PolicyType", policyType),
                    new SqlParameter("@AgentName", agentName),
                    new SqlParameter("@CoverageAmount", coverage),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@Remarks", remarks)
                };

                db.ExecuteNonQuery("sp_UpdatePolicyScheme", parameters);

                gvPolicies.EditIndex = -1;
                BindGrid();
                ShowSuccess("Record updated successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error updating record: " + ex.Message);
            }
        }

        protected void gvPolicies_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvPolicies.DataKeys[e.RowIndex].Value);

                DatabaseHelper db = new DatabaseHelper();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PolicyId", id)
                };

                db.ExecuteNonQuery("sp_DeletePolicyScheme", parameters);

                BindGrid();
                ShowSuccess("Record deleted successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error deleting record: " + ex.Message);
            }
        }

        protected void gvPolicies_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRecord")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int id = Convert.ToInt32(gvPolicies.DataKeys[index].Value);

                DatabaseHelper db = new DatabaseHelper();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PolicyId", id)
                };
                db.ExecuteNonQuery("sp_DeletePolicyScheme", parameters);
                BindGrid();
                ShowSuccess("Record deleted successfully.");
            }
        }

        protected void gvPolicies_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPolicies.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void ShowSuccess(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", $@"
                Swal.fire({{
                    icon: 'success',
                    title: 'Success',
                    text: '{message}'
                }});", true);
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

        protected void gvPolicies_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                if (ddlStatus != null)
                {
                    // Populate dropdown items
                    if (ddlStatus.Items.Count == 0) // Prevent duplicate items on postback
                    {
                        ddlStatus.Items.Add(new ListItem("-- Select --", ""));
                        ddlStatus.Items.Add(new ListItem("Active", "Active"));
                        ddlStatus.Items.Add(new ListItem("Pending", "Pending"));
                        ddlStatus.Items.Add(new ListItem("Expired", "Expired"));
                    }

                    // Set selected value
                    string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status")?.ToString();
                    if (!string.IsNullOrEmpty(currentStatus))
                    {
                        ListItem item = ddlStatus.Items.FindByValue(currentStatus);
                        if (item != null)
                        {
                            ddlStatus.SelectedValue = currentStatus;
                        }
                    }
                }
            }
        }
        private void BindGrid(string searchText = "")
        {
            try
            {
                DatabaseHelper db = new DatabaseHelper();
                DataTable dt = db.ExecuteQuery("sp_GetAllPolicies", new SqlParameter[] { });

                if (!string.IsNullOrEmpty(searchText))
                {
                    // Filter DataTable in memory
                    string filterExpression = $"Convert(PolicyNumber, 'System.String') LIKE '%{searchText}%' OR " +
                                              $"CustomerName LIKE '%{searchText}%' OR " +
                                              $"Status LIKE '%{searchText}%'";
                    DataRow[] filteredRows = dt.Select(filterExpression);

                    if (filteredRows.Length > 0)
                        dt = filteredRows.CopyToDataTable();
                    else
                        dt.Clear();
                }

                gvPolicies.DataSource = dt;
                gvPolicies.DataBind();
            }
            catch (Exception ex)
            {
                ShowError("Error loading data: " + ex.Message);
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            BindGrid(searchText);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindGrid();
        }


    }
}
