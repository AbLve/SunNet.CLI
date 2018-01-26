using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Sunnet.Framework.File
{
    internal class RealFileSystem : IFile
    {
        private readonly ISunnetLog _log;
        public RealFileSystem(ISunnetLog log)
        {
            this._log = log;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="physicalSource">物理地址</param>
        /// <param name="physicalDestination">物理地址</param>
        public void Move(string physicalSource, string physicalDestination)
        {
            System.IO.File.Move(physicalSource, physicalDestination);
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="physicalSource">物理地址</param>
        /// <param name="physicalDestination">物理地址</param>
        public void Copy(string physicalSource, string physicalDestination)
        {
            System.IO.File.Copy(physicalSource, physicalDestination);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="physicalPath">物理地址</param>
        /// <returns></returns>
        public bool Delete(string filePath)
        {
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除文件及缩略图
        /// </summary>
        /// <param name="physicalPath">物理地址</param>
        /// <param name="physicalThumbPath">物理地址</param>
        /// <returns></returns>
        public bool Delete(string filePath, string thumbPath)
        {
            try
            {
                System.IO.File.Delete(filePath);
                System.IO.File.Delete(thumbPath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileData">HttpPostedFile</param>
        /// <param name="filePhysicalPath">保存文件的物理地址</param>
        /// <param name="virtualDir">保存文件的虚拟目录</param>
        /// <returns></returns>
        public FileEntity Upload(HttpPostedFile fileData, string filePhysicalPath, string virtualDir = "")
        {
            FileEntity file = new FileEntity();
            try
            {
                string filePath = filePhysicalPath + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                file.DisplayName = Path.GetFileName(fileData.FileName);
                file.Extension = Path.GetExtension(file.DisplayName);
                file.Size = fileData.ContentLength;
                file.ContentType = fileData.ContentType;
                file.CreateTime = DateTime.Now;
                file.FileName = GetFileName(file.Extension);
                file.FilePath = "/" + virtualDir + "/" + file.FileName;
                file.FilePhysicalPath = string.Format(@"{0}\{1}", filePhysicalPath, file.FileName);
                fileData.SaveAs(file.FilePhysicalPath);
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                file.Result = false;
            }
            return file;
        }


        /// <summary>
        /// 上传文件，并且用 DES 进行加密
        /// </summary>
        /// <param name="fileData">HttpPostedFile</param>
        /// <param name="filePhysicalPath">保存文件的物理地址</param>
        /// <param name="virtualDir">保存文件的虚拟目录</param>
        /// <returns></returns>
        public FileEntity DESEncryptUploadFile(HttpPostedFile fileData, string filePhysicalPath, string virtualDir = "")
        {
            FileEntity file = new FileEntity();
            try
            {
                string filePath = filePhysicalPath + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                file.DisplayName = Path.GetFileName(fileData.FileName);
                file.Extension = Path.GetExtension(file.DisplayName);
                file.Size = fileData.ContentLength;
                file.ContentType = fileData.ContentType;
                file.CreateTime = DateTime.Now;
                file.FileName = GetFileName(file.Extension);
                file.FilePath = "/" + virtualDir + "/" + file.FileName;
                file.FilePhysicalPath = string.Format(@"{0}\{1}", filePhysicalPath, file.FileName);

                string desKey = "sunneTUs";
                string desIV = "sunneT.U";
                byte[] bytes = new byte[fileData.InputStream.Length];
                fileData.InputStream.Read(bytes, 0, bytes.Length);
                fileData.InputStream.Close();
                FileStream fileStream = new FileStream(file.FilePhysicalPath, FileMode.OpenOrCreate, FileAccess.Write);
                DES des = new DESCryptoServiceProvider();
                CryptoStream cryptoStream = new CryptoStream(fileStream, des.CreateEncryptor(Encoding.Default.GetBytes(desKey), Encoding.Default.GetBytes(desIV)), CryptoStreamMode.Write);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                file.Result = false;
            }
            return file;
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePhysicalPath">物理路径</param>
        /// <returns></returns>
        public byte[] DESDecryptFile(string filePhysicalPath)
        {
            if (System.IO.File.Exists(filePhysicalPath))
            {
                try
                {
                    string desKey = "sunneTUs";
                    string desIV = "sunneT.U";
                    FileStream fileStream = System.IO.File.OpenRead(filePhysicalPath);
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    fileStream.Close();
                    DES des = new DESCryptoServiceProvider();
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(Encoding.Default.GetBytes(desKey), Encoding.Default.GetBytes(desIV)), CryptoStreamMode.Write);
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Position = 0;
                    memoryStream.Read(buffer, 0, buffer.Length);
                    memoryStream.Close();
                    cryptoStream.Close();
                    return buffer;
                }
                catch (Exception ex)
                {
                    _log.Debug(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上传视频,且生成一张视频介绍图片
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="filePhysicalPath"></param>
        /// <param name="virtualDir"></param>
        /// <param name="thumbnailExtension"></param>
        /// <returns></returns>
        public FileEntity UploadVideo(HttpPostedFile fileData, string filePhysicalPath, string virtualDir = "", string thumbnailExtension = ".jpg")
        {
            FileEntity file = new FileEntity();
            try
            {
                string filePath = filePhysicalPath + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                file.DisplayName = Path.GetFileName(fileData.FileName);
                file.Extension = Path.GetExtension(file.DisplayName);
                file.Size = fileData.ContentLength;
                file.ContentType = fileData.ContentType;
                file.CreateTime = DateTime.Now;
                file.FileName = GetFileName(file.Extension);
                string imgName = GetFileName(thumbnailExtension);
                file.FilePath = "/" + virtualDir + "/" + file.FileName;
                file.FilePhysicalPath = string.Format(@"{0}\{1}", filePhysicalPath, file.FileName);
                fileData.SaveAs(file.FilePhysicalPath);
                CatchImg(file.FilePhysicalPath, filePath + imgName);
                file.ImgPath = "/" + virtualDir + "/" + imgName;
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                file.Result = false;
            }
            return file;
        }


        /// <summary>
        /// general video jpg.
        /// </summary>
        /// <param name="fileName">视频物理路径</param>
        /// <param name="imgFile">需要生成介绍图片的物理路径</param>
        /// <returns></returns>
        private string CatchImg(string fileName, string imgFile)
        {
            string ffmpeg = SFConfig.Ffmpeg;
            string flv_img = imgFile;
            string FlvImgSize = SFConfig.CatchFlvImgSize;
            System.Diagnostics.ProcessStartInfo ImgstartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            ImgstartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            ImgstartInfo.Arguments = "  -i  " + fileName + "  -y  -f  image2  -ss 1 -vframes 1  -s  " + FlvImgSize + " " + flv_img;
            try
            {
                System.Diagnostics.Process.Start(ImgstartInfo).WaitForExit();
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                return "";
            }
            if (System.IO.File.Exists(flv_img))
            {
                return flv_img;
            }

            return "";
        }


        public FileEntity UploadImageThumbnailByWidth
         (HttpPostedFile fileData, string filePhysicalPath, int width, int height, string virtualDir = "", string thumbnailExtension = ".jpg")
        {
            ThumnailMode mode = ThumnailMode.EqualW;

            FileEntity file = new FileEntity();
            try
            {
                string filePath = filePhysicalPath + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                file.DisplayName = Path.GetFileName(fileData.FileName);
                file.Extension = Path.GetExtension(file.DisplayName);
                file.Size = fileData.ContentLength;
                file.ContentType = fileData.ContentType;
                file.CreateTime = DateTime.Now;
                file.FileName = GetFileName(file.Extension);
                file.FilePath = "/" + virtualDir + "/" + file.FileName;
                file.FilePhysicalPath = string.Format(@"{0}\{1}", filePhysicalPath, file.FileName);

                Image image = Image.FromStream(fileData.InputStream);
                int outWidth = 0;
                int outHeight = 0;
                ThumbnailHelper.CreateOutWAndH(image, file.FilePhysicalPath, width, height, mode, thumbnailExtension, out outWidth, out outHeight);

                file.ImageWidth = outWidth;
                file.ImageHeight = outHeight;

            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                file.Result = false;
            }
            return file;
        }

        /// 生成缩略图保持原来的比例
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileFolder"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public FileEntity UploadImageThumbnail2
           (HttpPostedFile fileData, string filePhysicalPath, int width, int height, string virtualDir = "", string thumbnailExtension = ".jpg")
        {
            ThumnailMode mode = ThumnailMode.HW;
            string fileType = "jpg";

            FileEntity file = new FileEntity();
            try
            {
                string filePath = filePhysicalPath + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                file.DisplayName = Path.GetFileName(fileData.FileName);
                file.Extension = Path.GetExtension(file.DisplayName);
                file.Size = fileData.ContentLength;
                file.ContentType = fileData.ContentType;
                file.CreateTime = DateTime.Now;
                file.FileName = GetFileName(file.Extension);
                file.FilePath = "/" + virtualDir + "/" + file.FileName;
                file.FilePhysicalPath = string.Format(@"{0}\{1}", filePhysicalPath, file.FileName);

                Image image = Image.FromStream(fileData.InputStream);
                mode = ThumnailMode.Cut;
                ThumbnailHelper.Create(image, file.FilePhysicalPath, width, height, mode, fileType);
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                file.Result = false;
            }
            return file;
        }



        public FileEntity UploadCutImage(string inputFileName, string outputFileName, string fileFolder, string filePhysicalPath, int toWidth, int toHeight, int cropWidth, int cropHeight, int x, int y)
        {
            string fileType = "jpg";

            FileEntity file = new FileEntity();
            try
            {
                string filePath = filePhysicalPath + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                file.DisplayName = Path.GetFileName(outputFileName);
                file.Extension = Path.GetExtension(file.DisplayName);
                file.CreateTime = DateTime.Now;
                file.FileName = GetFileName(file.Extension);
                file.FilePath = "/" + fileFolder + "/" + file.FileName;
                file.ImgPath = file.FilePath;

                ThumbnailHelper.Cut(inputFileName, filePath + file.FileName, toWidth, toHeight, cropWidth
                    , cropHeight, x, y, fileType);

            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                file.Result = false;
            }
            return file;
        }


        private string GetFileName(string fileExt)
        {
            int rand = new Random().Next(1000, 9999);
            return DateTime.Now.ToString("yyMMddHHmmssffff") + "_" + rand + fileExt;
        }



    }
}
