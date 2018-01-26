using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.Helpers
{
    public static  class CSharpZipper
    {
        public static string Zip(string uncompressedString)
        {
            byte[] bytData = System.Text.Encoding.Unicode.GetBytes(uncompressedString);
            MemoryStream ms = new MemoryStream();
            Stream s = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(ms);
            s.Write(bytData, 0, bytData.Length);
            s.Close();
            byte[] compressedData = (byte[])ms.ToArray();
            return System.Convert.ToBase64String(compressedData, 0, compressedData.Length);
        }

        public static string UnZip(string compressedString)
        {
            System.Text.StringBuilder uncompressedString = new System.Text.StringBuilder();
            int totalLength = 0;
            byte[] bytInput = System.Convert.FromBase64String(compressedString); ;
            byte[] writeData = new byte[4096];
            Stream s2 = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(new MemoryStream(bytInput));
            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    totalLength += size;
                    uncompressedString.Append(System.Text.Encoding.Unicode.GetString(writeData, 0, size));
                }
                else
                {
                    break;
                }
            }
            s2.Close();
            return uncompressedString.ToString();
        }

        /// <summary>
        /// 创建zip  directories 和 filenames 是对应的
        /// </summary>
        /// <param name="directories">文件目录</param>
        /// <param name="filenames">文件路径</param>
        /// <param name="zipFileName">文件名及路径</param>
        /// <param name="dic">所在目录</param>
        /// <param>进行对加密文件压缩下载</param>
        public static bool CreateZip(List<string> directories, IEnumerable<string> filenames, IEnumerable<string> displayNames, string zipFileName, string dir)
        {
            if (!System.IO.Directory.Exists(dir + "/zip/"))
            {
                System.IO.Directory.CreateDirectory(dir + "/zip/");
            }
            using (ZipOutputStream ZipStream = new ZipOutputStream(System.IO.File.Create(dir + "/zip/" + zipFileName)))
            {
                ZipStream.SetLevel(9);
                ZipEntryFactory factory = new ZipEntryFactory();
                foreach (var directory in directories)
                {
                    if (!string.IsNullOrEmpty(directory))
                    {
                        string virtualDirectory = directory;
                        ZipEntry zipEntry = factory.MakeDirectoryEntry(virtualDirectory);
                        zipEntry.DateTime = DateTime.Now;
                        ZipStream.PutNextEntry(zipEntry);
                    }
                }

                byte[] buffer = new byte[4096];
                for (int i = 0; i < filenames.Count(); i++)
                {
                    string file = filenames.ElementAt(i);
                    if (!string.IsNullOrEmpty(file))
                    {
                        string newfileName = displayNames.ElementAt(i);
                        ZipEntry entry;
                        if (!string.IsNullOrEmpty(directories[i]))
                        {
                            entry = factory.MakeFileEntry(directories[i] + "//" + newfileName);
                        }
                        else
                        {
                            entry = factory.MakeFileEntry(newfileName);
                        }

                        entry.DateTime = DateTime.Now;
                        ZipStream.PutNextEntry(entry);
                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                }
                ZipStream.Finish();
                ZipStream.Close();
            }

            System.Web.HttpContext.Current.Response.ContentType = "application/x-compress zip";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + zipFileName);
            System.Web.HttpContext.Current.Response.TransmitFile(dir + "/zip/" + zipFileName);
            return true;
        }
    }
}
