using System;

namespace PxWebUser
{

	internal enum enumPxWebRightType 
	{
		QuestionBank=1,
		AdminTool=2,
		All=3
	}

	[Flags]
	public enum enumQuestionBankAdminRights : long
	{
		None = 0x0,
		ShowQuestionEditor = 0x1,
		AllowAddQuestion = 0x10,
		ShowQuestionNotes = 0x20,
		AllowEditQuestion = 0x40,
		AllowAddNote = 0x80,
		AllowFlagQuestion = 0x100,
		ShowHistory = 0x200,
		ShowQuestionBankManager = 0x400
	}



	[Flags]
	public enum enumAdminToolRights : long
	{
		None = 0x0,
		AllowEditSandboxCourse = 0x1,
        AllowPublishCourse = 0x10,
        AllowRevertCourse = 0x20
	}



	/// <summary>
	/// Enumeration Extension, which takes care of bitwise OR logic 
	/// </summary>
	public static class EnumerationExtensions {
		public static bool Is<T>(this Enum type, T value)
		{
						
			try {
				return ( ( (long)(object)type) == (long)(object)value );
			} 
			catch (Exception ex)
			{				
				return false;
			}
		}

		public static bool Has<T>(this Enum type, T value)
		{
			try {
				return ( (long)(object)type & (long)(object)value) == (long)(object)value ;
			}
			catch (Exception ex)
			{
				return false;
			}    
		}


		public static T Add<T>(this Enum type, T value)
		{
			try 
			{
				return (T)(object)( ( (long)(object)type | (long)(object)value ) );
			}
			catch(Exception ex) {
				throw new ArgumentException(
					string.Format(
						"Could not append value from enumerated type '{0}'.",
						typeof(T).Name
						), ex);
			}    
		}


		public static T Remove<T>(this Enum type, T value)
		{
			try {
				return (T)(object)( ( (long)(object)type & ~(long)(object)value ) );
			}
			catch (Exception ex) {
				throw new ArgumentException(
					string.Format(
						"Could not remove value from enumerated type '{0}'.",
						typeof(T).Name
						), ex);
			}  
		}

	}
}


