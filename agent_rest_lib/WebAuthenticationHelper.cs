using Kge.Agent.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public class SecureCredentials
                {
                    public string Domain { get; private set; }
                    public string Username { get; private set; }
                    public SecureString Password { get; private set; }

                    internal SecureCredentials(string userDomain, string userAccount, string cleartextPassword)
                    {
                        Domain = userDomain;
                        Username = userAccount;
                        Password = new SecureString();
                        foreach (var c in cleartextPassword) { Password.AppendChar(c); }
                    }
                }

                public static class WebAuthenticationHelper
                {
                    public static bool ExtractCredentials(this IWebAuthentication webAuth, out string domain, out string user, out string password)
                    {
                        domain = "";
                        user = "";
                        password = "";

                        try
                        {
                            var token = webAuth.GetToken();
                            if (token == null)
                            {
                                Log.Warning("Failed to retrieve Auth-Token.");
                                return false;
                            }

                            webAuth.GetUserCredentials(token, out domain, out user, out password);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Error while getting credentials, caught exception: " + e.Message);
                            return false;
                        }

                        return true;
                    }

                    public static SecureCredentials ExtractSecureCredentials(this IWebAuthentication webAuth)
                    {
                        string domain;
                        string user;
                        string password;
                        if (webAuth.ExtractCredentials(out domain, out user, out password))
                        {
                            return new SecureCredentials(domain, user, password);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}