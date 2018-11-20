using System;
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
                            public class HttpRequestHeadersWrapper
                            {
                                protected virtual HttpHeaderValueCollectionWrapper<MediaTypeWithQualityHeaderValue> mAccept { get; set; }
                                public virtual HttpHeaderValueCollectionWrapper<MediaTypeWithQualityHeaderValue> Accept { get { return mAccept; } }

                                protected virtual HttpRequestHeaders HttpRequestHeaders { get; set; }
                                [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
                                public HttpRequestHeadersWrapper(HttpRequestHeaders httpRequestHeaders)
                                {
                                    HttpRequestHeaders = httpRequestHeaders;

                                    if (HttpRequestHeaders != null)
                                    {
                                        mAccept = new HttpHeaderValueCollectionWrapper<MediaTypeWithQualityHeaderValue>(HttpRequestHeaders.Accept);
                                    }
                                }

                                public virtual bool Contains(string header)
                                {
                                    return HttpRequestHeaders.Contains(header);
                                }

                                public virtual void Remove(string header)
                                {
                                    HttpRequestHeaders.Remove(header);
                                }

                                public virtual void Add(string header, IEnumerable<string> values)
                                {
                                    HttpRequestHeaders.Add(header, values);
                                }

                                public virtual void Add(string header, string value)
                                {
                                    HttpRequestHeaders.Add(header, value);
                                }

                                public virtual IEnumerable<string> GetValues(string name)
                                {
                                    return HttpRequestHeaders.GetValues(name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}