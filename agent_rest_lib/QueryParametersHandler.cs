using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public class QueryParametersContainer : Dictionary<string, string>
                {
                }

                public class QueryParametersHandler
                {
                    public QueryParametersContainer ConsumeQueryParameters(ref string uri)
                    {
                        var dic = new QueryParametersContainer();

                        var match = Regex.Match(uri, @"\?(.*)");
                        if (match == null || match.Success == false)
                            return null;

                        if (match.Groups[1].Success == false)
                            return null;

                        var ret = HttpUtility.ParseQueryString(match.Groups[1].Value);
                        foreach (string k in ret.AllKeys)
                        {
                            dic.Add(k, ret[k]);
                        }

                        uri = uri.Replace(match.Groups[0].Value, "");

                        return dic;
                    }
                }
            }
        }
    }
}