using System;
using System.Collections.Generic;
using System.Linq;

using FieldTransformers = System.Collections.Generic.List<
	System.ValueTuple<
		System.Func<models.Student, models.StudentStatusRecord, bool>,
		System.Func<models.Student, models.StudentStatusRecord, models.StudentActivityRecord>,
		System.Action<models.Student, models.StudentStatusRecord>
	>
>;

namespace models.Transformers
{
	public class StatusToActivityTransformer :
		Transformer<StudentStatusRecord, StudentActivityRecord>
	{
		private readonly IStudentRepository _students;
		private readonly IStudentActivityRecordRepository _activities;

		public StatusToActivityTransformer(
			IStudentRepository students,
			IStudentActivityRecordRepository activities)
		{
			_students = students;
			_activities = activities;
		}

		private const string DATE_FORMAT = "yyyy/MM/dd";

		private static string Join(params string[] parts)
		{
			if (parts.All(s => s == null))
				return null;

			for (var i = 0; i < parts.Length; i++)
				parts[i] = parts[i] == null ? "" : parts[i];

			return String.Join("|", parts);
		}

		private static readonly FieldTransformers _fieldTransformers = new FieldTransformers
		{
			( // date of birth
				(student, status) => status.StudentDateOfBirth.Date != student.DateOfBirth.Date,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.DateOfBirthChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.DateOfBirth == default(DateTime) ? null : student.DateOfBirth.ToString(DATE_FORMAT),
					NextData = status.StudentDateOfBirth.ToString(DATE_FORMAT),
					BatchHash = status.BatchHash,
				},
				(student, status) => student.DateOfBirth = status.StudentDateOfBirth
			),
			( // name
				(student, status) => status.StudentFirstName != student.FirstName ||
					status.StudentMiddleInitial != student.MiddleInitial ||
					status.StudentLastName != student.LastName,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.NameChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = Join(student.FirstName, student.MiddleInitial, student.LastName),
					NextData = Join(status.StudentFirstName, status.StudentMiddleInitial, status.StudentLastName),
					BatchHash = status.BatchHash,
				},
				(student, status) => {
					student.FirstName = status.StudentFirstName;
					student.MiddleInitial = status.StudentMiddleInitial;
					student.LastName = status.StudentLastName;
				}
			),
			( // grade
				(student, status) => status.StudentGradeLevel != student.Grade,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.GradeChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.Grade,
					NextData = status.StudentGradeLevel,
					BatchHash = status.BatchHash,
				},
				(student, status) => student.Grade = status.StudentGradeLevel
			),
			( // address
				(student, status) => status.StudentStreet1 != student.Street1 ||
					status.StudentStreet2 != student.Street2 ||
					status.StudentCity != student.City ||
					status.StudentState != student.State ||
					status.StudentZipCode != student.ZipCode,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.AddressChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student == null ? null : Join(
						student.Street1,
						student.Street2,
						student.City,
						student.State,
						student.ZipCode),
					NextData = Join(
						status.StudentStreet1,
						status.StudentStreet2,
						status.StudentCity,
						status.StudentState,
						status.StudentZipCode),
					BatchHash = status.BatchHash,
				},
				(student, status) => {
					student.Street1 = status.StudentStreet1;
					student.Street2 = status.StudentStreet2;
					student.City = status.StudentCity;
					student.State = status.StudentState;
					student.ZipCode = status.StudentZipCode;
				}
			),
			( // PA secured
				(student, status) => status.StudentPaSecuredId != student.PASecuredId,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.PASecuredChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.PASecuredId?.ToString(),
					NextData = status.StudentPaSecuredId.ToString(),
					BatchHash = status.BatchHash,
				},
				(student, status) => student.PASecuredId = status.StudentPaSecuredId
			),
			( // district enroll
				(student, status) => status.SchoolDistrictId != student.SchoolDistrict?.Aun,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.DistrictEnrollment,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.SchoolDistrict == null ? null : Join(
						student.SchoolDistrict?.Aun.ToString(),
						student.SchoolDistrict?.Name),
					NextData = Join(status.SchoolDistrictId.ToString(), status.SchoolDistrictName),
					BatchHash = status.BatchHash,
				},
				(student, status) => {
					student.SchoolDistrict = student.SchoolDistrict ?? new SchoolDistrict();
					student.SchoolDistrict.Aun = status.SchoolDistrictId;
					student.SchoolDistrict.Name = status.SchoolDistrictName;
					student.StartDate = status.StudentEnrollmentDate;
					student.EndDate = null;
				}
			),
			( // special education enroll
				(student, status) => status.StudentIsSpecialEducation == true &&
					student.IsSpecialEducation == false,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.SpecialEducationEnrollment,
					Timestamp = status.StudentEnrollmentDate,
					BatchHash = status.BatchHash,
				},
				(student, status) => student.IsSpecialEducation = true
			),
			( // current iep
				(student, status) => status.StudentCurrentIep != null &&
					status.StudentCurrentIep != student.CurrentIep,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.CurrentIepChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.CurrentIep?.ToString(DATE_FORMAT),
					NextData = status.StudentCurrentIep?.ToString(DATE_FORMAT),
					BatchHash = status.BatchHash,
				},
				(student, status) => student.CurrentIep = status.StudentCurrentIep
			),
			( // former iep
				(student, status) => status.StudentFormerIep != null &&
					status.StudentFormerIep != student.FormerIep,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.FormerIepChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.FormerIep?.ToString(DATE_FORMAT),
					NextData = status.StudentFormerIep?.ToString(DATE_FORMAT),
					BatchHash = status.BatchHash,
				},
				(student, status) => student.FormerIep = status.StudentFormerIep
			),
			( // norep
				(student, status) => status.StudentNorep != null &&
					status.StudentNorep != student.NorepDate,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.NorepChange,
					Timestamp = status.StudentEnrollmentDate,
					PreviousData = student.NorepDate?.ToString(DATE_FORMAT),
					NextData = status.StudentNorep?.ToString(DATE_FORMAT),
					BatchHash = status.BatchHash,
				},
				(student, status) => student.NorepDate = status.StudentNorep
			),
			( // special education withdraw
				(student, status) => status.StudentIsSpecialEducation == false &&
					status.StudentWithdrawalDate != null &&
					student.IsSpecialEducation == true,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.SpecialEducationWithdrawal,
					Timestamp = status.StudentWithdrawalDate.Value,
					BatchHash = status.BatchHash,
				},
				(student, status) => student.IsSpecialEducation = false
			),
			( // district withdraw
				(student, status) => status.StudentWithdrawalDate != null &&
					status.StudentWithdrawalDate != student.EndDate,
				(student, status) => new StudentActivityRecord
				{
					PACyberId = status.StudentId,
					Activity = StudentActivity.DistrictWithdrawal,
					Timestamp = status.StudentWithdrawalDate.Value,
					PreviousData = Join(status.SchoolDistrictId.ToString(), status.SchoolDistrictName),
					BatchHash = status.BatchHash,
				},
				(student, status) => {
					student.SchoolDistrict = null;
					student.EndDate = status.StudentWithdrawalDate;
				}
			),
		};

		protected override IEnumerable<StudentActivityRecord> Transform(IEnumerable<StudentStatusRecord> statuses)
		{
			var sequences = new Dictionary<string, int>();
			var studentCache = _students.GetMany().ToDictionary(s => s.PACyberId);

			foreach (var status in statuses)
			{
				if (!sequences.ContainsKey(status.StudentId))
					sequences.Add(status.StudentId, 0);

				if (!studentCache.ContainsKey(status.StudentId))
				{
					var s = new Student { PACyberId = status.StudentId };
					var activity = new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.NewStudent,
						Timestamp = status.StudentEnrollmentDate,
						BatchHash = status.BatchHash,
						Sequence = ++sequences[status.StudentId],
					};

					_activities.Create(activity);
					yield return activity;

					studentCache.Add(status.StudentId, s);
				}

				var student = studentCache[status.StudentId];
				foreach (var transformer in _fieldTransformers)
				{
					if (transformer.Item1(student, status)) // predicate
					{
						var activity = transformer.Item2(student, status); // create activity record
						activity.Sequence = ++sequences[status.StudentId];
						_activities.Create(activity);
						yield return activity;

						transformer.Item3(student, status); // update student
					}
				}
			}
		}
	}
}
