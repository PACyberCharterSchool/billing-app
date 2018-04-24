using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using dotenv.net;

using models;

namespace import
{
	class Program
	{
		private static PacBillContext _context;

		static void Main(string[] args)
		{
			DotEnv.Config(false);

			var importDir = Environment.GetEnvironmentVariable("IMPORT_DIR");
			var importGlob = Environment.GetEnvironmentVariable("IMPORT_GLOB");

			var hostName = Environment.GetEnvironmentVariable("DATABASE_HOST");
			var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
			var userName = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
			var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
			var port = Environment.GetEnvironmentVariable("DATABASE_PORT");
			var connectionString = $"Server={hostName},{port};Database={databaseName};User Id={userName};Password={password}";

			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseSqlServer(connectionString).Options);

			var path = Path.Combine(Environment.CurrentDirectory, importDir);
			var watcher = new FileSystemWatcher(path, importGlob);
			watcher.Created += HandleFileChange;
			watcher.EnableRaisingEvents = true;

			Console.WriteLine("Watching directory...");
			while (true) { }
		}

		private static string HashBatch(DateTime time, string filename)
		{
			var milli = time.Millisecond.ToString();
			var bytes = Encoding.UTF8.GetBytes($"{milli}-{filename}");
			var hash = SHA256.Create().ComputeHash(bytes);

			var sb = new StringBuilder();
			foreach (var b in hash)
				sb.Append(b.ToString("x2"));

			return sb.ToString().Substring(0, 10);
		}

		private static void HandleFileChange(object source, FileSystemEventArgs e)
		{
			using (var tx = _context.Database.BeginTransaction())
			{
				try
				{
					var table = nameof(_context.PendingStudentStatusRecords);
					Console.WriteLine($"Truncating {table}...");
					_context.Database.ExecuteSqlCommand($"TRUNCATE TABLE " + table + ";");
					_context.SaveChanges();
					Console.WriteLine("Truncating done!");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to truncate table: {ex.Message}");
					return;
				}

				try
				{
					var count = 0;
					Console.WriteLine("Reading records...");
					using (var streamReader = File.OpenText(e.FullPath))
					{
						var csvReader = new CsvReader(streamReader);
						csvReader.Configuration.RegisterClassMap<StudentStatusRecordClassMap>();

						var batchFilename = e.Name;
						var batchTime = DateTime.Now;
						var batchHash = HashBatch(batchTime, batchFilename);
						Console.WriteLine($"Filename: {batchFilename}");
						Console.WriteLine($"Batch time: {batchTime}");
						Console.WriteLine($"Hash: {batchHash}");

						var records = csvReader.GetRecords<PendingStudentStatusRecord>();
						foreach (var record in records)
						{
							record.BatchTime = batchTime;
							record.BatchFilename = batchFilename;
							record.BatchHash = batchHash;

							_context.Add(record);
							count++;

							if (count % 1000 == 0)
								Console.WriteLine($"Read {count} records.");
						}

						Console.WriteLine($"Reading records done ({count})!");
					}

					Console.WriteLine("Writing changes to the database...");
					_context.SaveChanges();
					Console.WriteLine("Writing changes to the database done!");

					tx.Commit();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failure reading CSV: {ex.Message}.");
					return;
				}
			}
		}
	}
}
