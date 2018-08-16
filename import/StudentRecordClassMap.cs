using System;

using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

using models;

namespace import
{
	public class BoolConverter : DefaultTypeConverter
	{
		public override object ConvertFromString(string text, CsvHelper.IReaderRow row, MemberMapData memberMapData)
		{
			var s = text.ToLower().Trim();
			if (s == "n")
				return false;

			if (s == "y")
				return true;

			throw new ArgumentException($"Expected Y|N, got {text}.");
		}
	}

	public class StudentRecordClassMap : ClassMap<StudentRecord>
	{
		public StudentRecordClassMap()
		{
			Map(m => m.SchoolDistrictId).Name("schooldistrict");
			Map(m => m.SchoolDistrictName).Name("school_name");
			Map(m => m.StudentId).Name("StudentIndex"); // case in file
			Map(m => m.StudentFirstName).Name("first_name");
			Map(m => m.StudentMiddleInitial).Name("middle_initial");
			Map(m => m.StudentLastName).Name("last_name");
			Map(m => m.StudentGradeLevel).Name("grade_level");
			Map(m => m.StudentDateOfBirth).Name("date_of_birth");
			Map(m => m.StudentStreet1).Name("home_street1");
			Map(m => m.StudentStreet2).Name("home_Street2"); // case in file
			Map(m => m.StudentCity).Name("home_city");
			Map(m => m.StudentState).Name("home_state");
			Map(m => m.StudentZipCode).Name("home_zip");
			Map(m => m.ActivitySchoolYear).Name("School_Year"); // case in file
			Map(m => m.StudentEnrollmentDate).Name("first_date");
			Map(m => m.StudentWithdrawalDate).Name("last_date");
			Map(m => m.StudentIsSpecialEducation).Name("Education_Code").TypeConverter<BoolConverter>(); // case in file
			Map(m => m.StudentCurrentIep).Name("Current_IEP_Date"); // case in file
			Map(m => m.StudentFormerIep).Name("Former_IEP_Date"); // case in file
			Map(m => m.StudentNorep).Name("NORP_Date"); // case and spelling in file
			Map(m => m.StudentPaSecuredId).Name("pa_secured_id");
		}
	}
}
