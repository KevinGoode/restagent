/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.

  FILE NAME: CustomExceptions.cs

  DESCRIPTION:
  ------------
  This file contains Custom  exception class and all Common exception classes shared between components

  USAGE INSTRUCTIONS:
 -------------------
  new CustomException(<UInt32> errorCode, <string> message)

  CHANGE HISTORY:
  --------------- 
  07/12/2015 – New file

  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                /*Base exception class for all other derived exception classes */
                public class CustomException : Exception
                {
                    public UInt32 errorCode { get; protected set; }
                    public CustomException()
                        : base() { }
                    public CustomException(UInt32 errorCode, string message)
                        : base(message)
                    {
                        this.errorCode = errorCode;
                    }
                    public CustomException(UInt32 errorCode, string message, Exception innerException)
                        : base(message, innerException)
                    {
                        this.errorCode = errorCode;
                    }
                }

                /* Exception class used for REST request validation failure */
                public class RequestValidationException : CustomException
                {
                    public RequestValidationException()
                        : base() { }
                    public RequestValidationException(UInt32 errorCode, string message)
                        : base(errorCode, message) { }
                    public RequestValidationException(UInt32 errorCode, string message, Exception innerException)
                        : base(errorCode, message, innerException) { }
                }

                /* Exception class used for resource not found */
                public class ResourceNotFoundException : CustomException
                {
                    public ResourceNotFoundException()
                        : base() { }
                    public ResourceNotFoundException(UInt32 errorCode, string message)
                        : base(errorCode, message) { }
                    public ResourceNotFoundException(UInt32 errorCode, string message, Exception innerException)
                        : base(errorCode, message, innerException) { }
                }
                /* Exception class used for resource not found */
                public class SystemAuthenticationException : CustomException
                {
                    public SystemAuthenticationException()
                        : base() { }
                    public SystemAuthenticationException(UInt32 errorCode, string message)
                        : base(errorCode, message) { }
                    public SystemAuthenticationException(UInt32 errorCode, string message, Exception innerException)
                        : base(errorCode, message, innerException) { }
                }

                /* Exception class used for resource not found */
                public class ApplicationInstallationException : CustomException
                {
                    public ApplicationInstallationException()
                        : base() { }
                    public ApplicationInstallationException(UInt32 errorCode, string message)
                        : base(errorCode, message) { }
                    public ApplicationInstallationException(UInt32 errorCode, string message, Exception innerException)
                        : base(errorCode, message, innerException) { }
                }
               
                public class RegistryAccessException : CustomException
                {
                    public RegistryAccessException()
                        : base()
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public RegistryAccessException(UInt32 errorCode, string message)
                        : base(errorCode, message)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public RegistryAccessException(UInt32 errorCode, string message, Exception inner)
                        : base(errorCode, message, inner)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                }

                public class RemotingException : CustomException
                {
                    public RemotingException()
                        : base()
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public RemotingException(UInt32 errorCode, string message)
                        : base(errorCode, message)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public RemotingException(UInt32 errorCode, string message, Exception inner)
                        : base(errorCode, message, inner)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                }

                public class TaskTrackerException : CustomException
                {
                    public TaskTrackerException()
                        : base()
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public TaskTrackerException(UInt32 errorCode, string message)
                        : base(errorCode, message)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public TaskTrackerException(UInt32 errorCode, string message, Exception inner)
                        : base(errorCode, message, inner)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                }

                public class AppHostAssociationException : CustomException
                {
                    public AppHostAssociationException()
                        : base()
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public AppHostAssociationException(UInt32 errorCode, string message)
                        : base(errorCode, message)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                    public AppHostAssociationException(UInt32 errorCode, string message, Exception inner)
                        : base(errorCode, message, inner)
                    {
                        //Do nothing; No tasks required to be done here
                    }
                }
            }
        }
    }
}