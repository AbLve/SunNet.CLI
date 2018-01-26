using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Sunnet.Framework.SFTP
{
    public class SFTPHelper
    {
        /// <summary>
        /// 根据目录
        /// </summary>
        private static readonly string RootPath = "/";

        private static readonly int _port = 22;
        private Session session;
        private Channel channel;
        private ChannelSftp sftp;

        private ISunnetLog LoggerHelper;

        /// <summary>
        /// 配置初始化信息，使用默认端口
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="loggerHelper"></param>
        public SFTPHelper(string host, string userName, string pwd, ISunnetLog loggerHelper)
            : this(host, _port, userName, pwd, loggerHelper)
        {
        }

        /// <summary>
        /// 配置初始化信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="loggerHelper"></param>
        public SFTPHelper(string host, int port, string userName, string pwd, ISunnetLog loggerHelper)
        {
            LoggerHelper = loggerHelper;

            JSch jsch = new JSch();
            session = jsch.getSession(userName, host, port);
            SFTPUserInfo userInfo = new SFTPUserInfo(pwd);
            session.setUserInfo(userInfo);
        }


        //SFTP连接状态 
        public bool IsConnected { get { return session.isConnected(); } }

        //连接SFTP ，记得释放连接
        public bool Connect()
        {
            try
            {
                if (!IsConnected)
                {
                    session.connect();

                    channel = session.openChannel("sftp");
                    channel.connect();

                    sftp = (ChannelSftp)channel;
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return false;
            }
        }


        //断开SFTP
        public void Disconnect()
        {
            if (IsConnected)
            {
                channel.disconnect();
                session.disconnect();
            }
        }

        /// <summary>
        /// SFTP存放文件 ,不验证路径正确性
        /// </summary>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="remotePath">SFTP服务器文件路径</param>
        /// <returns></returns>
        public bool Put(string localPath, string remotePath)
        {
            try
            {
                string localFile = Path.GetFileName(localPath);
                Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
                Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
                sftp.put(src, dst);
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 从SFTP获取文件，不验证路径正确
        /// </summary>
        /// <param name="remotePath">SFTP服务器文件路径</param>
        /// <param name="localPath">本地文件路径</param>
        /// <returns></returns>
        public bool Get(string remotePath, string localPath)
        {
            try
            {
                Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
                Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(localPath);
                sftp.get(src, dst);
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取文件列表 ,不验证路径正确性
        /// </summary>
        /// <param name="remotePath">SFTP服务器目录</param>
        /// <param name="extensions">要获取文件的扩展名</param>
        /// <returns></returns>
        public List<string> GetFileList(string remotePath, string[] extensions)
        {
            if (extensions == null) return null;

            try
            {
                Tamir.SharpSsh.java.util.Vector v = sftp.ls(remotePath);
                List<string> list = new List<string>();

                foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry q in v)
                {
                    string name = q.getFilename();
                    string extension = System.IO.Path.GetExtension(name);
                    if (extensions.Contains(extension))
                    { list.Add(name); }
                    else { continue; }
                }

                return list;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return null;
            }
        }

        /// <summary>
        ///  获取文件列表 ,不验证路径正确性
        /// </summary>
        /// <param name="remotePath">SFTP服务器目录</param>
        /// <returns></returns>
        public List<string> GetFileList(string remotePath)
        {
            try
            {
                Tamir.SharpSsh.java.util.Vector v = sftp.ls(remotePath);
                List<string> list = new List<string>();

                foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry q in v)
                {
                    string name = q.getFilename();
                    list.Add(name);
                }
                return list;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// SFTP创建目录
        /// </summary>
        /// <param name="remoteDir">路径</param>
        /// <returns></returns>
        public bool MakeDir(string remoteDir)
        {
            if (remoteDir == string.Empty) return false;
            remoteDir = remoteDir.Replace("\\", "/");
            try
            {
                string[] dirNameArr = remoteDir.TrimStart('/').Split('/');
                if (dirNameArr.Length > 0)
                {
                    string createDir = string.Empty;
                    for (int i = 0; i < dirNameArr.Length; i++)
                    {
                        createDir += dirNameArr[i] + "/";
                        if (!DirExist(RootPath + createDir))
                            sftp.mkdir(RootPath + createDir);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 目录是否已经存在
        /// </summary>
        /// <param name="remoteDir">路径</param>
        public bool DirExist(string remoteDir)
        {
            try
            {
                if (remoteDir.StartsWith(RootPath))
                    sftp.ls(remoteDir);
                else
                    sftp.ls(RootPath + remoteDir);
                return true;
            }
            catch (Tamir.SharpSsh.jsch.SftpException)
            {
                return false;
            }
        }


        /// <summary>
        /// 删除文件，且只能删除文件，不验证路径正确
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        public bool DeleteFile(string remoteFile)
        {
            try
            {
                sftp.rm(remoteFile);
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex.Message);
                return false;
            }
        }
    }
}