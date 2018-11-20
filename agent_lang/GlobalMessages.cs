/*
 * (C) Copyright 2018 Kge
*/
using System;
using Kge.Agent.Lang;
using System.Linq;
using System.Net.Http.Headers;

namespace Kge
{
    namespace Agent
    {
        namespace Lang
        {
            /// <summary>
            /// Simple container class that contains message id and a language-country type string
            /// </summary>
            public class LocalMessage
            {
                /// <summary>
                /// Use this constructor to always print message in English.
                /// Do not use unless logging. Should mainly use other constructor
                /// </summary>
                /// <param name="msgId"></param>
                public LocalMessage(string msgId)
                {
                    language = "en";
                    messageId = msgId;
                }
                /// <summary>
                /// Create a message that can be localised
                /// </summary>
                /// <param name="lng">language-country type string eg en-EN. Nb only language part is significant</param>
                /// <param name="msgId">Message ID that appears in Language.resx</param>
                public LocalMessage(string lng, string msgId)
                {
                    var languages = lng.Split(new[] { ',' })
                        .Select(a => StringWithQualityHeaderValue.Parse(a))
                        .Select(a => new StringWithQualityHeaderValue(a.Value, a.Quality.GetValueOrDefault(1)))
                        .OrderByDescending(a => a.Quality).ToList();

                    if (languages.Count == 0 || String.IsNullOrWhiteSpace(languages[0].Value))
                        language = "en";
                    else
                    {
                        language = languages[0].Value;
                        if (language.Contains('-'))
                            language = language.Substring(0, language.IndexOf('-'));
                    }

                    messageId = msgId;
                }
                /// <summary>
                /// Language-country string
                /// </summary>
                public string language { get; set; }
                /// <summary>
                /// Message string
                /// </summary>
                public string messageId { get; set; }
            }

            /// <summary>
            /// Contains methods for getting messages by Id from a series of language dependent resource catalogues.
            /// </summary>
            public class Messages
            {

                private enum SupportedLanguages { English, Japanese, German, French, Spanish, Russian, SimplifiedChinese, TraditionalChinese };
                /// <summary>
                /// Gets formatted message in English given a resource ID.
                /// NOTE: This method should only be used when logging messages for which string ids exist.
                /// Since logging is always performed in english it is ok to log messages directly 
                /// without using this method. agent_lang only really intended for strings that might need to be returned to
                /// user EG via REST for via windows event log
                /// </summary>
                /// <param name="messId">Message ID that appears in English.resx</param>
                /// <param name="args">Optional arguments that are passed to message</param>
                /// <returns>Message string in English or 'Failed to find message' if message ID not found</returns>
                public static string GetMessage(String messId, params object[] args)
                {
                    //Localised message is enlish by default
                    LocalMessage message = new LocalMessage(messId);
                    return GetLocalisedMessage(message, args);
                }
                /// <summary>
                /// Gets formatted localised message given a resource ID.
                /// This is the main method that should be used to get a message string
                /// </summary>
                /// <param name="messId">Message ID that appears in Language.resx</param>
                /// <param name="args">Optional arguments that are passed to message</param>
                /// <returns>Localised Message string  or 'Failed to find message' if message ID not found</returns>
                public static string GetLocalisedMessage(LocalMessage mess, params object[] args)
                {
                    string message = "";
                    System.Resources.ResourceManager langResource = GetResourceManager(mess.language);
                    string messageResource = langResource.GetString(mess.messageId);
                    if (messageResource != null)
                    {
                        message = string.Format(messageResource, args);
                    }
                    else
                    {
                        message = "Failed to find message";
                    }
                    return message;
                }
                /// <summary>
                /// Converts a language-country type string into a supported language
                /// If the language is not supported or the language-country string cannot be recognised
                /// then the method returns english
                /// </summary>
                /// <param name="lang">language-country type string eg en-EN. Nb only language part is significant</param>
                /// <returns>language enumeration</returns>
                private static SupportedLanguages LangStrToLang(string lang)
                {
                    SupportedLanguages convertedLang = SupportedLanguages.English;
                    string langLower = lang.ToLower();
                    if (langLower.StartsWith("en"))
                    {
                        convertedLang = SupportedLanguages.English;
                    }
                    else if (langLower.StartsWith("jp"))
                    {
                        convertedLang = SupportedLanguages.Japanese;
                    }
                    else if (langLower.StartsWith("de"))
                    {
                        convertedLang = SupportedLanguages.German;
                    }
                    else if (langLower.StartsWith("fr"))
                    {
                        convertedLang = SupportedLanguages.French;
                    }
                    else if (langLower.StartsWith("es"))
                    {
                        convertedLang = SupportedLanguages.Spanish;
                    }
                    else if (langLower.StartsWith("rs"))
                    {
                        convertedLang = SupportedLanguages.Russian;
                    }
                    else if (langLower.StartsWith("zh-hans") || langLower.StartsWith("zh-chs"))
                    {
                        convertedLang = SupportedLanguages.SimplifiedChinese;
                    }
                    else if (langLower.StartsWith("zh-hant") || langLower.StartsWith("zh-cht"))
                    {
                        convertedLang = SupportedLanguages.TraditionalChinese;
                    }
                    return convertedLang;
                }
                /// <summary>
                /// Gets the resource manager associated with a language-country type string 
                /// </summary>
                /// <param name="lang">language-country type string eg en-EN. Nb only language part is significant</param>
                /// <returns>Resource Manager for particular language</returns>
                private static System.Resources.ResourceManager GetResourceManager(string lang)
                {
                    System.Resources.ResourceManager manager = null;
                    switch (LangStrToLang(lang))
                    {
                        case SupportedLanguages.English:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.Japanese:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.German:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.French:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.Spanish:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.Russian:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.SimplifiedChinese:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        case SupportedLanguages.TraditionalChinese:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                        default:
                            {
                                manager = English.ResourceManager;
                                break;
                            }
                    }
                    return manager;
                }
            }
        }
    }
}

