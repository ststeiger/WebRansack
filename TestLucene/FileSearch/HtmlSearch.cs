
using HtmlAgilityPack;

namespace TestLucene.FileSearch
{
    class HtmlSearch
    {

        public static void foo(HtmlDocument doc)
        {
            var root = doc.DocumentNode;
            var sb = new System.Text.StringBuilder();
            foreach (var node in root.DescendantNodesAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        sb.AppendLine(text.Trim());
                }
            }
        }

        public static void Extract()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(@"<html><body><p>foo <a href='http://www.example.com'>bar</a> baz</p></body></html>");
            Extract(doc);
        }



        // https://stackoverflow.com/questions/4182594/grab-all-text-from-html-with-html-agility-pack
        public static string Extract(HtmlDocument doc)
        {
            string ret = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                System.Console.WriteLine("text=" + node.InnerText);
                sb.Append(node.InnerText);
            }
            
            ret = sb.ToString();
            sb.Clear();
            sb = null;
            return ret;
        }

    }


    class rofl
    {
        public static void foo()
        {
            string html = @"
<html><body>
  <foo />
  <foo>
     <bar attr='value'/>
     <bar other='va' />
     <p>foo <a href='http://www.example.com'>bar</a> baz</p>
  </foo>
  <foo><bar /></foo>
</body></html>";
            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // https://www.codeproject.com/Articles/659019/Scraping-HTML-DOM-elements-using-HtmlAgilityPack-H
            // HtmlAgilityPack.HtmlNodeCollection aTags = doc.DocumentNode.SelectNodes("//a");
            // if (aTags != null) { foreach (HtmlAgilityPack.HtmlNode aTag in aTags) {} }
            // aTag.Attributes["href"].Value
            
            HtmlAgilityPack.HtmlNode node = doc.DocumentNode;

            if (node.HasChildNodes)
            {
                // node.NodeType == 
                // node.ChildNodes    
            }

            
            
        }
        
        
        
        
        
    }

    class lol
    {
        
        // https://stackoverflow.com/questions/4182594/grab-all-text-from-html-with-html-agility-pack
        public static string Extract(HtmlDocument doc)
        {
            string ret = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                System.Console.WriteLine("text=" + node.InnerText);
                sb.Append(node.InnerText);
            }
            
            ret = sb.ToString();
            sb.Clear();
            sb = null;
            return ret;
        }
        
        static void Test()
        {
            string html = @"
<html><body>
  <foo />
  <foo>
     <bar attr='value'/>
     <bar other='va' />
     <p>foo <a href='http://www.example.com'>bar</a> baz</p>
  </foo>
  <foo><bar /></foo>
</body></html>";
            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode("//@attr");
            System.Console.WriteLine(FindXPath(node));
            System.Console.WriteLine(doc.DocumentNode.SelectSingleNode(FindXPath(node)) == node);
        }

        
        static string FindXPath(HtmlAgilityPack.HtmlNode node)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case HtmlNodeType.Comment: // Was attribute
                        builder.Insert(0, "/@" + node.Name);
                        node = node.OwnerDocument.DocumentNode;
                        break;
                    case HtmlNodeType.Element:
                        int index = FindElementIndex(node);
                        builder.Insert(0, "/" + node.Name + "[" + index.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]");
                        node = node.ParentNode;
                        break;
                    case HtmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new System.ArgumentException("Only elements and attributes are supported");
                }
            }
            
            throw new System.ArgumentException("Node was not in a document");
        }
        
        
        
        static int FindElementIndex(HtmlAgilityPack.HtmlNode element)
        {
            HtmlAgilityPack.HtmlNode parentNode = element.ParentNode;
            if (parentNode is HtmlAgilityPack.HtmlDocument)
            {
                return 1;
            }
            
            HtmlAgilityPack.HtmlNode parent = parentNode;
            int index = 1;
            foreach (HtmlAgilityPack.HtmlNode candidate in parent.ChildNodes)
            {
                if (candidate is HtmlAgilityPack.HtmlNode && candidate.Name == element.Name)
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


    class TestXPath
    {
        static void Test()
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
