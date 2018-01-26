using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using System.IO;
using Sunnet.Cli.UIBase;

namespace Sunnet.Cli.Vcw.Controllers
{
    public class DownLoadFileController : BaseController
    {
        //
        // GET: /DownLoadFile/DownLoadFile
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult DownLoadFile(string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return View();
            }
            else
            {
                string fileExtension = string.Empty;
                string mimeType = string.Empty;
                if (filePath.IndexOf('.') > -1)
                {
                    fileExtension = filePath.Substring(filePath.LastIndexOf('.') + 1);
                }
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    switch (fileExtension.ToLower())
                    {
                        case "doc":
                            mimeType = "application/msword";
                            break;
                        case "docx":
                            mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        case "xls":
                            mimeType = "application/vnd.ms-excel";
                            break;
                        case "xlsx":
                            mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            break;
                        case "csv":
                            mimeType = "application/octet-stream";
                            break;
                        case "rtf":
                        case "rtfd":
                            mimeType = "application/rtf";
                            break;
                        case "txt":
                            mimeType = "text/plain";
                            break;
                        case "ppt":
                            mimeType = "application/vnd.ms-powerpoint";
                            break;
                        case "pptx":
                            mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                            break;
                        case "pdf":
                            mimeType = "application/pdf";
                            break;
                        case "png":
                            mimeType = "image/png";
                            break;
                        case "gif":
                            mimeType = "image/gif";
                            break;
                        case "jpg":
                        case "jpeg":
                            mimeType = "image/jpeg";
                            break;
                        case "mp4":
                            mimeType = "application/octet-stream";
                            break;
                        case "wmv":
                            mimeType = "video/x-ms-wmv";
                            break;
                        case "mov":
                            mimeType = "video/quicktime";
                            break;
                        case "m4v":
                            mimeType = "video/x-m4v";
                            break;
                    }
                    if (string.IsNullOrEmpty(mimeType))
                    {
                        return View();
                    }
                    else
                    {
                        string file_name = string.Empty;
                        if (string.IsNullOrEmpty(fileName))
                        {
                            file_name = "DownLoadFile." + fileExtension;
                        }
                        else
                        {
                            // 若类型文件名称格式为 test.mp4(17.70M)
                            if (fileName.LastIndexOf(')') > 0 && fileName.LastIndexOf(')') > fileName.LastIndexOf('.'))
                                file_name = fileName.Substring(0, fileName.LastIndexOf('('));
                            // 若类型文件名称格式为 test.mp4
                            else
                                file_name = fileName;
                        }
                        string physicPath = string.Format("{0}/{1}", SFConfig.UploadFile, filePath);
                        FileHelper.ResponseFileWithBlock(physicPath, file_name, mimeType, false);
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
        }
    }
}