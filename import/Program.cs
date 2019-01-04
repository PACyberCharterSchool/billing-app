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
using System.Text.RegularExpressions;

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
			watcher.Changed += HandleFileChange;
			watcher.EnableRaisingEvents = true;

			Console.WriteLine($"Watching directory {path} for '{importGlob}'...");
			Console.CancelKeyPress += OnExit;
			_closing.WaitOne();
		}

		private static (int First, int Second) GetYears(DateTime now)
		{
			if (now.Month >= 7)
				return (now.Year, now.Year + 1);
			else
				return (now.Year - 1, now.Year);
		}

		private static string GetReconScope()
		{
			var now = DateTime.Now;
			var (first, second) = GetYears(now);

			return $"{first.ToString("0000")}-{second.ToString("0000")}";
		}

		private static string GetMonthlyScope(PacBillContext _context)
		{
			var now = DateTime.Now;
			var (first, second) = GetYears(now);

			var last = _context.StudentRecordsHeaders.
				OrderBy(h => h.Scope).
				Where(h => Regex.IsMatch(h.Scope, @"^\d{4}\.\d{2}$")).
				Where(h =>
					String.Compare(h.Scope, $"{first.ToString("0000")}.08") == 1 &&
					String.Compare(h.Scope, $"{second.ToString("0000")}.08") == -1).
				LastOrDefault();
			if (last == null)
				return $"{now.Year.ToString("0000")}.09";

			Console.WriteLine($"last.Scope: {last.Scope}");

			var month = int.Parse(last.Scope.Substring(last.Scope.Length - 2));
			var year = int.Parse(last.Scope.Substring(0, 4));

			if (!last.Locked)
				return FormatScope(year, month);

			if (month >= 12)
			{
				month = 0;
				year = year + 1;
			}

			return FormatScope(year, month + 1);

			string FormatScope(int y, int m)
			{
				return $"{y.ToString("0000")}.{m.ToString("00")}";
			}
		}

		private static bool IsRecon(string filename) => filename.ToLower().Contains("recon");

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
					string scope;
					if (IsRecon(e.Name))
						scope = GetReconScope();
					else
						scope = GetMonthlyScope(_context);

					Console.WriteLine($"Scope: {scope}");

					var header = _context.StudentRecordsHeaders.Include(r => r.Records).SingleOrDefault(h => h.Scope == scope);
					if (header != null)
					{
						if (!header.Locked)
						{
							Console.WriteLine($"Data for {scope} exists and is not locked. Overwriting.");
							_context.Remove(header);
						}
						else
						{
							Console.WriteLine($"Data for {scope} exists and is locked. Aborting import.");
							return;
						}
					}
					else
					{
						header = new StudentRecordsHeader
						{
							Scope = scope,
							Filename = e.Name,
							Created = DateTime.Now,
							Locked = false,
						};
					}

					try
					{
						using (var streamReader = File.OpenText(e.FullPath))
						{
							var lastWrite = File.GetLastWriteTime(e.FullPath);
							_parser.Parse(lastWrite, streamReader, header);
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
						if (header.Id == 0)
							_context.Add(header);
						else
							_context.Update(header);

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
