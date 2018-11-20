using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                public static class ExceptionExtensions
                {
                    public static string ExtractTypeName(this Exception excep)
                    {
                        if (excep.InnerException != null)
                        {
                            excep = excep.InnerException;
                        }

                        return excep.GetType().Name;
                    }

                    public static string ExtractDetailMessage(this Exception excep)
                    {
                        if (excep.InnerException != null)
                        {
                            excep = excep.InnerException;
                        }

                        if (excep is WebResponseException)
                        {
                            return (excep as WebResponseException).Message;
                        }

                        return excep.Message;
                    }

                    public static string ExtractStackTrace(this Exception excep)
                    {
                        if (excep.InnerException != null)
                        {
                            excep = excep.InnerException;
                        }

                        return excep.StackTrace;
                    }

                    public static string ExtractFullTrace(this Exception excep)
                    {
                        return excep.ExtractTypeName() + ": " + excep.ExtractDetailMessage() + Environment.NewLine + excep.ExtractStackTrace();
                    }
                }
            }
        }
    }
}
