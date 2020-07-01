
namespace TestLucene.FileSearch
{


    class HtmlSearch
    {


        public static void Extract()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(@"<html><body><p>foo <a href='http://www.example.com'>bar</a> baz</p></body></html>");
            Extract(doc);
        }


        public static void ExtractText(HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlAgilityPack.HtmlNode rootNode = doc.DocumentNode;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (HtmlAgilityPack.HtmlNode node in rootNode.DescendantsAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        sb.AppendLine(text.Trim());
                } // End if (!node.HasChildNodes) 

            } // Next node 
        } // End Sub ExtractText 


        // https://stackoverflow.com/questions/4182594/grab-all-text-from-html-with-html-agility-pack
        public static string Extract(HtmlAgilityPack.HtmlDocument doc)
        {
            string ret = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                System.Console.WriteLine("text=" + node.InnerText);
                sb.Append(node.InnerText);
            } // Next node 

            ret = sb.ToString();
            sb.Clear();
            sb = null;
            return ret;
        } // End Sub Extract 


    } // End class HtmlSearch


    class HtmlSelectorTest
    {


        public static void Test()
        {
            string html = @"
<html><body>
  <foo />
  <foo id=""lol"" >
     <bar attr='value'/>
     <bar other='va' />
     <p>foo <a href='http://www.example.com'>bar</a> baz</p>
  </foo>
  <foo><bar /></foo>
</body></html>";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);


            HtmlAgilityPack.HtmlNode theEl = doc.DocumentNode.SelectSingleNode("//@attr");
            string xp = theEl.XPath;
            string css1 = CssSelector(theEl);
            string css2 = ShortCssSelector(theEl);
            
            System.Console.WriteLine(xp, css1, css2);

            // https://www.codeproject.com/Articles/659019/Scraping-HTML-DOM-elements-using-HtmlAgilityPack-H
            // HtmlAgilityPack.HtmlNodeCollection aTags = doc.DocumentNode.SelectNodes("//a");
            // if (aTags != null) { foreach (HtmlAgilityPack.HtmlNode aTag in aTags) {} }
            // aTag.Attributes["href"].Value

            HtmlAgilityPack.HtmlNode node = doc.DocumentNode;

            if (node.HasChildNodes)
            {
                // node.NodeType == 
                // node.ChildNodes    
            } // End if (node.HasChildNodes) 

        } // End Sub Test 


        // https://stackoverflow.com/questions/8588301/how-to-generate-unique-css-selector-for-dom-element
        // https://stackoverflow.com/questions/3640293/is-there-a-way-to-generate-a-css-selector-based-on-a-dom-element-in-jquery
        public static string CssSelector(HtmlAgilityPack.HtmlNode el)
        {
            System.Collections.Generic.List<string> path = new System.Collections.Generic.List<string>();
            
            HtmlAgilityPack.HtmlNode parent;
            while ((parent = el.ParentNode).NodeType != HtmlAgilityPack.HtmlNodeType.Document)
            {
                int ind = FindElementIndex(el);
                // path.unshift(`${el.tagName}:nth-child(${[].indexOf.call(parent.children, el)+1})`);
                path.Insert(0, el.Name + ":nth-child(" + ind.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");
                el = parent;
            } // Whend 

            string selector = string.Join(" > ", path.ToArray()).ToLowerInvariant();
            return selector;
        } // End Function CssSelector 


        // https://stackoverflow.com/questions/8588301/how-to-generate-unique-css-selector-for-dom-element
        public static string ShortCssSelector(HtmlAgilityPack.HtmlNode el)
        {
            System.Collections.Generic.List<string> path = new System.Collections.Generic.List<string>();
            HtmlAgilityPack.HtmlNode parent;

            while ((parent = el.ParentNode) != null)
            {
                string tag = el.Name;
                string sel = null;


                if (!string.IsNullOrWhiteSpace(el.Id))
                {
                    sel = "#" + el.Id;
                    path.Insert(0, sel);
                    break;
                } // End if (!string.IsNullOrWhiteSpace(el.Id)) 

                if (string.IsNullOrWhiteSpace(sel))
                {
                    HtmlAgilityPack.HtmlNodeCollection siblings = parent.ChildNodes;

                    int cnt = FindElementIndex(el);
                    if (cnt == 1)
                        sel = tag;
                    else
                        sel = tag + ":nth-child(" + cnt.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")";
                } // End if (string.IsNullOrWhiteSpace(sel)) 

                path.Insert(0, sel);
                el = parent;
            } // Whend 

            string selector = string.Join(" > ", path.ToArray()).ToLowerInvariant();
            return selector;
        } // End Function ShortCssSelector 


        private static int FindElementIndex(HtmlAgilityPack.HtmlNode element)
        {
            HtmlAgilityPack.HtmlNode parentNode = element.ParentNode;
            if (parentNode.NodeType == HtmlAgilityPack.HtmlNodeType.Document)
            {
                return 1;
            } // End if (parentNode.NodeType == HtmlNodeType.Document) 

            HtmlAgilityPack.HtmlNode parent = parentNode;
            int index = 1;
            foreach (HtmlAgilityPack.HtmlNode candidate in parent.ChildNodes)
            {
                if (candidate.NodeType == HtmlAgilityPack.HtmlNodeType.Element && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    } // End if (candidate == element) 

                    index++;
                } // End if (candidate.NodeType == HtmlNodeType.Element && candidate.Name == element.Name) 

            } // Next candidate 

            throw new System.ArgumentException("Couldn't find element within parent");
        } // End Function FindElementIndex 


    } // End Class HtmlSelectorTest 


    class XmlSelectorTest
    {
        
        
        public static void Test()
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
            
            string xpath = FindXPath(node);
            System.Console.WriteLine(xpath);
            System.Console.WriteLine(doc.SelectSingleNode(xpath) == node);
        } // End Sub Test 


        public static string FindXPath(System.Xml.XmlNode node)
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
                } // End Switch 

            } // Whend 

            throw new System.ArgumentException("Node was not in a document");
        } // End Function FindXPath 


        private static int FindElementIndex(System.Xml.XmlElement element)
        {
            System.Xml.XmlNode parentNode = element.ParentNode;
            if (parentNode is System.Xml.XmlDocument)
            {
                return 1;
            } // End if (parentNode is System.Xml.XmlDocument) 

            int index = 1;
            System.Xml.XmlElement parent = (System.Xml.XmlElement)parentNode;
            foreach (System.Xml.XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is System.Xml.XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    } // End if (candidate == element) 

                    index++;
                } // End if (candidate is System.Xml.XmlElement && candidate.Name == element.Name) 

            } // Next candidate 
            
            throw new System.ArgumentException("Couldn't find element within parent");
        } // End Function FindElementIndex 


    } // End Class XmlSelectorTest 


} // End Namespace 
