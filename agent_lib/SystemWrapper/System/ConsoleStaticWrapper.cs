using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            namespace SystemWrapper
            {
                namespace System
                {
                    [ExcludeFromCodeCoverage]
                    public class ConsoleStaticWrapper : IConsoleStaticWrapper
                    {
                        //
                        // Summary:
                        //     Writes the specified string value, followed by the current line terminator,
                        //     to the standard output stream.
                        //
                        // Parameters:
                        //   value:
                        //     The value to write.
                        //
                        // Exceptions:
                        //   System.IO.IOException:
                        //     An I/O error occurred.
                        public void WriteLine(string value)
                        {
                            Console.WriteLine(value);
                        }

                        //
                        // Summary:
                        //     Gets or sets the encoding the console uses to write output.
                        //
                        // Returns:
                        //     The encoding used to write console output.
                        //
                        // Exceptions:
                        //   System.ArgumentNullException:
                        //     The property value in a set operation is null.
                        //
                        //   System.IO.IOException:
                        //     An error occurred during the execution of this operation.
                        //
                        //   System.Security.SecurityException:
                        //     Your application does not have permission to perform this operation.
                        public Encoding OutputEncoding
                        {
                            get
                            {
                                return Console.OutputEncoding;
                            }
                            set
                            {
                                Console.OutputEncoding = value;
                            }
                        }
                    }
                }
            }
        }
    }
}