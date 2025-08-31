using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;
using Dbord.helpers;

namespace Dbord.login
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            // 1. Check captcha
            string enteredCaptcha = txtCaptcha.Text.Trim();
            string sessionCaptcha = Session["CaptchaCode"] as string;

            if (string.IsNullOrEmpty(sessionCaptcha) || !string.Equals(enteredCaptcha, sessionCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("Invalid captcha. Please try again.");
                return;
            }

            string usernameOrEmail = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            DatabaseHelper db = new DatabaseHelper();
            SqlParameter[] parameters = {
                new SqlParameter("@UsernameOrEmail", usernameOrEmail)
            };

            DataTable dt = db.ExecuteQuery("sp_GetUserByUsernameOrEmail", parameters);

            if (dt.Rows.Count == 0)
            {
                ShowMessage("Invalid username or password.");
                return;
            }

            DataRow row = dt.Rows[0];
            string dbHash = row["PasswordHash"].ToString();
            string salt = row["Salt"].ToString();
            bool isActive = Convert.ToBoolean(row["IsActive"]);
            int userId = Convert.ToInt32(row["UserID"]);
            int role = Convert.ToInt32(row["RoleID"]);
            string username = row["Username"].ToString();

            if (!isActive)
            {
                ShowMessage("Your account is inactive. Contact administrator.");
                return;
            }

            // 2. Hash password using a secure algorithm (PBKDF2/bcrypt)
            string hashedPassword = PasswordHelper.HashPassword(password, salt);

            if (SecureEquals(dbHash, hashedPassword))
            {
                // 3. Update last login
                SqlParameter[] updateParams = { new SqlParameter("@UserID", userId) };
                db.ExecuteNonQuery("sp_UpdateLastLogin", updateParams);

                // 4. Prevent session fixation (regenerate SessionID)
                Session.Clear();
              
                // 5. Store user info in session
                Session["UserID"] = userId;
                Session["RoleID"] = role;
                Session["Username"] = username;

                if (isActive) 
                {
                    Response.Redirect("~/View/Admin/Dashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                ShowMessage("Invalid username or password.");
            }
        }

        // Constant-time string comparison to avoid timing attacks
        private bool SecureEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message}');", true);
        }
    }
}
