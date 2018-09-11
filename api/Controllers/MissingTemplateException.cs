using System;
using models;

namespace api.Controllers
{
	public class MissingTemplateException : Exception
	{
		public MissingTemplateException(int templateId) :
			base($"Could not find template with Id '{templateId}'.")
		{ }

		public MissingTemplateException(ReportType type, string schoolYear) :
			base($"Could not find template for type '{type}' and school year '{schoolYear}'.")
		{ }
	}

}
