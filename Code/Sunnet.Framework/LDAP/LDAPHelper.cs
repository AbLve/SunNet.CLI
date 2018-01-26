using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.LDAP
{
    public enum ADLoginResult
    {
        OK,
        Invalid_UserName_or_Password,
        Account_Inactive,
        Account_Locked,
        Account_Expired,
        Failed
    }

    public class LDAPHelper
    {
        ISunnetLog log;
        public LDAPHelper(ISunnetLog log)
        {
            this.log = log;
        }

        public  ADLoginResult ValidateUser(string userName, string password, out string accountName)
        {
            accountName = "";
            PrincipalContext ctx = null;
            UserPrincipal up = null;
            try
            {
                ctx = new PrincipalContext(ContextType.Domain, null, SFConfig.LDAP, userName, password);
                up = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName);
                if (up != null)
                {
                    accountName = up.SamAccountName;
                    return ADLoginResult.OK;
                }
            }
            catch (Exception ex)
            {
                bool rethrow = Handle(ex);
                if (rethrow) throw;
            }
            finally
            {
                if (up != null)
                    up.Dispose();

                if (ctx != null)
                    ctx.Dispose();
            }

            return ADLoginResult.Failed;
        }

        public  bool ValidateADUser(string userName, string password)
        {
            using (DirectoryEntry de = new DirectoryEntry(SFConfig.LDAPUrl, userName, password, AuthenticationTypes.Secure))
            {
                try
                {
                    return !string.IsNullOrEmpty(de.Name);
                }
                catch (Exception ex)
                {
                    bool rethrow = Handle(ex);
                    //if (rethrow) throw;
                }
                finally
                {
                    if (de != null)
                        de.Dispose();
                }


                return false;
            }
        }

        public  bool SearchADUser(string userName, string password)
        {
            using (DirectoryEntry de = new DirectoryEntry(SFConfig.LDAPUrl, userName, password, AuthenticationTypes.Secure))
            {
                try
                {
                    DirectorySearcher search = new DirectorySearcher(de);
                    search.Filter = "(&(sAMAccountName=" + userName + "))";
                    search.SearchScope = SearchScope.Subtree;
                    SearchResultCollection results = search.FindAll();
                    if (results != null)
                    {
                        foreach (SearchResult sr in results)
                        {
                            DirectoryEntry subde = new DirectoryEntry(sr.Path, userName, password, AuthenticationTypes.Secure);
                            return !string.IsNullOrEmpty(subde.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    bool rethrow = Handle(ex);
                    //if (rethrow) throw;
                }
                finally
                {
                    if (de != null)
                        de.Dispose();
                }


                return false;
            }
        }


        private bool Handle(Exception ex)
        {
            if(ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            log.Debug(ex);
            return false;
        }
    }
}
