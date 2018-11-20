using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
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
                    namespace Net
                    {
                        namespace Headers
                        {
                            [ExcludeFromCodeCoverage]
                            public class HttpHeaderValueCollectionWrapper<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
                            {
                                protected virtual HttpHeaderValueCollection<T> HttpHeaderValueCollection { get; set; }
                                [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
                                public HttpHeaderValueCollectionWrapper(HttpHeaderValueCollection<T> httpHeaderValueCollection)
                                {
                                    HttpHeaderValueCollection = httpHeaderValueCollection;
                                }

                                public virtual void Add(T item)
                                {
                                    HttpHeaderValueCollection.Add(item);
                                }

                                public virtual void Clear()
                                {
                                    HttpHeaderValueCollection.Clear();
                                }

                                public virtual bool Contains(T item)
                                {
                                    return HttpHeaderValueCollection.Contains(item);
                                }

                                public virtual void CopyTo(T[] array, int arrayIndex)
                                {
                                    HttpHeaderValueCollection.CopyTo(array, arrayIndex);
                                }

                                public virtual int Count
                                {
                                    get { return HttpHeaderValueCollection.Count; }
                                }

                                public virtual bool IsReadOnly
                                {
                                    get { return HttpHeaderValueCollection.IsReadOnly; }
                                }

                                public virtual bool Remove(T item)
                                {
                                    return HttpHeaderValueCollection.Remove(item);
                                }

                                public virtual IEnumerator<T> GetEnumerator()
                                {
                                    return HttpHeaderValueCollection.GetEnumerator();
                                }

                                IEnumerator IEnumerable.GetEnumerator()
                                {
                                    return HttpHeaderValueCollection.GetEnumerator();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}