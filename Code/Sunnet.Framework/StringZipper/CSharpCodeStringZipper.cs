using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Sunnet.Framework.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.StringZipper
{
    public class CSharpCodeStringZipper : IStringZipper
    {
        public string Zip(string uncompressedString)
        {
            byte[] bytData = System.Text.Encoding.Unicode.GetBytes(uncompressedString);
            MemoryStream ms = new MemoryStream();
            Stream s = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(ms);
            s.Write(bytData, 0, bytData.Length);
            s.Close();
            byte[] compressedData = (byte[])ms.ToArray();
            return System.Convert.ToBase64String(compressedData, 0, compressedData.Length);
        }

        public string UnZip(string compressedString)
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

        // Recurses down the folder structure
        //
        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = System.IO.File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        /// <param name="destDir">压缩文件存放路径.</param>
        /// <param name="fileName">压缩文件名, 相对于<paramref name="destDir"/>.</param>
        /// <param name="folderToCompress">要压缩的文件夹.</param>
        /// <returns></returns>
        /// Author : JackZhang
        /// Date   : 6/25/2015 15:45:22
        public static bool CreateZip(string destDir, string fileName, string folderToCompress)
        {
            if (!System.IO.Directory.Exists(destDir))
            {
                System.IO.Directory.CreateDirectory(destDir);
            }
            var zipPath = Path.Combine(destDir, fileName);
            FileStream fsOut = System.IO.File.Create(zipPath);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(9); //0-9, 9 being the highest level of compression

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderToCompress.Length + (folderToCompress.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderToCompress, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
            return true;
        }

    }
}
