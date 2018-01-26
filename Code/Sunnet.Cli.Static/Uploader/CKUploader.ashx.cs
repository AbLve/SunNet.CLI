using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;

namespace Sunnet.Cli.Static.Uploader
{
    /// <summary>
    /// Summary description for CKUploader
    /// </summary>
    public class CKUploader : IHttpHandler
    {
        HttpRequest httpRequest;
        string uploadDir = "../Upload/ck_images";
        public void ProcessRequest(HttpContext context)
        {
            httpRequest = context.Request;
            string funcNum = httpRequest.QueryString["CKEditorFuncNum"];
            string fileName = null;
            string errorMsg = null;
            string extName = string.Empty;
            try
            {
                HttpPostedFile myfile = httpRequest.Files[0];
                string[] imgType = { "image/png", "image/PNG", "image/GIF", "image/gif", "image/jpeg", "image/JPEG", "image/bmp", "image/BMP" };
                if (imgType.Contains(myfile.ContentType))
                {
                    GetFileName(myfile.FileName, ref extName);
                    fileName = Guid.NewGuid().ToString() + "." + extName;

                    string uploadPhysicalDir = HttpContext.Current.Server.MapPath(uploadDir);
                    if (!Directory.Exists(uploadPhysicalDir))
                    {
                        Directory.CreateDirectory(uploadPhysicalDir);
                    }
                    myfile.SaveAs(uploadPhysicalDir + "\\" + fileName);
                    errorMsg = null;
                }
                else
                {
                    errorMsg = "Please select a png, gif, jpeg, bmp format picture.";
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            string mainSiteDomain = SFConfig.MainSiteDomain;

            context.Response.Redirect(mainSiteDomain + "Community/Community/CKUploadImage?funcNum=" + funcNum + "&fileName=" + fileName + "&errorMsg=" + errorMsg);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void GetFileName(string fullName, ref string extName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                extName = null;
            }
            else
            {
                int last;
                last = fullName.LastIndexOf(@".");
                extName = fullName.Substring(last + 1, fullName.Length - last - 1);
            }
        }
    }
}