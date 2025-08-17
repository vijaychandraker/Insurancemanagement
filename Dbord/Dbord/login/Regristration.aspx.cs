using System;
using System.Data.SqlClient;
using Dbord.helpers;

namespace Dbord.login
{
    public partial class Regristration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (password != confirmPassword)
            {
                Response.Write("<script>alert('Passwords do not match.');</script>");
                return;
            }

            // Generate salt & hash password
            string salt = PasswordHelper.GenerateSalt();
            string hashedPassword = PasswordHelper.HashPassword(password, salt);

            // Create parameters for stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", hashedPassword),
                new SqlParameter("@Salt", salt)
            };

            DatabaseHelper db = new DatabaseHelper();
            int result = db.ExecuteNonQuery("sp_InsertUser", parameters);

            if (result == -1)
            {
                Response.Write("<script>alert('Username or Email already exists.');</script>");
            }
            else
            {
                Response.Write("<script>alert('Registration successful!'); window.location='Login.aspx';</script>");
            }
        }
    }
}
