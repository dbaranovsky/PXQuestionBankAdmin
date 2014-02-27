namespace PxWebUser
{
	public class QuestionBankAccess
	{

		public bool None
		{
			get { return adminRights.Is(enumQuestionBankAdminRights.None); }
			internal set
			{
				if (value) adminRights= adminRights.Remove(adminRights);				
			}
		}

		public bool ShowQuestionEditor
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.ShowQuestionEditor); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.ShowQuestionEditor) : adminRights.Remove(enumQuestionBankAdminRights.ShowQuestionEditor);				
			}
		}

		public bool AllowAddQuestion
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.AllowAddQuestion); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.AllowAddQuestion) : adminRights.Remove(enumQuestionBankAdminRights.AllowAddQuestion);				
			}
		}

		public bool ShowQuestionNotes
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.ShowQuestionNotes); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.ShowQuestionNotes) : adminRights.Remove(enumQuestionBankAdminRights.ShowQuestionNotes);
			}
		}

		public bool AllowEditQuestion
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.AllowEditQuestion); }
			internal set 
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.AllowEditQuestion) : adminRights.Remove(enumQuestionBankAdminRights.AllowEditQuestion);
			}
		}

		public bool AllowAddNote
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.AllowAddNote); }
			internal set
			{				
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.AllowAddNote) : adminRights.Remove(enumQuestionBankAdminRights.AllowAddNote);
			}
		}

		public bool AllowFlagQuestion
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.AllowFlagQuestion); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.AllowFlagQuestion) : adminRights.Remove(enumQuestionBankAdminRights.AllowFlagQuestion);
			}
		}

		public bool ShowHistory
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.ShowHistory); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.ShowHistory) : adminRights.Remove(enumQuestionBankAdminRights.ShowHistory);
			}
		}

		public bool ShowQuestionBankManager
		{
			get { return adminRights.Has(enumQuestionBankAdminRights.ShowQuestionBankManager); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumQuestionBankAdminRights.ShowQuestionBankManager) : adminRights.Remove(enumQuestionBankAdminRights.ShowQuestionBankManager);
			}
		}

		
		internal  enumQuestionBankAdminRights adminRights;

		internal QuestionBankAccess(enumQuestionBankAdminRights rights)
		{
			adminRights = rights;
		}

	}
}
