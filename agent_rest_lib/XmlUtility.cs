/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: XmlUtility.cs

  DESCRIPTION: 
  ------------
  This file contains method for XML file related operations

  USAGE INSTRUCTIONS:
 -------------------
  XmlParser.createXml(<string> header, <string> comment, <string host> = "", <string> user = "")
  
  CHANGE HISTORY:
  --------------- 
  07/12/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Net;
using System.Security.Principal;
using Kge.Agent.Library;

namespace Kge
{
    namespace Agent
    {
        namespace RestServer
        {
            namespace Common
            {
                public class XmlParser
                {
                    protected string file = null;
                    protected XmlDocument doc = null;

                    /*Constructor*/
                    public XmlParser(string file)
                    {
                        doc = new XmlDocument();
                        this.file = file;
                    }

                    /*
                    Method: createXml
                    Description: Creates Xml file with basic contents
                    Arguments:
                              <string> header   < XML root >
                              <string> comment  < Document comment >
                              <string> host     < hostname >
                              <string> userid   < system user id>
                    Returns:
                             N/A
                    */
                    public void createXml(string header, string comment, string host = "", string user = "")
                    {
                        try
                        {
                            Log.Debug("Creating Xml file: " + file);
                            XmlProcessingInstruction pPI = null;
                            pPI = doc.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                            XmlNode vNullVal;
                            vNullVal = null;
                            doc.InsertBefore(pPI, vNullVal);
                            XmlNode fileHeader = doc.CreateElement(header);
                            doc.AppendChild(fileHeader);
                            XmlComment fileComment = doc.CreateComment(comment);
                            doc.DocumentElement.AppendChild(fileComment);
                            XmlElement dataHeader = doc.CreateElement("OutputFileHeader");
                            Log.Debug("New element OutputFileHeader created in XML file");
                            XmlElement versionNode = doc.CreateElement("Version");
                            versionNode.InnerText = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                            Log.Debug("New element Version created in XML file with text " + versionNode.InnerText);
                            XmlElement hostNode = doc.CreateElement("GeneratedOnHost");
                            hostNode.InnerText = host.Equals("") ? Dns.GetHostName() : host;
                            Log.Debug("New element GeneratedOnHost created in XML file with text " + hostNode.InnerText);
                            XmlElement generatedByUserNode = doc.CreateElement("GeneratedByUser");
                            generatedByUserNode.InnerText = user.Equals("") ? WindowsIdentity.GetCurrent().User.ToString() : user;
                            Log.Debug("New element GeneratedByUser created in XML file with text " + generatedByUserNode.InnerText);
                            XmlElement generatedOnNode = doc.CreateElement("GeneratedOn");
                            generatedOnNode.InnerText = System.DateTime.UtcNow.ToString();
                            Log.Debug("New element GeneratedOn created in XML file with text " + generatedOnNode.InnerText);
                            XmlElement updatedOnNode = doc.CreateElement("UpdatedOn");
                            updatedOnNode.InnerText = System.DateTime.UtcNow.ToString();
                            Log.Debug("New element UpdatedOn created in XML file with text " + updatedOnNode.InnerText);
                            dataHeader.AppendChild(versionNode);
                            Log.Debug("Element Version added to OutputFileHeader");
                            dataHeader.AppendChild(hostNode);
                            Log.Debug("Element GeneratedOnHost added to OutputFileHeader");
                            dataHeader.AppendChild(generatedByUserNode);
                            Log.Debug("Element generatedByUserNode added to OutputFileHeader");
                            dataHeader.AppendChild(generatedOnNode);
                            Log.Debug("Element generatedOnNode added to OutputFileHeader");
                            dataHeader.AppendChild(updatedOnNode);
                            Log.Debug("Element updatedOnNode added to OutputFileHeader");
                            fileHeader.AppendChild(dataHeader);
                            Log.Debug("Element OutputFileHeader added to Document header");
                            this.doc.Save(file);
                            Log.Debug("Saved XML contains in file: " + file);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error found while creating file: " + file + ", Details: " + ex.Message);
                            throw;
                        }
                    }

