using System;
using System.Collections.Generic;

using models;

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

		private static Dictionary<int, SchoolDistrict> _districtCache = null;

		private static void UpdateSchoolDistrict(Student student, string district)
		{
			var parts = district.Split("|");
			var aun = int.Parse(parts[0]);

			if (student.SchoolDistrict == null)
			{
				if (!_districtCache.ContainsKey(aun))
					_districtCache.Add(aun, new SchoolDistrict { Aun = aun });

				student.SchoolDistrict = _districtCache[aun];
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

		private static Dictionary<StudentActivity, Action<Student, StudentActivityRecord>> _fieldUpdaters =
			new Dictionary<StudentActivity, Action<Student, StudentActivityRecord>>
			{
				{StudentActivity.NewStudent, (s, r) => s.PACyberId = r.PACyberId },
				{StudentActivity.DateOfBirthChange, (s, r) => s.DateOfBirth = DateTime.Parse(r.NextData)},
				{StudentActivity.DistrictEnrollment, (s, r) => {
					s.StartDate = r.Timestamp;
					UpdateSchoolDistrict(s, r.NextData);
				}},
				{StudentActivity.DistrictWithdrawal, (s, r) => {
					s.EndDate = r.Timestamp;
					UpdateSchoolDistrict(s, r.PreviousData);
				}},
				{StudentActivity.NameChange, (s, r) => UpdateStudentName(s, r.NextData)},
				{StudentActivity.GradeChange, (s, r) => s.Grade = r.NextData},
				{StudentActivity.AddressChange, (s, r) => UpdateStudentAddress(s, r.NextData)},
				{StudentActivity.SpecialEducationEnrollment, (s, r) => s.IsSpecialEducation = bool.Parse(r.NextData)},
				{StudentActivity.SpecialEducationWithdrawal, (s, r) => s.IsSpecialEducation = bool.Parse(r.NextData)},
				{StudentActivity.CurrentIepChange, (s, r) => s.CurrentIep = DateTime.Parse(r.NextData)},
				{StudentActivity.FormerIepChange, (s, r) => s.FormerIep = DateTime.Parse(r.NextData)},
				{StudentActivity.NorepChange, (s, r) => s.NorepDate = DateTime.Parse(r.NextData)},
				{StudentActivity.PASecuredChange, (s, r) => UpdateStudentPASecuredId(s, r.NextData)},
			};

		protected override IEnumerable<Student> Transform(IEnumerable<StudentActivityRecord> records)
		{
			// TODO(Erik): mutex to prevent clobbering?
			_districtCache = new Dictionary<int, SchoolDistrict>();
			var studentCache = new Dictionary<string, Student>();

			foreach (var record in records)
			{
				if (!studentCache.ContainsKey(record.PACyberId))
					studentCache.Add(record.PACyberId, new Student { PACyberId = record.PACyberId });

				var student = studentCache[record.PACyberId];

				var update = _fieldUpdaters[record.Activity];
				update(student, record);

				if (student.SchoolDistrict != null)
					_districts.CreateOrUpdate(student.SchoolDistrict);

				_students.CreateOrUpdate(student);
				yield return student;
			}
		}
	}
}
