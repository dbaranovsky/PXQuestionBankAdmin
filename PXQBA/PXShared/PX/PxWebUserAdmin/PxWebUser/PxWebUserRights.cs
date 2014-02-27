
using System.Collections.Generic;
using System.Linq;
using Bfw.Common.Collections;
using Bfw.Common.Database;

namespace PxWebUser
{
    [System.Serializable]
	public class PxWebUserRights
	{

		/// <summary>
		/// Gets the logger.
		/// </summary>
		Bfw.Common.Logging.ILogger Logger { get; set; }

		/// <summary>
		/// Gets the tracer.
		/// </summary>
		Bfw.Common.Logging.ITraceManager Tracer { get; set; }


		private readonly string userId;

		private readonly string courseId;

		private QuestionBankAccess questionBank;

		private AdminToolAccess adminTool;

		private readonly Dictionary<enumPxWebRightType, object> allAdminRights = new Dictionary<enumPxWebRightType, object>();

		/// <summary>
		/// PxWebUserRights Constructor
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="CourseId"> </param>
		public PxWebUserRights(string UserId, string CourseId)
		{
			courseId = CourseId;
			userId = UserId;
			GetAllRightsFromDb();
		}

		/// <summary>
		/// Get UserId
		/// </summary>
		public string UserId
		{
			get { return userId; }
		}

		/// <summary>
		/// Get UserId
		/// </summary>
		public string CourseId
		{
			get { return courseId; }
		}

		/// <summary>
		/// Get QuestionBank Rights
		/// </summary>
		public QuestionBankAccess QuestionBank
		{
			get
			{
				if (questionBank == null)
				{
					enumQuestionBankAdminRights adminRights = enumQuestionBankAdminRights.None;
					if (allAdminRights.ContainsKey(enumPxWebRightType.QuestionBank)) adminRights = (enumQuestionBankAdminRights)allAdminRights[enumPxWebRightType.QuestionBank];
					questionBank = new QuestionBankAccess(adminRights);
				}

				if (allAdminRights.ContainsKey(enumPxWebRightType.QuestionBank))
					allAdminRights[enumPxWebRightType.QuestionBank] = questionBank.adminRights;
				else
					allAdminRights.Add(enumPxWebRightType.QuestionBank, questionBank.adminRights);

				return questionBank;
			}
		}


		/// <summary>
		/// Get ProductMaster Rights
		/// </summary>
		public AdminToolAccess AdminTool
		{
			get
			{
				if (adminTool == null)
				{
					enumAdminToolRights adminRights = enumAdminToolRights.None;
					if (allAdminRights.ContainsKey(enumPxWebRightType.AdminTool)) adminRights = (enumAdminToolRights)allAdminRights[enumPxWebRightType.AdminTool];
					adminTool = new AdminToolAccess(adminRights);
				}

				if (allAdminRights.ContainsKey(enumPxWebRightType.AdminTool))
					allAdminRights[enumPxWebRightType.AdminTool] = adminTool.adminRights;
				else
					allAdminRights.Add(enumPxWebRightType.AdminTool, adminTool.adminRights);

				return adminTool;
			}
		}


		/// <summary>
		/// Get all User Rights from Db
		/// </summary>
		private void GetAllRightsFromDb()
		{

			var db = new DatabaseManager("PXData");

			try
			{
				db.StartSession();

				List<DatabaseRecord> records = db.Query("GetPxWebUserRights @0, @1", userId, courseId).ToList();

				if (!records.IsNullOrEmpty())
				{
					foreach (var dbRecord in records)
					{
						long lRights = dbRecord.Int64("Rights");
						enumPxWebRightType rightType = (enumPxWebRightType)dbRecord.Int64("PxWebRightId");

						//If there will be more types of adminRights then cast and add them to dictionary allAdminRights
						if (rightType == enumPxWebRightType.QuestionBank)
						{
							object adminRight = (enumQuestionBankAdminRights)lRights;
							if (!allAdminRights.ContainsKey(rightType))
								allAdminRights.Add(rightType, adminRight);
							else
								allAdminRights[rightType] = adminRight;

						}

						if (rightType == enumPxWebRightType.AdminTool)
						{
							object adminRight = (enumAdminToolRights)lRights;
							if (!allAdminRights.ContainsKey(rightType))
								allAdminRights.Add(rightType, adminRight);
							else
								allAdminRights[rightType] = adminRight;
						}
					}
				}
			}

			finally { db.EndSession(); }

		}

		/// <summary>
		/// Save User Rights
		/// </summary>
		/// <param name="webRightType"></param>
		/// <returns></returns>
		internal int Save(enumPxWebRightType webRightType)
		{
			int retVal;
			var db = new DatabaseManager("PXData");

			try
			{
				db.StartSession();

				if (webRightType == enumPxWebRightType.All)
				{
					string sAllAdminRights = allAdminRights.ToDictionary(admRight => (int)admRight.Key, admRight => (long)admRight.Value).Fold();
					retVal = db.ExecuteNonQuery("SaveAllPxWebUserRights @0, @1, @2", courseId, userId, sAllAdminRights);
				}
				else
				{
					long lRights = (long)(enumQuestionBankAdminRights)allAdminRights[webRightType];
					retVal = db.ExecuteNonQuery("SavePxWebUserRights @0, @1, @2, @3", courseId, userId, (int)webRightType, lRights);
				}

			}

			finally { db.EndSession(); }

			return retVal;
		}
	}
}