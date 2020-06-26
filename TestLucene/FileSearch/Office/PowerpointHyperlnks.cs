
namespace TestLucene.FileSearch.Office
{


    // https://docs.microsoft.com/en-us/office/open-xml/how-to-get-all-the-external-hyperlinks-in-a-presentation
    class PowerpointHyperlnks
    {

        // Returns all the external hyperlinks in the slides of a presentation.
        public static System.Collections.Generic.IEnumerable<string> GetAllExternalHyperlinksInPresentation(string fileName)
        {
            // Declare a list of strings.
            System.Collections.Generic.List<string> ret = new System.Collections.Generic.List<string>();

            // Open the presentation file as read-only.
            using (DocumentFormat.OpenXml.Packaging.PresentationDocument document = DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(fileName, false))
            {
                // Iterate through all the slide parts in the presentation part.
                foreach (DocumentFormat.OpenXml.Packaging.SlidePart slidePart in document.PresentationPart.SlideParts)
                {
                    System.Collections.Generic.IEnumerable<DocumentFormat.OpenXml.Drawing.HyperlinkType> links = slidePart.Slide.Descendants< DocumentFormat.OpenXml.Drawing.HyperlinkType >();

                    // Iterate through all the links in the slide part.
                    foreach (DocumentFormat.OpenXml.Drawing.HyperlinkType link in links)
                    {
                        // Iterate through all the external relationships in the slide part. 
                        foreach (DocumentFormat.OpenXml.Packaging.HyperlinkRelationship relation in slidePart.HyperlinkRelationships)
                        {
                            // If the relationship ID matches the link ID…
                            if (relation.Id.Equals(link.Id))
                            {
                                // Add the URI of the external relationship to the list of strings.
                                ret.Add(relation.Uri.AbsoluteUri);
                            }
                        }
                    }
                }
            }

            // Return the list of strings.
            return ret;
        }


    }
}
