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
                        [DataContract]
                        public class TaskResponse : IGenericResponse
                        {
                            [DataMember(IsRequired = true)]
                            public TaskUriDetails task { get; set; }
                        }

                        [DataContract]
                        public class TaskUriDetails
                        {
                            [DataMember(IsRequired = true)]
                            public string taskUri { get; set; }

                            [DataMember(IsRequired = false)]
                            public string taskResultUri { get; set; }

                            [DataMember(IsRequired = true)]
                            public bool isFinished { get; set; }

                            public DateTime? TaskCreationTimeDetails;
                            [DataMember(IsRequired = true)]
                            public string taskCreationTime
                            {
                                get
                                {
                                    if (TaskCreationTimeDetails != null)
                                        return String.Format("{0:d/M/yyyy HH:mm:ss}", TaskCreationTimeDetails);
                                    else
                                        return null;
                                }
                            }

                            public DateTime? TaskCompletionTime;
                            [DataMember(IsRequired = false)]
                            public string taskCompletionTime
                            {
                                get
                                {
                                    if (TaskCompletionTime != null)
                                        return String.Format("{0:d/M/yyyy HH:mm:ss}", TaskCompletionTime);
                                    else
                                        return null;
                                }
                            }

                        }
                    }
                }
            }
        }
    }
}