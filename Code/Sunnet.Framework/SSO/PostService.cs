using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sunnet.Framework.SSO
{
    public class PostService
    {
        private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        public string Url = "";

        /// <summary>
        /// 添加需要提交的名和值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            Inputs.Add(name, value);
        }

        /// <summary>
        /// 脚本跳转
        /// </summary>
        public string Post()
        {
            System.Web.HttpContext.Current.Response.Clear();

            StringBuilder sb = new StringBuilder();

            sb.Append("<script type='text/javascript'>")
                .AppendFormat("location ='{0}?",Url);
            
            for (int i = 0; i < Inputs.Keys.Count; i++)
            {
                if (i == 0) sb.AppendFormat("{0}={1}", Inputs.Keys[i], Inputs[Inputs.Keys[i]]);
                else
                    sb.AppendFormat("&{0}={1}", Inputs.Keys[i], Inputs[Inputs.Keys[i]]);
            }
            sb.Append("';");
            sb.Append("</script>");
            return sb.ToString();
        }
    }
}