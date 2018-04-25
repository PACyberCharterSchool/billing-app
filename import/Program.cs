using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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

		private static PacBillContext _context;
		private static Parser _parser;

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
			_parser = new Parser();

			var path = Path.Combine(Environment.CurrentDirectory, importDir);
			var watcher = new FileSystemWatcher(path, importGlob);
			watcher.Created += HandleFileChange;
			watcher.EnableRaisingEvents = true;

			Console.WriteLine($"Watching directory {path} for '{importGlob}'...");
			Console.CancelKeyPress += OnExit;
			_closing.WaitOne();
		}

		private static AutoResetEvent _processing = new AutoResetEvent(true);

		private static void HandleFileChange(object source, FileSystemEventArgs e)
		{
			try
			{
				_processing.WaitOne();

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
							records = _parser.Parse(streamReader, e.Name);
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
						_context.AddRange(records);
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