                    /*
                    Method: AddEmptyNode
                    Description: add a empty node in to root node specified in xml file.
                    Arguments:
                           <string> root             < Root node >
                           <string> child            < child node to be added >
                           <bool>   force            < force to add even though the node is found under the specified root >
                    Returns:
                           true   < child node added successfully>
                           true   < child node already added successfully>
                    Exceptions:
                              incorrect parsing parameters used while parsing xml
                   */
                    public void AddEmptyNode(string root, string node, bool force = false)
                    {
                        bool IsNodeFound = false;
                        bool isFileModified = false;
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            foreach (XmlNode row in selectedRoot.ChildNodes)
                            {
                                if (row.Name.Equals(node))
                                {
                                    IsNodeFound = true;
                                    if (force)
                                    {
                                        Log.Debug("Child node " + node + "  added on root " + selectedRoot.Name);
                                        selectedRoot.AppendChild(doc.CreateElement(node));
                                        isFileModified = true;
                                    }
                                    else
                                    {
                                        Log.Debug("Child node " + node + "  is already added on selected root " + selectedRoot.Name);
                                    }
                                    break;
                                }
                            }
                            if (!IsNodeFound)
                            {
                                selectedRoot.AppendChild(doc.CreateElement(node));
                                Log.Debug("Child node " + node + "  added on selected root " + selectedRoot.Name);
                                isFileModified = true;
                            }
                            if (isFileModified)
                            {
                                doc.Save(file);
                                Log.Debug("Saved XML modified contains in file: " + file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error in adding empty node " + node + " under root  " + root + ", Details: " + ex.Message);
                            throw;
                        }
                    }

                    /*
                    Method: AddEmptySubnode
                    Description: add a empty subnode under node with specified child node, under root node.
                    Arguments:
                           <string> root             < Root node >
                           <string> node             < node >
                           <string> firstChildName   < first child name of node to be used for filter >
                           <string> firstChildText   < first child text of node to be used for filter > 
                           <bool>   force            < force to add even thoug the key is there >
                    Returns:
                          N/A
                   */
                    public void AddEmptySubNode(string root, string node, string firstChild, string firstChildText, string subnode, bool force = false)
                    {
                        bool isFileModified = false;
                        bool IsNodeFound = false;
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            foreach (XmlNode row in selectedRoot.ChildNodes)
                            {
                                if (row.FirstChild.Name.Equals(firstChild) && row.FirstChild.InnerText.Equals(firstChildText, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Log.Debug("Node " + node + " found as per first child data under selected root" + selectedRoot.Name);
                                    foreach (XmlNode row1 in row.ChildNodes)
                                    {
                                        if (row1.Name.Equals(subnode))
                                        {
                                            IsNodeFound = true;
                                            if (force)
                                            {
                                                Log.Debug("Subnode " + subnode + " added under node" + node);
                                                row.AppendChild(doc.CreateElement(subnode));
                                                isFileModified = true;
                                            }
                                            else
                                            {
                                                Log.Debug("Sub node " + subnode + "  is already added under node" + node);
                                            }
                                            break;
                                        }
                                    }
                                    if (!IsNodeFound)
                                    {
                                        Log.Debug("Subnode " + subnode + " added under node" + node);
                                        row.AppendChild(doc.CreateElement(subnode));
                                        isFileModified = true;
                                    }

                                }
                            }
                            if (isFileModified)
                            {
                                doc.Save(file);
                                Log.Debug("Saved XML modified contains in file: " + file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error in adding empty sub node: " + subnode + " under parent node " +
                                       node + " under root " + root + ", Details: " + ex.Message);
                            throw;
                        }
                    }

                    /*
                     Method: AddNode
                     Description: add node with having child node under root
                     Arguments:
                            <string> root            < Root node >
                            <string> node             < node >
                            <string> firstChildName   < first child name of node to be used for filter >
                            <string> firstChildText   < first child text of node to be used for filter >
                     Returns:
                           true   < node added successfully>
                           false  < node already present under root node >
                   */
                    public void AddNode(string root, string node, string firstChild, string firstChildText)
                    {
                        bool isFileModified = false;
                        bool IsNodeFound = false;
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedNode = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedNode.Name);
                            foreach (XmlNode row in selectedNode.ChildNodes)
                            {
                                if (row.FirstChild.InnerText.Equals(firstChildText, StringComparison.CurrentCultureIgnoreCase) && row.FirstChild.Name.Equals(firstChild))
                                {
                                    IsNodeFound = true;
                                    break;
                                }
                            }
                            if (!IsNodeFound)
                            {
                                XmlElement childElement = doc.CreateElement(node);
                                XmlElement nodeFirstChild = doc.CreateElement(firstChild);
                                nodeFirstChild.InnerText = firstChildText;
                                childElement.AppendChild(nodeFirstChild);
                                selectedNode.AppendChild(childElement);
                                Log.Debug("Node : " + node + " added under root: " + selectedNode.Name + " with first child " +
                                          firstChild + " and first child text " + firstChildText);
                                isFileModified = true;
                            }
                            else
                            {
                                Log.Error("Node : " + node + " is already added under root: " + selectedNode.Name);
                            }
                            if (isFileModified)
                            {
                                doc.Save(file);
                                Log.Debug("Saved XML modified contains in file: " + file);
                            };
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error in adding node " + node + " inside root " + root + ", Details: " + ex.Message);
                            throw;
                        }
                    }

