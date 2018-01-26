using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using WebGrease.Css.Extensions;

namespace Sunnet.Cli.UIBase
{
    public static class FileHelper
    {
        /// <summary>
        /// Gets the file last modified.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>File's last modified date or now</returns>
        public static DateTime GetFileLastModified(string fileFullPath)
        {
            var file = new FileInfo(fileFullPath);
            if (file.Exists)
                return file.LastWriteTimeUtc;
            return DateTime.Now;
        }

        public static string GetPreviewPathofUploadFileSameDomain(string uploadedPath)
        {
            try
            {
                if (string.IsNullOrEmpty(uploadedPath))
                {
                    return string.Empty;
                }
                var folder =
                    SFConfig.UploadFile.Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
                if (uploadedPath.StartsWith("/") || uploadedPath.StartsWith("\\"))
                    uploadedPath = uploadedPath.Substring(1);
                return ("/" + folder + "/" + uploadedPath).ToLower();
            }
            catch (IOException)
            {
                return "";
            }
            catch
            {
                return "";
            }
        }
        public static string GetPreviewPathofUploadFile(string uploadedPath)
        {
            try
            {
                if (string.IsNullOrEmpty(uploadedPath))
                {
                    return string.Empty;
                }
                var folder =
                    SFConfig.UploadFile.Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
                if (uploadedPath.StartsWith("/") || uploadedPath.StartsWith("\\"))
                    uploadedPath = uploadedPath.Substring(1);
                return Path.Combine(DomainHelper.StaticSiteDomain.AbsoluteUri, folder, uploadedPath);
            }
            catch (IOException)
            {

                return "";
            }
            catch
            {
                return "";
            }
        }

        public static string SaveProtectedFile(HttpPostedFileBase uploadedFile, string module)
        {
            var path = SFConfig.ProtectedFiles + module;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + uploadedFile.FileName.GetHashCode() +
                           Path.GetExtension(uploadedFile.FileName);
            path += "/" + filename;
            var virtualPath = module + "/" + filename;
            uploadedFile.SaveAs(path);
            return virtualPath;
        }

        public static string GetProtectedFilePhisycalPath(string file)
        {
            var fileFullPath = Path.Combine(SFConfig.ProtectedFiles, file);
            var dire = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(dire)) Directory.CreateDirectory(dire);
            return fileFullPath;
        }

        /// <summary>
        /// 查看本地是否已存在相关文件,如果存在则返回文件全路径.
        /// </summary>
        /// <param name="filename">相对于<see cref="SFConfig.ProtectedFiles"/>路径的文件名.</param>
        /// <returns>如果不存在, 返回 null</returns>
        public static string HasProtectedFile(string filename)
        {
            var fullpath = Path.Combine(SFConfig.ProtectedFiles, filename);
            return File.Exists(fullpath) ? fullpath : null;
        }

        private static Dictionary<string, string> ResponseContentType
        {
            get
            {
                var dics = new Dictionary<string, string>();
                dics.Add(".jpe", "image/jpeg");
                dics.Add(".jpeg", "image/jpeg");
                dics.Add(".jpg", "image/jpeg");
                dics.Add(".gif", "image/gif");
                dics.Add(".xls", "application/ms-excel");
                dics.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                dics.Add(".doc", "application/msword");
                dics.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
                dics.Add(".pdf", "application/pdf");
                dics.Add(".zip", "application/x-compressed");
                return dics;
            }
        }

        /// <summary>
        /// 把文件输出到客户端.
        /// </summary>
        /// <param name="filePath">文件的绝对路径.</param>
        /// <param name="fileName">输出文件名.</param>
        /// <param name="contentType">输出文件minetype.</param>
        /// <param name="deleteAfterResponsed">输出完成后是否删除</param>
        public static void ResponseFile(string filePath, string fileName, string contentType = "", bool deleteAfterResponsed = false)
        {
            if (HttpContext.Current == null)
                return;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;
            if (string.IsNullOrEmpty(contentType))
            {
                var ext = Path.GetExtension(filePath);
                if (ResponseContentType.ContainsKey(ext))
                    contentType = ResponseContentType[ext];
                else
                    throw new Exception("Please set ContentType for file:" + filePath);
            }
            var fileInfo = new FileInfo(filePath);
            var stream = fileInfo.OpenRead();
            var length = stream.Length;
            var Response = HttpContext.Current.Response;
            Response.Clear();
            Response.Charset = "utf-8";
            Response.Buffer = false;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            //在Firefox中，保存时文件名中空格后面的内容会被截断
            Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            Response.ContentType = contentType;

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)length);
            Response.AddHeader("Content-Length", length.ToString());
            Response.OutputStream.Write(bytes, 0, (int)length);
            Response.OutputStream.Flush();
            Response.End();
            stream.Dispose();
            if (fileInfo.Exists && deleteAfterResponsed)
                fileInfo.Delete();
        }

        /// <summary>
        /// 用于VCW站点下载大文件时，分块下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="deleteAfterResponsed"></param>
        public static void ResponseFileWithBlock(string filePath, string fileName, string contentType = "", bool deleteAfterResponsed = false)
        {
            if (HttpContext.Current == null)
                return;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;
            if (string.IsNullOrEmpty(contentType))
            {
                var ext = Path.GetExtension(filePath);
                if (ResponseContentType.ContainsKey(ext))
                    contentType = ResponseContentType[ext];
                else
                    throw new Exception("Please set ContentType for file:" + filePath);
            }

            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                var Response = HttpContext.Current.Response;
                int ChunkSize = SFConfig.ChunkSize == 0 ? 1048576 : SFConfig.ChunkSize;
                byte[] buffer = new byte[ChunkSize];

                Response.Clear();
                Response.Charset = "utf-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                FileStream iStream = File.OpenRead(filePath);
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, ChunkSize);//读取的大小
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.End();
                if (fileInfo.Exists && deleteAfterResponsed)
                    fileInfo.Delete();
            }
        }

        public static List<string> GetFiles(string folder, string pattern = "*.jpg;*.jpeg;*.png;*.bmp;*.gif")
        {
            folder = Path.Combine(SFConfig.UploadFile, folder);
            var files = new List<string>();
            var patterns = pattern.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            patterns.ForEach(x => FindFiles(folder, files, x));
            var uploaderFolderName =
                   SFConfig.UploadFile.Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
            files = files.Select(x => x = ("/" + uploaderFolderName + "/" + x.Replace(SFConfig.UploadFile, "").Replace("\\", "/")).ToLower()).ToList();
            return files;
        }

        private static void FindFiles(string folder, List<string> files, string pattern)
        {
            if (files == null)
                files = new List<string>();
            files.AddRange(Directory.GetFiles(folder, pattern, SearchOption.AllDirectories));
        }


    }
}