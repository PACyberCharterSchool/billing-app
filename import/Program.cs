using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CsvHelper;
using dotenv.net;

using models;

namespace import
{
	class Program
	{
		private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

		private static void OnExit(object source, ConsoleCancelEventArgs args)
		{
			_closing.Set();
		}

		private static string _connectionString;

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
			_connectionString = $"Server={hostName},{port};Database={databaseName};User Id={userName};Password={password}";

			var path = Path.Combine(Environment.CurrentDirectory, importDir);
			var watcher = new FileSystemWatcher(path, importGlob);
			watcher.Created += HandleFileChange;
			watcher.EnableRaisingEvents = true;

			Console.WriteLine($"Watching directory {path} for '{importGlob}'...");
			Console.CancelKeyPress += OnExit;
			_closing.WaitOne();
		}

		private static AutoResetEvent _processing = new AutoResetEvent(true);
		private static PacBillContext _context;
		private static Parser _parser = new Parser();

		private static void HandleFileChange(object source, FileSystemEventArgs e)
		{
			try
			{
				_processing.WaitOne();

				_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
					UseSqlServer(_connectionString).Options);

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
						if (ex.InnerException != null)
							Console.WriteLine($"  Inner exception: {ex.InnerException.Message}.");
						return;
					}

					IList<PendingStudentStatusRecord> records = null;
					try
					{
						using (var streamReader = File.OpenText(e.FullPath))
						{
							var lastWrite = File.GetLastWriteTime(e.FullPath);
							records = _parser.Parse(lastWrite, streamReader, e.Name);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to read CSV: {ex.Message}.");
						if (ex.InnerException != null)
							Console.WriteLine($"  Inner exception: {ex.InnerException.Message}.");

						return;
					}

					try
					{
						Console.WriteLine("Writing changes to the database...");
						_context.AddRange(records.Reverse()); // HACK(Erik): match order in CSV
						_context.SaveChanges();
						Console.WriteLine("Writing changes to the database done!");

						tx.Commit();
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to write data to database: {ex.Message}.");
						if (ex.InnerException != null)
							Console.WriteLine($"  Inner exception: {ex.InnerException.Message}.");

						return;
					}
				}
			}
			finally
			{
				_processing.Set();
			}
		}
	}
}
