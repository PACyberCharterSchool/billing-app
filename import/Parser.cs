using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using CsvHelper;

using models;

namespace import
{
	public class Parser
	{
		public void Parse(TextReader reader, StudentRecordsHeader header)
		 => Parse(DateTime.Now, reader, header);

		public void Parse(DateTime batchTime, TextReader reader, StudentRecordsHeader header)
		{
			Console.WriteLine($"Filename: {header.Filename}");
			Console.WriteLine($"Batch time: {batchTime}");

			var csvReader = new CsvReader(reader);
			csvReader.Configuration.RegisterClassMap<StudentStatusRecordClassMap>();

			Console.WriteLine("Reading records...");
			var count = 0;
			header.Records = new List<StudentRecord>();

			try
			{
				foreach (var record in csvReader.GetRecords<StudentRecord>())
				{
					header.Records.Append(record);

					count++;
					if (count % 1000 == 0)
						Console.WriteLine($"Read {count} records.");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e}");
			}

			Console.WriteLine($"Reading records done ({count})!");
			return;
		}
	}
}
