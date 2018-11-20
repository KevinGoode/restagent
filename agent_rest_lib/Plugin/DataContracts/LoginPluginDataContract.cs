using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
                namespace Plugin
                {
                    namespace DataContracts
                    {
                        namespace Login
                        {
                            /// <summary>
                            /// Request body for web server login
                            /// </summary>
                            [DataContract]
                            public class LoginRequest : IGenericRequest
                            {
                                [DataMember(IsRequired=true)]
                                public LoginDetails loginDetails { get; set; }
                            }


                            [DataContract]
                            public class LoginDetails
                            {
                                [DataMember(IsRequired = true)]
                                public string user { get; set; }
                                [DataMember(IsRequired = true)]
                                public string password { get; set; }
                            }

                            /// <summary>
                            /// Response body for login request
                            /// </summary>
                            [DataContract]
                            public class LoginResponse : IGenericResponse
                            {
                                [DataMember(IsRequired = true)]
                                public LoginTokenDetails accessToken { get; set; }
                            }

                            /// <summary>
                            /// Class for LoginToken details
                            /// </summary>
                            [DataContract]
                            public class LoginTokenDetails
                            {
                                [DataMember(Order = 0, IsRequired=true)]
                                public string id { get; set; }

                                [DataMember(Order = 1, IsRequired=true)]
                                public string issuedTime { get; set; }

                                [DataMember(Order = 2, IsRequired=true)]
                                public string expiredTime { get; set; }
                            }
                        }
                    }
                }
            }
        }
    }
}