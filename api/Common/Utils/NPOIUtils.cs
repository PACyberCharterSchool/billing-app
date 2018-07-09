using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace api.Common.Utils
{
  public class NPOIHelper
  {
    /// <param name="dt">the C# DataTable to build the workbook from</param>
    public static XSSFWorkbook BuildExcelWorkbookFromDataTable(DataTable dt, List<string> headers, string sheetName)
    {
      XSSFWorkbook wb = new XSSFWorkbook();
      ISheet sheet = wb.CreateSheet(sheetName);
      IFont font = wb.CreateFont();
      ICellStyle cs = wb.CreateCellStyle();
  
      font.FontHeightInPoints = (short)14;
      font.Boldweight = (short)FontBoldWeight.Bold;
      cs.SetFont(font);
      
      // make the header row
      IRow row = sheet.CreateRow(0);
      for (int j = 0; j < dt.Columns.Count; j++) {
        ICell cell = row.CreateCell(j);
        string columnName = dt.Columns[j].ToString();
        string columnLabel = headers[j];
        cell.SetCellValue(columnLabel);
        cell.CellStyle = cs;
      }

      font.Boldweight = (short)FontBoldWeight.Normal;
      font.FontHeightInPoints = (short)12;
      cs.SetFont(font);

      // now add in the data rows
      for (int i = 0; i < dt.Rows.Count; i++) {
        row = sheet.CreateRow(i + 1);
        for (int j = 0; j < dt.Columns.Count; j++) {
          ICell cell = row.CreateCell(j);
          string columnName = dt.Columns[j].ToString();
          cell.SetCellValue(dt.Rows[i][columnName].ToString());
          cell.CellStyle = cs;
        }
      }

      return wb;
    }

    /// <param name="destSheet"> the sheet to create from the copy. </param>
    /// <param name="newSheet"> the sheet to copy. </param>
    /// <param name="copyStyle"> true copy the style. </param>
    public static void MergeSheets(XSSFSheet destSheet, XSSFSheet srcSheet)
    {
      MergeSheets(destSheet, srcSheet, true);
    }

    /// <param name="destSheet"> the sheet being copied/merged. </param>
    /// <param name="newSheet"> the destination sheet being copied/merged into. </param>
    /// <param name="copyStyle"> true copy the style. </param>
    private static void MergeSheets(XSSFSheet destSheet, XSSFSheet srcSheet, bool copyStyle)
    {
      CopySheets(destSheet, srcSheet, copyStyle);
    }

    /// <param name="newSheet"> the sheet to create from the copy. </param>
    /// <param name="sheet"> the sheet to copy. </param>
    public static void CopySheets(XSSFSheet newSheet, XSSFSheet sheet)
    {
      CopySheets(newSheet, sheet, true);
    }
 
    /// <param name="newSheet"> the sheet to create from the copy. </param>
    /// <param name="sheet"> the sheet to copy. </param>
    /// <param name="copyStyle"> true copy the style. </param>
    private static void CopySheets(XSSFSheet newSheet, XSSFSheet sheet, bool copyStyle)
    {
      Console.WriteLine($"NPOIUtils.CopySheets(XSSFSheet, XSSFSheet, bool): sheet name is {sheet.SheetName}.  Number of rows is {sheet.LastRowNum}.");
      int maxColumnNum = 0;
      IDictionary<int?, ICellStyle> styleMap = (copyStyle) ? new Dictionary<int?, ICellStyle>() : null;
      for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
      {
        IRow srcRow = sheet.GetRow(i);
        IRow destRow = newSheet.CreateRow(i);
        Console.WriteLine($"NPOIUtils.CopySheets(XSSFSheet, XSSFSheet, bool): srcRow is {srcRow}.");
        if (srcRow != null)
        {
          Console.WriteLine($"NPOIUtils.CopySheets(XSSFSheet, XSSFSheet, bool):  copying row {i} from {sheet.SheetName} to {newSheet.SheetName}.");
          CopyRow(sheet, newSheet, srcRow, destRow, styleMap);
          if (srcRow.LastCellNum > maxColumnNum)
          {
              maxColumnNum = srcRow.LastCellNum;
          }
        }
      }

      for (int i = 0; i <= maxColumnNum; i++)
      {
        newSheet.SetColumnWidth(i, sheet.GetColumnWidth(i));
      }
    }
 
    /// <param name="srcSheet"> the sheet to copy. </param>
    /// <param name="destSheet"> the sheet to create. </param>
    /// <param name="srcRow"> the row to copy. </param>
    /// <param name="destRow"> the row to create. </param>
    /// <param name="styleMap"> - </param>
    private static void CopyRow(ISheet srcSheet, ISheet destSheet, IRow srcRow, IRow destRow,
                                IDictionary<int?, ICellStyle> styleMap)
    {
      // manage a list of merged zone in order to not insert two times a merged zone
      SortedSet<CellRangeAddressWrapper> mergedRegions = new SortedSet<CellRangeAddressWrapper>();
      destRow.Height = srcRow.Height;
      // reckoning delta rows
      int deltaRows = destRow.RowNum - srcRow.RowNum;
      // pour chaque row
      int j = srcRow.FirstCellNum;
      if (j < 0)
      {
        j = 0;
      }
      for (; j <= srcRow.LastCellNum; j++)
      {
        ICell oldCell = srcRow.GetCell(j); // ancienne cell
        ICell newCell = destRow.GetCell(j); // new cell
        if (oldCell != null)
        {
          if (newCell == null)
          {
              newCell = destRow.CreateCell(j);
          }
          // copy chaque cell
          CopyCell(oldCell, newCell, styleMap);
          // copy les informations de fusion entre les cellules
          CellRangeAddress mergedRegion = GetMergedRegion(srcSheet, srcRow.RowNum, (short)oldCell.ColumnIndex);

          if (mergedRegion != null)
          {
            CellRangeAddress newMergedRegion = new CellRangeAddress(mergedRegion.FirstRow + deltaRows,
                                                                    mergedRegion.LastRow + deltaRows,
                                                                    mergedRegion.FirstColumn,
                                                                    mergedRegion.LastColumn);
            CellRangeAddressWrapper wrapper = new CellRangeAddressWrapper(newMergedRegion);
            if (IsNewMergedRegion(wrapper, mergedRegions))
            {
                mergedRegions.Add(wrapper);
                destSheet.AddMergedRegion(wrapper.range);
            }
          }
        }
      }
    }
 
    /// <param name="oldCell"> </param>
    /// <param name="newCell"> </param>
    /// <param name="styleMap"> </param>
    private static void CopyCell(ICell oldCell, ICell newCell, IDictionary<int?, ICellStyle> styleMap)
    {
      if (styleMap != null)
      {
        if (oldCell.Sheet.Workbook == newCell.Sheet.Workbook)
        {
          newCell.CellStyle = oldCell.CellStyle;
        }
        else
        {
          int stHashCode = oldCell.CellStyle.GetHashCode();
          ICellStyle newCellStyle; // = styleMap.ContainsKey(stHashCode) ? styleMap[stHashCode] : null;
          if (!styleMap.ContainsKey(stHashCode))
          {
            newCellStyle = newCell.Sheet.Workbook.CreateCellStyle();
            newCellStyle.Alignment = oldCell.CellStyle.Alignment;
            newCellStyle.VerticalAlignment = oldCell.CellStyle.VerticalAlignment;
            newCellStyle.WrapText = oldCell.CellStyle.WrapText;
            newCellStyle.DataFormat = oldCell.CellStyle.DataFormat;
            //newCellStyle.CloneStyleFrom(oldCell.CellStyle);
            styleMap.Add(stHashCode, newCellStyle);
          }
          else
          {
            newCellStyle = styleMap[stHashCode];
          }
          newCell.CellStyle = newCellStyle;
        }
      }

      switch (oldCell.CellType)
      {
        case CellType.String:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value is {oldCell.ToString()}.");
          newCell.SetCellValue(oldCell.ToString());
          break;
        case CellType.Numeric:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value is {oldCell.NumericCellValue}.");
          newCell.SetCellValue(oldCell.NumericCellValue);
          break;
        case CellType.Blank:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value is blank.");
          newCell.SetCellType(CellType.Blank);
          break;
        case CellType.Boolean:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value is {oldCell.BooleanCellValue}.");
          newCell.SetCellValue(oldCell.BooleanCellValue);
          break;
        case CellType.Error:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value is {oldCell.ErrorCellValue}.");
          newCell.SetCellErrorValue(oldCell.ErrorCellValue);
          break;
        case CellType.Formula:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value is {oldCell.CellFormula}.");
          newCell.SetCellFormula(oldCell.CellFormula);
          break;
        default:
          Console.WriteLine($"NPOIHelper.CopyCell():  oldCell value type could not be determined.");
          break;
      }
    }
 
    /// <summary>
    /// Récupère les informations de fusion des cellules dans la sheet source pour les appliquer
    /// à la sheet destination...
    /// Récupère toutes les zones merged dans la sheet source et regarde pour chacune d'elle si
    /// elle se trouve dans la current row que nous traitons.
    /// Si oui, retourne l'objet CellRangeAddress.
    /// </summary>
    /// <param name="sheet"> the sheet containing the data. </param>
    /// <param name="rowNum"> the num of the row to copy. </param>
    /// <param name="cellNum"> the num of the cell to copy. </param>
    /// <returns> the CellRangeAddress created. </returns>
    private static CellRangeAddress GetMergedRegion(ISheet sheet, int rowNum, short cellNum)
    {
      for (int i = 0; i < sheet.NumMergedRegions; i++)
      {
        CellRangeAddress merged = sheet.GetMergedRegion(i);
        if (merged.IsInRange(rowNum, cellNum))
        {
          return merged;
        }
      }
      return null;
    }
 
    /// <summary>
    /// Check that the merged region has been created in the destination sheet. </summary>
    /// <param name="newMergedRegion"> the merged region to copy or not in the destination sheet. </param>
    /// <param name="mergedRegions"> the list containing all the merged region. </param>
    /// <returns> true if the merged region is already in the list or not. </returns>
    private static bool IsNewMergedRegion(CellRangeAddressWrapper newMergedRegion,
                                          SortedSet<CellRangeAddressWrapper> mergedRegions)
    {
      return !mergedRegions.Contains(newMergedRegion);
    }
  }
   
  public class CellRangeAddressWrapper : IComparable<CellRangeAddressWrapper>
  {
      public CellRangeAddress range;
   
      /// <param name="theRange"> the CellRangeAddress object to wrap. </param>
      public CellRangeAddressWrapper(CellRangeAddress theRange)
      {
        this.range = theRange;
      }
   
      /// <param name="o"> the object to compare. </param>
      /// <returns> -1 the current instance is prior to the object in parameter, 0: equal, 1: after... </returns>
      public virtual int CompareTo(CellRangeAddressWrapper o)
      {
        if (range.FirstColumn < o.range.FirstColumn && range.FirstRow < o.range.FirstRow)
        {
          return -1;
        }
        else if (range.FirstColumn == o.range.FirstColumn && range.FirstRow == o.range.FirstRow)
        {
          return 0;
        }
        else
        {
          return 1;
        }
      }
  }
}
