/*
 * (C) Copyright 2018 Kge
*/
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Kge.Agent.Library;
namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
                /// <summary>
                /// Base class to read elements from XML files use XPATH.
                /// </summary>
                public class XMLReader
                {
                    /// <summary>
                    /// Default constructor. Must call LoadXMLFromString before calling other public accessor methods 
                    /// </summary>
                    public XMLReader(string xml)
                    {
                        LoadXMLFromString(xml);
                    }
                    
                    /// <summary>
                    /// Get value of first node matching xpath in xml document.
                    /// This method public for test purposes. Should wrap this function when getting elements.
                    /// </summary>
                    /// <param name="xpath">Xpath of node</param>
                    /// <returns>String for node value or empty string (in the event of an error)</returns>
                    public string GetSingleNode(string xpath)
                    {
                        string value = "";
                        try
                        {
                            XmlNode node = xmlDocument.SelectSingleNode(xpath);
                            if (node != null)
                            {
                                value = node.InnerText;
                            }
                            else
                            {
                                Log.Error("Error getting value using xpath:" + xpath);
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = ex.ToString();
                            Log.Error("Error reading  xml file: " + message);

                        }
                        return value;
                    }

                    /// <summary>
                    /// Get value of all nodes matching xpath in xml document.
                    /// This method public for test purposes. Should wrap this function when getting elements.
                    /// </summary>
                    /// <param name="xpath">Xpath of nodes</param>
                    /// <returns>Array list ofstrings matching all values</returns>
                    public System.Collections.ArrayList GetNodes(string xpath)
                    {
                        System.Collections.ArrayList list = new System.Collections.ArrayList();
                        try
                        {
                          
                            XmlNodeList nodes = xmlDocument.SelectNodes(xpath);
                            if (nodes != null)
                            {
                                foreach( XmlNode node in nodes)
                                {
                                    list.Add(node.InnerText);
                                }
                            }
                            else
                            {
                                Log.Error("Error getting nodes using xpath:" + xpath);
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = ex.ToString();
                            Log.Error("Error reading  xml file: " + message);

                        }
                        return list;
                    }
                    private void LoadXMLFromString(string xml)
                    {
                        try
                        {
                            xmlDocument = new XmlDocument();
                            string cleanXml = RemoveNamespaces(xml);
                            xmlDocument.LoadXml(cleanXml);
                        }
                        catch (Exception ex)
                        {
                            string message = ex.ToString();
                            Log.Error("Error loading  xml : " + xml + ". Message:" + message);
                        }
                    }

                    private string RemoveNamespaces(string xml)
                    {

                        //Regex below finds strings that start with xmlns, may or may not have :and some text, then continue with =
                        //and ", have a streach of text that does not contain quotes and end with ". similar, will happen to an attribute
                        // that starts with xsi.
                        string strXMLPattern = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";
                        string cleanXml = Regex.Replace(xml, strXMLPattern, "");
                        return cleanXml;
                    }
                    private XmlDocument xmlDocument;
                }
            }
        }
    }
