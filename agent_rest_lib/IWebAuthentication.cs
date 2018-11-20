using System;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public interface IWebAuthentication
                {
                    void CreateToken(string user, string password, ref string token);
                    string GetToken();
                    DateTime GetTokenExpireTimeInUTC(string token);
                    DateTime GetTokenIssueTimeInUTC(string token);
                    void GetUserCredentials(string token, out string domainName, out string UserName, out string Password);
                    void ValidateToken(string token);
                }
            }
        }
    }
}