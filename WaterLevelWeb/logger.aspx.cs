using System;
using System.Collections.Specialized;

namespace WaterLevelWeb
{
    public partial class logger : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection nvc = Request.Form;
            string level = string.Empty;
            string temp = string.Empty;
            string uid = string.Empty;

            if (!string.IsNullOrEmpty(nvc["uid"]))
            {
                uid = nvc["uid"];
            }

            if (uid == "RandomStringNeedsToBeTheSameOnArduinoAndInVisualStudio")
            {
                if (!string.IsNullOrEmpty(nvc["level"]))
                {
                    level = nvc["level"];
                }

                if (!string.IsNullOrEmpty(nvc["temp"]))
                {
                    temp = nvc["temp"];
                }

                if (!string.IsNullOrEmpty(level) || !string.IsNullOrEmpty(temp))
                {
                    Response.Write(dataAccess.insertData(
                                decimal.Parse(level),
                                decimal.Parse(temp)));
                }
                else
                {
                    Response.Write("False");
                }
            }
            else
            {
                Response.Write("Invalid ID");
            }
        }
    }
}