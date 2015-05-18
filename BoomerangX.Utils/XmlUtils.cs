﻿using System.IO;
using System.Xml;

namespace BoomerangX.Utils
{
    public class XmlUtils
    {
        /// <summary>
        /// extract a single xml node from the given stream, given by the xPath
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlNode ExtractXmlElement(Stream xmlStream, string xPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStream);
            //Create namespace manager
            var nsmgr = CreateNamespaceManager(xmlDoc);

            var node = xmlDoc.SelectSingleNode(xPath, nsmgr);
            return node;
        }

        public static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
        {
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("a", "http://www.w3.org/2005/Atom");
            nsmgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            nsmgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            return nsmgr;
        }

        /// <summary>
        /// extract the xml nodes from the given stream, given by the xPath
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlNodeList ExtractXmlElementList(Stream xmlStream, string xPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStream);
            var nsmgr = CreateNamespaceManager(xmlDoc);
            var nodeList = xmlDoc.SelectNodes(xPath, nsmgr);
            return nodeList;
        }
    }
}
