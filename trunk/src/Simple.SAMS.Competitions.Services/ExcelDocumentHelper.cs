using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Simple.SimplyLog.ImportExport
{
    public class ExcelDocumentHelper
    {
        
        public static void LoadFromSheet(string fileName, DataTable outputTable, bool includeHeadersRow = true, bool inferColumns = false)
        {
            using (var spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                var workbookPart = spreadSheetDocument.WorkbookPart;
                var sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                var relationshipId = sheets.First().Id.Value;
                var worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                var workSheet = worksheetPart.Worksheet;
                var sheetData = workSheet.GetFirstChild<SheetData>();
                var rows = sheetData.Descendants<Row>();
                if (rows.Any())
                {
                    if (inferColumns)
                    {
                        includeHeadersRow = true;
                        outputTable.Columns.Clear();
                        var headersRow = rows.First();
                        var cells = headersRow.Descendants<Cell>().ToArray();
                        foreach (var cell in cells)
                        {
                            outputTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                        }
                    }

                    foreach (var row in rows.Skip(includeHeadersRow ? 1 : 0))
                    {
                        var tempRow = outputTable.NewRow();
                        var cells = row.Descendants<Cell>().ToArray();
                        foreach (var cell in cells)
                        {
                            var columnName = GetColumnName(cell.CellReference);
                            var cellColumnIndex = GetColumnIndexFromName(columnName);
                            if (cellColumnIndex.HasValue)
                            {
                                if (cellColumnIndex.Value < tempRow.Table.Columns.Count)
                                {
                                    tempRow[cellColumnIndex.Value] = GetCellValue(spreadSheetDocument, cell);
                                }
                            }
                        }

                        outputTable.Rows.Add(tempRow);
                    }
                }

            }
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);

            return match.Value;
        }
        private static List<char> Letters = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };
        /// <summary>
        /// Given just the column name (no row index), it will return the zero based column index.
        /// Note: This method will only handle columns with a length of up to two (ie. A to Z and AA to ZZ). 
        /// A length of three can be implemented when needed.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful; otherwise null</returns>
        public static int? GetColumnIndexFromName(string columnName)
        {
            int? columnIndex = null;

            string[] colLetters = Regex.Split(columnName, "([A-Z]+)");
            colLetters = colLetters.Where(s => !string.IsNullOrEmpty(s)).ToArray();

            if (colLetters.Count() <= 2)
            {
                int index = 0;
                foreach (string col in colLetters)
                {
                    List<char> col1 = colLetters.ElementAt(index).ToCharArray().ToList();
                    int? indexValue = Letters.IndexOf(col1.ElementAt(index));

                    if (indexValue != -1)
                    {
                        // The first letter of a two digit column needs some extra calculations
                        if (index == 0 && colLetters.Count() == 2)
                        {
                            columnIndex = columnIndex == null ? (indexValue + 1) * 26 : columnIndex + ((indexValue + 1) * 26);
                        }
                        else
                        {
                            columnIndex = columnIndex == null ? indexValue : columnIndex + indexValue;
                        }
                    }

                    index++;
                }
            }

            return columnIndex;
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            var stringTablePart = document.WorkbookPart.SharedStringTablePart;
            var value = cell.CellValue.IsNotNull() ? cell.CellValue.InnerXml : string.Empty;

            if (value.NotNullOrEmpty())
            {
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    value =  stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
                else if (cell.DataType.IsNotNull() && cell.DataType.Value == CellValues.Date)
                {
                    value = cell.CellValue.InnerText;
                }
            }

            return value;
        }
    }
}
