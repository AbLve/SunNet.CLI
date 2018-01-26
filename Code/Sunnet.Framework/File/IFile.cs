using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sunnet.Framework.File
{
    public interface IFile
    {
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="physicalSource">物理地址</param>
        /// <param name="physicalDestination">物理地址</param>
        void Move(string physicalSource, string physicalDestination);

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="physicalSource">物理地址</param>
        /// <param name="physicalDestination">物理地址</param>
        void Copy(string physicalSource, string physicalDestination);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="physicalPath">物理地址</param>
        /// <returns></returns>
        bool Delete(string physicalPath);

        /// <summary>
        /// 删除文件及缩略图
        /// </summary>
        /// <param name="physicalPath">物理地址</param>
        /// <param name="physicalThumbPath">物理地址</param>
        /// <returns></returns>
        bool Delete(string physicalPath, string physicalThumbPath);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileData">HttpPostedFile</param>
        /// <param name="filePhysicalPath">保存文件的物理地址</param>
        /// <param name="virtualDir">保存文件的虚拟目录</param>
        /// <returns></returns>
        FileEntity Upload(HttpPostedFile fileData, string filePhysicalPath, string virtualDir="");

        /// <summary>
        /// 上传文件，并且用 DES 进行加密
        /// </summary>
        /// <param name="fileData">HttpPostedFile</param>
        /// <param name="filePhysicalPath">保存文件的物理地址</param>
        /// <param name="virtualDir">保存文件的虚拟目录</param>
        /// <returns></returns>
        FileEntity DESEncryptUploadFile(HttpPostedFile fileData, string filePhysicalPath, string virtualDir = "");


        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePhysicalPath">物理路径</param>
        /// <returns></returns>
        byte[] DESDecryptFile(string filePhysicalPath);


         /// <summary>
        /// 上传视频,且生成一张视频介绍图片
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="filePhysicalPath"></param>
        /// <param name="virtualDir"></param>
        /// <param name="thumbnailExtension"></param>
        /// <returns></returns>
        FileEntity UploadVideo(HttpPostedFile fileData, string filePhysicalPath, string virtualDir = "", string thumbnailExtension = ".jpg");

        FileEntity UploadImageThumbnailByWidth
          (HttpPostedFile fileData, string filePhysicalPath, int width, int height, string virtualDir = "", string thumbnailExtension = ".jpg");
       
       FileEntity UploadImageThumbnail2
           (HttpPostedFile fileData, string filePhysicalPath, int width, int height, string virtualDir = "", string thumbnailExtension = ".jpg");
       
        FileEntity UploadCutImage(string inputFileName, string outputFileName, string fileFolder, string filePhysicalPath, int toWidth, int toHeight, int cropWidth, int cropHeight, int x, int y);
    }
}
