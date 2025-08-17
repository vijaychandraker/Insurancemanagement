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
                DataTable dt = db.ExecuteQuery("GetAllInsurancePolicies", new SqlParameter[] { });
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

                string Name = ((TextBox)row.FindControl("txtName")).Text.Trim();
                string OwnerName = ((TextBox)row.FindControl("txtOwnerName")).Text.Trim();
                string Address = ((TextBox)row.FindControl("txtAddress")).Text.Trim();
                string VehicleNo = ((TextBox)row.FindControl("txtVehicleNo")).Text.Trim();
                string Particular = ((TextBox)row.FindControl("txtParticular")).Text.Trim();
                string SumInsured = ((TextBox)row.FindControl("txtSumInsured")).Text.Trim();
                string Premium = ((TextBox)row.FindControl("txtPremium")).Text.Trim();
                string NCB = ((TextBox)row.FindControl("txtNCB")).Text.Trim();
                string PolicyNo = ((TextBox)row.FindControl("txtPolicyNo")).Text.Trim();
                DateTime InsuredDate;
                DateTime.TryParse(((TextBox)row.FindControl("txtStartDate")).Text.Trim(), out InsuredDate);
                DateTime ExpireDate;
                DateTime.TryParse(((TextBox)row.FindControl("txtEndDate")).Text.Trim(), out ExpireDate);
                int CompanyID = 0;
                int.TryParse(((TextBox)row.FindControl("txtCompanyID")).Text.Trim(), out CompanyID);

                int CategoryID = 0;
                int.TryParse(((TextBox)row.FindControl("txtCategoryID")).Text.Trim(), out CategoryID);

                DatabaseHelper db = new DatabaseHelper();
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@PolicyID", id),
            new SqlParameter("@Name", Name),
            new SqlParameter("@OwnerName", OwnerName),
            new SqlParameter("@Address", Address),
            new SqlParameter("@VehicleNo", VehicleNo),
            new SqlParameter("@Particular", Particular),
            new SqlParameter("@SumInsured", SumInsured),
            new SqlParameter("@Premium", Premium),
            new SqlParameter("@NCB", NCB),
            new SqlParameter("@PolicyNo", PolicyNo),
            new SqlParameter("@InsuredDate", (object)InsuredDate ?? DBNull.Value),
            new SqlParameter("@ExpireDate", (object)ExpireDate ?? DBNull.Value),
            new SqlParameter("@CompanyID", CompanyID),
            new SqlParameter("@CategoryID", CategoryID)
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
                    string filterExpression = $"Convert(PolicyNo, 'System.String') LIKE '%{searchText}%' OR " +
                                              $"Name LIKE '%{searchText}%' OR " +
                                             $"Convert(VehicleNo, 'System.String') LIKE '%{searchText}%'";
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
