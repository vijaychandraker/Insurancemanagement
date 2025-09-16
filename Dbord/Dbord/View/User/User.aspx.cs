using Dbord.helpers;
using Dbord.Helpers;
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

        // Cache keys
        private const string AllPoliciesCacheKey = "AllPolicies_All";
        private const string CategoryCompanyCacheKey = "CategoryCompanyData";
        private const string CompanyChartCacheKey = "CompanyChartData";
        private const string CategoryChartCacheKey = "CategoryChartData";

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
            if (!ValidateForm(out DateTime insuredDate, out DateTime expireDate)) return;

            try
            {
                SqlParameter[] parameters = {
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
                    new SqlParameter("@NewPolicyId", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                db.ExecuteNonQuery("InsertInsurancePolicy", parameters);

                int newPolicyId = (int)parameters[parameters.Length - 1].Value;


                if (newPolicyId > 0)
                {
                    AlertHelper.ShowSuccess(this, $"New policy saved with ID: {newPolicyId}");
                    ClearPolicyCache();
                    ClearForm();
                }
                else
                {
                    AlertHelper.ShowError(this, "Failed to insert record.");
                }
            }
            catch (Exception ex)
            {
                AlertHelper.ShowError(this, "An unexpected error occurred.");
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool ValidateForm(out DateTime insuredDate, out DateTime expireDate)
        {
            insuredDate = DateTime.MinValue;
            expireDate = DateTime.MinValue;

            // Required fields check
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
                SetMessage("Please fill all required fields.", false);
                return false;
            }

            // Date validation
            if (!DateTime.TryParse(txtStartDate.Text, out insuredDate))
            {
                SetMessage("Invalid insured date.", false);
                return false;
            }

            if (!DateTime.TryParse(txtEndDate.Text, out expireDate))
            {
                SetMessage("Invalid expire date.", false);
                return false;
            }

            if (expireDate < insuredDate)
            {
                SetMessage("Expire date must be after insured date.", false);
                return false;
            }

            return true;
        }

        private void SetMessage(string text, bool isSuccess)
        {
            lblMessage.Text = text;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private void LoadCompanies()
        {
            BindDropdown(ddlCompany, "sp_GetAllCompanies", "CompanyName", "c_id");
        }

        private void LoadCategories()
        {
            BindDropdown(ddlCategory, "sp_GetAllCategories", "CategoryName", "c_id");
        }

        private void BindDropdown(DropDownList ddl, string spName, string textField, string valueField)
        {
            DataTable dt = db.ExecuteQuery(spName, null);
            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem($"-- Select {textField.Replace("Name", "")} --", ""));
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtName.Text = txtOwnerName.Text = txtAddress.Text = txtVehicleNo.Text =
            txtParticular.Text = txtSumInsured.Text = txtPremium.Text = txtNCB.Text =
            txtPolicyNo.Text = txtStartDate.Text = txtEndDate.Text = string.Empty;

            ddlCompany.SelectedIndex = 0;
            ddlCategory.SelectedIndex = 0;
            lblMessage.Text = "";
        }

        private void ClearPolicyCache()
        {
            Cache.Remove(AllPoliciesCacheKey);
            Cache.Remove(CategoryCompanyCacheKey);
            Cache.Remove(CompanyChartCacheKey);
            Cache.Remove(CategoryChartCacheKey);
        }
    }
}
