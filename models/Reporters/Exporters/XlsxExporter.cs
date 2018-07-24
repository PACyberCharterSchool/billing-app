using System;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace models.Reporters.Exporters
{
	public interface IXlsxExporter
	{
		XSSFWorkbook Export(XSSFWorkbook wb, dynamic data);
	}

	public class XlsxExporter : IXlsxExporter
	{
		private static bool IsToken(string value) => (value.StartsWith("${") && value.EndsWith("}"));

		private static bool ContainsToken(string value) => (value.Contains("${") && value.Contains("}"));

		// TODO(Erik): generic overload?
		// TODO(Erik): dictionary overload?
		private static dynamic GetValue(JObject data, string value)
		{
			if (IsToken(value) && value.Count(c => c == '$') == 1)
			{
				var token = data.SelectToken(value.Trim().TrimStart("${".ToCharArray()).TrimEnd('}'));
				if (token == null)
					return null;

				switch (token.Type)
				{
					case JTokenType.Boolean:
						return token.Value<bool>() ? "Yes" : "No";

					case JTokenType.Date:
						return token.Value<DateTime>();

					case JTokenType.Float:
						return Math.Round(token.Value<float>(), 2);

					case JTokenType.Integer:
						return token.Value<int>();

					case JTokenType.String:
						return token.Value<string>();

					default:
						return null;
				}
			}

			var pos = 0;
			var sb = new StringBuilder();

			var inPath = false;
			var path = string.Empty;

			while (true)
			{
				if (pos >= value.Length)
					break;

				var s = value[pos];
				if (!inPath)
				{
					if (s != '$')
					{
						sb.Append(s);
						pos++;
						continue;
					}

					if (s == '$' && value[pos + 1] == '{')
					{
						inPath = true;
						pos += 2;
						continue;
					}
				}

				if (s == '}')
				{
					pos++;
					var token = data.SelectToken(path);
					if (token != null)
						sb.Append(token.ToString());

					path = string.Empty;
					inPath = false;
					continue;
				}

				path += s;
				pos++;
			}

			return sb.ToString();
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
					{
						if (ContainsToken(cell.StringCellValue))
						{
							var value = GetValue(data, cell.StringCellValue);
							if (value != null)
								cell.SetCellValue(value);
							else
								cell.SetCellValue((string)null);
						}
					}
				}
			}

			wb.SetForceFormulaRecalculation(true);
			return wb;
		}
	}
}
