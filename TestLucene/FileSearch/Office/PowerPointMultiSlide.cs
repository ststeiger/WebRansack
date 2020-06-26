
namespace TestLucene.FileSearch.Office
{


    // https://docs.microsoft.com/en-us/office/open-xml/how-to-get-all-the-text-in-all-slides-in-a-presentation
    class PowerPointMultiSlide
    {


        public static int CountSlides(string presentationFile)
        {
            // Open the presentation as read-only.
            using (DocumentFormat.OpenXml.Packaging.PresentationDocument presentationDocument = DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(presentationFile, false))
            {
                // Pass the presentation to the next CountSlides method
                // and return the slide count.
                return CountSlides(presentationDocument);
            }
        }

        // Count the slides in the presentation.
        public static int CountSlides(DocumentFormat.OpenXml.Packaging.PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new System.ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            DocumentFormat.OpenXml.Packaging.PresentationPart presentationPart = presentationDocument.PresentationPart;
            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = System.Linq.Enumerable.Count(presentationPart.SlideParts);
            }
            // Return the slide count to the previous method.
            return slidesCount;
        }

        public static void GetSlideIdAndText(out string sldText, string docName, int index)
        {
            using (DocumentFormat.OpenXml.Packaging.PresentationDocument ppt = DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(docName, false))
            {
                // Get the relationship ID of the first slide.
                DocumentFormat.OpenXml.Packaging.PresentationPart part = ppt.PresentationPart;
                DocumentFormat.OpenXml.OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;

                string relId = (slideIds[index] as DocumentFormat.OpenXml.Presentation.SlideId).RelationshipId;

                // Get the slide part from the relationship ID.
                DocumentFormat.OpenXml.Packaging.SlidePart slide = (DocumentFormat.OpenXml.Packaging.SlidePart)part.GetPartById(relId);

                // Build a StringBuilder object.
                System.Text.StringBuilder paragraphText = new System.Text.StringBuilder();

                // Get the inner text of the slide:
                System.Collections.Generic.IEnumerable<DocumentFormat.OpenXml.Drawing.Text> texts = slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>();
                foreach (DocumentFormat.OpenXml.Drawing.Text text in texts)
                {
                    paragraphText.Append(text.Text);
                }
                sldText = paragraphText.ToString();
            }
        }
    }

}