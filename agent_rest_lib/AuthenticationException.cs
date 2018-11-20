/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME:CommandExceptions.cs

  DESCRIPTION:
  ------------
  This file contains method for generating exceptions of Command creator Module

  USAGE INSTRUCTIONS:
 -------------------
  CreateCommandException(<string> message);
 
  CHANGE HISTORY:
  --------------- 
  06/19/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: June, 2015
********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                /* Web Authentication exception class */
                public class AuthenticationException : CustomException
                {
                    public AuthenticationException()
                        : base() { }
                    public AuthenticationException(UInt32 errorCode, string message)
                        : base(errorCode, message) { }
                    public AuthenticationException(UInt32 errorCode, string message, Exception innerException)
                        : base(errorCode, message, innerException) { }
                }
            }

        }
    }
}