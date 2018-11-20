using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            public static class SystemDriveHelper
            {
                public static string SystemDrive
                {
                    get
                    {
                        return Path.GetPathRoot(Environment.SystemDirectory);
                    }
                }
            }
        }
    }
}