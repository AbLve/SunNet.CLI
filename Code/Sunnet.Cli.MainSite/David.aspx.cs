using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace David
{
    public partial class David : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Write("<p Align=center>");
            Response.Write("<h1>You are visting CLI Engage web server : <b>" + Server.MachineName + "</b></H1><br>");
            Response.Write("The IP host address of the remote client:" + Request.UserHostAddress + "<br>");

            if (Request.ServerVariables["HTTP_VIA"] != null) // using proxy
            {

                Response.Write("Your IP is " + Request.ServerVariables["HTTP_X_FORWARDED_FOR"] + "<br>");
            }

            Response.Write("</P>");


            Response.Write("Date Time:" + DateTime.Now + "<br>");



        }
    }
}