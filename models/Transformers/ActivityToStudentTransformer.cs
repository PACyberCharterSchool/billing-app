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
		>,
		models.ISchoolDistrictRepository
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

		private static void UpdateSchoolDistrict(
			Student student,
			string district,
			Dictionary<int, SchoolDistrict> cache,
			ISchoolDistrictRepository districts)
		{
			Console.WriteLine($"Updating SchoolDistrict");
			var parts = district.Split("|");
			var aun = int.Parse(parts[0]);

			if (student.SchoolDistrict == null || student.SchoolDistrict.Aun != aun)
			{
				if (!cache.ContainsKey(aun))
				{
					var d = districts.GetByAun(aun);
					if (d == null)
						d = new SchoolDistrict { Aun = aun };

					cache.Add(aun, d);
				}

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
			{StudentActivity.NewStudent, (s, r, _, __) => s.PACyberId = r.PACyberId },
			{StudentActivity.DateOfBirthChange, (s, r, _, __) => s.DateOfBirth = DateTime.Parse(r.NextData)},
			{StudentActivity.DistrictEnrollment, (s, r, dc, dr) => {
				s.StartDate = r.Timestamp;
				s.EndDate = null;
				UpdateSchoolDistrict(s, r.NextData, dc, dr);
			}},
			{StudentActivity.DistrictWithdrawal, (s, r, dc, dr) => {
				s.EndDate = r.Timestamp;
				UpdateSchoolDistrict(s, r.PreviousData, dc, dr);
			}},
			{StudentActivity.NameChange, (s, r, _, __) => UpdateStudentName(s, r.NextData)},
			{StudentActivity.GradeChange, (s, r, _, __) => s.Grade = r.NextData},
			{StudentActivity.AddressChange, (s, r, _, __) => UpdateStudentAddress(s, r.NextData)},
			{StudentActivity.SpecialEducationEnrollment, (s, r, _, __) => s.IsSpecialEducation = true},
			{StudentActivity.SpecialEducationWithdrawal, (s, r, _, __) => s.IsSpecialEducation = false},
			{StudentActivity.CurrentIepChange, (s, r, _, __) => s.CurrentIep = DateTime.Parse(r.NextData)},
			{StudentActivity.FormerIepChange, (s, r, _, __) => s.FormerIep = DateTime.Parse(r.NextData)},
			{StudentActivity.NorepChange, (s, r, _, __) => s.NorepDate = DateTime.Parse(r.NextData)},
			{StudentActivity.PASecuredChange, (s, r, _, __) => UpdateStudentPASecuredId(s, r.NextData)},
		};

		protected override IEnumerable<Student> Transform(IEnumerable<StudentActivityRecord> records)
		{
			var studentCache = new Dictionary<string, Student>();
			var districtCache = new Dictionary<int, SchoolDistrict>();

			foreach (var record in records)
			{
				if (!studentCache.ContainsKey(record.PACyberId))
				{
					var s = _students.GetByPACyberId(record.PACyberId);
					if (s == null)
					{
						s = new Student();
					}

					studentCache.Add(record.PACyberId, s);
				}

				var student = studentCache[record.PACyberId];

				var update = _fieldUpdaters[record.Activity];
				update(student, record, districtCache, _districts);

				if (student.SchoolDistrict != null)
					student.SchoolDistrict = _districts.CreateOrUpdate(student.SchoolDistrict);

				_students.CreateOrUpdate(student);
				yield return student;
			}
		}
	}
}
