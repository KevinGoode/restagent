using Kge.Agent.Rest.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    public class ResolvedRoute
                    {
                        private IPlugin plugin;
                        public IPlugin Plugin
                        {
                            get { return plugin; }
                            set { plugin = value; }
                        }

                        private MethodInfo method;
                        public MethodInfo Method
                        {
                            get { return method; }
                            set { method = value; }
                        }

                        private Dictionary<string, string> resourceIds;
                        public Dictionary<string, string> ResourceIds
                        {
                            get { return resourceIds; }
                            set { resourceIds = value; }
                        }

                        private QueryParametersContainer queryParameters;
                        public QueryParametersContainer QueryParameters
                        {
                            get { return queryParameters; }
                            set { queryParameters = value; }
                        }

                        public ResolvedRoute(IPlugin plugin, MethodInfo method)
                        {
                            Plugin = plugin;
                            Method = method;
                            ResourceIds = new Dictionary<string, string>();
                            QueryParameters = new QueryParametersContainer();
                        }

                        public ResolvedRoute(IPlugin plugin, MethodInfo method, Dictionary<string, string> resourceIds, QueryParametersContainer queryParameters)
                        {
                            Plugin = plugin;
                            Method = method;
                            ResourceIds = resourceIds;
                            if (ResourceIds == null)
                                ResourceIds = new Dictionary<string, string>();

                            QueryParameters = queryParameters;
                            if (QueryParameters == null)
                                QueryParameters = new QueryParametersContainer();
                        }
                    }
                }
            }
        }
    }
}
