
namespace TestLucene.FileSearch.Office
{

    using System.Linq;


    // https://docs.microsoft.com/en-us/office/open-xml/how-to-get-the-titles-of-all-the-slides-in-a-presentation
    class PowerPointTitles
    {

        // Get a list of the titles of all the slides in the presentation.
        public static System.Collections.Generic.IList<string> GetSlideTitles(string presentationFile)
        {
            // Open the presentation as read-only.
            using (DocumentFormat.OpenXml.Packaging.PresentationDocument presentationDocument =
                DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(presentationFile, false))
            {
                return GetSlideTitles(presentationDocument);
            }
        }

        // Get a list of the titles of all the slides in the presentation.
        public static System.Collections.Generic.IList<string> GetSlideTitles(DocumentFormat.OpenXml.Packaging.PresentationDocument presentationDocument)
        {
            if (presentationDocument == null)
            {
                throw new System.ArgumentNullException("presentationDocument");
            }

            // Get a PresentationPart object from the PresentationDocument object.
            DocumentFormat.OpenXml.Packaging.PresentationPart presentationPart = presentationDocument.PresentationPart;

            if (presentationPart != null &&
                presentationPart.Presentation != null)
            {
                // Get a Presentation object from the PresentationPart object.
                DocumentFormat.OpenXml.Presentation.Presentation presentation = presentationPart.Presentation;

                if (presentation.SlideIdList != null)
                {
                    System.Collections.Generic.List<string> titlesList = new System.Collections.Generic.List<string>();

                    // Get the title of each slide in the slide order.
                    foreach (var slideId in presentation.SlideIdList.Elements<DocumentFormat.OpenXml.Presentation.SlideId>())
                    {
                        DocumentFormat.OpenXml.Packaging.SlidePart slidePart = presentationPart.GetPartById(slideId.RelationshipId) as DocumentFormat.OpenXml.Packaging.SlidePart;

                        // Get the slide title.
                        string title = GetSlideTitle(slidePart);

                        // An empty title can also be added.
                        titlesList.Add(title);
                    }

                    return titlesList;
                }

            }

            return null;
        }

        // Get the title string of the slide.
        public static string GetSlideTitle(DocumentFormat.OpenXml.Packaging.SlidePart slidePart)
        {
            if (slidePart == null)
            {
                throw new System.ArgumentNullException("presentationDocument");
            }

            // Declare a paragraph separator.
            string paragraphSeparator = null;

            if (slidePart.Slide != null)
            {
                // Find all the title shapes.

                var shapes = from shape in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Presentation.Shape>()
                             where IsTitleShape(shape)
                             select shape;

                System.Text.StringBuilder paragraphText = new System.Text.StringBuilder();

                foreach (var shape in shapes)
                {
                    // Get the text in each paragraph in this shape.
                    foreach (var paragraph in shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        // Add a line break.
                        paragraphText.Append(paragraphSeparator);

                        foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            paragraphText.Append(text.Text);
                        }

                        paragraphSeparator = "\n";
                    }
                }

                return paragraphText.ToString();
            }

            return string.Empty;
        }

        // Determines whether the shape is a title shape.
        private static bool IsTitleShape(DocumentFormat.OpenXml.Presentation.Shape shape)
        {
            var placeholderShape = shape.NonVisualShapeProperties.ApplicationNonVisualDrawingProperties.GetFirstChild<DocumentFormat.OpenXml.Presentation.PlaceholderShape>();
            if (placeholderShape != null && placeholderShape.Type != null && placeholderShape.Type.HasValue)
            {
                switch ((DocumentFormat.OpenXml.Presentation.PlaceholderValues)placeholderShape.Type)
                {
                    // Any title shape.
                    case DocumentFormat.OpenXml.Presentation.PlaceholderValues.Title:

                    // A centered title.
                    case DocumentFormat.OpenXml.Presentation.PlaceholderValues.CenteredTitle:
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }

    }
}
