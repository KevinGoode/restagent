using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public abstract class WCFSerializerExceptions : Exception
                {
                    public string Path { get; set; }
                    public string MemberName { get; set; }
                    public int Line { get; set; }
                    public int Position { get; set; }

                    public abstract int Priority { get; }

                    protected WCFSerializerExceptions(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        : base(args.ErrorContext.Error.Message)
                    {
                        ParseError(args);
                    }

                    protected WCFSerializerExceptions() { }

                    protected void ParseError(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        var match = Regex.Match(args.ErrorContext.Error.Message,
                                                @"line ([0-9]*)\, position ([0-9]*)");

                        if (args.ErrorContext.Member != null && args.ErrorContext.Path != null)
                        {
                            string path = args.ErrorContext.Path;
                            MemberName = args.ErrorContext.Member.ToString();

                            if (MemberName == path || String.IsNullOrWhiteSpace(path))
                                Path = args.ErrorContext.OriginalObject.GetType().Name;
                            else
                            {
                                if (path.Contains("."))
                                {
                                    int index = path.LastIndexOf("." + MemberName);
                                    if (index == -1)
                                        return;
                                    Path = path.Substring(0, index);
                                }
                                else
                                    Path = path;
                            }
                        }

                        if (match.Success && match.Groups.Count == 3)
                        {
                            if (match.Groups[1].Captures.Count > 0)
                            {
                                var captures = match.Groups[1].Captures;
                                Line = int.Parse(captures[captures.Count - 1].Value);
                            }

                            if (match.Groups[2].Captures.Count > 0)
                            {
                                var captures = match.Groups[2].Captures;
                                Position = int.Parse(captures[captures.Count - 1].Value);
                            }
                        }
                    }
                }

                public class MissingMemberException : WCFSerializerExceptions
                {
                    public override int Priority
                    {
                        get { return 1; }
                    }

                    public MissingMemberException(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        : base(args) // Required property 'password' not found in JSON. Path 'loginDetails', line 1, position 35.
                    {
                    }
                }

                public class ExtraMemberException : WCFSerializerExceptions
                {
                    public override int Priority
                    {
                        get { return 3; }
                    }

                    public ExtraMemberException(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        : base(args) // Could not find member 'ipAddress' on object of type 'LoginDetails'. Path 'loginDetails.ipAddress', line 1, position 71.
                    {
                    }
                }

                public class MemberTypeException : WCFSerializerExceptions
                {
                    public override int Priority
                    {
                        get { return 2; }
                    }

                    public MemberTypeException(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        : base(args)
                    {
                    }
                }

                public class JsonSyntaxException : WCFSerializerExceptions
                {
                    public override int Priority
                    {
                        get { return 0; }
                    }

                    public JsonSyntaxException(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        : base(args)
                    {
                    }
                }
            }
        }
    }
}