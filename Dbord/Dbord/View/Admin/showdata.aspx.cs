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
        private readonly DatabaseHelper db = new DatabaseHelper();

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid(string searchText = "")
        {
            try
            {
                DataTable dt = db.ExecuteQuery("sp_GetAllPolicies", null);

                // 🔎 Apply search filter
                if (!string.IsNullOrEmpty(searchText))
                {
                    string filter = $"Convert(PolicyNo, 'System.String') LIKE '%{searchText}%' OR " +
                                    $"Name LIKE '%{searchText}%' OR " +
                                    $"Convert(VehicleNo, 'System.String') LIKE '%{searchText}%' OR " +
                                    $"CompanyName LIKE '%{searchText}%' OR " +
                                    $"CategoryName LIKE '%{searchText}%'";
                    DataRow[] filtered = dt.Select(filter);
                    dt = filtered.Length > 0 ? filtered.CopyToDataTable() : dt.Clone();
                }

                gvPolicies.DataSource = dt;
                gvPolicies.DataBind();

                // ✅ Save filtered data in ViewState
                ViewState["GridData"] = dt;
            }
            catch (Exception ex)
            {
                ShowError("Error loading data: " + ex.Message);
            }
        }

        protected void gvPolicies_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPolicies.EditIndex = e.NewEditIndex;
            RebindFromViewState();
        }

        protected void gvPolicies_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPolicies.EditIndex = -1;
            RebindFromViewState();
        }

        protected void gvPolicies_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvPolicies.DataKeys[e.RowIndex]["PolicyID"]);
                GridViewRow row = gvPolicies.Rows[e.RowIndex];

                // Text fields
                string Name = ((TextBox)row.FindControl("txtName")).Text.Trim();
                string OwnerName = ((TextBox)row.FindControl("txtOwnerName")).Text.Trim();
                string Address = ((TextBox)row.FindControl("txtAddress")).Text.Trim();
                string VehicleNo = ((TextBox)row.FindControl("txtVehicleNo")).Text.Trim();
                string Particular = ((TextBox)row.FindControl("txtParticular")).Text.Trim();
                string SumInsured = ((TextBox)row.FindControl("txtSumInsured")).Text.Trim();
                string Premium = ((TextBox)row.FindControl("txtPremium")).Text.Trim();
                string NCB = ((TextBox)row.FindControl("txtNCB")).Text.Trim();
                string PolicyNo = ((TextBox)row.FindControl("txtPolicyNo")).Text.Trim();

                DateTime.TryParse(((TextBox)row.FindControl("txtStartDate")).Text.Trim(), out DateTime InsuredDate);
                DateTime.TryParse(((TextBox)row.FindControl("txtEndDate")).Text.Trim(), out DateTime ExpireDate);

                // Dropdowns
                DropDownList ddlCompany = (DropDownList)row.FindControl("ddlCompany");
                DropDownList ddlCategory = (DropDownList)row.FindControl("ddlCategory");

                int.TryParse(ddlCompany?.SelectedValue, out int CompanyID);
                int.TryParse(ddlCategory?.SelectedValue, out int CategoryID);

                // Update DB
                SqlParameter[] parameters =
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
                    new SqlParameter("@InsuredDate", InsuredDate == DateTime.MinValue ? (object)DBNull.Value : InsuredDate),
                    new SqlParameter("@ExpireDate", ExpireDate == DateTime.MinValue ? (object)DBNull.Value : ExpireDate),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@CategoryID", CategoryID)
                };

                db.ExecuteNonQuery("sp_UpdatePolicyScheme", parameters);

                gvPolicies.EditIndex = -1;
                BindGrid(txtSearch.Text.Trim()); // ✅ reload with current filter
                ShowSuccess("Record updated successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error updating record: " + ex.Message);
            }
        }

        protected void gvPolicies_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow &&
                (e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                // Company Dropdown
                DropDownList ddlCompany = (DropDownList)e.Row.FindControl("ddlCompany");
                if (ddlCompany != null)
                {
                    DataTable dtCompany = db.ExecuteQuery("sp_GetAllCompanies", null);
                    ddlCompany.DataSource = dtCompany;
                    ddlCompany.DataTextField = "CompanyName";
                    ddlCompany.DataValueField = "c_id";
                    ddlCompany.DataBind();

                    string currentCompany = gvPolicies.DataKeys[e.Row.RowIndex]["CompanyID"].ToString();
                    if (ddlCompany.Items.FindByValue(currentCompany) != null)
                        ddlCompany.SelectedValue = currentCompany;
                }

                // Category Dropdown
                DropDownList ddlCategory = (DropDownList)e.Row.FindControl("ddlCategory");
                if (ddlCategory != null)
                {
                    DataTable dtCategory = db.ExecuteQuery("sp_GetAllCategories", null);
                    ddlCategory.DataSource = dtCategory;
                    ddlCategory.DataTextField = "CategoryName";
                    ddlCategory.DataValueField = "c_id";
                    ddlCategory.DataBind();

                    string currentCategory = gvPolicies.DataKeys[e.Row.RowIndex]["CategoryID"].ToString();
                    if (ddlCategory.Items.FindByValue(currentCategory) != null)
                        ddlCategory.SelectedValue = currentCategory;
                }
            }
        }

        protected void gvPolicies_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvPolicies.DataKeys[e.RowIndex]["PolicyID"]);
                SqlParameter[] parameters = { new SqlParameter("@PolicyID", id) };

                db.ExecuteNonQuery("sp_DeletePolicyScheme", parameters);

                BindGrid(txtSearch.Text.Trim());
                ShowSuccess("Record deleted successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error deleting record: " + ex.Message);
            }
        }

        protected void gvPolicies_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPolicies.PageIndex = e.NewPageIndex;
            RebindFromViewState();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(txtSearch.Text.Trim());
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            BindGrid();
        }

        private void RebindFromViewState()
        {
            if (ViewState["GridData"] != null)
            {
                DataTable dt = (DataTable)ViewState["GridData"];
                gvPolicies.DataSource = dt;
                gvPolicies.DataBind();
            }
            else
            {
                BindGrid();
            }
        }

        // Alerts
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
    }
}
