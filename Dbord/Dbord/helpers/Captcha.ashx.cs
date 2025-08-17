
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
namespace Dbord.helpers
{
    public class Captcha : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // generate random text
            string captchaCode = GenerateCode(5);
            context.Session["CaptchaCode"] = captchaCode;

            using (Bitmap bmp = new Bitmap(120, 40))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Font font = new Font("Arial", 18, FontStyle.Bold))
            {
                g.Clear(Color.White);
                g.DrawString(captchaCode, font, Brushes.Black, 10, 5);

                // little noise
                Random rnd = new Random();
                for (int i = 0; i < 6; i++)
                {
                    g.DrawLine(Pens.Gray, rnd.Next(bmp.Width), rnd.Next(bmp.Height),
                                          rnd.Next(bmp.Width), rnd.Next(bmp.Height));
                }

                context.Response.ContentType = "image/png";
                bmp.Save(context.Response.OutputStream, ImageFormat.Png);
            }
        }

        public bool IsReusable => false;

        private string GenerateCode(int len)
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Random rnd = new Random();
            char[] buffer = new char[len];
            for (int i = 0; i < len; i++)
                buffer[i] = chars[rnd.Next(chars.Length)];
            return new string(buffer);
        }
    }
}