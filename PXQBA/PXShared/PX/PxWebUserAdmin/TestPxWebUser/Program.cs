
using PxWebUser;

namespace TestPxWebUser
{

	class Program
	{
		static void Main(string[] args)
		{

			//QuestionBankAdminRights value = QuestionBankAdminRights.AllowAddNote;

			//bool isAddNote = value.Is(QuestionBankAdminRights.AllowAddNote); //true
			//bool hasAddNote = value.Has(QuestionBankAdminRights.AllowAddNote); //true

			//value = value.Add(QuestionBankAdminRights.ShowHistory);
			//value = value.Add(QuestionBankAdminRights.ShowQuestionEditor);

			//value = value.Remove(QuestionBankAdminRights.ShowQuestionNotes);

			//bool hasShowHistory = value.Has(QuestionBankAdminRights.ShowHistory); //true
			//bool isShowHistory = value.Is(QuestionBankAdminRights.ShowHistory); //true

			//bool hasQuestionEditor = value.Has(QuestionBankAdminRights.ShowQuestionEditor); //false
			//bool isQuestionEditor = value.Is(QuestionBankAdminRights.ShowQuestionEditor); //false

			//bool isShowQuestionNotes = value.Is(QuestionBankAdminRights.ShowQuestionNotes); //false
			//bool hasShowQuestionNotes = value.Has(QuestionBankAdminRights.ShowQuestionNotes); //false

			//string s = value.ToString();

			//QuestionBankAdminRights v;

			//if (Enum.TryParse(s, out v))
			//{
			//    bool isNote = v.Is(QuestionBankAdminRights.AllowAddNote); //true

			//    bool isQuestionNotes = v.Is(QuestionBankAdminRights.ShowQuestionNotes); //false
			//    bool hasQuestionNotes = v.Has(QuestionBankAdminRights.ShowQuestionNotes); //false
			//}

			const string userId = "10452";
			const string courseId = "55704";

			PxWebUserRights pxWebUserRights = new PxWebUserRights(userId, courseId);

			pxWebUserRights.QuestionBank.AllowEditQuestion = false;
			pxWebUserRights.QuestionBank.AllowAddQuestion = false;
			pxWebUserRights.QuestionBank.AllowFlagQuestion = false;
			pxWebUserRights.QuestionBank.AllowAddNote = false;
			pxWebUserRights.QuestionBank.ShowQuestionBankManager = true;
			pxWebUserRights.QuestionBank.ShowHistory = false;
			pxWebUserRights.QuestionBank.ShowQuestionEditor = false;
			pxWebUserRights.QuestionBank.ShowQuestionNotes = false;

			//pxWebUserRights.ProductMaster.AllowAddProduct = false;
			//pxWebUserRights.ProductMaster.AllowEditProduct = false;
			//pxWebUserRights.ProductMaster.ShowProductManager = false;

			pxWebUserRights.Save(enumPxWebRightType.All);

			pxWebUserRights.QuestionBank.None = true;
			//pxWebUserRights.QuestionBank.
		}
	}
}
