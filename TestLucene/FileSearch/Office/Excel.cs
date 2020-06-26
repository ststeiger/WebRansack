
namespace TestLucene.FileSearch.Office
{


    // https://github.com/tonyqus/toxy
    // https://github.com/tonyqus/npoi
    class Excel
    {


        // The DOM approach.
        // Note that the code below works only for cells that contain numeric values.
        // 
        static void ReadExcelFileDOM(string fileName)
        {
            using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument spreadsheetDocument = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(fileName, false))
            {
                DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

                DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = System.Linq.Enumerable.First(workbookPart.WorksheetParts);
                DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = System.Linq.Enumerable.First(worksheetPart.Worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.SheetData>());
                string text;
                foreach (DocumentFormat.OpenXml.Spreadsheet.Row r in sheetData.Elements<DocumentFormat.OpenXml.Spreadsheet.Row>())
                {
                    foreach (DocumentFormat.OpenXml.Spreadsheet.Cell c in r.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                    {
                        text = c.CellValue.Text;
                        System.Console.Write(text + " ");
                    }
                }

                System.Console.WriteLine();
                System.Console.ReadKey();
            }
        }


        // The SAX approach.
        static void ReadExcelFileSAX(string fileName)
        {
            using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument spreadsheetDocument = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(fileName, false))
            {
                DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

                DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = System.Linq.Enumerable.First(workbookPart.WorksheetParts);

                DocumentFormat.OpenXml.OpenXmlReader reader = DocumentFormat.OpenXml.OpenXmlReader.Create(worksheetPart);
                string text;
                while (reader.Read())
                {
                    if (reader.ElementType == typeof(DocumentFormat.OpenXml.Spreadsheet.CellValue))
                    {
                        text = reader.GetText();
                        System.Console.Write(text + " ");
                    }
                }

                System.Console.WriteLine();
                System.Console.ReadKey();
            }
        }


    }


}
