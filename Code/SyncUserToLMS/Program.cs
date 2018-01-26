using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework;

namespace SyncUserToLms
{
    class Program
    {
        static System.Threading.Mutex _mutex;
        static void Main(string[] args)
        {
            IoC.Init();
            bool createNew;
            Attribute guid_attr = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute));
            string guid = "Global\\" + ((GuidAttribute)guid_attr).Value;
            _mutex = new System.Threading.Mutex(true, guid, out createNew);
            if (false == createNew)
            {
                Environment.Exit(0);
            }
            _mutex.ReleaseMutex();
            SyncUsersToLmsSystem();
        }
        /// <summary>
        /// sync user to lms system
        /// </summary>
        private static void SyncUsersToLmsSystem()
        {
            DateTime startTime = DateTime.Now;
            Log("Running....." + startTime);
            UserBusiness userBusiness = new UserBusiness();
            
            List<UserBaseEntity> userList =null;
            try
            {
                userList = userBusiness.GetAllUsersByLmsPermission(4000);
            }
            catch (Exception ex)
            {
                Log("GetAllUsersByLmsPermission error-->" + ex);
                return;

            }

            Log("Processing " + userList.Count + " users");
            string userIds = "";
            foreach (var user in userList)
            {
                Console.WriteLine(user.ID +"-->Syncing to LMS......");
                WebRequest request=null;
                HttpWebResponse response = null;
                try
                {
                    string lmsurl = userBusiness.GeneralLmsUrl(user, SFConfig.LMSDomain.ToString());
                    Console.WriteLine(lmsurl);
                    if (lmsurl.ToLower().StartsWith("https")) //David 06/26/2016
                    {

                        string HttpsProtocol = ConfigurationManager.AppSettings["HttpsProtocol"];

                        Console.WriteLine("----Using HttpsProtocol=" + HttpsProtocol);
                        #region
                        //System.Net.WebException: The underlying connection was closed: An unexpected error occurred on a send. --->
                        //System.IO.IOException: Authentication failed because the remote party has closed the transport stream.
                        #endregion
                        if (HttpsProtocol == "TLS1.2")
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        else if (HttpsProtocol == "TLS1.1")
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                        else if (HttpsProtocol == "TLS1.0")
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                        else if (HttpsProtocol == "SSL3")
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    }
                    request = WebRequest.Create(lmsurl);
                    //request.Timeout =
                    request.Credentials = CredentialCache.DefaultCredentials;
                    // Get the response.
                    response = (HttpWebResponse)request.GetResponse();
                    string result = response.StatusDescription; //response.StatusCode.ToString()
                    Console.WriteLine("response.StatusDescription= " + result);
                    if (result == "OK")
                    {
                        Console.WriteLine(user.ID + "-->Sync to LMS successfully");
                        /*IsSyncLms
                         *True: Need to sync to Lms
                         *False: No need to sync to LMS
                         */
                        user.IsSyncLms = false;
                        try
                        {
                            //Update user-- database
                            userBusiness.UpdateUser(user, true, false);
                            Console.WriteLine(user.ID + "-->UpdateUser successfully");
                        }
                        catch (Exception ex)
                        {
                            Log(user.ID + "-->UpdateUser exception: " + ex);
                        }
                    }
                    else
                    {
                        string message = user.ID + "-->Sync failed, result=" + result;
                        Log(message);
                    }
                }
                catch (Exception ex)
                {
                    userIds += user.ID + ",";
                    Log(user.ID + "-->Exception:" + ex);
                    continue;
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                    if (request != null)
                    {
                        request.Abort();
                    }
                    response = null;
                    request = null;
                    //System.GC.Collect();
                }
            }

            DateTime finishTime = DateTime.Now;
            Log("catch users:" + userIds);
            Log("Finished....." + finishTime + " it has been taken " +(finishTime-startTime).TotalSeconds +" s");

        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            string LogPath = ConfigurationManager.AppSettings["LogPath"] + "";
            if (LogPath == string.Empty)
                LogPath = @"c:\Cli\Log.txt";

            //if file length more than 5MB,move file to new file,create new file
            FileInfo fileInfo = new FileInfo(LogPath);
            if (fileInfo.Exists)
            {
                if (fileInfo.Length > 5*1024*1024)//5MB
                {
                    string copyPath = fileInfo.DirectoryName;
                    File.Copy(LogPath, string.Format(copyPath + "log{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                    fileInfo.Delete();
                }
            }

            using (StreamWriter write = new StreamWriter(LogPath, true, Encoding.Default))
            {
                write.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                write.WriteLine(message);
                write.WriteLine("----------------------------------------------------");
            }
        }
    }
}
