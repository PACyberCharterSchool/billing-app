using System;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using Aspose.Cells;

namespace models.Reporters.Exporters
{
	public interface IXlsxExporter
	{
		Workbook Export(Workbook wb, dynamic data);
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
						return token.Value<ulong>();

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

		public Workbook Export(Workbook wb, dynamic data)
		{
			for (var s = 0; s < wb.Worksheets.Count; s++)
			{
				var sheet = wb.Worksheets[s];
				Cells cells = sheet.Cells;

				for (int r = 0; r < cells.MaxDataRow; r++) {
					Row row = cells.Rows[r];
					if (row.IsBlank)
						continue;

					for (int c = 0; c < cells.MaxDataColumn; c++) {
						if (cells[r,c].Type == CellValueType.IsString) {
							if (ContainsToken(cells[r,c].StringValue))
							{
								var value = GetValue(data, cells[r,c].StringValue);
								if (value != null)
								{
									cells[r,c].PutValue(value);
								}
								else
								{
									cells[r,c].PutValue(null);
								}
							}
						}
					}
				}
			}
			return wb;
		}
	}
}