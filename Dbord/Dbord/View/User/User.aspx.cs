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
        private readonly DatabaseHelper db = new DatabaseHelper();
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                LoadCompanies();
                LoadCategories();
            }
        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtOwnerName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtVehicleNo.Text) ||
                string.IsNullOrWhiteSpace(txtParticular.Text) ||
                string.IsNullOrWhiteSpace(txtSumInsured.Text) ||
                string.IsNullOrWhiteSpace(txtPremium.Text) ||
                string.IsNullOrWhiteSpace(txtPolicyNo.Text) ||
                string.IsNullOrWhiteSpace(txtStartDate.Text) ||
                string.IsNullOrWhiteSpace(txtEndDate.Text) ||
                string.IsNullOrWhiteSpace(ddlCompany.SelectedValue) ||
                string.IsNullOrWhiteSpace(ddlCategory.SelectedValue))
            {
                lblMessage.Text = "Please fill all required fields.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Validate dates
            if (!DateTime.TryParse(txtStartDate.Text, out DateTime insuredDate))
            {
                lblMessage.Text = "Invalid insured date.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!DateTime.TryParse(txtEndDate.Text, out DateTime expireDate))
            {
                lblMessage.Text = "Invalid expire date.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (expireDate < insuredDate)
            {
                lblMessage.Text = "Expire date must be after insured date.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Insert into DB
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@Name", txtName.Text.Trim()),
            new SqlParameter("@OwnerName", txtOwnerName.Text.Trim()),
            new SqlParameter("@Address", txtAddress.Text.Trim()),
            new SqlParameter("@VehicleNo", txtVehicleNo.Text.Trim()),
            new SqlParameter("@Particular", txtParticular.Text.Trim()),
            new SqlParameter("@SumInsured", txtSumInsured.Text.Trim()),
            new SqlParameter("@Premium", txtPremium.Text.Trim()),
            new SqlParameter("@NCB", txtNCB.Text.Trim()),
            new SqlParameter("@PolicyNo", txtPolicyNo.Text.Trim()),
            new SqlParameter("@InsuredDate", insuredDate),
            new SqlParameter("@ExpireDate", expireDate),
            new SqlParameter("@CompanyID", ddlCompany.SelectedValue),
            new SqlParameter("@CategoryID", ddlCategory.SelectedValue),

            // OUTPUT parameter
            new SqlParameter("@NewPolicyId", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                db.ExecuteNonQuery("InsertInsurancePolicy", parameters);

                int newPolicyId = (int)parameters[parameters.Length - 1].Value;

                if (newPolicyId > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "successAlert",
                        "<script>Swal.fire({ icon: 'success', title: 'Inserted Successfully!', text: 'New policy saved with ID: " + newPolicyId + "' });</script>",
                        false);

                    ClearForm();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert",
                        "<script>Swal.fire({ icon: 'error', title: 'Error!', text: 'Failed to insert record.' });</script>",
                        false);
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


        private void LoadCompanies()
        {
            DataTable dt = db.ExecuteQuery("sp_GetAllCompanies", null);
            ddlCompany.DataSource = dt;
            ddlCompany.DataTextField = "CompanyName";
            ddlCompany.DataValueField = "c_id";
            ddlCompany.DataBind();
            ddlCompany.Items.Insert(0, new ListItem("-- Select Company --", ""));
        }

        private void LoadCategories()
        {
            DataTable dt = db.ExecuteQuery("sp_GetAllCategories", null);
            ddlCategory.DataSource = dt;
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataValueField = "c_id";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("-- Select Category --", ""));
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtOwnerName.Text = "";
            txtAddress.Text = "";
            txtVehicleNo.Text = "";
            txtParticular.Text = "";
            txtSumInsured.Text = "";
            txtPremium.Text = "";
            txtNCB.Text = "";
            txtPolicyNo.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";

            ddlCompany.SelectedIndex = 0;
            ddlCategory.SelectedIndex = 0;

            lblMessage.Text = "";
        }
    }
}
