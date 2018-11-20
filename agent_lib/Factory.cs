using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            public interface IFactory<T>
            {
                T CreateInstance();
            }

            public class Factory<T> : IFactory<T>
            {
                private Func<T> Constructor { get; set; }

                public Factory(Func<T> constructionFunction)
                {
                    Constructor = constructionFunction;
                }

                public T CreateInstance()
                {
                    return Constructor.Invoke();
                }
            }
        }
    }
}
