using System.Text;
using System.IO;
using System.Collections;
using System;

namespace BUP
{
    public class Helper
    {
        public static void CheckAndCreatePath(string filepath)
        {
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
        }

        public static void WriteLog(string path, ArrayList contents)
        {
            if (contents != null && contents.Count > 0)
            {
                using (StreamWriter write = new StreamWriter(path, true, Encoding.Default))
                {
                    foreach (string item in contents)
                    {
                        write.WriteLine(item);
                    }
                    write.WriteLine("----------------------------------------------------");
                }
            }
        }

        public static void RemoveFileToProcess(string path, string newPath, string name, int taskId)
        {
            CheckAndCreatePath(newPath);
            string newFilePath = GetFilePath(newPath, name);
            File.Move(path, newFilePath);
            WriteLog(newFilePath + ".log", new ArrayList { "TaskId: " + taskId, "DateTime: " + DateTime.Now.ToString() });
        }

        public static void RemoveFileToFail(string path, string newPath, string name, string contents)
        {
            CheckAndCreatePath(newPath);
            string newFilePath = GetFilePath(newPath, name);
            File.Move(path, newFilePath);
            WriteLog(newFilePath + ".log", new ArrayList { contents, "DateTime: " + DateTime.Now.ToString() });
        }

        public static string GetFilePath(string path, string name)
        {
            string fileNameNoExt = name.Substring(0, name.LastIndexOf("."));
            string fileExt = name.Substring(name.LastIndexOf(".")).ToLower();
            string newFilePath = path + name;
            int count = 1;
            while (File.Exists(newFilePath))
            {
                newFilePath = path + fileNameNoExt + "(" + count + ")" + fileExt;
                count++;
            }
            return newFilePath;
        }
    }
}
