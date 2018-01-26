using StructureMap;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sunnet.Framework.File
{
    public static class FileHelper
    {
        static IFile f = new RealFileSystem(ObjectFactory.GetInstance<ISunnetLog>());

        public static void Move(string physicalSource, string physicalDestination)
        {
            f.Move(physicalSource, physicalDestination);
        }

        public static void Copy(string physicalSource, string physicalDestination)
        {
            f.Copy(physicalSource, physicalDestination);
        }

        public static bool Delete(string filePath)
        {
            return f.Delete(filePath);
        }

        public static bool Delete(string filePath, string thumbPath)
        {
            return f.Delete(filePath, thumbPath);
        }

        //public static FileEntity UploadReturnFileEntity(HttpPostedFile fileData, string fileFolder, string filePhysicalPath, string virtualDir)
        //{
        //    return f.UploadReturnFileEntity(fileData, fileFolder, filePhysicalPath, virtualDir);
        //}

        //public static string DESEncryptUploadFile(HttpPostedFile fileData, string fileFolder, string filePhysicalPath)
        //{
        //    return f.DESEncryptUploadFile(fileData, fileFolder, filePhysicalPath);
        //}

        //public static FileEntity DESEncryptUploadFileReturnFileEntity(HttpPostedFile fileData, string fileFolder, string filePhysicalPath, string virtualDir)
        //{
        //    return f.DESEncryptUploadFileReturnFileEntity(fileData, fileFolder, filePhysicalPath, virtualDir);
        //}

        public static byte[] DESDecryptFile(string filePhysicalPath)
        {
            return f.DESDecryptFile(filePhysicalPath);
        }

        //public static FileEntity UploadImageThumbnailByWidth
        //    (HttpPostedFile fileData, string fileFolder, string physicalpath, int width, int height, string prefix = "")
        //{
        //    return f.UploadImageThumbnailByWidth(fileData, fileFolder, physicalpath, width, height, prefix);
        //}

        /// <summary>
        /// 生成缩略图保持比例
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileFolder"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        //public static FileEntity UploadImageThumbnail2
        //   (HttpPostedFile fileData, string fileFolder, string filePhysicalPath, int width, int height, string prefix = "")
        //{
        //    return f.UploadImageThumbnail2(fileData, fileFolder, filePhysicalPath, width, height, prefix);
        //}

        public static FileEntity UploadCutImage
            (string inputFileName, string outputFileName, string fileFolder, string filePhysicalPath, int toWidth, int toHeight, int cropWidth, int cropHeight, int x, int y)
        {
            return f.UploadCutImage(inputFileName, outputFileName, fileFolder, filePhysicalPath, toWidth, toHeight, cropWidth, cropHeight, x, y);
        }
    }
}
