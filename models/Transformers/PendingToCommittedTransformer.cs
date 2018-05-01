using System.Collections.Generic;

namespace models.Transformers
{
	public class PendingToCommittedTransformer :
		Transformer<PendingStudentStatusRecord, CommittedStudentStatusRecord>
	{
		private readonly ICommittedStudentStatusRecordRepository _committed;

		public PendingToCommittedTransformer(ICommittedStudentStatusRecordRepository committed)
		{
			_committed = committed;
		}

		protected override IEnumerable<CommittedStudentStatusRecord> Transform(IEnumerable<PendingStudentStatusRecord> pendings)
		{
			foreach (var pending in pendings)
			{
				var committed = CommittedStudentStatusRecord.FromPendingStudentStatusRecord(pending);
				_committed.Create(committed);
				yield return committed;
			}
		}
	}
}
