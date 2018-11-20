/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: WebAuthentication.cs

  DESCRIPTION: 
  ------------
  This file contains Web service Authentication related methods

  USAGE INSTRUCTIONS:
  -------------------
  WebAuthentication.createToken(<string> user, <string> password, <ref string> token)
  
  CHANGE HISTORY:
  --------------- 
  07/05/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Lang;
using Kge.Agent.Library;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public class WebAuthentication : IWebAuthentication
                {
                
                    public static  bool LogonUser(string userName, string domainName, string password, int LogonType, int LogonProvider, ref IntPtr phToken)
                    {
                        //TODO this code should call same function in advapi32.dll. Stubbing out at moment
                        return true;
                    }

                    /// <summary>
                    /// Creates access token
                    /// </summary>
                    /// <param name="user"></param>
                    /// <param name="password"></param>
                    /// <param name="token">Reference to token which will be refered after creation</param>
                    public void CreateToken(string user, string password, ref string token)
                    {
                        Log.Debug("Recieved User credential:" + "username :" + user + ", password :" + "******");
                        if (IsUserCredentialValid(user, password))
                        {
                            Log.Debug("User credentials validation successful");
                            string userPasswdCombo = user + ":" + password;
                            string encodedToken = "";
                            //Keeping timestamp in UTC format to avoid change in zone
                            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                            Log.Debug("System time in UTC format found : " + time.ToString());
                            byte[] key = System.Text.Encoding.UTF8.GetBytes(userPasswdCombo);
                            encodedToken = Convert.ToBase64String(time.Concat(key).ToArray());
                            token = encodedToken;//token validity 1 hour
                            Log.Debug("encoded token:" + encodedToken);
                            Log.Debug("Created web service access token: " + token);
                        }
                        else
                        {
                            Log.Error("User credentials validation failed.");
                            throw new AuthenticationException(CommonErrors.ERROR_INVALID_USR_CREDENTIAL,
                                                              LanguageHelper.Resolve("CORE_ERROR_INVALID_USR_CREDENTIAL"));
                        }
                    }

                    /// <summary>
                    /// Gets access tokenissue time
                    /// </summary>
                    /// <param name="token"></param>
                    /// <returns>Timestamp</returns>
                    public DateTime GetTokenIssueTimeInUTC(string token)
                    {
                        Log.Debug("Recived token id:" + token);
                        Log.Debug("Searching token creation time stamp in UTC format.");
                        byte[] data = Convert.FromBase64String(token.ToString());
                        DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                        Log.Debug("Token creation time in UTC:" + when.ToString());
                        Log.Debug("Token creation time in local system time zone:" + when.ToLocalTime().ToString());
                        return (when);
                    }

                    /// <summary>
                    /// Gets access token expire time
                    /// </summary>
                    /// <param name="token"></param>
                    /// <returns>Timestamp</returns>
                    public DateTime GetTokenExpireTimeInUTC(string token)
                    {
                        Log.Debug("Searching token expire time stamp in UTC format.");
                        byte[] data = Convert.FromBase64String(token.ToString());
                        DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                        when = when.AddHours(1);
                        Log.Debug("Token expire time in UTC:" + when.ToString());
                        Log.Debug("Token expire time in local system time zone:" + when.ToLocalTime().ToString());
                        return (when);
                    }

                    /// <summary>
                    /// Validates  access token
                    /// </summary>
                    /// <param name="token"></param>
                    public void ValidateToken(string token)
                    {
                        string UserName, Password, domainName;
                        string loginUser = string.Empty;
                        Log.Debug("Checking token validation");
                        GetUserCredentials(token, true, out domainName, out UserName, out Password);
                        if (domainName.Equals(".") || string.IsNullOrEmpty(domainName))
                        {
                            loginUser = UserName;
                        }
                        else
                        {
                            loginUser = domainName + @"\" + UserName;
                        }
                        if (!IsUserCredentialValid(loginUser, Password))
                        {
                            Log.Error("Exception: Token invalid");
                            throw new AuthenticationException(CommonErrors.ERROR_ACCESS_TOKEN,
                                                              LanguageHelper.Resolve("CORE_ERROR_ACCESS_TOKEN"));
                        }
                        Log.Debug("Token validation successful");
                    }

                    /// <summary>
                    /// Retrieves user credentials from a token
                    /// </summary>
                    /// <param name="token"></param>
                    /// <param name="domainName"></param>
                    /// <param name="UserName"></param>
                    /// <param name="Password"></param>
                    public void GetUserCredentials(string token, out string domainName, out string UserName, out string Password)
                    {
                        GetUserCredentials(token, false, out domainName, out UserName, out Password);
                    }

                    protected void GetUserCredentials(string token, bool validateTimeout, out string domainName, out string UserName, out string Password)
                    {
                        //TODO
                        //REMOVE THese lines
                        /*
                        domainName = "";
                        UserName="";
                        Password = "";
                        */
                        byte[] data;
                        CryptographyWrapper registrationObj = new CryptographyWrapper();
                        StringBuilder decodedToken = new StringBuilder();
                        string extractUserPasswd = string.Empty;
                        if (token == null)
                        {
                            Log.Error("Exception: Access token value is null");
                            throw new AuthenticationException(CommonErrors.ERROR_UNAUTH_ACCESS,
                                                              LanguageHelper.Resolve("CORE_ERROR_UNAUTH_ACCESS"));
                        }
                        try
                        {
                            Log.Debug("Token received in WebAuthentication.");
                            bool r = registrationObj.GenerateEncryptedString(false, token, decodedToken);
                            Log.Debug("Succesfully decrypted token.");
                            data = System.Convert.FromBase64String(decodedToken.ToString());
                            extractUserPasswd = System.Text.Encoding.UTF8.GetString(data, 8, data.Length - 8);
                        }
                        catch (System.Exception e)
                        {
                            Log.Error("Exception:" + e.Message);
                            Log.Error("Exception: Token invalid");
                            throw new AuthenticationException(CommonErrors.ERROR_ACCESS_TOKEN,
                                                              LanguageHelper.Resolve("CORE_ERROR_ACCESS_TOKEN"));
                        }
                        string[] words = extractUserPasswd.Split(':');
                        UserName = words[0];
                        Password = words[1];
                        domainName = ".";
                        if (UserName.Contains('\\'))
                        {
                            string[] domainUser = UserName.Split('\\');
                            domainName = domainUser[0];
                            UserName = domainUser[1];
                        }
                        Log.Debug("Recived token ID: " + token);
                        if (validateTimeout)
                        {
                            if ((DateTime.UtcNow) > GetTokenExpireTimeInUTC(decodedToken.ToString())) //Token validated with expiration Time. dec
                            {
                                Log.Error("Exception: token expired");
                                throw new AuthenticationException(CommonErrors.ERROR_EXPIRED_TOKEN,
                                                                  LanguageHelper.Resolve("CORE_ERROR_EXPIRED_TOKEN"));
                            }
                        }
                    }

                    /// <summary>
                    /// Returns the current session authentication token
                    /// </summary>
                    /// <returns></returns>
                    public string GetToken()
                    {
                        if (OperationContextHelper.RetrieveContext() == null || OperationContextHelper.RetrieveContext().RequestHeaders== null)
                            return null;
                        return OperationContextHelper.RetrieveContext().RequestHeaders["X-Auth-Token"];
                    }

                    /// <summary>
                    /// Checks if user name and password is valid
                    /// </summary>
                    /// <param name="user"></param>
                    /// <param name="password"></param>
                    /// <returns>True on success, false on failure</returns>
                    private bool IsUserCredentialValid(string user, string password)  //verifies system credentials 
                    {
                        bool ret = true;
                        ResourceLockHelper loginLock = ResourceLockHelper.CreateResourceLock("WEB_EXT_LOGIN");
                        try
                        {
                            string domainName = "";
                            if (user.Contains('\\'))
                            {
                                string[] domainUser = user.Split('\\');
                                domainName = domainUser[0];
                                user = domainUser[1];
                                if (domainName.Equals(".") || domainName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                                {
                                    domainName = Environment.MachineName;
                                }
                                else if (domainName.Equals(Environment.UserDomainName, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(IPGlobalProperties.GetIPGlobalProperties().DomainName)
                                    && domainName.Equals(IPGlobalProperties.GetIPGlobalProperties().DomainName, StringComparison.OrdinalIgnoreCase)))
                                {
                                    domainName = Environment.UserDomainName;
                                }
                            }
                            else
                            {
                                domainName = Environment.MachineName;
                            }
                            loginLock.GetLockOnResource();
                            IntPtr tokenHandler = IntPtr.Zero;
                            ret = LogonUser(user, domainName, password, 2, 0, ref tokenHandler);
                            loginLock.ReleaseLockOnResource();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("System Exception: " + ex.Message);
                            throw new SystemAuthenticationException(CommonErrors.ERROR_SYSTEM, ex.Message);
                        }
                        finally
                        {
                            loginLock.DisposeLockOnResource();
                        }
                        return (ret);
                    }
                }
            }
        }
    }
}
