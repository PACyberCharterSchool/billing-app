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
using NPOI.XSSF.UserModel.Extensions;
using NPOI.XSSF.Model;
using NPOI.HSSF.UserModel;

namespace api.Common.Utils
{
  public class NPOIHelper
  {
    public static void CloneWorkbookFormatInfo(IWorkbook destWorkbook, IWorkbook srcWorkbook)
    {
      // you have to clone many of the items associated with formatting at the workbook level in
      // order to have resource for the cell styles, etc.
      StylesTable srcStyles = ((XSSFWorkbook)srcWorkbook).GetStylesSource();
      StylesTable destStyles = ((XSSFWorkbook)destWorkbook).GetStylesSource();
      
      foreach (var font in srcStyles.GetFonts()) {
        destStyles.PutFont(font, true);
      }

      foreach (var fill in srcStyles.GetFills()) {
        /* destStyles.PutFill(new XSSFCellFill(fill.GetCTFill())); */
      }

      foreach (var border in srcStyles.GetBorders()) {
        destStyles.PutBorder(new XSSFCellBorder(border.GetCTBorder()));
      }
    }

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
      AppendSheet(destSheet, srcSheet, copyStyle);
    }

    /// <param name="destSheet"> the sheet being copied/merged. </param>
    /// <param name="newSheet"> the destination sheet being copied/merged into. </param>
    /// <param name="copyStyle"> true copy the style. </param>
    private static void AppendSheet(XSSFSheet destSheet, XSSFSheet srcSheet, bool copyStyle)
    {
      int maxColumnNum = 0;
      IDictionary<int?, ICellStyle> styleMap = copyStyle ? new Dictionary<int?, ICellStyle>() : null;

      int numRowsSrcSheet = GetLastRowWithData(srcSheet); 
      Console.WriteLine($"NPOIHelper.AppendSheet():  destination sheet {destSheet.SheetName} has {numRowsSrcSheet} rows.");
      for (int i = srcSheet.FirstRowNum, j = destSheet.LastRowNum; i <= numRowsSrcSheet; i++, j++) {
        IRow srcRow = srcSheet.GetRow(i);
        IRow destRow = destSheet.CreateRow(j);

        Console.WriteLine($"NPOIHelper.AppendSheet():  copying row {i} of sheet {srcSheet.SheetName}.");
        if (srcRow != null) {
          CopyRow(srcSheet, destSheet, srcRow, destRow, styleMap);
          if (srcRow.LastCellNum > maxColumnNum) {
            maxColumnNum = srcRow.LastCellNum;
          }
        }
      }

      for (int i = 0; i <= maxColumnNum; i++)
      {
        destSheet.SetColumnWidth(i, srcSheet.GetColumnWidth(i));
      }
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
      int maxColumnNum = 0;
      IDictionary<int?, ICellStyle> styleMap = (copyStyle) ? new Dictionary<int?, ICellStyle>() : null;
      for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
      {
        IRow srcRow = sheet.GetRow(i);
        IRow destRow = newSheet.CreateRow(i);
        if (srcRow != null)
        {
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
      int j = srcRow.FirstCellNum < 0 ? 0 : srcRow.FirstCellNum;
      
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
          ICellStyle newCellStyle;
          if (!styleMap.ContainsKey(stHashCode))
          {
            newCellStyle = newCell.Sheet.Workbook.CreateCellStyle();
            newCellStyle.Alignment = oldCell.CellStyle.Alignment;
            newCellStyle.VerticalAlignment = oldCell.CellStyle.VerticalAlignment;
            newCellStyle.WrapText = oldCell.CellStyle.WrapText;
            newCellStyle.DataFormat = oldCell.CellStyle.DataFormat;
            newCellStyle.CloneStyleFrom(oldCell.CellStyle);
            newCellStyle.SetFont(oldCell.CellStyle.GetFont(oldCell.Sheet.Workbook));
            newCellStyle.FillForegroundColor = oldCell.CellStyle.FillForegroundColor;
            newCellStyle.FillBackgroundColor = oldCell.CellStyle.FillBackgroundColor;
            styleMap.Add(stHashCode, newCellStyle);

            Console.WriteLine($"Adding cell style to styleMap.  Hash is {stHashCode}, CellStyle is {newCellStyle}.");
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
          newCell.SetCellValue(oldCell.ToString());
          break;
        case CellType.Numeric:
          newCell.SetCellValue(oldCell.NumericCellValue);
          break;
        case CellType.Blank:
          newCell.SetCellType(CellType.Blank);
          break;
        case CellType.Boolean:
          newCell.SetCellValue(oldCell.BooleanCellValue);
          break;
        case CellType.Error:
          newCell.SetCellErrorValue(oldCell.ErrorCellValue);
          break;
        case CellType.Formula:
          newCell.SetCellFormula(oldCell.CellFormula);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Get the index value of the last row with actual data in it from the specified sheet.
    /// </summary>
    /// <param name="sheet"> the sheet containing the data. </param>
    /// <returns> index of the last row with actual data in it.</returns>
    private static int GetLastRowWithData(ISheet sheet)
    {
      IFormulaEvaluator evaluator = sheet.Workbook.GetCreationHelper().CreateFormulaEvaluator();
      DataFormatter formatter = new DataFormatter( true );

      int lastRowIndex = -1;
      if( sheet.PhysicalNumberOfRows > 0 )
      {
          // getLastRowNum() actually returns an index, not a row number
          lastRowIndex = sheet.LastRowNum;

          // now, start at end of spreadsheet and work our way backwards until we find a row having data
          for( ; lastRowIndex >= 0; lastRowIndex-- )
          {
              IRow row = sheet.GetRow( lastRowIndex );
              if( !IsRowEmpty( row, evaluator, formatter ) )
              {
                  break;
              }
          }
      }
      return lastRowIndex;
    }

    /// <summary>
    /// Get the index value of the last row with actual data in it from the specified sheet.
    /// </summary>
    /// <param name="sheet"> the sheet containing the data. </param>
    /// <returns> bool indicating whether it contains any actual data in it.</returns>
    private static bool IsRowEmpty(IRow row, IFormulaEvaluator evaluator, DataFormatter formatter)
    {
      if( row == null ) {
        return true;
      }

      int cellCount = row.LastCellNum + 1;
      for( int i = 0; i < cellCount; i++ ){
        string cellValue = GetCellValue(row, i, evaluator, formatter);
        if (cellValue != null && cellValue.Length > 0) {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Get the formatted value of the specified cell.
    /// </summary>
    /// <param name="row"> the row containing the relevant cell.</param>
    /// <param name="columnIndex"> the index value for the cell in the row.</param>
    private static string GetCellValue(IRow row, int columnIndex, IFormulaEvaluator evaluator, DataFormatter formatter)
    {
      string cellValue;
      ICell cell = row.GetCell( columnIndex );

      if( cell == null ){
        // no data in this cell
        cellValue = null;
      }
      else{
        if( cell.CellType != CellType.Formula){
            // cell has a value, so format it into a string
            cellValue = formatter.FormatCellValue( cell );
        }
        else {
            // cell has a formula, so evaluate it
            cellValue = formatter.FormatCellValue( cell, evaluator );
        }
      }
      return cellValue;
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
