using Dbord.helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dbord.View.User
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(txtPolicyNumber.Text) ||
                string.IsNullOrWhiteSpace(txtCustomerName.Text) ||
                string.IsNullOrWhiteSpace(txtPremiumAmount.Text) ||
                string.IsNullOrWhiteSpace(txtStartDate.Text) ||
                string.IsNullOrWhiteSpace(txtEndDate.Text) ||
                string.IsNullOrWhiteSpace(ddlPolicyType.SelectedValue) ||
                string.IsNullOrWhiteSpace(txtAgentName.Text) ||
                string.IsNullOrWhiteSpace(txtCoverageAmount.Text) ||
                string.IsNullOrWhiteSpace(ddlStatus.SelectedValue))
            {
                lblMessage.Text = "Please fill all required fields.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Validate numeric fields
            if (!decimal.TryParse(txtPremiumAmount.Text, out decimal premium))
            {
                lblMessage.Text = "Invalid premium amount.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!decimal.TryParse(txtCoverageAmount.Text, out decimal coverage))
            {
                lblMessage.Text = "Invalid coverage amount.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Validate dates
            if (!DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
            {
                lblMessage.Text = "Invalid start date.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
            {
                lblMessage.Text = "Invalid end date.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (endDate < startDate)
            {
                lblMessage.Text = "End date must be after start date.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // If validation passes, insert into DB
            try
            {
                DatabaseHelper db = new DatabaseHelper();

                SqlParameter outputId = new SqlParameter("@NewPolicyId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PolicyNumber", txtPolicyNumber.Text),
                    new SqlParameter("@CustomerName", txtCustomerName.Text),
                    new SqlParameter("@PremiumAmount", premium),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@PolicyType", ddlPolicyType.SelectedValue),
                    new SqlParameter("@AgentName", txtAgentName.Text),
                    new SqlParameter("@CoverageAmount", coverage),
                    new SqlParameter("@Status", ddlStatus.SelectedValue),
                    new SqlParameter("@Remarks", txtRemarks.Text),
                    outputId
                };

                db.ExecuteNonQuery("sp_InsertPolicyScheme", parameters);

                // Check success from Output Parameter
                if (outputId.Value != DBNull.Value && Convert.ToInt32(outputId.Value) > 0)
                {
                    int newId = Convert.ToInt32(outputId.Value);
                    ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", $@"
                        Swal.fire({{
                            icon: 'success',
                            title: 'Inserted Successfully!',
                            text: 'New Policy ID: {newId}'
                        }});", true);

                    ClearForm();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", @"
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'Failed to insert record.'
                        });", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", @"
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'An unexpected error occurred.'
                    });", true);
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all textboxes
            txtPolicyNumber.Text = "";
            txtCustomerName.Text = "";
            txtPremiumAmount.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtAgentName.Text = "";
            txtCoverageAmount.Text = "";
            txtRemarks.Text = "";

            // Reset dropdowns
            ddlPolicyType.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;

            // Clear message
            lblMessage.Text = "";
        }
    }
}
