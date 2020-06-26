
namespace TestLucene.FileSearch.Office
{


    // https://docs.microsoft.com/en-us/office/open-xml/how-to-get-all-the-text-in-a-slide-in-a-presentation
    class PowerPointOneSlide
    {

        // Get all the text in a slide.
        public static string[] GetAllTextInSlide(string presentationFile, int slideIndex)
        {
            // Open the presentation as read-only.
            using (DocumentFormat.OpenXml.Packaging.PresentationDocument presentationDocument = DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(presentationFile, false))
            {
                // Pass the presentation and the slide index
                // to the next GetAllTextInSlide method, and
                // then return the array of strings it returns. 
                return GetAllTextInSlide(presentationDocument, slideIndex);
            }
        }
        public static string[] GetAllTextInSlide(DocumentFormat.OpenXml.Packaging.PresentationDocument presentationDocument, int slideIndex)
        {
            // Verify that the presentation document exists.
            if (presentationDocument == null)
            {
                throw new System.ArgumentNullException("presentationDocument");
            }

            // Verify that the slide index is not out of range.
            if (slideIndex < 0)
            {
                throw new System.ArgumentOutOfRangeException("slideIndex");
            }

            // Get the presentation part of the presentation document.
            DocumentFormat.OpenXml.Packaging.PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Verify that the presentation part and presentation exist.
            if (presentationPart != null && presentationPart.Presentation != null)
            {
                // Get the Presentation object from the presentation part.
                DocumentFormat.OpenXml.Presentation.Presentation presentation = presentationPart.Presentation;

                // Verify that the slide ID list exists.
                if (presentation.SlideIdList != null)
                {
                    // Get the collection of slide IDs from the slide ID list.
                    DocumentFormat.OpenXml.OpenXmlElementList slideIds =
                        presentation.SlideIdList.ChildElements;

                    // If the slide ID is in range...
                    if (slideIndex < slideIds.Count)
                    {
                        // Get the relationship ID of the slide.
                        string slidePartRelationshipId = (slideIds[slideIndex] as DocumentFormat.OpenXml.Presentation.SlideId).RelationshipId;

                        // Get the specified slide part from the relationship ID.
                        DocumentFormat.OpenXml.Packaging.SlidePart slidePart =
                            (DocumentFormat.OpenXml.Packaging.SlidePart)presentationPart.GetPartById(slidePartRelationshipId);

                        // Pass the slide part to the next method, and
                        // then return the array of strings that method
                        // returns to the previous method.
                        return GetAllTextInSlide(slidePart);
                    }
                }
            }

            // Else, return null.
            return null;
        }
        public static string[] GetAllTextInSlide(DocumentFormat.OpenXml.Packaging.SlidePart slidePart)
        {
            // Verify that the slide part exists.
            if (slidePart == null)
            {
                throw new System.ArgumentNullException("slidePart");
            }

            // Create a new linked list of strings.
            System.Collections.Generic.LinkedList<string> texts = new System.Collections.Generic.LinkedList<string>();

            // If the slide exists...
            if (slidePart.Slide != null)
            {
                // Iterate through all the paragraphs in the slide.
                foreach (DocumentFormat.OpenXml.Drawing.Paragraph paragraph in
                    slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                {
                    // Create a new string builder.                    
                    System.Text.StringBuilder paragraphText = new System.Text.StringBuilder();

                    // Iterate through the lines of the paragraph.
                    foreach (DocumentFormat.OpenXml.Drawing.Text text in
                        paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                    {
                        // Append each line to the previous lines.
                        paragraphText.Append(text.Text);
                    }

                    if (paragraphText.Length > 0)
                    {
                        // Add each paragraph to the linked list.
                        texts.AddLast(paragraphText.ToString());
                    }
                }
            }

            if (texts.Count > 0)
            {
                // Return an array of strings.
                return System.Linq.Enumerable.ToArray(texts);
            }
            else
            {
                return null;
            }
        }

    }
}
