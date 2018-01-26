using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/14 2:49:13
 * Description:		Please input class summary
 * Version History:	Created,2014/11/14 2:49:13
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Sunnet.Framework.Excel
{
    public static class WorksheetExtensions
    {
        /// <param name="worksheet"></param>
        /// <param name="cellses">A4:A11 , A12:A21</param>
        public static  void MergeCells(this Worksheet worksheet, List<string> cellses)
        {
            MergeCells mergeCells;
            if (worksheet.Elements<MergeCells>().Any())
            {
                mergeCells = worksheet.Elements<MergeCells>().First();
            }
            else
            {
                mergeCells = new MergeCells();
                // Insert a MergeCells object into the specified position.
                if (worksheet.Elements<CustomSheetView>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<CustomSheetView>().First());
                }
                else if (worksheet.Elements<DataConsolidate>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<DataConsolidate>().First());
                }
                else if (worksheet.Elements<SortState>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SortState>().First());
                }
                else if (worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.AutoFilter>().Any())
                {
                    worksheet.InsertAfter(mergeCells,
                        worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.AutoFilter>().First());
                }
                else if (worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Scenarios>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Scenarios>().First());
                }
                else if (worksheet.Elements<ProtectedRanges>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<ProtectedRanges>().First());
                }
                else if (worksheet.Elements<SheetProtection>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetProtection>().First());
                }
                else if (worksheet.Elements<SheetCalculationProperties>().Any())
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetCalculationProperties>().First());
                }
                else
                {
                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                }
            }
            cellses.ForEach(cells =>
            {
                MergeCell mergeCell = new MergeCell() { Reference = new StringValue(cells) };
                mergeCells.Append(mergeCell);
            });
        }

    }
}
