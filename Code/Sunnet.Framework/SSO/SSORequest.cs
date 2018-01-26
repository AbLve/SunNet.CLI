using Sunnet.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Sunnet.Framework.SSO
{
    [Serializable]
    public class SSORequest : MarshalByRefObject
    {
        /// <summary>
        /// 各独立站点标识ID
        /// </summary>
        public string IASID; 
        
        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp;    

        /// <summary>
        /// 各独立站点的访问地址
        /// </summary>
        public string AppUrl;        

        /// <summary>
        /// 各独立站点的 Token
        /// </summary>
        public string Authenticator; 

        /// <summary>
        /// 账号
        /// </summary>
        public string UserAccount;

        /// <summary>
        /// 额外加的，不参与到加密中
        /// </summary>
        public string Email;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password;      

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress;     

        /// <summary>
        /// 为ssresponse对象做准备
        /// 用户认证通过,认证失败,包数据格式不正确,数据校验不正确
        /// </summary>
        public string ErrorDescription = "认证失败";   
        public int Result = -1;

        public SSORequest()
        {

        }


        /// <summary>
        /// 获取当前页面上的SSORequest对象
        /// </summary>
        /// <param name="CurrentPage"></param>
        /// <returns></returns>
        public static SSORequest GetRequest(Page CurrentPage)
        {
            SSORequest request = new SSORequest();
            request.IPAddress = CommonHelper.GetIPAddress(CurrentPage.Request);
            request.IASID = CurrentPage.Request["IASID"].ToString();// Request本身会Decode
            request.UserAccount = CurrentPage.Request["UserAccount"].ToString();//this.Text
            request.Password = CurrentPage.Request["Password"].ToString();
            request.AppUrl = CurrentPage.Request["AppUrl"].ToString();
            request.Authenticator = CurrentPage.Request["Authenticator"].ToString();
            request.TimeStamp = CurrentPage.Request["TimeStamp"].ToString();
            return request;
        }
    }
}
