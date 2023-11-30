using ClosedXML.Excel;

namespace events_api.Extensions
{
    public static class IXLWorksheetExtensions
    {
        public static void ApplyTableStyle(this IXLWorksheet worksheet)
        {
            worksheet.Columns().AdjustToContents();
            worksheet.Cells().Style.Alignment.WrapText = true;
            worksheet.Cells().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cells().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            foreach (var column in worksheet.Columns()) column.Width += 2; // Padding between columns
            var range = worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed());
            var table = range.CreateTable();
            table.Theme = XLTableTheme.TableStyleLight2;
        }

        public static void AddColumns(this IXLWorksheet worksheet, string[] columns)
        {
            for (var i = 0; i < columns.Length; ++i) worksheet.Cell(1, i + 1).Value = columns[i];
        }
    }
}
