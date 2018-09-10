using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace models.Reporters
{
	public static class StudentRecordsExtensions
	{
		public static IEnumerable<StudentRecord> Enrolled(
			this IQueryable<StudentRecord> records,
			DateTime start,
			DateTime end)
		{
			return records.Where(r => r.StudentEnrollmentDate <= end && (
				r.StudentWithdrawalDate == null || (
					r.StudentWithdrawalDate >= start && (
						r.StudentWithdrawalDate != r.StudentEnrollmentDate || (
							r.StudentWithdrawalDate == r.StudentEnrollmentDate &&
							r.StudentCurrentIep.Value.Month == r.StudentEnrollmentDate.Month &&
							r.StudentCurrentIep.Value.Day == r.StudentEnrollmentDate.Day
						)
					)
				)
			));
		}

		public static IEnumerable<StudentRecord> Enrolled(
			this DbSet<StudentRecord> records,
			DateTime start,
			DateTime end) => records.AsQueryable().Enrolled(start, end);

		public static IEnumerable<StudentRecord> Enrolled(
			this IEnumerable<StudentRecord> records,
			DateTime start,
			DateTime end) => records.AsQueryable().Enrolled(start, end);
	}
}
