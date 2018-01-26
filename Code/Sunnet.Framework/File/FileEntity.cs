using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.File
{
    public class FileEntity
    {
        public FileEntity()
        {
            Result = true;
        }

        /// <summary>
        /// 上传文件的原名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 保存后的文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件的绝对路径(包括 FileName)
        /// </summary>
        public string FilePhysicalPath { get; set; }
        public int Size { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 文件的MIME内容类型。
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 上传文件的扩展名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 文件的虚拟路径(包括 FileName)
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 缩略图 的虚路径（包括文件名)
        /// </summary>
        public string ImgPath { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        /// <summary>
        /// 还回结果，是否操作成功
        /// </summary>
        public bool Result { get; set; }
    }
}
