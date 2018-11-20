///////////////////////////////////////////////////////////////////////////////
//
// (C) Copyright 2018 Kge
// All Rights Reserved.
//
// FILE NAME:Constants.cs 
//
// DESCRIPTION: 
// ------------
// This file contains the defined return values
//
// CHANGE HISTORY:
// ---------------
// 07/03/2015 – Initial version
// 
//
// AUTHOR: Kge
// DATE LAST MODIFIED: July, 2015
//
///////////////////////////////////////////////////////////////////////////////

using System;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            /*
             *  Description:
             *  ------------
             * This namespace is to be used for definition of constants.
             */
            public static class Constants
            {
                public const UInt32 STATUS_SUCCESS = 0x0000;
                public const UInt32 STATUS_FAILURE = 0x0001;
                public const UInt32 STATUS_REGISTRATION_BASE = 0x7000;
                public const UInt32 STATUS_REGISTRATION_SUCCESS = STATUS_SUCCESS;
                public const UInt32 STATUS_REGISTRATION_FAILURE = (STATUS_REGISTRATION_BASE + 0x0002);
                public const UInt32 STATUS_REGISTRATION_EXIST = (STATUS_REGISTRATION_BASE + 0x0003);
                public const UInt32 STATUS_REGISTRATION_NOT_FOUND = (STATUS_REGISTRATION_BASE + 0x0004);
                public const UInt32 REMOTING_ERROR = (STATUS_REGISTRATION_BASE + 0x0005);
                public const UInt32 REGISTRY_ERROR = (STATUS_REGISTRATION_BASE + 0x0006);
            }
        }


    }
}
