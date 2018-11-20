using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
                    public class APIRouteDescription
                    {
                        private string uriTemplate;
                        public string UriTemplate
                        {
                            get { return uriTemplate; }
                            set { uriTemplate = value; }
                        }

                        private string queryMethod;
                        public string QueryMethod
                        {
                            get { return queryMethod; }
                            set { queryMethod = value; }
                        }

                        public APIRouteDescription(string uriTemplate, string queryMethod)
                        {
                            UriTemplate = uriTemplate;
                            QueryMethod = queryMethod.ToUpper();
                        }

                        public override bool Equals(object obj)
                        {
                            if (obj == null)
                                return false;

                            var other = obj as APIRouteDescription;
                            if (other == null)
                                return false;

                            return this.UriTemplate == other.UriTemplate && this.QueryMethod == other.QueryMethod;
                        }

                        public override int GetHashCode()
                        {
                            return UriTemplate.GetHashCode() ^ QueryMethod.GetHashCode();
                        }
                    }
                }
            }
        }
    }
}