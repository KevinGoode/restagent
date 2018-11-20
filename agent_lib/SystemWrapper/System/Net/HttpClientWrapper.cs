using Kge.Agent.Library.SystemWrapper.System.Net.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
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
                        [ExcludeFromCodeCoverage]
                        public class HttpClientWrapper
                        {
                            public virtual Uri BaseAddress { get { return HttpClient.BaseAddress; } set { HttpClient.BaseAddress = value; } }

                            protected virtual HttpRequestHeadersWrapper mDefaultRequestHeaders { get; set; }
                            public virtual HttpRequestHeadersWrapper DefaultRequestHeaders { get { return mDefaultRequestHeaders; } }

                            public virtual Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
                            {
                                return HttpClient.PostAsync(requestUri, content);
                            }

                            public virtual Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
                            {
                                return HttpClient.PutAsync(requestUri, content);
                            }

                            public virtual Task<HttpResponseMessage> DeleteAsync(string requestUri)
                            {
                                return HttpClient.DeleteAsync(requestUri);
                            }

                            public virtual Task<HttpResponseMessage> GetAsync(string requestUri)
                            {
                                return HttpClient.GetAsync(requestUri);
                            }

                            protected virtual HttpClient HttpClient { get; set; }
                            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
                            public HttpClientWrapper(HttpClient httpClient)
                            {
                                HttpClient = new HttpClient();

                                if (HttpClient != null)
                                {
                                    mDefaultRequestHeaders = new HttpRequestHeadersWrapper(HttpClient.DefaultRequestHeaders);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}