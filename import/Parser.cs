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
		private static string HashBatch(DateTime time, string filename)
		{
			var bytes = Encoding.UTF8.GetBytes($"{time}-{filename}");
			var hash = SHA256.Create().ComputeHash(bytes);

			var sb = new StringBuilder();
			foreach (var b in hash)
				sb.Append(b.ToString("x2"));

			return sb.ToString().Substring(0, 10);
		}

		public IList<PendingStudentStatusRecord> Parse(TextReader reader, string batchFilename)
		 => Parse(DateTime.Now, reader, batchFilename);

		public IList<PendingStudentStatusRecord> Parse(DateTime batchTime, TextReader reader, string batchFilename)
		{
			var batchHash = HashBatch(batchTime, batchFilename);
			Console.WriteLine($"Filename: {batchFilename}");
			Console.WriteLine($"Batch time: {batchTime}");
			Console.WriteLine($"Hash: {batchHash}");

			var csvReader = new CsvReader(reader);
			csvReader.Configuration.RegisterClassMap<StudentStatusRecordClassMap>();

			Console.WriteLine("Reading records...");
			var count = 0;
			var records = new List<PendingStudentStatusRecord>();
			try {
				foreach (var record in csvReader.GetRecords<PendingStudentStatusRecord>())
				{
					record.BatchTime = batchTime;
					record.BatchFilename = batchFilename;
					record.BatchHash = batchHash;

					records.Add(record);

					count++;
					if (count % 1000 == 0)
						Console.WriteLine($"Read {count} records.");
				}
			}
			catch (Exception e) {
				Console.WriteLine($"e");
			}

			Console.WriteLine($"Reading records done ({count})!");
			return records;
		}
	}
}
