using System;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;

namespace Demo
{
    public class LDAPUtil
    {
        public static string Host { get; private set; }
        public static string BindDN { get; private set; }
        public static string BindPassword { get; private set; }
        public static int Port { get; private set; }
        public static string BaseDC { get; private set; }
        public static string CookieName { get; private set; }

        public static void Register(IConfigurationRoot configuration)
        {
            Host = configuration.GetValue<string>("LDAPServer");
            Port = configuration?.GetValue<int>("LDAPPort") ?? 389;
            BindDN = configuration.GetValue<string>("BindDN");
            BindPassword = configuration.GetValue<string>("BindPassword");
            BaseDC = configuration.GetValue<string>("LDAPBaseDC");
            CookieName = configuration.GetValue<string>("CookieName");
        }

        

        public static bool Validate(string username, string password)
        {
            try
            {
                using (var conn = new LdapConnection())
                {
                    conn.Connect(Host, Port);
                    conn.Bind($"{BindDN},{BaseDC}", BindPassword);
                    var entities =
                        conn.Search(BaseDC,LdapConnection.SCOPE_SUB,
                            $"(sAMAccountName={username})",
                            new string[] { "sAMAccountName" }, false);
                    string userDn = null;
                    while (entities.hasMore())
                    {
                        var entity = entities.next();
                        var account = entity.getAttribute("sAMAccountName");
                        //If you need to Case insensitive, please modify the below code.
                        if (account != null && account.StringValue == username)
                        {
                            userDn = entity.DN;
                            break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(userDn)) return false;
                    conn.Bind(userDn, password);
                    // LdapAttribute passwordAttr = new LdapAttribute("userPassword", password);
                    // var compareResult = conn.Compare(userDn, passwordAttr);
                    conn.Disconnect();
                    return true;
                }
            }
            catch (LdapException)
            {
               
                return false;
            }
            catch (Exception)
            { 
                return false;
            }
        }

    }
}
