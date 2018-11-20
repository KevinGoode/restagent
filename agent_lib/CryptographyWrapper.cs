// 
// CHANGE HISTORY:
// ---------------
// AUTHOR: Kge
// DATE LAST MODIFIED: July, 2015
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System.Net;


namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            /// <summary>
            /// Cryto 
            /// </summary>
            public class CryptographyWrapper
            {
               

                public CryptographyWrapper()
                {
                    
                }

                /// <summary>
                /// Generates EncryptedString
                /// </summary>
                /// <param name="bEncryptString">True ->encryption ,False -> decryption</param>
                /// <param name="sCredential">Password to be enrypted</param>
                /// <param name="sbString"></param>
                /// <param name="iStringSize"></param>
                /// <returns>Status of the operation</returns>
                public bool GenerateEncryptedString(bool bEncryptString, string sCredential, StringBuilder sbString)
                {
                    AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptStream = null;
                    byte[] byteInputString;
                    string sKey = GlobalKeys.EncryptKey;

                    if (bEncryptString == false)
                    {
                        int iInputByteLength = sCredential.Length / 2 - 1;
                        byteInputString = new byte[iInputByteLength];

                        for (int index = 0; index < iInputByteLength; index++)
                        {
                            string sHexChar = String.Format("0x{0}", sCredential.Substring(1 + (index * 2), 2));
                            byteInputString[index] = (byte)Convert.ToUInt32(sHexChar, 16);
                        }
                    }
                    else
                    {
                        byteInputString = Encoding.ASCII.GetBytes(sCredential);
                    }
                    byte[] byteKey = Encoding.ASCII.GetBytes(sKey.ToCharArray(), 0, 32); //obtainig 32 byte key for encryption
                    byte[] byteIV = Encoding.ASCII.GetBytes(sKey.ToCharArray(), 0, 16);  //obtaining Initial Vector for encryption from first 16 bytes of key

                    if (bEncryptString)
                    {
                        cryptStream = new CryptoStream(memoryStream, aes.CreateEncryptor(byteKey, byteIV), CryptoStreamMode.Write);
                    }
                    else
                    {
                        cryptStream = new CryptoStream(memoryStream, aes.CreateDecryptor(byteKey, byteIV), CryptoStreamMode.Write);
                    }

                    try
                    {
                        cryptStream.Write(byteInputString, 0, byteInputString.Length);
                        cryptStream.FlushFinalBlock();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                        aes.Clear();
                        memoryStream.Close();
                        return false;
                    }
                    byte[] byteResult = new Byte[memoryStream.Length];
                    memoryStream.Position = 0;
                    memoryStream.Read(byteResult, 0, memoryStream.ToArray().Length);
                    cryptStream.Close();
                    aes.Clear(); //clear the cache..
                    if (bEncryptString == true)
                    {
                        sbString.Append("{");
                        for (int index = 0; index < byteResult.Length; index++)
                        {
                            sbString.Append(byteResult[index].ToString("X2"));

                        }
                        sbString.Append("}");
                    }
                    else
                        sbString.Append(Encoding.ASCII.GetString(byteResult));

                    return true;
                }

                
            }
        }
    }
}

