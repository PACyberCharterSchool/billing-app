using System;
using System.Linq;
using System.IO;

using Newtonsoft.Json;
using Aspose.Cells;
using NUnit.Framework;

using models.Reporters.Exporters;

namespace models.Tests.Reporters.Exporters
{
	[TestFixture]
	public class XlsxExporterTests
	{
		private XlsxExporter _uut;

		[SetUp]
		public void SetUp()
		{
			_uut = new XlsxExporter();
		}

		[Test]
		public void ExportExportsJSON()
		{
			var template = new Workbook(File.OpenRead("../../../TestData/test-template.xlsx"));
			var data = new
			{
				String = "bob", // JTokenType.String
				Object = new
				{
					Float = 1m, // JTokenType.Float
					Deeper = new
					{
						Boolean = true, // JTokenType.Boolean
					},
				},
				Array = new dynamic[] {
					DateTime.Now.Date, // JTokenType.Date
					new
					{
						Integer = 3, // JTokenType.Integer
					},
				},
				Me = "me",
				Multiple = new
				{
					Tokens = new dynamic[] {
						"One",
						"Two",
					},
				},
			};

			var actual = _uut.Export(template, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data)));
			var sheet = actual.Worksheets[0];
			Assert.That(sheet.Cells[0, 0].StringValue,
				Is.EqualTo(data.String));
			Assert.That(sheet.Cells[1, 0].FloatValue,
				Is.EqualTo(data.Object.Float));
			Assert.That(sheet.Cells[2, 0].DateTimeValue,
				Is.EqualTo(data.Array[0]));
			Assert.That(sheet.Cells[3, 0].IntValue,
				Is.EqualTo(data.Array[1].Integer));
			Assert.That(sheet.Cells[5, 0].StringValue,
				Is.EqualTo(data.Object.Deeper.Boolean ? "Yes" : "No"));
			Assert.That(sheet.Cells[6, 0].StringValue,
				Is.EqualTo("Please replace me!"));
			Assert.That(sheet.Cells[7, 0].Type, Is.EqualTo(CellValueType.IsNull));
			Assert.That(sheet.Cells[8, 0].StringValue,
				Is.EqualTo("One Two"));

			Assert.That(sheet.Cells[4, 0].StringValue,
				Is.EqualTo($"{data.String}, {data.Object.Float}"));
		}
	}
}
