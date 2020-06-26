﻿
namespace TestLucene.FileSearch
{
    class HtmlSearch
    {
    }




    class Test
    {
        static void Main()
        {
            string xml = @"
<root>
  <foo />
  <foo>
     <bar attr='value'/>
     <bar other='va' />
  </foo>
  <foo><bar /></foo>
</root>";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            System.Xml.XmlNode node = doc.SelectSingleNode("//@attr");
            System.Console.WriteLine(FindXPath(node));
            System.Console.WriteLine(doc.SelectSingleNode(FindXPath(node)) == node);
        }

        static string FindXPath(System.Xml.XmlNode node)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case System.Xml.XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((System.Xml.XmlAttribute)node).OwnerElement;
                        break;
                    case System.Xml.XmlNodeType.Element:
                        int index = FindElementIndex((System.Xml.XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case System.Xml.XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new System.ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new System.ArgumentException("Node was not in a document");
        }

        static int FindElementIndex(System.Xml.XmlElement element)
        {
            System.Xml.XmlNode parentNode = element.ParentNode;
            if (parentNode is System.Xml.XmlDocument)
            {
                return 1;
            }
            System.Xml.XmlElement parent = (System.Xml.XmlElement)parentNode;
            int index = 1;
            foreach (System.Xml.XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is System.Xml.XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new System.ArgumentException("Couldn't find element within parent");
        }
    }

}
