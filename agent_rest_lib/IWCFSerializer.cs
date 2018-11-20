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
                public interface IWCFSerializer
                {
                    string Serialize(object obj);
                    object Deserialize(string json, Type toType);
                }
            }
        }
    }
}
