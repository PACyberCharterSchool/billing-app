using System;
using System.Collections.Generic;

using FieldTransformers = System.Collections.Generic.List<
	System.ValueTuple<
		System.Func<models.Student, models.StudentStatusRecord, bool>,
		System.Func<models.Student, models.StudentStatusRecord, models.StudentActivityRecord>
	>
>;

namespace models.Transformers
{
	public class StudentStatusRecordTransformer :
		Transformer<StudentStatusRecord, StudentActivityRecord>
	{
		private readonly IStudentRepository _students;
		private readonly IStudentActivityRecordRepository _activities;

		public StudentStatusRecordTransformer(
			IStudentRepository students,
			IStudentActivityRecordRepository activities)
		{
			_students = students;
			_activities = activities;
		}

		private const string DATE_FORMAT = "yyyy/MM/dd";

		private static string Join(params string[] parts)
		{
			for (var i = 0; i < parts.Length; i++)
				parts[i] = parts[i] == null ? "" : parts[i];

			return String.Join("|", parts);
		}

		private static readonly FieldTransformers _fieldTransformers = new FieldTransformers
		{
			( // new student
				(student, status) => student == null,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.NEW_STUDENT,
						Timestamp = status.StudentEnrollmentDate,
						BatchHash = status.BatchHash,
					}
			),
			( // date of birth
				(student, status) => student == null || status.StudentDateOfBirth != student.DateOfBirth,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.DATEOFBIRTH_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.DateOfBirth.ToString(DATE_FORMAT),
						NextData = status.StudentDateOfBirth.ToString(DATE_FORMAT),
						BatchHash = status.BatchHash,
					}
			),
			( // district withdraw
				(student, status) => student != null && status.StudentWithdrawalDate.HasValue,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.DISTRICT_WITHDRAW,
						Timestamp = status.StudentWithdrawalDate.Value,
						PreviousData = status.SchoolDistrictId.ToString(),
						BatchHash = status.BatchHash,
					}
			),
			( // district enroll
				(student, status) => student == null || status.SchoolDistrictId != student.SchoolDistrict?.Aun,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.DISTRICT_ENROLL,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = Join(student?.SchoolDistrict?.Aun.ToString(), student?.SchoolDistrict?.Name),
						NextData = Join(status.SchoolDistrictId.ToString(), status.SchoolDistrictName),
						BatchHash = status.BatchHash,
					}
			),
			( // special education withdraw
				(student, status) => student != null &&
					!status.StudentIsSpecialEducation &&
					status.StudentWithdrawalDate.HasValue &&
					status.StudentIsSpecialEducation != student.IsSpecialEducation,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.SPECIAL_WITHDRAW,
						Timestamp = status.StudentWithdrawalDate.Value,
						PreviousData = student?.IsSpecialEducation.ToString(),
						NextData = status.StudentIsSpecialEducation.ToString(),
						BatchHash = status.BatchHash,
					}
			),
			( // special education enroll
				(student, status) => student == null && status.StudentIsSpecialEducation ||
					status.StudentIsSpecialEducation && status.StudentIsSpecialEducation != student.IsSpecialEducation,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.SPECIAL_ENROLL,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.IsSpecialEducation.ToString(),
						NextData = status.StudentIsSpecialEducation.ToString(),
						BatchHash = status.BatchHash,
					}
			),
			( // current iep
				(student, status) => student == null && status.StudentCurrentIep != null ||
					status.StudentCurrentIep != null && status.StudentCurrentIep != student.CurrentIep,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.CURRENTIEP_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.CurrentIep?.ToString(DATE_FORMAT),
						NextData = status.StudentCurrentIep?.ToString(DATE_FORMAT),
						BatchHash = status.BatchHash,
					}
			),
			( // former iep
				(student, status) => student == null && status.StudentFormerIep != null ||
					status.StudentFormerIep != null && status.StudentFormerIep != student.FormerIep,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.FORMERIEP_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.FormerIep?.ToString(DATE_FORMAT),
						NextData = status.StudentFormerIep?.ToString(DATE_FORMAT),
						BatchHash = status.BatchHash,
					}
			),
			( // norep
				(student, status) => student == null && status.StudentNorep != null || status.StudentNorep != null &&
					status.StudentNorep != student.NorepDate,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.NOREP_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.NorepDate?.ToString(DATE_FORMAT),
						NextData = status.StudentNorep?.ToString(DATE_FORMAT),
						BatchHash = status.BatchHash,
					}
			),
			( // PA secured
				(student, status) => student == null || status.StudentPaSecuredId != student.PASecuredId,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.PASECURED_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.PASecuredId?.ToString(),
						NextData = status.StudentPaSecuredId.ToString(),
						BatchHash = status.BatchHash,
					}
			),
			( // name
				(student, status) => student == null ||
					status.StudentFirstName != student.FirstName ||
					status.StudentMiddleInitial != student.MiddleInitial ||
					status.StudentLastName != student.LastName,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.NAME_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student == null ? null : Join(student.FirstName, student.MiddleInitial, student.LastName),
						NextData = Join(status.StudentFirstName, status.StudentMiddleInitial, status.StudentLastName),
						BatchHash = status.BatchHash,
					}
			),
			( // grade
				(student, status) => student == null || status.StudentGradeLevel != student.Grade,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.GRADE_CHANGE,
						Timestamp = status.StudentEnrollmentDate,
						PreviousData = student?.Grade,
						NextData = status.StudentGradeLevel,
						BatchHash = status.BatchHash,
					}
			),
			( // address
				(student, status) => student == null ||
					status.StudentStreet1 != student.Street1 ||
					status.StudentStreet2 != student.Street2 ||
					status.StudentCity != student.City ||
					status.StudentState != student.State ||
					status.StudentZipCode != student.ZipCode,
				(student, status) => new StudentActivityRecord
					{
						PACyberId = status.StudentId,
						Activity = StudentActivity.ADDRESS_CHANGE,
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
					}
			),
		};

		protected override IEnumerable<StudentActivityRecord> Transform(IEnumerable<StudentStatusRecord> statuses)
		{
			foreach (var status in statuses)
			{
				// TODO(Erik): sequence #?
				var student = _students.GetByPACyberId(status.StudentId);

				foreach (var transformer in _fieldTransformers)
				{
					if (transformer.Item1(student, status))
					{
						var activity = transformer.Item2(student, status);
						_activities.Create(activity);
						yield return activity;
					}
				}
			}
		}
	}
}
