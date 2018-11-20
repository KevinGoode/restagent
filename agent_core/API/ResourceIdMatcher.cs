using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace API
                {
                    public class ResourceIdMatcher
                    {
                        /// <summary>
                        /// Try to match the url patterns.
                        /// </summary>
                        /// <param name="uriTemplate"></param>
                        /// <param name="queryString"></param>
                        /// <returns>Return true if those match and false if not.</returns>
                        public bool Match(string uriTemplate, string queryString)
                        {
                            if (uriTemplate.Equals(queryString))
                                return true;

                            UriTemplate.Core.UriTemplateMatch results = getUriTemplateMatch(uriTemplate, queryString);

                            if (results == null)
                                return false;
                            return true;
                        }

                        /// <summary>
                        /// Retrieves the parameters through the url.
                        /// </summary>
                        /// <param name="uriTemplate"></param>
                        /// <param name="queryString"></param>
                        /// <returns></returns>
                        public Dictionary<string, string> ExtractResourceIds(string uriTemplate, string queryString)
                        {
                            UriTemplate.Core.UriTemplateMatch  results = getUriTemplateMatch(uriTemplate, queryString);
                            if (results == null)
                                return null;

                            var dic = new Dictionary<string, string>();
                            var bindings = results.Bindings.ToArray();
                            for (int i=0;i<bindings.Length;i++)
                            {
                                string key=bindings[i].Key;
                                dic.Add(key, bindings[i].Value.Value.ToString());
                            }

                            return dic;
                        }

                        protected virtual UriTemplate.Core.UriTemplateMatch getUriTemplateMatch(string uriTemplate, string queryString)
                        {
                            UriTemplate.Core.UriTemplate template = null;
                            Uri fullUri = null;
                            Uri baseUri = null;

                            try
                            {
                                // using localhost just to be able to build Uris
                                template = new UriTemplate.Core.UriTemplate("http://localhost" + uriTemplate);
                                fullUri = new Uri("http://localhost" + queryString);
                                //baseUri = new Uri("http://localhost");
                            }
                            catch
                            {
                                return null;
                            }

                            UriTemplate.Core.UriTemplateMatch results = null;

                            try
                            {
                                //results = template.Match(baseUri, fullUri);
                                results = template.Match(fullUri);
                            }
                            catch { return null; }

                            return results;
                        }
                    }
                }
            }
        }
    }
}