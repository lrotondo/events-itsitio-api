using ClosedXML.Excel;

namespace events_api.Extensions
{
    public static class XLWorkbookExtensions
    {
        public static string ToBase64(this XLWorkbook workbook)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
