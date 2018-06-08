using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace models.Reporters.Exporters
{
	public interface IExporter
	{
		XSSFWorkbook Export(XSSFWorkbook template, dynamic data);
	}

	public class XlsxExporter : IExporter
	{
		private static bool IsToken(string value) => (value.StartsWith("${") && value.EndsWith("}"));

		// TODO(Erik): generic overload?
		// TODO(Erik): dictionary overload?
		private static dynamic GetValue(JObject data, string path)
		{
			path = path.Trim().TrimStart("${".ToCharArray()).TrimEnd('}');

			var token = data.SelectToken(path);
			switch (token.Type)
			{
				case JTokenType.Boolean:
					return token.Value<bool>();

				case JTokenType.Date:
					return token.Value<DateTime>();

				case JTokenType.Float:
					return token.Value<float>();

				case JTokenType.Integer:
					return token.Value<int>();

				case JTokenType.String:
					return token.Value<string>();

				default:
					return null;
			}
		}

		public XSSFWorkbook Export(XSSFWorkbook wb, dynamic data)
		{
			for (var s = 0; s < wb.NumberOfSheets; s++)
			{
				var sheet = wb.GetSheetAt(s);

				for (var r = sheet.FirstRowNum; r < sheet.LastRowNum; r++)
				{
					var row = sheet.GetRow(r);

					if (row.Cells.All(c => c.CellType == CellType.Blank))
						continue;

					foreach (var cell in row.Cells.Where(c => c.CellType == CellType.String))
						if (IsToken(cell.StringCellValue))
							cell.SetCellValue(GetValue(data, cell.StringCellValue));
				}
			}

			wb.SetForceFormulaRecalculation(true);
			return wb;
		}
	}
}
