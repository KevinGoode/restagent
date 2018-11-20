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
                namespace Plugin
                {
                    [AttributeUsage(AttributeTargets.Method)]
                    public class SuppressRequestBodyLoggingAttribute : Attribute
                    {
                    }

                    [AttributeUsage(AttributeTargets.Method)]
                    public sealed class SuppressSensitiveRequestBodyLoggingAttribute : SuppressRequestBodyLoggingAttribute
                    {
                    }

                    [AttributeUsage(AttributeTargets.Method)]
                    public sealed class SuppressRequestResourceLoggingAttribute : Attribute
                    {
                    }
                }
            }
        }
    }
}
