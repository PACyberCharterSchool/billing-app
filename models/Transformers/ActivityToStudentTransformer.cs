using System;
using System.Collections.Generic;

using models;

using FieldUpdaters = System.Collections.Generic.Dictionary<
	models.StudentActivity,
	System.Action<
		models.Student,
		models.StudentActivityRecord,
		System.Collections.Generic.Dictionary<
			int,
			models.SchoolDistrict
		>
	>
>;

namespace models.Transformers
{
	public class ActivityToStudentTransformer : Transformer<StudentActivityRecord, Student>
	{
		private readonly IStudentRepository _students;
		private readonly ISchoolDistrictRepository _districts;

		public ActivityToStudentTransformer(IStudentRepository students, ISchoolDistrictRepository districts)
		{
			_students = students;
			_districts = districts;
		}

		private static void UpdateSchoolDistrict(Student student, string district, Dictionary<int, SchoolDistrict> cache)
		{
			var parts = district.Split("|");
			var aun = int.Parse(parts[0]);

			if (student.SchoolDistrict == null)
			{
				if (!cache.ContainsKey(aun))
					cache.Add(aun, new SchoolDistrict { Aun = aun });

				student.SchoolDistrict = cache[aun];
			}

			student.SchoolDistrict.Name = parts[1];
		}

		private static void UpdateStudentName(Student student, string name)
		{
			var parts = name.Split("|");
			student.FirstName = parts[0];
			student.MiddleInitial = parts[1];
			student.LastName = parts[2];
		}

		private static void UpdateStudentAddress(Student student, string address)
		{
			var parts = address.Split("|");
			student.Street1 = parts[0];
			student.Street2 = parts[1];
			student.City = parts[2];
			student.State = parts[3];
			student.ZipCode = parts[4];
		}

		private static void UpdateStudentPASecuredId(Student student, string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				student.PASecuredId = null;
				return;
			}

			var n = ulong.Parse(id);
			student.PASecuredId = n;
		}

		private static readonly FieldUpdaters _fieldUpdaters = new FieldUpdaters
		{
			{StudentActivity.NewStudent, (s, r, _) => s.PACyberId = r.PACyberId },
			{StudentActivity.DateOfBirthChange, (s, r, _) => s.DateOfBirth = DateTime.Parse(r.NextData)},
			{StudentActivity.DistrictEnrollment, (s, r, dc) => {
				s.StartDate = r.Timestamp;
				s.EndDate = null;
				UpdateSchoolDistrict(s, r.NextData, dc);
			}},
			{StudentActivity.DistrictWithdrawal, (s, r, dc) => {
				s.EndDate = r.Timestamp;
				UpdateSchoolDistrict(s, r.PreviousData, dc);
			}},
			{StudentActivity.NameChange, (s, r, _) => UpdateStudentName(s, r.NextData)},
			{StudentActivity.GradeChange, (s, r, _) => s.Grade = r.NextData},
			{StudentActivity.AddressChange, (s, r, _) => UpdateStudentAddress(s, r.NextData)},
			{StudentActivity.SpecialEducationEnrollment, (s, r, _) => s.IsSpecialEducation = true},
			{StudentActivity.SpecialEducationWithdrawal, (s, r, _) => s.IsSpecialEducation = false},
			{StudentActivity.CurrentIepChange, (s, r, _) => s.CurrentIep = DateTime.Parse(r.NextData)},
			{StudentActivity.FormerIepChange, (s, r, _) => s.FormerIep = DateTime.Parse(r.NextData)},
			{StudentActivity.NorepChange, (s, r, _) => s.NorepDate = DateTime.Parse(r.NextData)},
			{StudentActivity.PASecuredChange, (s, r, _) => UpdateStudentPASecuredId(s, r.NextData)},
		};

		protected override IEnumerable<Student> Transform(IEnumerable<StudentActivityRecord> records)
		{
			var studentCache = new Dictionary<string, Student>();
			var districtCache = new Dictionary<int, SchoolDistrict>();

			// TODO(Erik): group by PACyberId to only return a single student once
			foreach (var record in records)
			{
				if (!studentCache.ContainsKey(record.PACyberId))
					studentCache.Add(record.PACyberId, new Student { PACyberId = record.PACyberId });

				var student = studentCache[record.PACyberId];

				var update = _fieldUpdaters[record.Activity];
				update(student, record, districtCache);

				if (student.SchoolDistrict != null)
					student.SchoolDistrict = _districts.CreateOrUpdate(student.SchoolDistrict);

				_students.CreateOrUpdate(student);
				yield return student;
			}
		}
	}
}
