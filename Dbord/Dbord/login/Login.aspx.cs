using System;
using System.Data;
using System.Data.SqlClient;
using Dbord.helpers;

namespace Dbord.login
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            {
                // Check captcha first
                string enteredCaptcha = txtCaptcha.Text.Trim();
                string sessionCaptcha = Session["CaptchaCode"] as string;

                if (string.IsNullOrEmpty(sessionCaptcha) || enteredCaptcha != sessionCaptcha)
                {
                    Response.Write("<script>alert('Invalid captcha. Please try again.');</script>");
                    return;
                }

                string usernameOrEmail = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();

                DatabaseHelper db = new DatabaseHelper();

                SqlParameter[] parameters = new SqlParameter[]
                {
        new SqlParameter("@UsernameOrEmail", usernameOrEmail)
                };

                DataTable dt = db.ExecuteQuery("sp_GetUserByUsernameOrEmail", parameters);

                if (dt.Rows.Count == 0)
                {
                    Response.Write("<script>alert('User not found.');</script>");
                    return;
                }

                DataRow row = dt.Rows[0];
                string dbHash = row["PasswordHash"].ToString();
                string salt = row["Salt"].ToString();
                bool isActive = Convert.ToBoolean(row["IsActive"]);
                int userId = Convert.ToInt32(row["UserID"]);

                if (!isActive)
                {
                    Response.Write("<script>alert('Your account is inactive.');</script>");
                    return;
                }

                string hashedPassword = PasswordHelper.HashPassword(password, salt);

                if (hashedPassword == dbHash)
                {
                    SqlParameter[] updateParams = new SqlParameter[]
                    {
            new SqlParameter("@UserID", userId)
                    };
                    db.ExecuteNonQuery("sp_UpdateLastLogin", updateParams);

                    Session["UserID"] = userId;
                    Response.Redirect("~/View/Admin/Dashboard.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Invalid credentials.');</script>");
                }
            }
        }
    }
}