                    /*
                     Method: AddNodeChildSiblings
                     Description: add siblings to the child of a node under root
                     Arguments:
                            <string> root            < Root node >
                            <string> node             < node >
                            <string> firstChildName   < first child name of node to be used for filter >
                            <string> firstChildText   < first child text of node to be used for filter >
                            <Dictionary<string, string>> siblings <dictionary of siblings to be added> 
                     Returns:
                           true         < Child siblings added successfully >
                           false        < Same child sibling already present >
                   */
                    public bool AddNodeChildSiblings(string root, string node, string firstChild, string firstChildText, Dictionary<string, string> siblings)
                    {
                        bool ret = true;
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedNode = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedNode.Name);
                            XmlNode parentNode = null;
                            foreach (XmlNode row in selectedNode.ChildNodes)
                            {
                                if (row.Name.Equals(node) && row.FirstChild.InnerText.Equals(firstChildText, StringComparison.CurrentCultureIgnoreCase) && row.FirstChild.Name.Equals(firstChild))
                                {
                                    parentNode = row;
                                    break;
                                }
                            }
                            if (parentNode != null)
                            {
                                bool IsNodeFound = false;
                                Log.Debug("Parent node selected for adding siblings to its child: " + parentNode.Name);
                                foreach (var item in siblings)
                                {
                                    foreach (XmlNode row in parentNode.ChildNodes)
                                    {
                                        if (row.Name.ToString().Equals(item.Key))
                                        {
                                            IsNodeFound = true;
                                            break;
                                        }
                                    }
                                    if (!IsNodeFound)
                                    {
                                        XmlElement newElement = doc.CreateElement(item.Key);
                                        newElement.InnerText = item.Value;
                                        parentNode.AppendChild(newElement);
                                        Log.Debug("Child node sibling " + item.Key + " with text " + item.Value + " added under parent node " + parentNode.Name);
                                    }
                                    else
                                    {
                                        Log.Debug("Child node sibling " + item.Key + " with text " + item.Value + " already added to parent " + parentNode.Name);
                                        ret = false;
                                        break;
                                    }
                                }
                            }
                            if (ret)
                            {
                                doc.Save(file);
                                Log.Debug("Saved XML contains in file: " + file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error in adding child's sibling nodes on " + node + " inside root " + root + ", Details: " + ex.Message);
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                        return (ret);
                    }

                    /*
                     Method: RemoveNode
                     Description: Removes node from root
                     Arguments:
                            <string> root            < Root node >
                            <string> node             < node >
                            <string> firstChildName   < first child name of node to be used for filter >
                            <string> firstChildText   < first child text of node to be used for filter >
                     Returns:
                           true    < Node removed successfully>
                           false   < Node to be deleted not found >
                   */
                    public bool RemoveNode(string root, string node, string firstChild, string firstChildText)
                    {
                        bool ret = false;
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            foreach (XmlNode row in selectedRoot.ChildNodes)
                            {
                                if (row.Name.Equals(node) && row.FirstChild.Name.Equals(firstChild) && row.FirstChild.InnerText.Equals(firstChildText, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Log.Debug("Node " + node + " to be deleted found under root " + selectedRoot);
                                    row.RemoveAll();
                                    selectedRoot.RemoveChild(row);
                                    ret = true;
                                    break;
                                }
                            }
                            if (ret == true)
                            {
                                doc.Save(file);
                                Log.Debug("Saved XML contains in file: " + file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error in removing node " + node + " inside root " + root + ", Details: " + ex.Message);
                            throw;
                        }
                        return (ret);
                    }

                    /*
                   Method: GetNodeTypeCount
                   Description: Gets count of node types under root
                   Arguments:
                            <string> root            < Root node >
                            <string> node             < node >
                            <string> firstChildName   < first child name of node to be used for filter >
                            <string> firstChildText   < first child text of node to be used for filter >
                   Returns:
                           N/A
                   */

                    public int GetNodeTypeCount(string root, string node, string firstChild = null, string firstChildText = null)
                    {
                        int count = 0;
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            foreach (XmlNode row in selectedRoot.ChildNodes)
                            {
                                if (row.Name.Equals(node))
                                {
                                    count++;
                                    Log.Debug("Node type " + node + " found count increased to  " + count);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML Error in retrieving node count of type  " + node + " under root " + root + ", Details: " + ex.Message);
                            throw;
                        }
                        return (count);
                    }

                    /*
                     Method: GetNodeFirstChildInnerTextList
                     Description: Returns list of sub nodes inner texts of named node.
                     Arguments:
                              <string> root      < Root node >
                              <string> node      < node >
                     Returns:
                             List of node's first child inner text [if node has inner text]
                    */
                    public List<string> GetNodesFirstChildInnerTextList(string root, string node)
                    {
                        List<string> innerTextList = new List<string>();
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            if (selectedRoot.ChildNodes.Count > 0)
                            {
                                foreach (XmlNode row in selectedRoot.ChildNodes)
                                {
                                    if (row.FirstChild != null)
                                    {
                                        Log.Debug("Found text node : " + row.FirstChild.Name + " with text " + row.FirstChild.InnerText);
                                        innerTextList.Add(row.FirstChild.InnerText);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in retrieving child node info under" + root + ", Details: " + ex.Message);
                            throw;
                        }
                        return (innerTextList);
                    }

                    /*
                   Method: GetNodesFirstChildSiblings
                   Description: Gets all node first child siblings
                   Arguments:
                            <string> root     < Root node >
                            <string> node     < node >
                            <string> firstChildName   < first child name of node >
                            <string> firstChildText   < first child text of node >
                   Returns:
                           Dictionary<string, string> firstChildSiblings  < node's first child siblings>
                   */
                    public Dictionary<string, string> GetNodesFirstChildSiblings(string root, string node, string firstChildName, string firstChildText)
                    {
                        Dictionary<string, string> textNodes = new Dictionary<string, string>();
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            foreach (XmlNode row in selectedRoot.ChildNodes)
                            {
                                if (row.FirstChild.Name.Equals(firstChildName) && row.FirstChild.InnerText.Equals(firstChildText, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Log.Debug("Node " + node + " found under root " + selectedRoot.Name);
                                    foreach (XmlNode row1 in row.ChildNodes)
                                    {
                                        Log.Debug("Found Node " + node + " first child sibling with node name " + row1.Name + " and text " + row1.InnerText);
                                        textNodes.Add(row1.Name, row1.InnerText);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in retrieving child's siblings for node " + node + "under root " + root + ", Details: " + ex.Message);
                            throw;
                        }
                        return (textNodes);
                    }

                    /*
                     Method: IsXmlNodeExists
                     Description: Checks if node exists
                     Arguments:
                              <string> node     < xml node >
                
                     Returns:
                             true  < Incase node exist>
                             false < Incase Node does not exist>
                     */
                    public bool IsXmlNodeExists(string node)
                    {
                        bool ret = false;
                        try
                        {
                            Log.Info("Checking if node " + node + " exists");
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(node);
                            if (selectedRoot != null)
                            {
                                Log.Info("Node " + node + " Found");
                                ret = true;
                            }
                            else
                            {
                                Log.Info("Node " + node + " not Found");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in selecting  Node " + node + ", Details: " + ex.Message);
                        }
                        return (ret);
                    }

                    /*
                   Method: GetChildNodeAttributes
                   Description: Gets child node attribute
                   Arguments:
                            <string> root     < Root node >
                            <string> node     < node >
                            <string> firstChildName   < first child name of node >
                            <string> firstChildText   < first child text of node >
                            <string> childNode        < child node whose attribute will be retived >
                   Returns:
                           Dictionary<string, string> attributes  < node's attributes >
                   */
                    public Dictionary<string, string> GetChildNodeAttributes(string root, string node, string firstChildName, string firstChildText, string childNode)
                    {
                        Dictionary<string, string> attributes = new Dictionary<string, string>();
                        try
                        {
                            doc.Load(file);
                            XmlNode selectedRoot = doc.SelectSingleNode(root);
                            Log.Debug("Root node selected: " + selectedRoot.Name);
                            foreach (XmlNode row in selectedRoot.ChildNodes)
                            {
                                if (row.FirstChild.Name.Equals(firstChildName) && row.FirstChild.InnerText.Equals(firstChildText, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Log.Debug("Node " + node + " found under root " + selectedRoot.Name);
                                    foreach (XmlNode row1 in row.ChildNodes)
                                    {
                                        if (row1.Name.Equals(childNode, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            Log.Debug("Found Child " + childNode + ", will query the attributes");
                                            if (row1.Attributes != null)
                                            {
                                                foreach (XmlAttribute item in row1.Attributes)
                                                {
                                                    Log.Debug("Found attribute " + item.Name.ToString() + " with value " + item.Value);
                                                    attributes.Add(item.Name.ToString(), item.Value.ToString());
                                                }
                                            }
                                            else
                                            {
                                                Log.Debug("No attributes found for node " + childNode);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in retrieving child's siblings for node " + node + "under root " + root + ", Details: " + ex.Message);
                            throw;
                        }
                        return (attributes);
                    }
                }
            }
        }
    }
}
